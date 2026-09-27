using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Repositories
{
    /// <summary>
    /// Loads normalized report content from MySQL in a small number of batched queries.
    /// The astronomy calculation remains in KundliWorkflowService; this repository only
    /// resolves legacy VB6 reading text and rule metadata.
    /// </summary>
    public sealed class KundliReportRepository
    {
        public void Enrich(KundliDetailsModel details)
        {
            if (details == null) throw new ArgumentNullException("details");

            var content = new KundliDatabaseContentModel();
            using (var db = new DatabaseHelper())
            {
                var ds = db.ExecuteDataSet(@"
SELECT nr.reading_text, nr.past_birth_q1, nr.past_birth_q2, nr.past_birth_q3, nr.past_birth_q4
FROM tbl_nakshatra_reading nr
JOIN tbl_nakshatra n ON n.nakshatra_id = nr.nakshatra_id
JOIN tbl_language l ON l.language_id = nr.language_id
WHERE n.nakshatra_no = @Nakshatra AND l.language_code = @Language LIMIT 1;

SELECT nc.reading_text
FROM tbl_nakshatra_charan_reading nc
JOIN tbl_nakshatra n ON n.nakshatra_id = nc.nakshatra_id
JOIN tbl_language l ON l.language_id = nc.language_id
WHERE n.nakshatra_no = @Nakshatra AND nc.charan_no = @Pada AND l.language_code = @Language LIMIT 1;

SELECT sr.reading_text, sr.source_reading, sr.rajyog_reading, sr.spouse_strategy
FROM tbl_sign_reading sr
JOIN tbl_sign s ON s.sign_id = sr.sign_id
JOIN tbl_language l ON l.language_id = sr.language_id
WHERE s.sign_no = @Sign AND l.language_code = @Language LIMIT 1;

SELECT hr.reading_text
FROM tbl_house_reading hr
JOIN tbl_language l ON l.language_id = hr.language_id
WHERE hr.house_id = @House AND l.language_code = @Language LIMIT 1;

SELECT p.planet_code, p.planet_name, ph.house_id, ph.birth_reading, ph.gochar_reading, ph.lal_kitab_reading, ph.source_reading
FROM tbl_planet_house_reading ph
JOIN tbl_planet p ON p.planet_id = ph.planet_id
JOIN tbl_language l ON l.language_id = ph.language_id
WHERE l.language_code = @Language;

SELECT p.planet_code, p.planet_name, ps.sign_id, ps.source_reading
FROM tbl_planet_sign_reading ps
JOIN tbl_planet p ON p.planet_id = ps.planet_id
JOIN tbl_language l ON l.language_id = ps.language_id
WHERE l.language_code = @Language;

SELECT p.planet_code, p.planet_name, ua.rule_value, ua.status_code
FROM tbl_udaya_astha_rule ua
JOIN tbl_planet p ON p.planet_id = ua.planet_id;
", new MySqlParameter("@Nakshatra", details.NakshatraNumber),
                   new MySqlParameter("@Pada", details.NakshatraPada),
                   new MySqlParameter("@Sign", details.BirthRashi),
                   new MySqlParameter("@House", 1),
                   new MySqlParameter("@Language", "en"));

                ReadNakshatra(ds, content);
                ReadCharan(ds, content);
                ReadSign(ds, content);
                ReadHouse(ds, content);
                ReadPlanetHouse(ds, content);
                ReadPlanetSign(ds, content);
                ReadUdayaAstha(ds, content);
            }
            details.DatabaseContent = content;
        }

        private static void ReadNakshatra(DataSet ds, KundliDatabaseContentModel content)
        {
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return;
            var row = ds.Tables[0].Rows[0];
            content.NakshatraReading = Text(row, "reading_text");
        }

        private static void ReadCharan(DataSet ds, KundliDatabaseContentModel content)
        {
            if (ds.Tables.Count < 2 || ds.Tables[1].Rows.Count == 0) return;
            content.NakshatraCharanReading = Text(ds.Tables[1].Rows[0], "reading_text");
        }

        private static void ReadSign(DataSet ds, KundliDatabaseContentModel content)
        {
            if (ds.Tables.Count < 3 || ds.Tables[2].Rows.Count == 0) return;
            var row = ds.Tables[2].Rows[0];
            content.SignReading = JoinText(Text(row, "reading_text"), Text(row, "source_reading"), Text(row, "rajyog_reading"), Text(row, "spouse_strategy"));
        }

        private static void ReadHouse(DataSet ds, KundliDatabaseContentModel content)
        {
            if (ds.Tables.Count < 4 || ds.Tables[3].Rows.Count == 0) return;
            content.HouseReading = Text(ds.Tables[3].Rows[0], "reading_text");
        }

        private static void ReadPlanetHouse(DataSet ds, KundliDatabaseContentModel content)
        {
            if (ds.Tables.Count < 5) return;
            foreach (DataRow row in ds.Tables[4].Rows)
            {
                content.PlanetHouseReadings.Add(new PlanetReadingModel
                {
                    PlanetCode = Text(row, "planet_code"), PlanetName = Text(row, "planet_name"),
                    House = Number(row, "house_id"), BirthReading = Text(row, "birth_reading"),
                    GocharReading = Text(row, "gochar_reading"), LalKitabReading = Text(row, "lal_kitab_reading"),
                    SourceReading = Text(row, "source_reading")
                });
            }
        }

        private static void ReadPlanetSign(DataSet ds, KundliDatabaseContentModel content)
        {
            if (ds.Tables.Count < 6) return;
            foreach (DataRow row in ds.Tables[5].Rows)
            {
                content.PlanetSignReadings.Add(new PlanetReadingModel
                {
                    PlanetCode = Text(row, "planet_code"), PlanetName = Text(row, "planet_name"),
                    Sign = Number(row, "sign_id"), SourceReading = Text(row, "source_reading")
                });
            }
        }

        private static void ReadUdayaAstha(DataSet ds, KundliDatabaseContentModel content)
        {
            if (ds.Tables.Count < 7) return;
            content.UdayaAsthaRule = String.Join("; ", ds.Tables[6].AsEnumerable().Select(r =>
                Text(r, "planet_code") + ":" + Text(r, "status_code") + (Text(r, "rule_value") == "" ? "" : " (" + Text(r, "rule_value") + ")")));
        }

        private static string Text(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? Convert.ToString(row[column]) : "";
        }

        private static int Number(DataRow row, string column)
        {
            int value;
            return Int32.TryParse(Text(row, column), out value) ? value : 0;
        }

        private static string JoinText(params string[] values)
        {
            return String.Join("\n", values.Where(x => !String.IsNullOrWhiteSpace(x)));
        }
    }
}
