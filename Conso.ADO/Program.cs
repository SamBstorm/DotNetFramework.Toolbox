using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conso.ADO
{
    class Program
    {
        private const string CONSTRING = @"Data Source=desktop-lgurcco;Initial Catalog=MoviesDB;Integrated Security=True";

        static void Main(string[] args)
        {
            int result;

            using(SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = CONSTRING;
                connection.Open();
                if (connection.State != System.Data.ConnectionState.Open) throw new Exception("Impossible d'ouvrir la connection");
                string sqlQuery = "SELECT COUNT(*) FROM Movie";
                using(SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlQuery;
                    command.CommandType = System.Data.CommandType.Text;
                    result = (int)command.ExecuteScalar();
                }
                //connection.Close();
            }

            Console.WriteLine(result);
            Console.ReadLine();

            using (SqlConnection connection = new SqlConnection()) 
            {
                connection.ConnectionString = CONSTRING;
                connection.Open();
                if (connection.State != System.Data.ConnectionState.Open) throw new Exception("Impossible d'ouvrir la connection");
                string sqlQuery = "UPDATE Movie SET title = @title WHERE id = @id";
                SqlParameter paramTitle = new SqlParameter();
                paramTitle.ParameterName = "title";
                paramTitle.Value = "Le monde de Némo";
                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "id";
                paramId.Value = 1;
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlQuery;
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(paramTitle);
                    command.Parameters.Add(paramId);
                    Console.WriteLine( command.ExecuteNonQuery() );
                }
            }

            Console.ReadLine();

            ArrayList arrayList = new ArrayList();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = CONSTRING;
                connection.Open();
                if (connection.State != System.Data.ConnectionState.Open) throw new Exception("Impossible d'ouvrir la connection");
                string sqlQuery = "SELECT * FROM Movie";
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlQuery;
                    command.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var movie = new
                            {
                                Id = (int)reader["id"],
                                Title = (string)reader["title"],
                                Synopsis = (reader["synopsis"] == DBNull.Value)?null:(string)reader["synopsis"],
                                ReleaseYear = (reader["release_year"] == DBNull.Value)?null:(Nullable<int>)reader["release_year"],
                                PosterURI = (reader["poster_uri"] == DBNull.Value)?null:(string)reader["poster_uri"],
                                CategoryId = (reader["category_id"] == DBNull.Value)?null:(int?)reader["category_id"]
                            };

                            arrayList.Add(movie);
                        }
                    }
                }

            }

            Console.ReadLine();

            DataTable movies = new DataTable();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = CONSTRING;
                connection.Open();
                if (connection.State != System.Data.ConnectionState.Open) throw new Exception("Impossible d'ouvrir la connection");
                string sqlQuery = "SELECT * FROM Movie";
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlQuery;
                    command.CommandType = System.Data.CommandType.Text;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.Fill(movies);
                }
            }

            foreach (DataRow row in movies.Rows)
            {
                Console.WriteLine($"{row["id"]} : {row["title"]}");
            }

            Console.ReadLine();
        }
    }
}
