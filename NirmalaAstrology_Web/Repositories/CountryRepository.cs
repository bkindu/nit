using System;
using System.Collections.Generic;
using System.Data;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Repositories
{
    /// <summary>
    /// Repository for Country-related database operations.
    /// Uses ADO.NET with MySQL stored procedures.
    /// </summary>
    public class CountryRepository
    {
        /// <summary>
        /// Gets all countries ordered by CountryName for dropdown binding.
        /// </summary>
        public List<CountryModel> GetAllCountries()
        {
            List<CountryModel> countries = new List<CountryModel>();

            using (DatabaseHelper db = new DatabaseHelper())
            {
                DataTable dt = db.ExecuteDataTableSP("sp_GetAllCountries");
                foreach (DataRow row in dt.Rows)
                {
                    countries.Add(new CountryModel
                    {
                        CountryID = Convert.ToInt32(row["CountryID"]),
                        CountryName = row["CountryName"].ToString()
                    });
                }
            }
            return countries;
        }
    }
}