using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Hosting;
using System.Xml;
using MySql.Data.MySqlClient;

namespace NirmalaAstrology_Web.DAL
{
    /// <summary>
    /// ADO.NET Database Helper for MySQL.
    /// Provides reusable methods for all database operations using parameterized queries.
    /// All methods use using blocks for proper resource disposal.
    /// </summary>
    public class DatabaseHelper : IDisposable
    {
        private MySqlConnection _connection;
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            _connectionString = ResolveConnectionString();
            _connection = new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// Prefers private App_Data/connection.local.config (gitignored, not for public repos).
        /// Falls back to Web.config connectionStrings.
        /// On BigRock the site runs on the same machine as MySQL — use Server=localhost there.
        /// </summary>
        private static string ResolveConnectionString()
        {
            try
            {
                var path = HostingEnvironment.MapPath("~/App_Data/connection.local.config");
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    var doc = new XmlDocument();
                    doc.Load(path);
                    var node = doc.SelectSingleNode("//add[@name='NirmalaAstrologyDB']/@connectionString");
                    if (node != null && !string.IsNullOrWhiteSpace(node.Value))
                        return node.Value.Trim();
                }
            }
            catch
            {
                // Fall back to Web.config
            }

            var cs = ConfigurationManager.ConnectionStrings["NirmalaAstrologyDB"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                throw new ConfigurationErrorsException("Missing connection string NirmalaAstrologyDB. Add App_Data/connection.local.config or set Web.config.");
            return cs.ConnectionString;
        }

        /// <summary>
        /// Opens the MySQL database connection.
        /// </summary>
        public void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        /// <summary>
        /// Closes the MySQL database connection.
        /// </summary>
        public void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Returns the underlying MySqlConnection for parameter binding.
        /// </summary>
        public MySqlConnection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// Executes a SQL query and returns a MySqlDataReader.
        /// Caller must close the reader and connection when done.
        /// </summary>
        /// <param name="query">Parameterized SQL query</param>
        /// <param name="parameters">Array of MySqlParameter objects</param>
        /// <returns>MySqlDataReader</returns>
        public IDataReader ExecuteReader(string query, params MySqlParameter[] parameters)
        {
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, _connection);
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Executes a SQL query and returns a DataTable.
        /// </summary>
        /// <param name="query">Parameterized SQL query</param>
        /// <param name="parameters">Array of MySqlParameter objects</param>
        /// <returns>DataTable with results</returns>
        public DataTable ExecuteDataTable(string query, params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
            {
                cmd.CommandType = CommandType.Text;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    OpenConnection();
                    adapter.Fill(dt);
                    CloseConnection();
                    return dt;
                }
            }
        }

        /// <summary>
        /// Executes a stored procedure and returns a DataTable.
        /// </summary>
        /// <param name="spName">Stored procedure name</param>
        /// <param name="parameters">Array of MySqlParameter objects</param>
        /// <returns>DataTable with results</returns>
        public DataTable ExecuteDataTableSP(string spName, params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = new MySqlCommand(spName, _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    OpenConnection();
                    adapter.Fill(dt);
                    CloseConnection();
                    return dt;
                }
            }
        }

        /// <summary>
        /// Executes a SQL query and returns a DataSet.
        /// </summary>
        /// <param name="query">Parameterized SQL query</param>
        /// <param name="parameters">Array of MySqlParameter objects</param>
        /// <returns>DataSet with results</returns>
        public DataSet ExecuteDataSet(string query, params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    OpenConnection();
                    adapter.Fill(ds);
                    CloseConnection();
                    return ds;
                }
            }
        }

        /// <summary>
        /// Executes a scalar query and returns the first column of the first row.
        /// </summary>
        /// <param name="query">Parameterized SQL query</param>
        /// <param name="parameters">Array of MySqlParameter objects</param>
        /// <returns>Object (first column, first row) or null</returns>
        public object ExecuteScalar(string query, params MySqlParameter[] parameters)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Executes a non-query SQL command (INSERT, UPDATE, DELETE).
        /// Returns the number of rows affected.
        /// </summary>
        /// <param name="query">Parameterized SQL query</param>
        /// <param name="parameters">Array of MySqlParameter objects</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Executes a stored procedure for INSERT/UPDATE/DELETE.
        /// Returns the number of rows affected.
        /// </summary>
        /// <param name="spName">Stored procedure name</param>
        /// <param name="parameters">Array of MySqlParameter objects</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuerySP(string spName, params MySqlParameter[] parameters)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand(spName, _connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Executes an INSERT query and returns the last auto-increment ID.
        /// </summary>
        /// <param name="query">Parameterized INSERT query</param>
        /// <param name="parameters">Array of MySqlParameter objects</param>
        /// <returns>Last inserted ID</returns>
        public int ExecuteScalarWithIdentity(string query, params MySqlParameter[] parameters)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand(query + "; SELECT LAST_INSERT_ID();", _connection))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Executes a stored procedure (with SELECT LAST_INSERT_ID() inside the SP)
        /// and returns the scalar value (e.g., newly inserted ID).
        /// </summary>
        /// <param name="spName">Stored procedure name</param>
        /// <param name="parameters">Array of MySqlParameter objects</param>
        /// <returns>Scalar result from the SP</returns>
        public int ExecuteScalarSP(string spName, params MySqlParameter[] parameters)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand(spName, _connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}