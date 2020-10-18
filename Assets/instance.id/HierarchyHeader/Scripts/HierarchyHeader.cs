// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/HierarchyHeader               --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace instance.id.HierarchyHeader
{
    [InitializeOnLoad]
    public static class HierarchyHeader
    {
        private static string hhConfigurationType = "HierarchyHeaderSettings";

        static HierarchyHeaderSettings hhSettings;
        static readonly List<GUIStyle> styleData = new List<GUIStyle>();
        private static string hhPath;

        // @formatter:off --------------------------- SelectSettingsObject
        // -- Main menu item to select and configure AAI settings       --
        // -- SelectSettingsObject ---------------------------------------
        [MenuItem("Tools/instance.id/HierarchyHeader Settings", false)]
        public static void SelectSettingsObject() // @formatter:on
        {
            Selection.objects = new Object[] {HHSettings()};
        }

        // @formatter:off ------------------------------------- HHSettings
        // -- Checks for existence of HierarchyHeaderSettings object    --
        // -- If not present, one is created with default settings      --
        // -- HHSettings -------------------------------------------------
        private static HierarchyHeaderSettings HHSettings()
        {
            try { if (!(hhSettings is null)) return hhSettings; LoadAsset(); }
            catch (Exception) { LoadAsset(); }

            return hhSettings;
        } // @formatter:on

        static HierarchyHeader()
        {
            HHSettings();
            SetStyleData();
            hhSettings.NeedsUpdate.AddListener(SetStyleData);
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        private static void SetStyleData()
        {
            if (styleData.Count != hhSettings.settingsDatas.Count)
            {
                styleData.Clear();
                for (var i = 0; i < hhSettings.settingsDatas.Count; i++)
                {
                    styleData.Add(new GUIStyle());
                }
            }

            for (var i = 0; i < hhSettings.settingsDatas.Count; i++)
            {
                styleData[i].fontSize = hhSettings.settingsDatas[i].FontSize;
                styleData[i].fontStyle = hhSettings.settingsDatas[i].FontStyle;
                styleData[i].alignment = hhSettings.settingsDatas[i].Alignment;
                styleData[i].normal.textColor = hhSettings.settingsDatas[i].TextColor;
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var hierarchyItem = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            var settingList = hhSettings.settingsDatas;
            for (var i = 0; i < settingList.Count; i++)
            {
                if (hierarchyItem == null || !hierarchyItem.name.StartsWith(settingList[i].headerPrefix, StringComparison.Ordinal)) continue;

                EditorGUI.DrawRect(selectionRect, settingList[i].BackgroundColor);
                EditorGUI.LabelField(selectionRect, hierarchyItem.name.Replace(settingList[i].characterStrip, "").ToUpperInvariant(), styleData[i]);
            }
        }

        private static void Assignments()
        {
            hhPath = hhSettings.GetLocation();
        }

        // ------------------------------------------------------------------------------ LoadAsset
        // -- HierarchyHeaderSettings keeps track of the current path of the HH Asset as         --
        // -- well as stores various other configuration data needed by HierarchyHeader          --
        // -- LoadAsset ---------------------------------------------------------------------------
        private static void LoadAsset()
        {
            if (!(hhSettings is null)) return;

            hhSettings = AssetDatabase.FindAssets($"t:{hhConfigurationType}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<HierarchyHeaderSettings>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault();

            if (!(hhSettings is null)) Assignments();
            else
            {
                var hhConfig = ScriptableObject.CreateInstance<HierarchyHeaderSettings>();
                hhPath = hhConfig.GetLocation();
                Object.DestroyImmediate(hhConfig);
                var assetName = $"{hhConfigurationType}.asset";
                var path = $"{hhPath}/HierarchyHeader/{assetName}";
                hhSettings = ScriptableObjectExtensions.CreateAsset<HierarchyHeaderSettings>(path);
                Assignments();
                Debug.Log($"Creating HierarchyHeaderSettings object: {path}");
            }
        }
    }
}
