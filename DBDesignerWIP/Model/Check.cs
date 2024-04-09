using System;
using System.Collections.Generic;
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
            if (value == null && nullAllowed == false)
            {
                errorMessage = "Column doesn't support NULL values.";
                return false;
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

        public static bool CheckDefaultInteger(string value, string type, bool nullAllowed, bool unsigned, bool zerofill, bool unique, bool autoincrement, out string errorMessage)
        {
            if (autoincrement && !unique)
            {
                errorMessage = "Auto-incremented columns must have UNIQUE values.";
                return false;
            }
            if (value == null && !nullAllowed)
            {
                errorMessage = "Column doesn't support NULL values.";
                return false;
            }
            if (zerofill && !unsigned)
            {
                errorMessage = "Zero-filled columns must be UNSIGNED.";
                return false;
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
            if (!options.Contains(value))
            {
                errorMessage = "Default value not in ENUM/SET options.";
                return false;
            }

            errorMessage = "";
            return true;
        }

        public static bool CheckBinary(string value, string type, bool nullAllowed, out string errorMessage)
        {
            if (type.Contains("BLOB"))
            {
                errorMessage = "BLOB columns cannot have default value.";
                return false;
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

        public static bool CheckTextColumn()
        {
            
            return false;
        }



    }
}
