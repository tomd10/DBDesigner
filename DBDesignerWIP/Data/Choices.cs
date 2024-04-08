﻿using DBDesignerWIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDesignerWIP
{
    public static class Choices
    {
        public static readonly List<string> charsets = new List<string>() { "utf8mb4", "utf8mb3", "latin2" };
        public static readonly List<string> collates = new List<string>() { "_general_ci", "_general_cs" };
        public static readonly List<string> integerColumn = new List<string>() { "INT", "TINYINT", "SMALLINT", "MEDIUMINT",  "BIGINT" };
        public static readonly List<string> enumColumn = new List<string>() { "ENUM", "SET" };
        public static readonly List<string> textColumn = new List<string>() { "VARCHAR", "CHAR", "TEXT", "TINYTEXT", "MEDIUMTEXT", "LONGTEXT"  };
        public static readonly List<string> binaryColumn = new List<string>() { "VARBINARY", "BINARY", "BLOB", "TINYBLOB", "MEDIUMBLOB", "LONGBLOB" };
        public static readonly List<string> decimalColumn = new List<string>() { "FLOAT", "DOUBLE", "DECIMAL" };
        public static readonly List<string> datetimeColumn = new List<string>() { "DATETIME", "DATE", "TIME", "TIMESTAMP", "YEAR" };
        public static readonly List<string> onUpdateDelete = new List<string>() { "", "RESTRICT", "CASCADE", "SET NULL", "NO ACTION", "SET DEFAULT" };

        public static List<string> dbNames = new List<string>();
        public static List<string> tableNames = new List<string>();

        public static void SetDbNames()
        {
            dbNames = new List<string>();
            foreach (Database database in DataStore.databases)
            {
                dbNames.Add(database.name);
            }
        }

        public static void SetTableNames()
        {
            tableNames = new List<string>();
            foreach (Table t in DataStore.activeDatabase.tables)
            {
                tableNames.Add(t.name);
            }
        }
    }
}