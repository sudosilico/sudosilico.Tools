using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace sudosilico.Tools.TypeReferences.Editor
{
    [CustomPropertyDrawer(typeof(TypeReference))]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {
        private TypeSearchProvider _searchProvider;

        private const string _searchButtonTooltip = "Search for a Type value.";
        private const string _clearButtonTooltip = "Clear the Type value.";

        private static bool _stylesInitialized;
        private static GUIStyle _buttonStyle;

        private static GUIContent _searchButtonContent;
        private static GUIContent _clearButtonContent;

        private static Type _selectedType = null;

        private void InitStyles()
        {
            _buttonStyle ??= new(GUI.skin.button);

            _searchButtonContent ??= new GUIContent(EditorGUIUtility.IconContent("d_ViewToolZoom"))
            {
                tooltip = _searchButtonTooltip
            };

            _clearButtonContent ??= new GUIContent(EditorGUIUtility.IconContent("d_TreeEditor.Trash"))
            {
                tooltip = _clearButtonTooltip
            };

            _stylesInitialized = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_stylesInitialized)
                InitStyles();

            EditorGUI.BeginProperty(position, GUIContent.none, property);

            position = EditorGUI.PrefixLabel(position, label);

            var assemblyQualifiedTypeNameProperty = property.FindPropertyRelative("_assemblyQualifiedTypeName");
            var typeNameProperty = property.FindPropertyRelative("_typeName");

            if (assemblyQualifiedTypeNameProperty != null && typeNameProperty != null)
            {
                string typeName = assemblyQualifiedTypeNameProperty.stringValue;

                EditorGUILayout.BeginHorizontal();

                if (!string.IsNullOrEmpty(typeName))
                {
                    var textFieldPosition = GUILayoutHelper.TwoButtonFieldLayout(position,
                        out var searchButtonPos,
                        out var clearButtonPos);

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.TextField(textFieldPosition, GUIContent.none, typeNameProperty.stringValue);
                    EditorGUI.EndDisabledGroup();

                    if (GUILayoutHelper.IconButton(searchButtonPos, _searchButtonContent))
                    {
                        ShowTypePicker(type =>
                        {
                            assemblyQualifiedTypeNameProperty.stringValue = type.AssemblyQualifiedName;
                            typeNameProperty.stringValue = type.Name;

                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }

                    if (GUILayoutHelper.IconButton(clearButtonPos, _clearButtonContent))
                    {
                        assemblyQualifiedTypeNameProperty.stringValue = null;
                        typeNameProperty.stringValue = null;

                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                else
                {
                    var textFieldPos = GUILayoutHelper.OneButtonFieldLayout(position, out var buttonPos);

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.TextField(textFieldPos, GUIContent.none, "None");
                    EditorGUI.EndDisabledGroup();

                    if (GUILayoutHelper.IconButton(buttonPos, _searchButtonContent))
                    {
                        ShowTypePicker(type =>
                        {
                            assemblyQualifiedTypeNameProperty.stringValue = type.AssemblyQualifiedName;
                            typeNameProperty.stringValue = type.Name;

                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.EndProperty();
        }

        public void ShowTypePicker(Action<Type> onTypeSelected)
        {
            Vector2 pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

            var searchWindowContext = new SearchWindowContext(pos);

            _searchProvider = TypeSearchProvider.Get();
            _searchProvider.OnTypeSelected = onTypeSelected;

            SearchWindow.Open(searchWindowContext, _searchProvider);
        }

        public class TypeSearchProvider : ScriptableObject, ISearchWindowProvider
        {
            public Action<Type> OnTypeSelected;

            private static TypeSearchProvider _provider;

            public static TypeSearchProvider Get()
            {
                if (_provider == null)
                {
                    _provider = CreateInstance<TypeSearchProvider>();
                }

                return _provider;
            }

            private List<Type> GetTypes()
            {
                var typePaths = new List<Type>();

                foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()))
                {
                    if (typeof(MonoBehaviour).IsAssignableFrom(type) && !type.IsGenericType && !type.IsAbstract)
                    {
                        typePaths.Add(type);
                    }
                }

                return typePaths;
            }

            public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
            {
                var tree = new List<SearchTreeEntry>
                {
                    new SearchTreeGroupEntry(new GUIContent("Select Type"), 0),
                };

                var groups = new List<string>();
                var types = GetTypes();
                
                types.Sort(TypeComparisonFunc);

                foreach (var typePath in types)
                {
                    string[] splits = typePath.FullName!.Split('.');
                    string groupName = "";

                    for (int i = 0; i < splits.Length - 1; i++)
                    {
                        groupName += splits[i];
                        if (!groups.Contains(groupName))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(splits[i]), i + 1));
                            groups.Add(groupName);
                        }

                        groupName += "/";
                    }

                    var entry = new SearchTreeEntry(new GUIContent(splits.Last(), typePath.FullName))
                    {
                        level = splits.Length,
                        userData = typePath.AssemblyQualifiedName
                    };

                    tree.Add(entry);
                }

                return tree;
            }
            
            private static int TypeComparisonFunc(Type a, Type b)
            {
                string[] splits1 = a.FullName!.Split('.');
                string[] splits2 = b.FullName!.Split('.');

                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length)
                    {
                        return 1;
                    }

                    int value = string.Compare(splits1[i], splits2[i], StringComparison.Ordinal);
                    if (value != 0)
                    {
                        // Make sure that leaves go before nodes
                        if (splits1.Length != splits2.Length &&
                            (i == splits1.Length - 1 || i == splits2.Length - 1))
                            return splits1.Length < splits2.Length ? 1 : -1;

                        return value;
                    }
                }

                return 0;
            }

            public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
            {
                if (entry.userData is string typeName)
                {
                    _selectedType = Type.GetType(typeName);
                    OnTypeSelected?.Invoke(_selectedType);
                }

                return true;
            }
        }
    }
}