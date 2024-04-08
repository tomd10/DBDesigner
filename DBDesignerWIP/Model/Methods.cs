using Microsoft.AspNetCore.Http.HttpResults;

namespace DBDesignerWIP
{
    public static class Methods
    {
        public static void LoadDatabases()
        {
            foreach (string database in DataStore.dbConnection.GetDatabases())
            {
                Database db = new Database(DataStore.dbConnection.GetCreateDatabase(database));
                DataStore.databases.Add(db);

                List<string> tables = DataStore.dbConnection.GetTables(database);
                foreach (string table in tables)
                {
                    db.tables.Add(new Table(db, DataStore.dbConnection.GetCreateTable(database, table)));
                }
            }

            foreach (Database db in DataStore.databases)
            {
                db.LoadConstraints();
            }
        }

        public static Database GetNthDatabase(int n)
        {
            if (n < 0 || n >= DataStore.databases.Count)
            {
                return null;
            }
            else return DataStore.databases[n];
        }

        public static bool CreateDatabase(string name, string charset, string collate, out string errorMessage)
        {
            if (!Check.CheckDatabaseName(name, out errorMessage))
            {
                return false;
            }
            else
            {
                Database db = new Database(name, charset, charset + collate, new List<Table>());
                DataStore.databases.Add(db);

                DataStore.batch.Add(db.GetStatement());

                errorMessage = "";
                return true;
            }
        }

        public static void DropDatabase (int n, out string name)
        {
            Database db = GetNthDatabase(n);
            name = db.name;
            DataStore.batch.Add(db.GetDropStatement());

            if (DataStore.activeTable != null && DataStore.activeTable.parent == db) DataStore.activeTable = null;
            if (DataStore.activeDatabase == db) DataStore.activeDatabase = null;

            DataStore.databases.Remove(db);
        }
    }
}
