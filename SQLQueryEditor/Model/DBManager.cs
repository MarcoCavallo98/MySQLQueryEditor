using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SQLQueryEditor
{
    class DBManager
    {
        //Connection to DB
        private MySqlConnection Connection;

        //DB info
        private string DbName { get; set; }
        private string Server { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }


        //Var for a singleton istance
        private static DBManager manager;

        private DBManager() 
        { 
            
        }

        //Get singleton istance
        public static DBManager GetIstance() 
        {
            if (manager == null) { 
                manager = new DBManager();
}
            return manager;
        }

        /*This method opens a connection with the DB.
            return values:
                true = connection extablished and no exception was thrown
                false = an exception occurred
         */
        public bool Connect(string DbName, string Server, string Username, string Password) 
        {
            //Saving DB info
            this.DbName = DbName;
            this.Server = Server;
            this.Username = Username;
            this.Password = Password;

            //Create connection string
            string connection_string = $"Server={Server}; database={DbName}; UID={Username}; password={Password}";
            

            //Open connection
            Connection = new MySqlConnection(connection_string);
            try
            {
                Connection.Open();
                return true;
            }
            catch(Exception e)
            {
                if (e is InvalidOperationException || e is MySqlException)
                    return false;

                throw e;
            }
        }

        //Method for closing connection
        public void CloseConnection()
        {
            if (Connection != null && !Connection.State.ToString().Equals("Closed"))
                Connection.Clone();
        }

       
        //This method executes the query (parameter) on the db. It returns a Tuple with columns list and rows list
        public Tuple<List<string>, List<string[]>> ExecuteQuery(string query) 
        {
            if (Connection == null)
                throw new NullReferenceException("No connection opened");

            if (query == null || query == "")
                throw new ArgumentNullException();

            List<string[]> res = new List<string[]>();
            MySqlCommand command = Connection.CreateCommand();
            command.Connection = Connection;
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            //Get columns name
            var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            
            //Read data
            while (reader.Read())
            {
                string[] to_add = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                    to_add[i] = reader.GetValue(i).ToString();

                res.Add(to_add);
            }

            //THIS IS IMPORTANT!
            reader.Close();

            return Tuple.Create(columns, res);
        }
    }
}
