using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGenesis.Mod;

namespace ZGenesis.Configuration {
    public class ConfigFile {
        public static readonly string[] CONFIG_VALUE_TYPES;
        public string Path { get; }
        public Dictionary<string, ConfigValue> options = new Dictionary<string, ConfigValue>();
        private readonly GenesisMod owner;
        
        static ConfigFile() {
            CONFIG_VALUE_TYPES = Enum.GetNames(typeof(EConfigValueType));
        }
        public ConfigFile(GenesisMod owner, string path) {
            Path = path;
            this.owner = owner;
            LoadFile();
        }

        public void LoadFile() {
            using(FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Read)) {
                using(StreamReader sr = new StreamReader(fs)) {
                    string line;
                    bool multilineComment = false;
                    bool multilineJustSet = false;
                    int lineNum = 0;
                    while((line = sr.ReadLine()) != null) {
                        lineNum++;
                        if(multilineJustSet) multilineJustSet = false;

                        int multilineStartIdx = line.IndexOf("/*");
                        int multilineEndIdx = line.IndexOf("*/");

                        if(multilineStartIdx != -1) {
                            multilineComment = true;
                            multilineJustSet = true;
                            line = line.Substring(0, multilineStartIdx);
                        } else if(multilineEndIdx != -1) {
                            multilineComment = false;
                            line = line.Substring(multilineEndIdx);
                        }

                        if(multilineComment && !multilineJustSet) continue;
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

                        bool successfulSplit = true;
                        if(type == null) {
                            Logger.Log(Logger.LogLevel.ERROR, owner.Name, "CONFIG ERROR: Line {0}: Could not find type for config entry in file '{1}'.", lineNum, Path);
                            successfulSplit = false;
                        }
                        if(key == null) {
                            Logger.Log(Logger.LogLevel.ERROR, owner.Name, "CONFIG ERROR: Line {0}: Could not find key for config entry in file '{1}'.", lineNum, Path);
                            successfulSplit = false;
                        } else if(options.ContainsKey(key)) {
                            Logger.Log(Logger.LogLevel.ERROR, owner.Name, "CONFIG ERROR: Line {0}: Duplicate configuration key in file '{1}'", lineNum, Path);
                        }
                        string value = "";
                        if(valueStart != -1) {
                            value = line.Substring(valueStart).Trim();
                        }
                        if(value == "") {
                            Logger.Log(Logger.LogLevel.ERROR, owner.Name, "CONFIG ERROR: Line {0}: Could not find value for config entry in file '{1}'.", lineNum, Path);
                            successfulSplit = false;
                        }
                        if(!successfulSplit) continue;

                        EConfigValueType t = EConfigValueType.COUNT;
                        foreach(string valueType in CONFIG_VALUE_TYPES) {
                            if(type == valueType.ToLower()) {
                                t = (EConfigValueType) Enum.Parse(typeof(EConfigValueType), valueType);
                                break;
                            }
                        }
                        if(t != EConfigValueType.COUNT) {

                        } else {
                            Logger.Log(Logger.LogLevel.ERROR, owner.Name, "CONFIG ERROR: Line {0}: Invalid type '{1}' for configuration key '{2}' in file '{3}'.", lineNum, type, key, Path);
                        }
                    }
                }
            }
        }
    }
}
