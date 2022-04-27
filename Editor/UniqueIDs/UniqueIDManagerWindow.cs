using System;
using System.Collections.Generic;
using sudosilico.Tools.Elements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace sudosilico.Tools
{
    public class UniqueIDManagerWindow : EditorWindow
    {
        private UniqueIDManagerListView _list;
        private UniqueIDManagerInspectorView _inspector;
        private IMGUIContainer _inspectorImguiContainer;
        
        private Label _trackedObjectsLabel;
        private Label _selectedIDLabel;
        private ScrollView _scrollView;

        private GameObject _currentSelectedGameObject;
        
        private Editor _currentEditor;
        private List<Editor> _componentEditors;

        [MenuItem("Tools/UniqueID Manager")]
        public static void ShowExample()
        {
            var nextTo = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            UniqueIDManagerWindow wnd = GetWindow<UniqueIDManagerWindow>("UniqueID Manager", nextTo);
            wnd.Show();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree =
                AssetDatabase
                    .LoadAssetAtPath<VisualTreeAsset
                    >("Packages/sudosilico.tools/Editor/UniqueIDs/UniqueIDManagerWindow.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            _list = Query<UniqueIDManagerListView>("list-view");
            _inspector = Query<UniqueIDManagerInspectorView>("inspector-view");
            _trackedObjectsLabel = Query<Label>("tracked-objects-label");
            _selectedIDLabel = Query<Label>("selected-id-label");
            _scrollView = Query<ScrollView>("scrollview");

            _inspector.Clear();
            _inspectorImguiContainer = new IMGUIContainer(InspectorViewOnGUI);
            _inspector.Add(_inspectorImguiContainer);

            _list.SetWindow(this);
            _list.UpdateTrackedGameObjects();

            UpdateLabels();
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

        private void UpdateLabels()
        {
            _trackedObjectsLabel.text = $"{UniqueIDManager.Components.Values.Count} tracked GameObjects";
        }

        private T Query<T>(string query)
            where T : VisualElement
        {
            return rootVisualElement.Q<T>(query)
                   ?? throw new InvalidOperationException($"Could not locate '{query}' in uxml.");
        }

        public void SelectGameObject(GameObject gameObject)
        {
            _selectedIDLabel.text =
                gameObject.GetComponent<UniqueIDComponent>()?.ID.ToString() ?? "No UniqueID Component";

            if (_currentEditor != null)
            {
                DestroyImmediate(_currentEditor);
                _currentEditor = null;
            }

            // clean up old editors
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
            
            _currentEditor = Editor.CreateEditor(gameObject);
            
            // load editors from gameObject
            foreach (var component in gameObject.GetComponents<Component>())
            {
                _componentEditors.Add(Editor.CreateEditor(component));
            }
        }

        private void OnEnable()
        {
            UniqueIDManager.OnComponentAdded += OnIDAdded;
            UniqueIDManager.OnComponentRemoved += OnIDRemoved;

            if (_list != null)
            {
                _list.SetWindow(this);
                _list.UpdateTrackedGameObjects();
            }
        }

        private void OnDisable()
        {
            UniqueIDManager.OnComponentAdded -= OnIDAdded;
            UniqueIDManager.OnComponentRemoved -= OnIDRemoved;
        }

        private void OnIDAdded(UniqueID id)
        {
            _list?.UpdateTrackedGameObjects();
            UpdateLabels();
        }

        private void OnIDRemoved(UniqueID id)
        {
            _list?.UpdateTrackedGameObjects();
            UpdateLabels();
        }
    }
}