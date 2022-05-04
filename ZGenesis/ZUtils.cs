using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace ZGenesis {
    public static class ZUtils {
        public static Sprite LoadSpriteAsset(string modname, string path) {
            try {
                path = "res/" + path;
                Texture2D tex = new Texture2D(0, 0);
                tex.LoadImage(File.ReadAllBytes(path));
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));
                return sprite;
            } catch(Exception e) {
                Logger.Log(Logger.LogLevel.ERROR, modname, "Could not load sprite @ '{0}'. Reason: {1}", path, e);
            }
            return null;
        }
    }
}
