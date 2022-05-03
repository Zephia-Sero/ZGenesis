using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Configuration {
    public class ConfigFile {
        public static readonly string[] CONFIG_VALUE_TYPES;
        public string Path { get; }
        public Dictionary<string, object> Options = new Dictionary<string, object>();
        
        static ConfigFile() {
            CONFIG_VALUE_TYPES = Enum.GetNames(typeof(EConfigValueType));
        }
        public ConfigFile(string path) {
            Path = path;
            LoadFile();
        }

        public void LoadFile() {
            using(FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Read)) {
                using(StreamReader sr = new StreamReader(fs)) {
                    string line;
                    while((line = sr.ReadLine()) != null) {
                        line = line.Substring(0, line.IndexOf("//"));
                        if(line.Trim() == "") continue;
                        string type = null;
                        string key = null;
                        foreach(string part in line.Trim().Split(' ')) {
                            if(part == "") continue;
                            if(type == null) {
                                type = part;
                            } else if(key == null) {
                                key = part;
                            }
                        }
                        int valueStart = line.IndexOf("=");
                        string value = line.Substring(valueStart).Trim();
                        EConfigValueType t = EConfigValueType.COUNT;
                        foreach(string valueType in CONFIG_VALUE_TYPES) {
                            if(type == valueType.ToLower()) {
                                t = (EConfigValueType) Enum.Parse(typeof(EConfigValueType), valueType);
                                break;
                            }
                        }
                        if(t != EConfigValueType.COUNT) {

                        } else {
                            Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "Invalid type '{0}' for configuration key '{1}' in file '{2}'", type, );
                        }
                    }
            }
        }
    }
}
