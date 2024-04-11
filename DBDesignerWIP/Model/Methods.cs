using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using System.Drawing;

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

        public static bool CreateTextColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, int size, string charset, string collate, out string errorMessage)
        {
            if (!Check.CheckTextColumn(name, type, nullAllowed, defaultValue, comment, size, charset, collate, out errorMessage))
            {
                return false;
            }
            else
            {
                string? defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
                bool defaultValueSupported = !(defaultValue == "");
                TextColumn tc = new TextColumn(name, nullAllowed, type, defaultValueSupported, defa, comment, DataStore.activeTable, size, charset, charset + collate);
                DataStore.activeTable.columns.Add(tc);
                DataStore.batch.Add(tc.GetAddColumnStatement());

                errorMessage = "";
                return true;
            }
        }

        public static bool CreateIntegerColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, int size, bool unsigned, bool zerofill, bool autoIncrement, out string errorMessage)
        {
            if(!Check.CheckIntegerColumn(name, type, nullAllowed, defaultValue, comment, size, unsigned, zerofill, autoIncrement, out errorMessage))
            {
                return false;
            }
            else
            {
                string? defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
                bool defaultValueSupported = !(defaultValue == "");
                IntegerColumn ic = new IntegerColumn(name, nullAllowed, type, defaultValueSupported, defa, comment, DataStore.activeTable, size, unsigned, zerofill, autoIncrement);
                DataStore.activeTable.columns.Add(ic);
                DataStore.batch.Add(ic.GetAddColumnStatement());

                errorMessage = "";
                return true;
            }
        }

        public static bool CreateDecimalColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, int size, int d, out string errorMessage)
        {
            if (!Check.CheckDecimalColumn(name, type, nullAllowed, defaultValue, comment, size, d, out errorMessage))
            {
                return false;
            }
            else
            {
                string? defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
                bool defaultValueSupported = !(defaultValue == "");
                DecimalColumn dc = new DecimalColumn(name, nullAllowed, type, defaultValueSupported, defa, comment, DataStore.activeTable, size, d);
                DataStore.activeTable.columns.Add(dc);
                DataStore.batch.Add(dc.GetAddColumnStatement());

                errorMessage = "";
                return true;
            }
        }

        public static bool CreateEnumColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, string options, out string errorMessage)
        {
            if(!Check.CheckEnumColumn(name, type, nullAllowed, defaultValue, comment, options, out errorMessage))
            {
                return false;
            }
            else
            {
                List<string> opt = options.Trim().Split(",").ToList(); for (int i = 0; i < opt.Count; i++) { opt[i] = opt[i].Trim(); }
                string? defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
                bool defaultValueSupported = !(defaultValue == "");
                EnumColumn ec = new EnumColumn(name, nullAllowed, type, defaultValueSupported, defa, comment, DataStore.activeTable, opt);
                DataStore.activeTable.columns.Add(ec);
                DataStore.batch.Add(ec.GetAddColumnStatement());

                errorMessage = "";
                return true;
            }
        }

        public static bool CreateBinaryColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, int size, out string errorMessage)
        {
            if (!Check.CheckBinaryColumn(name, type, nullAllowed, defaultValue, comment, size, out errorMessage))
            {
                return false;
            }
            else
            {
                string? defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
                bool defaultValueSupported = !(defaultValue == "");
                BinaryColumn bc = new BinaryColumn(name, nullAllowed, type, defaultValueSupported, defa, comment, DataStore.activeTable, size);
                DataStore.activeTable.columns.Add(bc);
                DataStore.batch.Add(bc.GetAddColumnStatement());

                errorMessage = "";
                return true;
            }
        }

        public static bool CreateDateTimeColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, out string errorMessage)
        {
            if (!Check.CheckDateTimeColumn(name, type, nullAllowed, defaultValue, comment, out errorMessage))
            {
                return false;
            }
            else
            {
                string? defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
                bool defaultValueSupported = !(defaultValue == "");
                DateTimeColumn dt = new DateTimeColumn(name, nullAllowed, type, defaultValueSupported, defa, comment, DataStore.activeTable, "");
                DataStore.activeTable.columns.Add(dt);
                DataStore.batch.Add(dt.GetAddColumnStatement());

                errorMessage = "";
                return true;
            }
        }

    }
}
