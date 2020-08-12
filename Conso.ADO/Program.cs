using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolBox.ADO;

namespace Conso.ADO
{
    class Program
    {
        private const string CONSTRING = @"Data Source=desktop-lgurcco;Initial Catalog=MoviesDB;Integrated Security=True";

        static void Main(string[] args)
        {
            int result;

            Command com = new Command("SELECT COUNT(*) FROM Movie");
            Connection con = new Connection(CONSTRING,"System.Data.SqlClient");
            /*result = (int)con.ExecuteScalar(com);

            Console.WriteLine(result);
            Console.ReadLine();

            com = new Command("UPDATE Movie SET title = @title WHERE id = @id");
            com.AddParameter("title", "Le monde de Nemo");
            com.AddParameter("id", 1);
            Console.WriteLine(con.ExecuteNonQuery(com));

            Console.ReadLine();

            ArrayList arrayList = new ArrayList();

            Connection.ConvertMethod<object> convert = (reader) => new
            {
                Id = (int)reader["id"],
                Title = (string)reader["title"],
                Synopsis = (reader["synopsis"] == DBNull.Value) ? null : (string)reader["synopsis"],
                ReleaseYear = (reader["release_year"] == DBNull.Value) ? null : (Nullable<int>)reader["release_year"],
                PosterURI = (reader["poster_uri"] == DBNull.Value) ? null : (string)reader["poster_uri"],
                CategoryId = (reader["category_id"] == DBNull.Value) ? null : (int?)reader["category_id"]
            };

            com = new Command("SELECT * FROM Movie");
            var listOfMovies = con.ExecuteReader(com, convert);
            foreach (object item in listOfMovies)
            {
                arrayList.Add(item);
            }

            Console.ReadLine();

            DataTable movies = con.GetDataTable(com);

            foreach (DataRow row in movies.Rows)
            {
                Console.WriteLine($"{row["id"]} : {row["title"]}");
            }

            Console.ReadLine();
            */int id=default;
            string title=default;

            com = new Command("SELECT @id = id , @film = title FROM Movie");

            com.AddParameter("id", id, true);
            com.AddParameter("film", title, true,256);
            IEnumerable<string> titles = con.ExecuteReader(com, reader=>reader["title"].ToString());
            id = (int)com.Parameters["id"].Value;
            title = (string)com.Parameters["film"].Value;

            Console.WriteLine($"{id} - {title}");

            foreach (string t in titles)
            {
                Console.WriteLine(t);
            }
            Console.ReadLine();
        }
    }
}