using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Configuration {
    public class ConfigHeader {
        public readonly string[] loadAfter;
        public ConfigHeader(string modName, string path) {
            try {
                using(FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read)) {
                    using(StreamReader sr = new StreamReader(fs)) {
                        string line;
                        while((line = sr.ReadLine()) != null) {
                            if(!line.StartsWith("#")) return;
                        }
                    }
                }
            } catch(Exception e) {
                Logger.Log(Logger.LogLevel.ERROR, modName, "CONFIG ERROR: Could not open config file '{0}'. Exception: '{1}'.", path, e);
            }
        }
    }
}
