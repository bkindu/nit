using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Hosting;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// VB6 <c>a3Functions_Gen.Uday_Astha</c> against legacy table <c>Uday_Astha</c>
    /// (Indu.mdb → MySQL indu_astro). For Mars..Saturn: first row where JDF_* &gt; birth JD;
    /// code "A" → Uday, otherwise Astha.
    /// </summary>
    public static class UdayAsthaLookup
    {
        private static readonly object Sync = new object();
        private static List<UaEvent>[] _events; // indices 4..8
        private static string _loadSource;

        /// <summary>Diagnostic: "mysql", "csv", or "empty".</summary>
        public static string LoadSource
        {
            get
            {
                EnsureLoaded();
                return _loadSource ?? "empty";
            }
        }

        public static string ForPlanet(string planetName, double dJdf)
        {
            int idx = PlanetIndex(planetName);
            if (idx < 4 || idx > 8) return null;
            EnsureLoaded();
            var list = _events[idx];
            if (list == null || list.Count == 0) return null;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Jdf > dJdf)
                    return string.Equals(list[i].Code, "A", StringComparison.OrdinalIgnoreCase)
                        ? "Uday"
                        : "Astha";
            }
            return "Uday";
        }

        /// <summary>Resolve Mars..Saturn status map for one birth JD (VB6 iUA / sUdayAsta).</summary>
        public static IDictionary<string, string> ResolveAll(double dJdf)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Mars", ForPlanet("Mars", dJdf) ?? "Uday" },
                { "Mercury", ForPlanet("Mercury", dJdf) ?? "Uday" },
                { "Jupiter", ForPlanet("Jupiter", dJdf) ?? "Uday" },
                { "Venus", ForPlanet("Venus", dJdf) ?? "Uday" },
                { "Saturn", ForPlanet("Saturn", dJdf) ?? "Uday" }
            };
        }

        private static int PlanetIndex(string name)
        {
            switch ((name ?? "").Trim().ToLowerInvariant())
            {
                case "mars":
                case "mangla":
                case "mangal":
                case "mar":
                    return 4;
                case "mercury":
                case "budha":
                case "mer":
                    return 5;
                case "jupiter":
                case "guru":
                case "jup":
                    return 6;
                case "venus":
                case "sukra":
                case "ven":
                    return 7;
                case "saturn":
                case "sani":
                case "sat":
                    return 8;
                default:
                    return -1;
            }
        }

        private static void EnsureLoaded()
        {
            if (_events != null) return;
            lock (Sync)
            {
                if (_events != null) return;
                var loaded = NewBuckets();

                if (TryLoadFromMysql(loaded))
                {
                    _loadSource = "mysql";
                    _events = loaded;
                    return;
                }

                loaded = NewBuckets();
                var path = ResolveCsvPath();
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    LoadCsv(path, loaded);
                    _loadSource = "csv";
                    _events = loaded;
                    return;
                }

                _loadSource = "empty";
                _events = loaded;
            }
        }

        private static List<UaEvent>[] NewBuckets()
        {
            var loaded = new List<UaEvent>[9];
            for (int i = 4; i <= 8; i++)
                loaded[i] = new List<UaEvent>(512);
            return loaded;
        }

        /// <summary>
        /// Primary path: MySQL legacy flat table (same columns as Indu.mdb Uday_Astha).
        /// </summary>
        private static bool TryLoadFromMysql(List<UaEvent>[] loaded)
        {
            try
            {
                // Prefer exact legacy name; also accept lower-case MySQL fold.
                const string sql = @"
SELECT JDF_Mar, Mar_UA, JDF_Mer, Mer_UA, JDF_Jup, Jup_UA, JDF_Ven, Ven_UA, JDF_Sat, Sat_UA
FROM Uday_Astha";

                using (var db = new DatabaseHelper())
                {
                    DataTable dt;
                    try
                    {
                        dt = db.ExecuteDataTable(sql);
                    }
                    catch (MySqlException)
                    {
                        dt = db.ExecuteDataTable(@"
SELECT JDF_Mar, Mar_UA, JDF_Mer, Mer_UA, JDF_Jup, Jup_UA, JDF_Ven, Ven_UA, JDF_Sat, Sat_UA
FROM uday_astha");
                    }

                    if (dt == null || dt.Rows.Count == 0)
                        return false;

                    foreach (DataRow row in dt.Rows)
                    {
                        Append(loaded[4], Cell(row, "JDF_Mar"), Cell(row, "Mar_UA"));
                        Append(loaded[5], Cell(row, "JDF_Mer"), Cell(row, "Mer_UA"));
                        Append(loaded[6], Cell(row, "JDF_Jup"), Cell(row, "Jup_UA"));
                        Append(loaded[7], Cell(row, "JDF_Ven"), Cell(row, "Ven_UA"));
                        Append(loaded[8], Cell(row, "JDF_Sat"), Cell(row, "Sat_UA"));
                    }

                    for (int i = 4; i <= 8; i++)
                        loaded[i].Sort((a, b) => a.Jdf.CompareTo(b.Jdf));

                    return loaded[4].Count > 0 || loaded[5].Count > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string Cell(DataRow row, string col)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(col))
                return null;
            if (row[col] == null || row[col] == DBNull.Value)
                return null;
            return Convert.ToString(row[col], CultureInfo.InvariantCulture);
        }

        private static string ResolveCsvPath()
        {
            try
            {
                var mapped = HostingEnvironment.MapPath("~/App_Data/Uday_Astha.csv");
                if (!string.IsNullOrEmpty(mapped) && File.Exists(mapped))
                    return mapped;
            }
            catch
            {
                // non-web host
            }

            var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? "";
            var candidates = new[]
            {
                Path.Combine(baseDir, "App_Data", "Uday_Astha.csv"),
                Path.Combine(baseDir, "..", "App_Data", "Uday_Astha.csv"),
                Path.Combine(baseDir, "..", "..", "App_Data", "Uday_Astha.csv")
            };
            foreach (var c in candidates)
            {
                try
                {
                    var full = Path.GetFullPath(c);
                    if (File.Exists(full)) return full;
                }
                catch
                {
                    // ignore
                }
            }
            return null;
        }

        private static void LoadCsv(string path, List<UaEvent>[] loaded)
        {
            var lines = File.ReadAllLines(path);
            for (int li = 1; li < lines.Length; li++)
            {
                var line = lines[li];
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length < 10) continue;

                Append(loaded[4], parts[0], parts[1]);
                Append(loaded[5], parts[2], parts[3]);
                Append(loaded[6], parts[4], parts[5]);
                Append(loaded[7], parts[6], parts[7]);
                Append(loaded[8], parts[8], parts[9]);
            }

            for (int i = 4; i <= 8; i++)
                loaded[i].Sort((a, b) => a.Jdf.CompareTo(b.Jdf));
        }

        private static void Append(List<UaEvent> list, string jdfText, string code)
        {
            if (string.IsNullOrWhiteSpace(jdfText) || string.IsNullOrWhiteSpace(code)) return;
            double jdf;
            if (!double.TryParse(jdfText.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out jdf))
                return;
            list.Add(new UaEvent { Jdf = jdf, Code = code.Trim() });
        }

        private struct UaEvent
        {
            public double Jdf;
            public string Code;
        }
    }
}
