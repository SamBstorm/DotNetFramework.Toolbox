using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.ADO
{
    public class Connection
    {
        private string _connectionString;

        public delegate T ConvertMethod<T>(SqlDataReader reader); // Func<SqlDataReader,T>

        public Connection(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection CreateConnection()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();
            if (connection.State != System.Data.ConnectionState.Open) throw new Exception("Impossible d'ouvrir la connection.");
            return connection;
        }

        private SqlCommand CreateCommand(SqlConnection connection, Command command)
        {
            SqlCommand dbCommand = new SqlCommand();
            dbCommand.Connection = connection;
            dbCommand.CommandText = command.SqlQuery;
            dbCommand.CommandType = (command.IsStoredProcedure) ? System.Data.CommandType.StoredProcedure : System.Data.CommandType.Text;
            foreach (SqlParameter parameter in command.Parameters.Values)
            {
                dbCommand.Parameters.Add(parameter);
            }
            return dbCommand;
        }

        public int ExecuteNonQuery(Command command) {
            using(SqlConnection connection = CreateConnection())
            {
                using (SqlCommand dbCommand = CreateCommand(connection, command))
                {                    
                    return dbCommand.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(Command command) {
            using(SqlConnection connection = CreateConnection())
            {
                using(SqlCommand dbCommand = CreateCommand(connection, command))
                {
                    return dbCommand.ExecuteScalar();
                }
            }
        }
        public DataTable GetDataTable(Command command)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = CreateConnection())
            {
                using (SqlCommand dbCommand = CreateCommand(connection, command))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = dbCommand;
                    adapter.Fill(table);
                }
            }
            return table;
        }

        
        public IEnumerable<T> ExecuteReader<T>(Command command, ConvertMethod<T> convert) //ConvertMethod<T> est le même type que le délégué générique de Func<SqlDataReader,T>
        {
            //List<T> list = new List<T>();
            using (SqlConnection connection = CreateConnection())
            {
                using (SqlCommand dbCommand = CreateCommand(connection,command))
                {
                    using (SqlDataReader reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return convert(reader);
                            //list.Add(item);
                        }
                    }
                }
            }
            //return list;
        }
    }
}
