using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace sudosilico.Tools
{
    public class UniqueIDManagerWindow_Old : EditorWindow
    {
        public static void ShowWindow()
        {
            var type = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            var window = GetWindow<UniqueIDManagerWindow_Old>("UniqueID Manager", type);
            
            window.Show();
        }

        private ListView _listView;
        private IMGUIContainer _imguiContainer;
        private ScrollView _gameObjectScrollView;
        private TwoPaneSplitView _splitView;

        private TwoPaneSplitView CreateVerticalSplitView(VisualElement top, 
                                                         VisualElement bottom, 
                                                         out VisualElement topContainer, 
                                                         out VisualElement bottomContainer)
        {
            var splitView = new TwoPaneSplitView  { orientation = TwoPaneSplitViewOrientation.Vertical };

            topContainer = new VisualElement();
            topContainer.Add(top);
            splitView.Add(topContainer);

            bottomContainer = new VisualElement();
            bottomContainer.Add(bottom);
            splitView.Add(bottomContainer);
            
            return splitView;
        }
        
        private void OnEnable()
        {
            var root = rootVisualElement;

            _listView = CreateListView();
            
            _gameObjectScrollView = new ScrollView(ScrollViewMode.Vertical);
            
            _imguiContainer = new IMGUIContainer(InspectorViewOnGUI);
            _gameObjectScrollView.Add(_imguiContainer);
            SetUpListViewStyle(_gameObjectScrollView.style);

            _splitView = CreateVerticalSplitView(_listView, _gameObjectScrollView, 
                                                 out var topContainer, out var bottomContainer);
            root.Add(_splitView);

            topContainer.style.minHeight = 200;
            bottomContainer.style.height = StyleKeyword.Auto;
            bottomContainer.style.minHeight = 100;
            
            UniqueIDManager.OnComponentAdded += OnIDAdded;
            UniqueIDManager.OnComponentRemoved += OnIDRemoved;
        }

        private void InspectorViewOnGUI()
        {
            if (_currentEditor != null)
            {
                _currentEditor.DrawHeader();
            }
            
            if (_componentEditors != null)
            {
                for (int index = 0; index < _componentEditors.Count; index++)
                {
                    var e = _componentEditors[index];

                    EditorGUILayout.Space();
                    
                    e.DrawHeader();
                    e.OnInspectorGUI();
                }
                
                EditorGUILayout.Space();
            }
        }
        
        private ListView CreateListView()
        {
            var listView = new ListView();
            SetUpListViewStyle(listView.style);
            
            // Populate List View
            listView.Clear();
            listView.makeItem = MakeItem;
            listView.bindItem = BindItem;
            listView.itemsSource = GetItemNames();
            listView.selectionType = SelectionType.Single;
            listView.onSelectedIndicesChange += OnSelectionChanged;

            return listView;
        }
        private string GetCountLabelString() => $"Dictionary contains {UniqueIDManager.Components.Count} references.";

        private void SetUpListViewStyle(IStyle style)
        {
            float margin = 10;
            style.marginLeft = margin;
            style.marginTop = margin;
            style.marginRight = margin;
            style.marginBottom = margin;

            float padding = 5;
            style.paddingLeft = padding;
            style.paddingTop = padding;
            style.paddingRight = padding;
            style.paddingBottom = padding;

            var borderColor = new StyleColor(Color.black);
            var borderColorValue = borderColor.value;
            borderColorValue.a = 0.3f;
            borderColor.value = borderColorValue;

            style.borderBottomColor = borderColor;
            style.borderTopColor = borderColor;
            style.borderLeftColor = borderColor;
            style.borderRightColor = borderColor;

            float borderWidth = 1f;
            style.borderTopWidth = borderWidth;
            style.borderBottomWidth = borderWidth;
            style.borderLeftWidth = borderWidth;
            style.borderRightWidth = borderWidth;
        }

        private void OnDisable()
        {
            UniqueIDManager.OnComponentAdded -= OnIDAdded;
            UniqueIDManager.OnComponentRemoved -= OnIDRemoved;
            
            DisposeEditors();
        }
        
        private void OnIDAdded(UniqueID id)
        {
            _listView.itemsSource = GetItemNames();
            _listView.Rebuild();
        }
        
        private void OnIDRemoved(UniqueID id)
        {
            _listView.itemsSource = GetItemNames();
            _listView.Rebuild();
        }

        private List<string> _itemNames = new();
        private List<GameObject> _itemObjects = new();
        
        private List<string> GetItemNames()
        {
            _itemNames.Clear();
            _itemObjects.Clear();
            
            foreach (var kvp in UniqueIDManager.Components)
            {
                UniqueID id = kvp.Key;

                if (id == null)
                {
                    Debug.LogError("UniqueIDManager.Components contains a null ID.");
                }
                else if (id.IsEmpty())
                {
                    Debug.LogError("UniqueIDManager.Components contains an empty ID.");
                }
                
                GameObject go = kvp.Value.gameObject;

                _itemNames.Add($"{go.name}\n{id}");
                _itemObjects.Add(go);
            }
            
            return _itemNames;
        }

        private Editor _currentEditor;
        private List<Editor> _componentEditors;
        
        private void OnSelectionChanged(IEnumerable<int> indices)
        {
            foreach (var index in indices)
            {
                var gameObject = _itemObjects[index];

                DisposeEditors();

                _currentEditor = Editor.CreateEditor(gameObject);

                foreach (var component in gameObject.GetComponents<Component>())
                {
                    _componentEditors.Add(Editor.CreateEditor(component));
                }
            }
        }

        private void DisposeEditors()
        {
            if (_currentEditor != null)
            {
                DestroyImmediate(_currentEditor);
                _currentEditor = null;
            }
            
            if (_componentEditors != null)
            {
                foreach (var editor in _componentEditors)
                {
                    DestroyImmediate(editor);
                }
                
                _componentEditors.Clear();
            }
            else
            {
                _componentEditors = new List<Editor>();                
            }
        }
        
        private void BindItem(VisualElement visualElement, int index)
        {
            if (visualElement is Label label)
            {
                label.text = _itemNames[index];
                label.userData = _itemObjects[index];
            }
        }
            
        private VisualElement MakeItem() => new Label();
    }
}