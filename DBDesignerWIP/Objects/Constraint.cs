
namespace DBDesignerWIP
{
    public class Constraint
    {
        public Table parent { get; set; }

        public Constraint (Table parent)
        {

        }

        public Constraint (Table parent, string cmd)
        {

        }

        public virtual string GetStatement()
        {
            return "";
        }

        public virtual string GetDropStatement()
        {
            return "";
        }

        public static Constraint CreateConstraint(Table parent, string cmd) 
        {
            string type = Text.GetConstraintType(cmd);
            if (type == "UNIQUE")
            {
                return new ConstraintUQ(parent, cmd);
            }
            if (type == "PRIMARY")
            {
                return new ConstraintPK(parent, cmd);
            }
            if (type == "CONSTRAINT")
            {
                return new ConstraintFK(parent, cmd);
            }
            if (type == "KEY")
            {
                return new ConstraintK(parent, cmd);
            }
            return null;
        }
    }

    public class ConstraintPK : Constraint
    {
        public List<Column> localColumns { get; set; } = new List<Column>();

        public ConstraintPK(Table parent, List<Column> localColumns) : base (parent)
        {
            this.localColumns = localColumns;
            this.parent = parent;
        }

        public ConstraintPK(Table parent, string cmd) : base (parent, cmd)
        {
            this.parent = parent;
            List<string> colNames = Text.GetUnwrappedBracket(cmd, 0);
            foreach (string colName in colNames)
            {
                localColumns.Add(parent.GetColumnByName(colName));
            }
        }

        public override string GetStatement()
        {
            string result = "PRIMARY KEY (";
            foreach (Column c in localColumns)
            {
                result = result + "`" + c.name+ "`,";
            }
            result = result.Substring(0, result.Length - 1);
            result = result + ")";
            return result;
        }

        public override string GetDropStatement()
        {
            return "ALTER TABLE `" + parent.name + "` DROP PRIMARY KEY";
        }
    }

    public class ConstraintFK : Constraint
    {
        public string name { get; set; } = "";
        public List<Column> localColumns { get; set; } = new List<Column>();
        public List<Column> remoteColumns { get; set; } = new List<Column>();
        public Table remoteTable { get; set; }
        public string onUpdate { get; set; } = "";
        public string onDelete { get; set; } = "";

        public ConstraintFK(Table parent, string name, List<Column> localColumns, List<Column> remoteColumns, Table remoteTable, string onUpdate, string onDelete) : base (parent)
        {
            this.parent = parent;
            this.name = name;
            this.localColumns = localColumns;
            this.remoteColumns = remoteColumns;
            this.remoteTable = remoteTable;
            this.onUpdate = onUpdate;
            this.onDelete = onDelete;
        }
        public ConstraintFK(Table parent, string cmd) : base(parent, cmd)
        {
            this.name = StatementText.GetFKName(cmd);
            this.remoteTable = parent.parent.GetTableByName(StatementText.GetFKRemoteTable(cmd));
            this.onUpdate = StatementText.GetOnUpdate(cmd);
            this.onDelete = StatementText.GetOnDelete(cmd);
            List<string> colNamesLocal = Text.GetUnwrappedBracket(cmd, 0);
            foreach (string colName in colNamesLocal)
            {
                localColumns.Add(parent.GetColumnByName(colName));
            }
            List<string> colNamesRemote = Text.GetUnwrappedBracket(cmd, 1);
            foreach (string colName in colNamesRemote)
            {
                remoteColumns.Add(remoteTable.GetColumnByName(colName));
            }
            this.parent = parent;
        }

        public override string GetStatement()
        {
            string result = "CONSTRAINT `" + name + "` FOREIGN KEY(";
            foreach (Column c in localColumns)
            {
                result = result + "`" + c.name + "`,";
            }
            result = result.Substring(0, result.Length - 1);
            result = result + ") REFERENCES `" + remoteTable.name + "` (";
            foreach (Column c in remoteColumns)
            {
                result = result + "`" + c.name + "`,";
            }
            result = result.Substring(0, result.Length - 1);
            result = result + ")";
            return result;
        }

        public override string GetDropStatement()
        {
            return "ALTER TABLE `" + parent.name + "` DROP CONSTRAINT `" + this.name + "`;";
        }
    }

    public class ConstraintUQ : Constraint
    {
        public string name { get; set; }
        public List<Column> localColumns { get; set; } = new List<Column>();
        public ConstraintUQ(Table parent, string name, List<Column> localColumns) : base(parent)
        {
            this.name = name;
            this.localColumns = localColumns;
            this.parent = parent;
        }

        public ConstraintUQ(Table parent, string cmd) : base(parent, cmd)
        {
            this.name = StatementText.GetUQName(cmd);
            List<string> colNamesLocal = Text.GetUnwrappedBracket(cmd, 0);
            foreach (string colName in colNamesLocal)
            {
                localColumns.Add(parent.GetColumnByName(colName));
            }
            this.parent = parent;
        }

        public override string GetStatement()
        {
            string result = "UNIQUE KEY `" + name + "` (";
            foreach (Column c in localColumns)
            {
                result = result + "`" + c.name+ "`,";
            }
            result = result.Substring(0, result.Length - 1);
            result = result + ")";
            return result;
        }
        public override string GetDropStatement()
        {
            return "ALTER TABLE `" + parent.name + "` DROP CONSTRAINT `" + this.name + "`;";
        }
    }

    public class ConstraintK : Constraint
    {
        public string name { get; set; } = "";
        public List<Column> localColumns { get; set; } = new List<Column>();

        public ConstraintK(Table parent, string name, List<Column> localColumns) : base(parent)
        {
            this.localColumns = localColumns;
            this.parent = parent;
            this.name = name;
        }

        public ConstraintK(Table parent, string cmd) : base(parent, cmd)
        {
            this.parent = parent;
            this.name = StatementText.GetFKName(cmd);
            List<string> colNames = Text.GetUnwrappedBracket(cmd, 0);
            foreach (string colName in colNames)
            {
                localColumns.Add(parent.GetColumnByName(colName));
            }
        }

        public override string GetStatement()
        {
            string result = "KEY `" + name + "` (";
            foreach (Column c in localColumns)
            {
                result = result + "`" + c.name + "`,";
            }
            result = result.Substring(0, result.Length - 1);
            result = result + ")";
            return result;
        }

        public override string GetDropStatement()
        {
            return "ALTER TABLE `" + parent.name + "` DROP CONSTRAINT `" + this.name + "`;";
        }
    }

    
}
