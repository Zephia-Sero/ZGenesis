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
        public static Dictionary<string, ConfigValue> options = new Dictionary<string, ConfigValue>();
        public static List<ConfigFile> unloadedConfigFiles = new List<ConfigFile>();

        public string Path { get; }
        public readonly ConfigHeader header;
        private readonly string ownerName;
        internal delegate bool OnLoad();
        internal OnLoad onLoad;
        private readonly Dictionary<string, ConfigValue> defaults;
        static ConfigFile() {
            CONFIG_VALUE_TYPES = Enum.GetNames(typeof(EConfigValueType));
        }
        public ConfigFile(GenesisMod owner, string path, Dictionary<string, ConfigValue> defaults) {
            Path = "config/" + path;
            int dirpathLen = Path.LastIndexOf('/');
            if(dirpathLen == -1)
                dirpathLen = Path.LastIndexOf('\\');
            string dirpath = Path.Substring(0, dirpathLen);
            if(dirpathLen != -1 && !Directory.Exists(dirpath))
                Directory.CreateDirectory(dirpath);
            if(!File.Exists(Path)) {
                File.Create(Path).Close();
            }
            ownerName = owner.Name;
            header = new ConfigHeader(ownerName, Path);
            onLoad = new OnLoad(() => { return true; });
            unloadedConfigFiles.Add(this);
            this.defaults = defaults;
        }
        public ConfigFile(GenesisMod owner, string path) : this(owner, path, new Dictionary<string, ConfigValue>()) { }
        public void PreloadPrep() {
            foreach(string loadFirst in header.loadAfter) {
                unloadedConfigFiles.ForEach(cfg => {
                    if(cfg.Path == loadFirst) {
                        cfg.onLoad += TryLoadConfig;
                    }
                });
            }
        }
        public bool TryLoadConfig() {
            foreach(string loadFirst in header.loadAfter) {
                if(unloadedConfigFiles.Any(cfg => {
                    return cfg.Path == loadFirst;
                })) return false;
            }
            ForceLoadConfig();
            return true;
        }
        public void ForceLoadConfig() {
            try {
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
                                if(key != null)
                                    Logger.Log(Logger.LogLevel.ERROR, ownerName, "CONFIG ERROR: Line {0}: Could not find type for config entry with key '{2}' in file '{1}'.", lineNum, Path, key);
                                else
                                    Logger.Log(Logger.LogLevel.ERROR, ownerName, "CONFIG ERROR: Line {0}: Could not find type for config entry in file '{1}'.", lineNum, Path);
                                successfulSplit = false;
                            }
                            if(key == null) {
                                Logger.Log(Logger.LogLevel.ERROR, ownerName, "CONFIG ERROR: Line {0}: Could not find key for config entry in file '{1}'.", lineNum, Path);
                                successfulSplit = false;
                            } else if(options.ContainsKey(key)) {
                                Logger.Log(Logger.LogLevel.ERROR, ownerName, "CONFIG ERROR: Line {0}: Duplicate configuration key '{2}' in file '{1}'", lineNum, Path, key);
                                successfulSplit = false;
                            }
                            string value = "";
                            if(valueStart != -1) {
                                value = line.Substring(valueStart).Trim();
                            }
                            if(value == "") {
                                if(key != null)
                                    Logger.Log(Logger.LogLevel.ERROR, ownerName, "CONFIG ERROR: Line {0}: Could not find value for config entry with key '{2}' in file '{1}'.", lineNum, Path, key);
                                else
                                    Logger.Log(Logger.LogLevel.ERROR, ownerName, "CONFIG ERROR: Line {0}: Could not find value for config entry in file '{1}'.", lineNum, Path);
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
                                ConfigValue val = ConfigValue.TryCreateFromString(ownerName, value, t);
                                options.Add(key, val);
                            } else {
                                Logger.Log(Logger.LogLevel.ERROR, ownerName, "CONFIG ERROR: Line {0}: Invalid type '{1}' for configuration key '{2}' in file '{3}'.", lineNum, type, key, Path);
                            }
                        }
                    }
                }
            } catch(Exception e) {
                Logger.Log(Logger.LogLevel.ERROR, ownerName, "CONFIG ERROR: Could not open config file '{0}'. Exception: {1}", Path, e);
            }
            foreach(KeyValuePair<string, ConfigValue> keyValuePair in defaults) {
                if(
            }
            unloadedConfigFiles.Remove(this);
            onLoad();
        }
    }
}
