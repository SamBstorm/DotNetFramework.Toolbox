using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.ADO
{
    public class Connection
    {
        private string _connectionString;

        private DbProviderFactory _factory;

        public delegate T ConvertMethod<T>(IDataRecord reader); // Func<IDataRecord,T>

        public Connection(string connectionString, string invariantName="System.Data.SqlClient")
        {
            _connectionString = connectionString;
            _factory = DbProviderFactories.GetFactory(invariantName);
        }

        private DbConnection CreateConnection()
        {
            DbConnection connection = _factory.CreateConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();
            if (connection.State != System.Data.ConnectionState.Open) throw new Exception("Impossible d'ouvrir la connection.");
            return connection;
        }

        private DbCommand CreateCommand(DbConnection connection, Command command)
        {
            DbCommand dbCommand = connection.CreateCommand();
            dbCommand.CommandText = command.SqlQuery;
            dbCommand.CommandType = (command.IsStoredProcedure) ? System.Data.CommandType.StoredProcedure : System.Data.CommandType.Text;
            foreach (Parameter parameter in command.Parameters.Values)
            {
                DbParameter dbParameter = _factory.CreateParameter();
                dbParameter.ParameterName = parameter.ParameterName;
                dbParameter.Value = parameter.Value;
                dbParameter.Direction = parameter.Direction;
                dbParameter.Size = parameter.Size;
                dbCommand.Parameters.Add(dbParameter);
            }
            return dbCommand;
        }

        public int ExecuteNonQuery(Command command) {
            int result;
            using(DbConnection connection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(connection, command))
                {                    
                    result = dbCommand.ExecuteNonQuery();
                    GetOutputParameterValue(dbCommand, command);
                }
            }
            return result;
        }

        public object ExecuteScalar(Command command) {
            object result;
            using(DbConnection connection = CreateConnection())
            {
                using(DbCommand dbCommand = CreateCommand(connection, command))
                {
                    result = dbCommand.ExecuteScalar();
                    GetOutputParameterValue(dbCommand, command); 
                }
            }
            return result;
        }
        public DataTable GetDataTable(Command command)
        {
            DataTable table = new DataTable();
            using (DbConnection connection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(connection, command))
                {
                    DbDataAdapter adapter = _factory.CreateDataAdapter();
                    adapter.SelectCommand = dbCommand;
                    adapter.Fill(table);
                    GetOutputParameterValue(dbCommand, command);
                }
            }
            return table;
        }

        
        public IEnumerable<T> ExecuteReader<T>(Command command, ConvertMethod<T> convert) //ConvertMethod<T> est le même type que le délégué générique de Func<IDataRecord,T>
        {
            //List<T> list = new List<T>();
            using (DbConnection connection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(connection,command))
                {
                    using (DbDataReader reader = dbCommand.ExecuteReader())
                    {
                        GetOutputParameterValue(dbCommand, command);
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

        private void GetOutputParameterValue(DbCommand dbCommand, Command command)
        {
            foreach (DbParameter p in dbCommand.Parameters)
            {
                if(p.Direction == ParameterDirection.Output)
                {
                    command.Parameters[p.ParameterName].Value = p.Value;
                }
            }
        }
    }
}
