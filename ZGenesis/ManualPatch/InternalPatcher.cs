﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InternalPatcher {
	public static void Patch() {
		if(!Directory.Exists("./logs")) {
			Directory.CreateDirectory("./logs");
		}
		if(!InternalPatcher.modLoaderStarted) {
			using(FileStream fileStream = File.Open("./logs/zgenesis_internal_init.log", FileMode.Create)) {
				using(StreamWriter streamWriter = new StreamWriter(fileStream)) {
					try {
						RuntimeHelpers.RunClassConstructor(Assembly.LoadFrom("ZGenesis.dll").GetType("ZGenesis.Patcher").TypeHandle);
						streamWriter.WriteLine("Successfully initialized ZGenesis patcher.");
					} catch(Exception ex) {
						streamWriter.WriteLine("Failed to initialize ZGenesis patcher: " + ex.ToString());
						Application.Quit();
					}
				}
			}
			InternalPatcher.modLoaderStarted = true;
		}
	}
	private static bool modLoaderStarted;
}
