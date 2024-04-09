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

        public static bool CreateTable(string name, bool isTemporary, string engine, string charset, string collate, int auto_increment, string comment, out string errorMessage)
        {
            if (!Check.CheckTableName(name, out errorMessage))
            {
                return false;
            }
            else
            {
                Table t = new Table(name, isTemporary, engine, charset, collate, auto_increment.ToString(), comment, DataStore.activeDatabase);
                t.CreateDefaultColumn();
                DataStore.activeDatabase.tables.Add(t);

                DataStore.batch.Add(t.GetStatement());

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

        public static bool DropTable(int n, out string name, out string errorMessage)
        {
            Table t = DataStore.activeDatabase.GetNthTable(n);
            List<ConstraintFK> constraints = DataStore.activeDatabase.GetTableFKReference(t);
            name = t.name;
            if (constraints.Count == 0)
            {
                errorMessage = "";
                DataStore.batch.Add(t.GetDropStatement());
                DataStore.activeDatabase.tables.Remove(t);
                return true;
            }
            else
            {
                errorMessage = "Can't drop table " + t.name + ". It's referenced by:\n";
                foreach (ConstraintFK constraintFK in constraints)
                {
                    errorMessage = errorMessage + constraintFK.name + " of table " + constraintFK.parent.name + "\n";
                }
                return false;
            }

        }
    }
}
