using System.Collections.Generic;
using System.Data;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;
using MySql.Data.MySqlClient;
using System;

namespace NirmalaAstrology_Web.Repositories
{
    /// <summary>
    /// Repository for Zodiac-related database operations.
    /// Uses ADO.NET with parameterized queries and explicit column lists.
    /// </summary>
    public class ZodiacRepository
    {
        private const string SelectColumns = @"ZodiacId, Name, HindiName, Icon, Image, Description,
            LuckyNumber, LuckyColor, LuckyDay, Planet, Element, Nature, Strength, Weakness,
            Career, Love, Health, Finance, Personality, Characteristics, CompatibleSigns, CreatedDate";

        /// <summary>
        /// Gets all 12 zodiac signs ordered by ZodiacId.
        /// </summary>
        public List<ZodiacModel> GetAllZodiacs()
        {
            List<ZodiacModel> zodiacs = new List<ZodiacModel>();
            string query = "SELECT " + SelectColumns + " FROM Zodiac ORDER BY ZodiacId ASC";

            using (DatabaseHelper db = new DatabaseHelper())
            {
                DataTable dt = db.ExecuteDataTable(query);
                foreach (DataRow row in dt.Rows)
                {
                    zodiacs.Add(MapRowToZodiac(row));
                }
            }
            return zodiacs;
        }

        /// <summary>
        /// Gets a single zodiac sign by its URL-friendly name.
        /// e.g., "aries" matches the Name "Aries" (case-insensitive).
        /// </summary>
        public ZodiacModel GetZodiacByName(string name)
        {
            string query = "SELECT " + SelectColumns + " FROM Zodiac WHERE LOWER(Name) = LOWER(@Name) LIMIT 1";

            using (DatabaseHelper db = new DatabaseHelper())
            {
                MySqlParameter[] parameters = {
                    new MySqlParameter("@Name", name.Replace("-", " "))
                };
                DataTable dt = db.ExecuteDataTable(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    return MapRowToZodiac(dt.Rows[0]);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a zodiac by its primary key ID.
        /// </summary>
        public ZodiacModel GetZodiacById(int zodiacId)
        {
            string query = "SELECT " + SelectColumns + " FROM Zodiac WHERE ZodiacId = @ZodiacId LIMIT 1";

            using (DatabaseHelper db = new DatabaseHelper())
            {
                MySqlParameter[] parameters = {
                    new MySqlParameter("@ZodiacId", zodiacId)
                };
                DataTable dt = db.ExecuteDataTable(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    return MapRowToZodiac(dt.Rows[0]);
                }
            }
            return null;
        }

        /// <summary>
        /// Maps a DataRow to a ZodiacModel.
        /// </summary>
        private ZodiacModel MapRowToZodiac(DataRow row)
        {
            return new ZodiacModel
            {
                ZodiacId = Convert.ToInt32(row["ZodiacId"]),
                Name = row["Name"].ToString(),
                HindiName = row["HindiName"].ToString(),
                Icon = row["Icon"].ToString(),
                Image = row["Image"].ToString(),
                Description = row["Description"].ToString(),
                LuckyNumber = row["LuckyNumber"].ToString(),
                LuckyColor = row["LuckyColor"].ToString(),
                LuckyDay = row["LuckyDay"].ToString(),
                Planet = row["Planet"].ToString(),
                Element = row["Element"].ToString(),
                Nature = row["Nature"].ToString(),
                Strength = row["Strength"].ToString(),
                Weakness = row["Weakness"].ToString(),
                Career = row["Career"].ToString(),
                Love = row["Love"].ToString(),
                Health = row["Health"].ToString(),
                Finance = row["Finance"].ToString(),
                Personality = row["Personality"].ToString(),
                Characteristics = row["Characteristics"].ToString(),
                CompatibleSigns = row["CompatibleSigns"].ToString(),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }
    }
}
