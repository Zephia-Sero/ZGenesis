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
                        
                        List<string> loadAfter = new List<string>();

                        string line;
                        while((line = sr.ReadLine()) != null) {
                            if(line.Trim() == "") continue;
                            int hashIdx = line.IndexOf('#');
                            if(hashIdx == -1) break;
                            
                            string[] ls = line.Substring(hashIdx+1).Trim().Split(' ');
                            if(ls.Length == 0) continue;
                            string instruction = ls[0].ToUpper();

                            switch(instruction) {
                            case "REQUIRE":
                                for(int i = 1; i < ls.Length; i++) {
                                    if(ls[i] == "") continue;
                                    else if(!loadAfter.Contains(ls[i])) {
                                        loadAfter.Add(ls[i]);
                                    }
                                }
                                break;
                            case "COMMENT":
                                break;
                            }
                        }
                        this.loadAfter = loadAfter.ToArray();
                    }
                }
            } catch(Exception e) {
                Logger.Log(Logger.LogLevel.ERROR, modName, "CONFIG ERROR: Could not open config file '{0}'. Exception: '{1}'.", path, e);
            }
        }
    }
}
