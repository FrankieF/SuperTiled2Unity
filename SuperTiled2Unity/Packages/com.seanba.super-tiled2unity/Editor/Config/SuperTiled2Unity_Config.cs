﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperTiled2Unity.Editor
{
    internal class SuperTiled2Unity_Config
    {
        public const string Version = "1.10.7"; // fixit - how to get version of package in package.json?
        internal const string DefaultSettingsFileName = "ST2U Settings.asset";

        public static ST2USettings CreateDefaultSettings()
        {
            var scriptPath = AssetDatabaseEx.GetFirstPathOfScriptAsset<SuperTiled2Unity_Config>();
            var settingsPath = Path.GetDirectoryName(scriptPath);
            settingsPath = Path.Combine(settingsPath, DefaultSettingsFileName).SanitizePath();

            var settings = ScriptableObject.CreateInstance<ST2USettings>();
            AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();

            return settings;
        }

        public static string GetVersionError()
        {
            return string.Format("SuperTiled2Unity requires Unity 2020.3 or later. You are using {0}", Application.unityVersion);
        }

        [MenuItem("Assets/Super Tiled2Unity/Export ST2U Asset...", true)]
        public static bool ExportSuperAssetValidate()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<SuperAsset>(path) != null;
            }

            return false;
        }

        [MenuItem("Assets/Super Tiled2Unity/Export ST2U Asset...")]
        public static void ExportSuperAsset()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var tracker = new RecursiveAssetDependencyTracker(path);
            SuperPackageExport.ShowWindow(Path.GetFileNameWithoutExtension(path), tracker.Dependencies);
        }

        [MenuItem("Assets/Super Tiled2Unity/Apply Default Settings to ST2U Assets")]
        public static void ReimportWithDefaults()
        {
            UnityEngine.Object[] selectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            HashSet<TiledAssetImporter> tiledImporters = new HashSet<TiledAssetImporter>();

            foreach (var obj in selectedAsset)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                var importer = AssetImporter.GetAtPath(path) as TiledAssetImporter;
                if (importer != null)
                {
                    tiledImporters.Add(importer);
                }
            }

            foreach (var importer in tiledImporters)
            {
                importer.ApplyDefaultSettings();
            }

            foreach (var importer in tiledImporters)
            {
                importer.SaveAndReimport();
            }
        }
    }
}
