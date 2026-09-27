using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Repositories
{
    public class PlaceRepository
    {
        public List<PlaceModel> GetAllPlaces(int limit = 2000)
        {
            var places = new List<PlaceModel>();
            const string sql = @"SELECT place_id, place_name, country_name, latitude, longitude,
                                        time_zone, summer_time_correction
                                 FROM tbl_place
                                 ORDER BY place_name, country_name
                                 LIMIT @Limit";
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(sql, new MySqlParameter("@Limit", limit));
                foreach (DataRow row in dt.Rows) places.Add(MapRowToPlace(row));
            }
            return places;
        }

        public List<PlaceModel> SearchPlaces(string keyword)
        {
            var places = new List<PlaceModel>();
            var term = (keyword ?? "").Trim();
            // Require a real prefix — empty "%" searches pull arbitrary atlas rows.
            if (term.Length < 2)
                return places;

            const string sql = @"SELECT place_id, place_name, country_name, latitude, longitude,
                                        time_zone, summer_time_correction
                                 FROM tbl_place
                                 WHERE place_name LIKE @Starts OR country_name LIKE @Starts
                                    OR place_name LIKE @Contains OR country_name LIKE @Contains
                                 ORDER BY CASE WHEN place_name LIKE @Starts THEN 0
                                               WHEN country_name LIKE @Starts THEN 1 ELSE 2 END,
                                          place_name, country_name
                                 LIMIT 20";
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(sql,
                    new MySqlParameter("@Starts", term + "%"),
                    new MySqlParameter("@Contains", "%" + term + "%"));
                foreach (DataRow row in dt.Rows) places.Add(MapRowToPlace(row));
            }
            return places;
        }

        public PlaceModel GetPlaceById(int placeId)
        {
            const string sql = @"SELECT place_id, place_name, country_name, latitude, longitude,
                                        time_zone, summer_time_correction
                                 FROM tbl_place WHERE place_id = @PlaceId LIMIT 1";
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(sql, new MySqlParameter("@PlaceId", placeId));
                return dt.Rows.Count == 0 ? null : MapRowToPlace(dt.Rows[0]);
            }
        }

        public int InsertPlace(PlaceModel place)
        {
            if (place == null) throw new ArgumentNullException("place");
            const string sql = @"INSERT INTO tbl_place
                (place_name, country_name, latitude, longitude, time_zone, summer_time_correction)
                VALUES (@Name, @Country, @Latitude, @Longitude, @Timezone, @SmtCorr)";
            using (var db = new DatabaseHelper())
            {
                return db.ExecuteScalarWithIdentity(sql,
                    new MySqlParameter("@Name", place.PlaceName),
                    new MySqlParameter("@Country", String.IsNullOrWhiteSpace(place.CountryName) ? "India" : place.CountryName),
                    new MySqlParameter("@Latitude", place.Latitude),
                    new MySqlParameter("@Longitude", place.Longitude),
                    new MySqlParameter("@Timezone", (object)place.TimeZone ?? DBNull.Value),
                    new MySqlParameter("@SmtCorr", (object)place.SMTCorr ?? DBNull.Value));
            }
        }

        private static PlaceModel MapRowToPlace(DataRow row)
        {
            return new PlaceModel
            {
                PlaceID = Convert.ToInt32(row["place_id"]),
                PlaceName = Convert.ToString(row["place_name"]),
                CountryID = 0,
                CountryName = Convert.ToString(row["country_name"]),
                Latitude = Convert.ToDecimal(row["latitude"]),
                Longitude = Convert.ToDecimal(row["longitude"]),
                TimeZone = Convert.ToString(row["time_zone"]),
                SMTCorr = Convert.ToString(row["summer_time_correction"])
            };
        }
    }
}
