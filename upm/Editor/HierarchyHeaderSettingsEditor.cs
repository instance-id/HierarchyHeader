// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/HierarchyHeader               --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.HierarchyHeader.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HierarchyHeaderSettings))]
    public class HierarchyHeaderSettingsEditor : UnityEditor.Editor
    {
        [UsedImplicitly] private string imGUIPropNeedsRelayout;
        private ScrollView scrollView;
#if !UNITY_2020_2_OR_NEWER
        private ReorderableList reorderableList;
#endif

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement {name = "HierarchyHeaderRoot"};
            scrollView = new ScrollView();

#if UNITY_2020_2_OR_NEWER
            var boxContainer = new Box();
            boxContainer.AddToClassList("mainBoxContainer");
            boxContainer.Add(scrollView);
#endif
            serializedObject.Update();
            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    var propPath = property.propertyPath;
                    var propertyField = new PropertyField(property) {name = "PropertyField:" + propPath};
                    switch (propPath)
                    {
                        case "m_Script" when serializedObject.targetObject != null:
                            propertyField.visible = true;
                            propertyField.SetEnabled(true);
                            break;
                        default:
                            if (property.IsReallyArray() && serializedObject.targetObject != null)
                            {
                                var copiedProperty = property.Copy(); // @formatter:off
#if UNITY_2020_2_OR_NEWER
                                var imDefaultProperty = new IMGUIContainer(() =>
                                {
                                    DoDrawDefaultIMGUIProperty(serializedObject, copiedProperty);
                                }) {name = propPath};
#else
                                reorderableList = new ReorderableList(serializedObject, copiedProperty)
                                {
                                    drawHeaderCallback = DrawHeaderCallback,
                                    drawElementCallback = DrawElementCallback,
                                    elementHeightCallback = ElementHeightCallback,
                                    onAddCallback = OnAddCallback
                                }; // @formatter:on

                                reorderableList.elementHeightCallback = ElementHeightCallback;

                                var imDefaultProperty = new IMGUIContainer(() =>
                                {
                                    serializedObject.Update();
                                    reorderableList.DoLayoutList();
                                    serializedObject.ApplyModifiedProperties();
                                }) {name = propPath};
#endif
                                imDefaultProperty.RegisterCallback<ChangeEvent<bool>>(evt => RecomputeSize(imDefaultProperty));
                                scrollView.Add(imDefaultProperty);
                            }

                            break; // @formatter:on
                    }
                } while (property.NextVisible(false));
            }

            foreach (var foldoutList in scrollView.Query<Foldout>().ToList())
            {
                foldoutList.RegisterValueChangedCallback(e =>
                {
                    if (!(e.target is Foldout fd)) return;
                    var path = fd.bindingPath;
                    var container = scrollView.Q<IMGUIContainer>(path);
                    RecomputeSize(container);
                });
            }

            serializedObject.ApplyModifiedProperties();

#if UNITY_2020_2_OR_NEWER
            root.Add(boxContainer);
#else
            root.Add(scrollView);
#endif
            return root;
        }

        #region IMGUI

#if !UNITY_2020_2_OR_NEWER
        private void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Header Settings");
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            var elementName = element.FindPropertyRelative("m_Name");
            var elementTitle = string.IsNullOrEmpty(elementName.stringValue)
                ? "Add New Settings"
                : $"Settings: {elementName.stringValue}";

            EditorGUI.PropertyField(new Rect(rect.x += 10, rect.y, Screen.width * .8f, EditorGUIUtility.singleLineHeight), element, new GUIContent(elementTitle), true);
        }

        private float ElementHeightCallback(int index)
        {
            var propertyHeight =
                EditorGUI.GetPropertyHeight(reorderableList.serializedProperty.GetArrayElementAtIndex(index), true);

            var spacing = EditorGUIUtility.singleLineHeight / 2;
            return propertyHeight + spacing;
        }

        private void OnAddCallback(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
        }
#endif

        private void RecomputeSize(IMGUIContainer container) // @formatter:on
        {
            if (container == null) return;
            var parent = container.parent;
            container.RemoveFromHierarchy();
            parent.Add(container);
        }

        private void DoDrawDefaultIMGUIProperty(SerializedObject serializedObj, SerializedProperty property)
        {
            EditorGUI.BeginChangeCheck();
            serializedObj.Update();
            var wasExpanded = property.isExpanded;
            EditorGUILayout.PropertyField(property, true);
            if (property.isExpanded != wasExpanded) imGUIPropNeedsRelayout = property.propertyPath;
            serializedObj.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        #endregion
    }
}
