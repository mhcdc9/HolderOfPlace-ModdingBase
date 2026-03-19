using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModUtils
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigAttribute : Attribute
    {
        public string name;
        public object defaultValue;

        public ConfigAttribute(string name, object defaultValue)
        {
            this.name = name;
            this.defaultValue = defaultValue;
        }

        public bool Validate() => (defaultValue is int || defaultValue is float || defaultValue is bool || defaultValue is string);

        public static bool TrySet(FieldInfo field, object instance, string value)
        {
            Type type = field.FieldType;
            if (type == typeof(string))
            {
                field.SetValue(instance, value);
                return true;
            }
            else if (type == typeof(int))
            {
                if (int.TryParse(value, out int result))
                {
                    field.SetValue(instance, result);
                    return true;
                }
            }
            else if (type == typeof(float))
            {
                if (float.TryParse(value, out float result))
                {
                    field.SetValue(instance, result);
                    return true;
                }
            }
            else if (type == typeof(bool))
            {
                if (bool.TryParse(value, out bool result))
                {
                    field.SetValue(instance, result);
                    return true;
                }
            }
            return false;
        }

        public static void SaveConfigs(Type type, object instance, string path)
        {
            IEnumerable<FieldInfo> fields = type.GetFields();
            string contents = String.Empty;
            foreach (FieldInfo field in fields)
            {
                ConfigAttribute config = field.GetCustomAttribute<ConfigAttribute>();
                if (config != null)
                {
                    contents += config.name + "=";
                    if (field.FieldType == typeof(string))
                    {
                        contents += ((string)field.GetValue(instance)).Replace("\\n", "\\:n").Replace("\n", "\\n") + "\n";
                    }
                    else
                    {
                        contents += field.GetValue(instance).ToString();
                    }
                }
            }
            File.WriteAllText(path, contents);

        }
        public static void LoadConfigs(Type type, object instance, string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            foreach (string line in File.ReadAllLines(path))
            {
                string[] parts = line.Split('=');
                FieldInfo field = type.GetFields().FirstOrDefault(f => f.GetCustomAttribute<ConfigAttribute>()?.name == parts[0]);
                if (field != null)
                {
                    if (field.FieldType == typeof(string))
                    {
                        field.SetValue(instance, string.Join("=", parts.Skip(1).ToArray()).Replace("\\n", "\n").Replace("\\:n", "\\n"));
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        field.SetValue(instance, int.Parse(parts[1]));
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(instance, float.Parse(parts[1]));
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(instance, bool.Parse(parts[1]));
                    }
                }
            }
        }
    }
}
