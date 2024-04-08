using MySqlConnector;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace DBDesignerWIP
{
    public class SqlDb
    {
        private MySqlConnection conn;
        private MySqlCommand cmd;

        public SqlDb(string hostname, string username, string password, string database)
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Server = hostname;
            sb.UserID = username;
            sb.Password = password;
            sb.Database = database;

            conn = new MySqlConnection(sb.ToString());
            conn.Open();
            cmd = conn.CreateCommand();
        }

        public static SqlDb CreateConnection(string hostname, string username, string password)
        {      
            try
            {
                SqlDb ret = new SqlDb(hostname, username, password, "");
                return ret;
            }
            catch
            {
                return null;
            }
        }

        public void Close()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open) conn.Close();
        }

        public List<string> GetDatabases()
        {
            List<string> list = new List<string>();

            cmd.CommandText = "SHOW DATABASES;";

            return GetColumn(Read(), 0);
        }

        public List<string> GetTables(string db)
        {
            List<string> list = new List<string>();

            SetDb(db);
            cmd.CommandText = "SHOW TABLES;";

            return GetColumn(Read(), 0);
        }

        public string GetCreateTable(string database, string table)
        {
            SetDb(database);
            cmd.CommandText = "SHOW CREATE TABLE " + table + ";";


            return ReadStr(1);
        }

        public string GetCreateDatabase(string database)
        {
            cmd.CommandText = "SHOW CREATE DATABASE " + database + ";";


            return ReadStr(1);
        }

        public List<List<string>> GetTable(string database, string table)
        {
            SetDb(database);
            cmd.CommandText = "SELECT * FROM " + table + ";";

            return Read();
        }

        private void SetDb(string name)
        {
            cmd.CommandText = "USE " + name + ";";
            cmd.ExecuteNonQuery();
        }

        private List<List<string>> Read()
        {
            List<List<string>> list = new List<List<string>>();

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader[i].ToString()); 
                    }
                    list.Add(row);
                }
            }

            reader.Close();
            return list;
        }

        private string ReadStr(int ord)
        {

            MySqlDataReader reader = cmd.ExecuteReader();
            string s = "";
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    s = reader.GetString(ord);
                }
                

            }

            reader.Close();
            return s;
        }

        private List<string> GetRow(List<List<string>> table, int row)
        {
            List<string> list = new List<string>();
            if (list.Count > row)
            {
                foreach (string s in table[row]) list.Add(s);
            }

            return list;
        }

        private List<string> GetColumn(List<List<string>> table, int column)
        {
            List<string> list = new List<string>();

            foreach(List<string> row in table)
            {
                if (row.Count > column) list.Add(row[column]);
            }

            return list;
        }

        ~SqlDb()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open) conn.Close();
        }
    }
}
