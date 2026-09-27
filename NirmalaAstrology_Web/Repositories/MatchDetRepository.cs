using System;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;

namespace NirmalaAstrology_Web.Repositories
{
    /// <summary>
    /// Legacy equivalent: Access <c>MatchDet</c> 36×36 guna matrix
    /// (<c>nMatchMaking.FindGunaString_FrVb6</c> / <c>mMatching.bas</c>).
    /// MySQL: <c>tbl_match_det</c> columns <c>field_01</c>…<c>field_36</c> map to Access <c>Field1</c>…<c>Field36</c>.
    /// Matrix is static master data — loaded once per AppDomain (safe cache; same results as per-query SELECT).
    /// </summary>
    public sealed class MatchDetRepository
    {
        private static readonly object Gate = new object();
        private static string[,] _matrix; // [row 1..36, col 1..36]

        public string GetGunaString(int rowId, int column)
        {
            if (rowId < 1 || rowId > 36 || column < 1 || column > 36)
                throw new ArgumentOutOfRangeException("MatchDet coordinates must be 1-36.");

            EnsureLoaded();
            return _matrix[rowId, column] ?? "";
        }

        private static void EnsureLoaded()
        {
            if (_matrix != null) return;
            lock (Gate)
            {
                if (_matrix != null) return;
                var grid = new string[37, 37];
                using (var db = new DatabaseHelper())
                {
                    // Single round-trip for the entire legacy MatchDet table (36 rows).
                    var dt = db.ExecuteDataTable("SELECT * FROM tbl_match_det ORDER BY id");
                    foreach (DataRow row in dt.Rows)
                    {
                        var id = Convert.ToInt32(row["id"]);
                        if (id < 1 || id > 36) continue;
                        for (var col = 1; col <= 36; col++)
                        {
                            var name = "field_" + col.ToString("00");
                            if (!row.Table.Columns.Contains(name)) continue;
                            var v = row[name];
                            grid[id, col] = v == null || v == DBNull.Value ? "" : Convert.ToString(v);
                        }
                    }
                }
                Thread.MemoryBarrier();
                _matrix = grid;
            }
        }
    }
}
