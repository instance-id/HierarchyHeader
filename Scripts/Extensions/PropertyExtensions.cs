// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/HierarchyHeader               --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace instance.id.HierarchyHeader
{
    public static class PropertyExtensions
    {
        public static object GetValue(this SerializedProperty property)
        {
            var parentType = property.serializedObject.targetObject.GetType();
            var fi = parentType.GetField(property.propertyPath);
            return fi.GetValue(property.serializedObject.targetObject);
        }

        public static Type GetTypeExt(this SerializedProperty property)
        {
            var parentType = property.serializedObject.targetObject.GetType();
            var fi = parentType.GetField(property.propertyPath);
            return fi.ReflectedType;
        }

        // -- IsArray sometimes lies -------------------------------------
        public static bool IsReallyArray(this SerializedProperty property)
        {
            return property.isArray && property.propertyType != SerializedPropertyType.String;
        }

        public static Type GetTypeReflection(this SerializedProperty prop)
        {
            var parentType = prop.serializedObject.targetObject.GetType();
            var fields = parentType.GetFields();
            Type result = null;
            for (var i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType.IsGenericType)
                {
                    var genericArgs = fields[i].FieldType.GetGenericArguments();
                    for (var j = 0; j < genericArgs.Length; j++)
                    {
                        var genericArg = genericArgs[j];
                        if (prop.IsType(genericArgs[j])) return genericArgs[j];
                    }
                }
                else if (prop.IsType(fields[i].FieldType)) return fields[i].FieldType;
            }

            return result;
        }

        private static bool IsType(this SerializedProperty prop, Type type)
        {
            return prop.type == type.ToString();
        }

        public static bool IsNumerical(this SerializedProperty property)
        {
            var propertyType = property.propertyType;
            switch (propertyType)
            {
                case SerializedPropertyType.Float:
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.FixedBufferSize: return true;
                default: return false;
            }
        }

        public static bool IsObject(this SerializedProperty property)
        {
            var propertyType = property.propertyType;
            switch (propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.ManagedReference: return true;
                default: return false;
            }
        }

        public static string GetAdjustedPath(this SerializedProperty property)
        {
            return property.propertyPath.Replace("Array.data", "").Replace("[", "").Replace("]", "");
        }

        public static Type GetPropertyType(this SerializedProperty serializedProperty)
        {
            var slices = serializedProperty.propertyPath.Split('.');
            var type = serializedProperty.serializedObject.targetObject.GetType();
            for (var i = 0; i < slices.Length; i++)
            {
                if (slices[i] == "Array")
                {
                    i++; //skips "data[x]"
                    if (type.IsArray)
                    {
                        type = type.GetElementType(); //gets info on array elements
                    }
                    else
                    {
                        type = type.GetGenericArguments()[0];
                    }
                }
                else
                {
                    type = type.GetField(slices[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).FieldType;
                }
            }

            return type;
        }

        public static Type GetParentType(this SerializedProperty serializedProperty, int parentDepth = 1)
        {
            var targetObject = serializedProperty.serializedObject.targetObject;
            var targetObjectType = targetObject.GetType();
            if (serializedProperty.depth > 0)
            {
                var path = serializedProperty.propertyPath.Split('.');
                var currentType = targetObjectType;
                var i = 0;
                while (i < path.Length - parentDepth)
                {
                    if (path[i] == "Array")
                    {
                        i++; //skips "data[x]"
                        if (currentType.IsArray)
                        {
                            currentType = currentType.GetElementType(); //gets info on array elements
                        }
                        else
                        {
                            currentType = currentType.GetGenericArguments()[0];
                        }
                    }
                    else
                    {
                        currentType = currentType.GetField(path[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).FieldType;
                    }

                    i++;
                }

                return currentType;
            }
            else
            {
                return targetObjectType;
            }
        }
    }
}
