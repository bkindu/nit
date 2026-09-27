using System.Data;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;
using MySql.Data.MySqlClient;
using System;

namespace NirmalaAstrology_Web.Repositories
{
    /// <summary>
    /// Repository for Customer-related database operations.
    /// Uses ADO.NET with parameterized queries only.
    /// </summary>
    public class CustomerRepository
    {
        /// <summary>
        /// Saves a new customer / Kundli request.
        /// Returns the newly created CustomerId.
        /// </summary>
        public int SaveCustomer(CustomerModel customer)
        {
            string query = @"INSERT INTO Customer (Name, DOB, BirthTime, PlaceId, Latitude, Longitude, Timezone, CreatedDate)
                            VALUES (@Name, @DOB, @BirthTime, @PlaceId, @Latitude, @Longitude, @Timezone, NOW())";

            using (DatabaseHelper db = new DatabaseHelper())
            {
                MySqlParameter[] parameters = {
                    new MySqlParameter("@Name", customer.Name),
                    new MySqlParameter("@DOB", customer.DOB),
                    new MySqlParameter("@BirthTime", customer.BirthTime.ToString(@"hh\:mm")),
                    new MySqlParameter("@PlaceId", customer.PlaceId),
                    new MySqlParameter("@Latitude", customer.Latitude),
                    new MySqlParameter("@Longitude", customer.Longitude),
                    new MySqlParameter("@Timezone", customer.Timezone ?? string.Empty)
                };
                return db.ExecuteScalarWithIdentity(query, parameters);
            }
        }

        /// <summary>
        /// Get a customer by their ID.
        /// </summary>
        public CustomerModel GetCustomer(int customerId)
        {
            string query = @"SELECT c.CustomerId, c.Name, c.DOB, c.BirthTime, c.PlaceId,
                                    c.Latitude, c.Longitude, c.Timezone, c.CreatedDate,
                                    p.PlaceName
                             FROM Customer c
                             LEFT JOIN Place p ON c.PlaceId = p.PlaceId
                             WHERE c.CustomerId = @CustomerId";

            using (DatabaseHelper db = new DatabaseHelper())
            {
                MySqlParameter[] parameters = {
                    new MySqlParameter("@CustomerId", customerId)
                };
                DataTable dt = db.ExecuteDataTable(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new CustomerModel
                    {
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        Name = row["Name"].ToString(),
                        DOB = Convert.ToDateTime(row["DOB"]),
                        BirthTime = TimeSpan.Parse(row["BirthTime"].ToString()),
                        PlaceId = Convert.ToInt32(row["PlaceId"]),
                        Latitude = Convert.ToDecimal(row["Latitude"]),
                        Longitude = Convert.ToDecimal(row["Longitude"]),
                        Timezone = row["Timezone"].ToString(),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        PlaceName = row["PlaceName"].ToString()
                    };
                }
            }
            return null;
        }
    }
}