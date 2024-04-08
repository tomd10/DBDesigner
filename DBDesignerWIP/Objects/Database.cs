

namespace DBDesignerWIP
{
    public class Database
    {
        public string name { get; set; } = "";
        public string charset { get; set; } = "";
        public string collate { get; set; } = "";
        public List<Table> tables { get; set; } = new List<Table>();

        public Database (string name, string charset, string collate, List<Table> tables)
        {
            this.name = name;
            this.charset = charset;
            this.collate = collate;
            this.tables = tables;
        }

        public Database(string cmd)
        {
            this.name = StatementText.GetDatabaseName(cmd);
            this.collate = StatementText.GetCollate(cmd);
            this.charset = StatementText.GetCharset(cmd);
        }

        public string GetStatement()
        {
            string result = "CREATE DATABASE IF NOT EXISTS `" + name + "`";
            result = result + " CHARACTER SET " + charset;
            result = result + " COLLATE " + collate + ";";

            return result;
        }

        public string GetDropStatement()
        {
            return "DROP DATABASE IF EXISTS `" + name + "`;";
        }

        public void LoadConstraints()
        {
            foreach (Table table in tables)
            {
                table.LoadConstraints();
            }
        }

        public Table GetTableByName(string s)
        {
            foreach (Table t in tables)
            {
                if (t.name == s) return t;
            }
            return null;
        }

        public bool GetTableNameAvailable(string s)
        {
            foreach (Table t in tables)
            {
                if (t.name.ToLower() == s.ToLower()) return false;
            }
            return true;
        }

        public List<ConstraintFK> GetColumnFKReference(Column col)
        {
            List<ConstraintFK> list = new List<ConstraintFK>();
            foreach (Table t in tables)
            {
                foreach (Constraint c in t.constraints)
                {
                    if (c is ConstraintFK)
                    {
                        ConstraintFK potential = (ConstraintFK)c;
                        if (potential.remoteTable == col.parent)
                        {
                            if (potential.remoteColumns.Contains(col))
                            {
                                list.Add(potential);
                            }
                        }
                    }
                }
            }

            return list;
        }

        public List<ConstraintFK> GetTableFKReference(Table tab)
        {
            List<ConstraintFK> list = new List<ConstraintFK>();
            foreach (Table t in tables)
            {
                foreach (Constraint c in t.constraints)
                {
                    if (c is ConstraintFK)
                    {
                        ConstraintFK potential = (ConstraintFK)c;
                        if (potential.remoteTable == tab)
                        {
                            list.Add(potential);
                        }
                    }
                }
            }

            return list;
        } 
    }
}
