using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Repositories
{
    public sealed class NativeRepository
    {
        private const string SelectColumns = @"native_id, serial_no, native_name, gender, birth_date, birth_date_text,
            birth_time, birth_time_text, birth_place, country_name, place_id, latitude, longitude, time_zone,
            nick_name, father_name, address_text, remarks, temp_record, life_spheres, created_at";

        public IList<NativeRecordModel> Search(NativeSearchRequest request, out int totalCount)
        {
            if (request == null) request = new NativeSearchRequest();
            if (request.Page < 1) request.Page = 1;
            if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 25;

            var where = new List<string>();
            var parameters = new List<MySqlParameter>();
            AddLike(where, parameters, "native_name", "Name", request.Name);
            AddLike(where, parameters, "birth_place", "Place", request.Place);
            AddLike(where, parameters, "address_text", "Address", request.Address);
            AddLike(where, parameters, "remarks", "Remarks", request.Remarks);
            if (!String.IsNullOrWhiteSpace(request.BirthDate))
            {
                where.Add("(birth_date_text LIKE @BirthDate OR CAST(birth_date AS CHAR) LIKE @BirthDate)");
                parameters.Add(new MySqlParameter("@BirthDate", "%" + request.BirthDate.Trim() + "%"));
            }
            if (request.SerialNo.HasValue)
            {
                where.Add("serial_no = @SerialNo");
                parameters.Add(new MySqlParameter("@SerialNo", request.SerialNo.Value));
            }

            var filter = where.Count == 0 ? "1=1" : String.Join(" AND ", where);
            var countSql = "SELECT COUNT(*) FROM tbl_native WHERE " + filter;
            var sql = "SELECT " + SelectColumns + " FROM tbl_native WHERE " + filter +
                      " ORDER BY native_id DESC LIMIT @Limit OFFSET @Offset";

            parameters.Add(new MySqlParameter("@Limit", request.PageSize));
            parameters.Add(new MySqlParameter("@Offset", (request.Page - 1) * request.PageSize));

            using (var db = new DatabaseHelper())
            {
                var countParams = CloneWithout(parameters, "@Limit", "@Offset");
                totalCount = Convert.ToInt32(db.ExecuteScalar(countSql, countParams) ?? 0);
                var dt = db.ExecuteDataTable(sql, Clone(parameters));
                var list = new List<NativeRecordModel>();
                foreach (DataRow row in dt.Rows) list.Add(Map(row));
                return list;
            }
        }

        public NativeRecordModel GetById(int nativeId)
        {
            const string sql = "SELECT " + SelectColumns + " FROM tbl_native WHERE native_id = @Id LIMIT 1";
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(sql, new MySqlParameter("@Id", nativeId));
                return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
            }
        }

        public int Save(NativeRecordModel native)
        {
            if (native == null) throw new ArgumentNullException("native");
            const string sql = @"INSERT INTO tbl_native
                (serial_no, native_name, gender, gender_code, birth_date, birth_date_text, birth_date_legacy,
                 birth_time, birth_time_text, birth_time_legacy, birth_at,
                 birth_place, country_name, place_id, latitude, longitude, time_zone, nick_name, father_name,
                 address_text, remarks, temp_record, is_temporary, life_spheres)
                VALUES
                (@SerialNo, @Name, @Gender, @Gender, @BirthDate, @BirthDateText, @BirthDateText,
                 @BirthTime, @BirthTimeText, @BirthTimeText, @BirthAt,
                 @Place, @Country, @PlaceId, @Latitude, @Longitude, @TimeZone, @Nick, @Father,
                 @Address, @Remarks, @Temp, @Temp, @Spheres)";
            using (var db = new DatabaseHelper())
            {
                return db.ExecuteScalarWithIdentity(sql, Parameters(native));
            }
        }

        public void Update(NativeRecordModel native)
        {
            if (native == null) throw new ArgumentNullException("native");
            const string sql = @"UPDATE tbl_native SET
                native_name=@Name, gender=@Gender, gender_code=@Gender,
                birth_date=@BirthDate, birth_date_text=@BirthDateText, birth_date_legacy=@BirthDateText,
                birth_time=@BirthTime, birth_time_text=@BirthTimeText, birth_time_legacy=@BirthTimeText,
                birth_at=@BirthAt,
                birth_place=@Place, country_name=@Country,
                place_id=@PlaceId, latitude=@Latitude, longitude=@Longitude, time_zone=@TimeZone,
                nick_name=@Nick, father_name=@Father, address_text=@Address, remarks=@Remarks,
                temp_record=@Temp, is_temporary=@Temp, life_spheres=@Spheres, serial_no=@SerialNo
                WHERE native_id=@Id";
            var parameters = new List<MySqlParameter>(Parameters(native)) { new MySqlParameter("@Id", native.NativeId) };
            using (var db = new DatabaseHelper())
            {
                db.ExecuteNonQuery(sql, parameters.ToArray());
            }
        }

        public void Delete(int nativeId)
        {
            const string sql = "DELETE FROM tbl_native WHERE native_id = @Id";
            using (var db = new DatabaseHelper())
            {
                db.ExecuteNonQuery(sql, new MySqlParameter("@Id", nativeId));
            }
        }

        private static MySqlParameter[] Clone(List<MySqlParameter> source)
        {
            var copy = new MySqlParameter[source.Count];
            for (var i = 0; i < source.Count; i++)
                copy[i] = new MySqlParameter(source[i].ParameterName, source[i].Value);
            return copy;
        }

        private static MySqlParameter[] CloneWithout(List<MySqlParameter> source, params string[] skip)
        {
            var list = new List<MySqlParameter>();
            foreach (var parameter in source)
            {
                var ignored = false;
                foreach (var name in skip)
                {
                    if (parameter.ParameterName == name) { ignored = true; break; }
                }
                if (!ignored) list.Add(new MySqlParameter(parameter.ParameterName, parameter.Value));
            }
            return list.ToArray();
        }

        private static void AddLike(List<string> where, List<MySqlParameter> parameters, string column, string name, string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return;
            where.Add(column + " LIKE @" + name);
            parameters.Add(new MySqlParameter("@" + name, "%" + value.Trim() + "%"));
        }

        private static MySqlParameter[] Parameters(NativeRecordModel native)
        {
            object birthAt = DBNull.Value;
            if (native.BirthDate.HasValue)
            {
                var dt = native.BirthDate.Value.Date;
                if (native.BirthTime.HasValue) dt = dt.Add(native.BirthTime.Value);
                birthAt = dt;
            }

            return new[]
            {
                new MySqlParameter("@SerialNo", (object)native.SerialNo ?? DBNull.Value),
                new MySqlParameter("@Name", native.NativeName ?? ""),
                new MySqlParameter("@Gender", String.IsNullOrWhiteSpace(native.Gender) ? "M" : native.Gender.Substring(0, 1).ToUpperInvariant()),
                new MySqlParameter("@BirthDate", (object)native.BirthDate ?? DBNull.Value),
                new MySqlParameter("@BirthDateText", (object)native.BirthDateText ?? DBNull.Value),
                new MySqlParameter("@BirthTime", native.BirthTime.HasValue ? (object)native.BirthTime.Value.ToString(@"hh\:mm\:ss") : DBNull.Value),
                new MySqlParameter("@BirthTimeText", (object)native.BirthTimeText ?? DBNull.Value),
                new MySqlParameter("@BirthAt", birthAt),
                new MySqlParameter("@Place", (object)native.BirthPlace ?? DBNull.Value),
                new MySqlParameter("@Country", (object)native.CountryName ?? DBNull.Value),
                new MySqlParameter("@PlaceId", (object)native.PlaceId ?? DBNull.Value),
                new MySqlParameter("@Latitude", (object)native.Latitude ?? DBNull.Value),
                new MySqlParameter("@Longitude", (object)native.Longitude ?? DBNull.Value),
                new MySqlParameter("@TimeZone", (object)native.TimeZone ?? DBNull.Value),
                new MySqlParameter("@Nick", (object)native.NickName ?? DBNull.Value),
                new MySqlParameter("@Father", (object)native.FatherName ?? DBNull.Value),
                new MySqlParameter("@Address", (object)native.AddressText ?? DBNull.Value),
                new MySqlParameter("@Remarks", (object)native.Remarks ?? DBNull.Value),
                new MySqlParameter("@Temp", native.TempRecord ? 1 : 0),
                new MySqlParameter("@Spheres", (object)native.LifeSpheres ?? DBNull.Value)
            };
        }

        private static NativeRecordModel Map(DataRow row)
        {
            return new NativeRecordModel
            {
                NativeId = Convert.ToInt32(row["native_id"]),
                SerialNo = row["serial_no"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["serial_no"]),
                NativeName = Convert.ToString(row["native_name"]),
                Gender = Convert.ToString(row["gender"]),
                BirthDate = row["birth_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["birth_date"]),
                BirthDateText = Convert.ToString(row["birth_date_text"]),
                BirthTime = ParseTime(row["birth_time"]),
                BirthTimeText = Convert.ToString(row["birth_time_text"]),
                BirthPlace = Convert.ToString(row["birth_place"]),
                CountryName = Convert.ToString(row["country_name"]),
                PlaceId = row["place_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["place_id"]),
                Latitude = row["latitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["latitude"]),
                Longitude = row["longitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["longitude"]),
                TimeZone = row["time_zone"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["time_zone"]),
                NickName = Convert.ToString(row["nick_name"]),
                FatherName = Convert.ToString(row["father_name"]),
                AddressText = Convert.ToString(row["address_text"]),
                Remarks = Convert.ToString(row["remarks"]),
                TempRecord = row["temp_record"] != DBNull.Value && Convert.ToInt32(row["temp_record"]) == 1,
                LifeSpheres = Convert.ToString(row["life_spheres"]),
                CreatedAt = row["created_at"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["created_at"])
            };
        }

        private static TimeSpan? ParseTime(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            TimeSpan time;
            if (TimeSpan.TryParse(Convert.ToString(value), out time)) return time;
            return null;
        }
    }
}
