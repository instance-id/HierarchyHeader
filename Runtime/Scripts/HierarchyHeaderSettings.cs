// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/HierarchyHeader               --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace instance.id.HierarchyHeader
{
    [Serializable]
    public class HierarchyHeaderSettings : ScriptableObject
    {
        [HideInInspector] public UnityEvent NeedsUpdate;
        [HideInInspector] public string location;
        [HideInInspector] public string pathString;
        [SerializeField] public List<HierarchyHeaderSettingsData> settingsDatas = new List<HierarchyHeaderSettingsData>();
        public string GetLocation() => GetPaths();

        private void OnEnable()
        {
            if (settingsDatas.Count == 0) settingsDatas.Add(new HierarchyHeaderSettingsData());
        }

        private void OnValidate()
        {
            if (settingsDatas.Count == 0) settingsDatas.Add(new HierarchyHeaderSettingsData());
            NeedsUpdate?.Invoke();
        }


        // ------------------------------------------------------------------------------- GetPaths
        // -- GetPaths ----------------------------------------------------------------------------
        private string GetPaths(bool refresh = false)
        {
            var currentLocation = GetAssetPath();
            if (location == currentLocation && !refresh) return currentLocation;

            location = currentLocation;

            var dataPath = Application.dataPath;
            pathString = dataPath.Substring(0, dataPath.Length - "Assets".Length);

            return  location;
        }

#if UNITY_EDITOR
        // --------------------------------------------------------------------------- GetAssetPath
        // -- GetAssetPath ------------------------------------------------------------------------
        private string GetAssetPath()
        {
            var dirName = Path.GetDirectoryName(CurrentPath()) + "\\..\\" + "\\..\\";
            var fPath = Path.GetFullPath(ConvertPath(dirName));
            return DetermineAssetPath(ConvertPath(fPath));
        }

        private static string DetermineAssetPath(string absolutePath)
        {
            Debug.Log($"absolutePath {absolutePath}");
            Debug.Log($"Application.dataPath {Application.dataPath}");

            if (absolutePath.StartsWith(Application.dataPath)) return "Packages" + absolutePath.Substring(Application.dataPath.Length);
            else throw new ArgumentException("Full path does not contain the current project's Assets folder", nameof(absolutePath));
        }


        private string CurrentPath()
        {
            var script = MonoScript.FromScriptableObject(this);
            return AssetDatabase.GetAssetPath(script);
        }

        private string ConvertPath(string aPath)
        {
            return aPath.Replace("\\", "/");
        }
#endif
    }
}
