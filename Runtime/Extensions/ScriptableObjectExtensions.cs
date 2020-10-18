// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/HierarchyHeader               --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.IO;
using UnityEngine;

namespace instance.id.HierarchyHeader
{
    public static class ScriptableObjectExtensions
    {
#if UNITY_EDITOR
        public static T CreateAsset<T>(string filePath = null) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory ?? throw new InvalidOperationException());

            if (filePath == null || filePath.Length <= 0)  filePath = "Assets/" + typeof(T).ToString() + ".asset";

            var assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(filePath);
            SaveAsset(asset, assetPathAndName);

            return asset;
        }

        public static void SaveAsset<T>(T asset, string path) where T : ScriptableObject
        {
            UnityEditor.AssetDatabase.CreateAsset(asset, path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}
