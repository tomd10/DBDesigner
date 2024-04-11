using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DBDesignerWIP
{
    public static class Check
    {
        public static bool CheckDefaultString(string value, string type, int size, bool nullAllowed, out string errorMessage)
        {
            if ((value == null || value.ToUpper() == "#NULL") && nullAllowed == false)
            {
                errorMessage = "Column doesn't support NULL values.";
                return false;
            }
            if (value == null && nullAllowed)
            {
                errorMessage = "";
                return true;
            }
            if ((type == "VARCHAR" || type == "CHAR") && value.Length > size)
            {
                errorMessage = "Default value length exceeds column length.";
                return false;
            }
            if (type.Contains("TEXT"))
            {
                errorMessage = "TEXT columns cannot have default value.";
                return false;
            }

            errorMessage = "";
            return true;
        }

        public static bool CheckDefaultInteger(string value, string type, bool nullAllowed, bool unsigned, bool zerofill, bool autoincrement, out string errorMessage)
        {
            if (value == null && !nullAllowed)
            {
                errorMessage = "Column doesn't support NULL values.";
                return false;
            }
            if (value == null && nullAllowed)
            {
                errorMessage = "";
                return true;
            }
            long val = 0;
            if (value != null && !long.TryParse(value, out val))
            {
                errorMessage = "Incorrect integer literal format.";
                return false;
            }
            else if (long.TryParse(value, out val))
            {
                if (type == "TINYINT" && !unsigned && (val < -128 || val > 127))
                {
                    errorMessage = "TINYINT default value must be within (-128, 127) inclusive.";
                    return false;
                }
                else if (type == "TINYINT" && unsigned && (val < 0 || val > 255))
                {
                    errorMessage = "Unsigned TINYINT default value must be within (0, 255) inclusive.";
                    return false;
                }
                if (type == "SMALLINT" && !unsigned && (val < -8388608 || val > 8388607))
                {
                    errorMessage = "SMALLINT default value must be within (-8388608, 8388607) inclusive.";
                    return false;
                }
                else if (type == "SMALLINT" && unsigned && (val < 0 || val > 16777215))
                {
                    errorMessage = "Unsigned SMALLINT default value must be within (0, 16777215) inclusive.";
                    return false;
                }
                else if (type == "MEDIUMINT" && !unsigned && (val < -32768 || val > 32767))
                {
                    errorMessage = "MEDIUMINT default value must be within (-32768, 32767) inclusive.";
                    return false;
                }
                else if (type == "MEDIUMINT" && unsigned && (val < 0 || val > 65535))
                {
                    errorMessage = "Unsigned MEDIUMINT default value must be within (0, 65535) inclusive.";
                    return false;
                }
                else if (type == "INT" && !unsigned && (val < -2147483648 || val > 2147483647))
                {
                    errorMessage = "INT default value must be within (-2147483648, 2147483647) inclusive.";
                    return false;
                }
                else if (type == "INT" && unsigned && (val < 0 || val > 4294967295))
                {
                    errorMessage = "Unsigned INT default value must be within (0, 4294967295) inclusive.";
                    return false;
                }
                else if (type == "BIGINT" && !unsigned && (val < -9223372036854775808 || val > 9223372036854775807))
                {
                    errorMessage = "BIGINT default value must be within (-9223372036854775808, 9223372036854775807) inclusive.";
                    return false;
                }
                else if (type == "BIGINT" && unsigned && (val < 0 || (ulong)val > 18446744073709551615))
                {
                    errorMessage = "Unsigned BIGINT default value must be within (0, 18446744073709551615) inclusive.";
                    return false;
                }
            }
            errorMessage = "";
            return true;
        }

        public static bool CheckDecimal(string value, string type, bool nullAllowed, out string errorMessage)
        {
            if (value == null && nullAllowed == false)
            {
                errorMessage = "Column doesn't support NULL values.";
                return false;
            }
            if (value == null && nullAllowed)
            {
                errorMessage = "";
                return true;
            }
            double val;
            if (value != null && !double.TryParse(value, out val))
            {
                errorMessage = "Incorrect double literal format.";
                return false;
            }
            errorMessage = "";
            return true;
        }

        public static bool CheckEnum(string value, string type, bool nullAllowed, List<string> options, out string errorMessage)
        {
            if (value == null && !nullAllowed)
            {
                errorMessage = "Column doesn't support NULL values.";
                return false;
            }
            if (value == null && nullAllowed)
            {
                errorMessage = "";
                return true;
            }
            if (!options.Contains(value))
            {
                errorMessage = "Default value not in ENUM/SET options.";
                return false;
            }

            errorMessage = "";
            return true;
        }

        public static bool CheckBinary(string value, string type, bool nullAllowed, int size, out string errorMessage)
        {
            if (type.Contains("BLOB"))
            {
                errorMessage = "BLOB columns cannot have default value.";
                return false;
            }
            if (value == null && nullAllowed)
            {
                errorMessage = "";
                return true;
            }
            if (value == null && !nullAllowed)
            {
                errorMessage = "Column doesn't support NULL values.";
                return false;
            }
            errorMessage = "Binary defaults not supported.";
            return false;
        }

        public static bool CheckDateTime(string value, string type, bool nullAllowed, out string errorMessage)
        {
            if (value == null && !nullAllowed)
            {
                errorMessage = "Column doesn't support NULL values.";
                return false;
            }
            if (value == null && nullAllowed)
            {
                errorMessage = "";
                return true;
            }
            if (type == "DATETIME" || type == "TIMESTAMP")
            {
                string format = "yyyy-MM-dd HH:mm:ss";
                if (!(DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt)))
                {
                    errorMessage = "Incorrect date/time literal format.";
                    return false;
                }
            }
            else if (type == "TIME")
            {
                string format = "HH:mm:ss";
                if (!(DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt)))
                {
                    errorMessage = "Incorrect time literal format.";
                    return false;
                }
            }
            else if (type == "DATE")
            {
                string format = "yyyy-MM-dd";
                if (!(DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt)))
                {
                    errorMessage = "Incorrect date literal format.";
                    return false;
                }
            }
            else if (type == "YEAR")
            {
                string format = "yyyy";
                if (!(DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt)))
                {
                    errorMessage = "Incorrect date literal format.";
                    return false;
                }
            }

            errorMessage = "";
            return true;
        }

        public static bool IsValidName(string name, out string errorMessage)
        {
            if (name.Length > 64)
            {
                errorMessage = "Name maximum length is 64 characters";
                return false;
            }
            if (name == null ||  name.Length == 0)
            {
                errorMessage = "Name mustn't be empty.";
                return false;
            }
            for (int i = 0; i < name.Length; i++)
            {
                char ch = name[i];
                if (!(char.IsAsciiDigit(ch) || char.IsAsciiLetter(ch) || ch == '_' || ch == '$' ))
                {
                    errorMessage = "Name must contain only 0-9, A-Z, a-z and $ or _ characters.";
                    return false;
                }
            }
            errorMessage = "";
            return true;
        }

        public static bool CheckDatabaseName(string name, out string errorMessage)
        {
            if (!IsValidName(name, out errorMessage)) return false;
            else
            {
                foreach (Database d in DataStore.databases)
                {
                    if (d.name ==  name)
                    {
                        errorMessage = "Duplicate database name.";
                        return false;
                    }
                }
                errorMessage = "";
                return true;
            }
        }

        public static bool CheckTableName(string name, out string errorMessage)
        {
            if (!IsValidName(name, out errorMessage)) return false;
            else
            {
                foreach (Table t in DataStore.activeDatabase.tables)
                {
                    if (t.name == name)
                    {
                        errorMessage = "Duplicate table name.";
                        return false;
                    }
                }
                errorMessage = "";
                return true;
            }
        }

        public static bool CheckTextColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, int size, string charset, string collate, out string errorMessage)
        {
            if (!IsValidName(name, out errorMessage))
            {
                return false;
            }
            if(!DataStore.activeTable.GetColumnNameAvailable(name))
            {
                errorMessage = "Duplicate column name.";
                return false;
            }
            if (size < 1 || size > 255)
            {
                errorMessage = "Size must be within (1, 255) inclusive.";
                return false;
            }
            string defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
            if (defa != "")
            {
                if(!Check.CheckDefaultString(defa, type, size, nullAllowed, out errorMessage))
                {
                    return false;
                }
            }

            errorMessage = "";
            return true;
        }

        public static bool CheckIntegerColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, int size, bool unsigned, bool zerofill, bool autoIncrement, out string errorMessage)
        {
            if (!IsValidName(name, out errorMessage))
            {
                return false;
            }
            if (!DataStore.activeTable.GetColumnNameAvailable(name))
            {
                errorMessage = "Duplicate column name.";
                return false;
            }
            if(zerofill && !unsigned)
            {
                errorMessage = "ZEROFILLed column must be UNSIGNED.";
                return false;
            }
            if (DataStore.activeTable.GetAutoIncrement() && autoIncrement)
            {
                errorMessage = "There can be only one auto-incremented columns.";
                return false;
            }
            if (size < 1 || size > 255)
            {
                errorMessage = "Size must be within (1, 255) inclusive.";
                return false;
            }
            string defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
            if (defa != "")
            {
                if (!CheckDefaultInteger(defa, type, nullAllowed, unsigned, zerofill, autoIncrement, out errorMessage))
                {
                    return false;
                }
            }

            return true;

        }

        public static bool CheckDecimalColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, int size, int d, out string errorMessage)
        {
            if (!IsValidName(name, out errorMessage))
            {
                return false;
            }
            if (!DataStore.activeTable.GetColumnNameAvailable(name))
            {
                errorMessage = "Duplicate column name.";
                return false;
            }
            if (d > size)
            {
                errorMessage = "d must be smaller than size.";
                return false;
            }
            string defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
            if (defa != "")
            {
                if (!CheckDecimal(defa, type, nullAllowed, out errorMessage))
                {
                    return false;
                }
            }
            errorMessage = "";
            return true;
        }

        public static bool CheckEnumColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, string options, out string errorMessage)
        {
            if (!IsValidName(name, out errorMessage))
            {
                return false;
            }
            if (!DataStore.activeTable.GetColumnNameAvailable(name))
            {
                errorMessage = "Duplicate column name.";
                return false;
            }
            if (options.Trim() == "")
            {
                errorMessage = "Empty options";
                return false;
            }

            List<string> opt = options.Trim().Split(",").ToList(); for (int i = 0; i < opt.Count; i++) { opt[i] = opt[i].Trim(); }
            if (opt.Count != opt.Distinct().Count())
            {
                errorMessage = "Duplicate options";
                return false;
            }
            string defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
            if (defa != "")
            {
                if (!CheckEnum(defa, type, nullAllowed, opt, out errorMessage))
                {
                    return false;
                }
            }
            errorMessage = "";
            return true;
        }

        public static bool CheckBinaryColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, int size, out string errorMessage)
        {
            if (!IsValidName(name, out errorMessage))
            {
                return false;
            }
            if (!DataStore.activeTable.GetColumnNameAvailable(name))
            {
                errorMessage = "Duplicate column name.";
                return false;
            }
            if (size < 1 || size > 255)
            {
                errorMessage = "Size must be within (1, 255) inclusive.";
                return false;
            }
            string defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
            if (defa != "")
            {
                if (!CheckBinary(defa, type, nullAllowed, size, out errorMessage))
                {
                    return false;
                }
            }
            errorMessage = "";
            return true;
        }

        public static bool CheckDateTimeColumn(string name, string type, bool nullAllowed, string defaultValue, string comment, out string errorMessage)
        {
            if (!IsValidName(name, out errorMessage))
            {
                return false;
            }
            if (!DataStore.activeTable.GetColumnNameAvailable(name))
            {
                errorMessage = "Duplicate column name.";
                return false;
            }
            string defa = (defaultValue.ToUpper() == "#NULL") ? null : defaultValue;
            if (defa != "")
            {
                if (!CheckDateTime(defa, type, nullAllowed, out errorMessage))
                {
                    return false;
                }
            }
            errorMessage = "";
            return true;
        }
    }
}
