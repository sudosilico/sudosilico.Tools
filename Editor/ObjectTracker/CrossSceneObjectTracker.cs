using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace sudosilico.Tools
{
    [Overlay(typeof(SceneView), "Object Tracker")]
    public class CrossSceneObjectTracker : Overlay
    {
        private static GUIContent _eyeIcon = EditorGUIUtility.IconContent("animationvisibilitytoggleon");
        private static GUIContent _revertIcon = EditorGUIUtility.IconContent("preAudioLoopOff");
        private static GUIContent _trashIcon = EditorGUIUtility.IconContent("TreeEditor.Trash");

        private VisualElement _root;
        private VisualElement _rowContainer;
        private Button _pinSelectionButton;
        private Button _viewPinnedObjectButton;

        private static Dictionary<string, Texture2D> _previewMap = new Dictionary<string, Texture2D>(); 

        public override VisualElement CreatePanelContent()
        {
            _root = new VisualElement() { name = "Object Tracker Root" };

            _pinSelectionButton = new Button
            {
                text = GetPinButtonText()
            };

            if (Selection.activeGameObject != null)
            {
                _pinSelectionButton.SetEnabled(!IsTracked(Selection.activeGameObject));
            }

            _pinSelectionButton.clicked += PinSelectionButtonOnClicked;

            _root.Add(_pinSelectionButton);

            _rowContainer = new VisualElement();

            _root.Add(_rowContainer);

            UpdateRows();

            return _root;
        }

        public void UpdateRows()
        {
            _rowContainer.Clear();

            var state = GetObjectTrackerState();

            foreach (TrackedObject trackedObject in state.TrackedObjects)
            {
                var row = CreateRow(trackedObject);
                _rowContainer.Add(row);
            }
            
            if (Selection.activeGameObject != null)
            {
                _pinSelectionButton.SetEnabled(!IsTracked(Selection.activeGameObject));
            }

            _rowContainer.MarkDirtyRepaint();
        }

        public string GetPinButtonText()
        {
            var selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject == null)
            {
                return null;
            }

            return $"Pin {selectedGameObject.name}...";
        }

        public VisualElement CreateRow(TrackedObject trackedObject)
        {
            var viewButton = new Button();
            var eyeImage = new Image { image = _eyeIcon.image };
            viewButton.contentContainer.Add(eyeImage);
            viewButton.clicked += () =>
            {   
                bool save = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                if (save)
                {
                    EditorSceneManager.OpenScene(trackedObject.Scene.GetSceneAssetPath());
                    var match = UniqueIDManager.GetGameObject(trackedObject.ID);
                    if (match != null)
                    {
                        Selection.activeGameObject = match;

                        var activeScene = SceneView.lastActiveSceneView;

                        var pivot = activeScene.pivot;
                        var matchPos = match.transform.position;
                        
                        pivot.x = matchPos.x;
                        pivot.y = matchPos.y;

                        if (!activeScene.in2DMode)
                        {
                            pivot.z = matchPos.z;
                        }
                        else
                        {
                            activeScene.size = 7;                            
                        }
                        
                        activeScene.pivot = pivot;
                    }
                    else
                    {
                        Debug.LogError("Could not find game object with ID: " + trackedObject.ID);
                    }
                }
            };

            var removeButton = new Button();
            var trashImage = new Image { image = _trashIcon.image };
            removeButton.contentContainer.Add(trashImage);
            removeButton.clicked += () =>
            {
                UnpinObject(trackedObject.ID);
            };

            var row = new VisualElement();

            row.style.flexDirection = FlexDirection.Row;

            var rowLabel = new Button()
            {
                text = trackedObject.Name,
            };
            rowLabel.SetEnabled(false);

            rowLabel.style.flexGrow = 1f;

            row.Add(rowLabel);

            row.Add(viewButton);
            row.Add(removeButton);

            return row;
        }

        private void PinSelectionButtonOnClicked()
        {
            var sel = Selection.activeGameObject;

            if (sel != null)
            {
                PinObject(sel);
            }
        }

        public bool IsTracked(GameObject obj)
        {
            var state = GetObjectTrackerState();
            
            var trackedID = obj.GetComponent<UniqueIDComponent>();
            if (trackedID == null)
            {
                return false;
            }
            
            foreach (var trackedObject in state.TrackedObjects)
            {
                if (!trackedID.ID.IsEmpty() && trackedID.ID.Equals(trackedObject.ID))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public void PinObject(GameObject obj)
        {
            var state = GetObjectTrackerState();

            var trackedID = obj.GetComponent<UniqueIDComponent>();
            if (trackedID == null)
            {
                trackedID = obj.AddComponent<UniqueIDComponent>();
                trackedID.ID = UniqueID.Generate();
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssetIfDirty(obj);
            }

            var sceneAssetPath = SceneManager.GetActiveScene().path;
            var sceneReference = SceneReference.FromAssetPath(sceneAssetPath);

            if (!IsTracked(obj))
            {
                TrackedObject to = new TrackedObject(sceneReference, trackedID.ID, obj.name);
                state.TrackedObjects.Add(to);
                EditorUtility.SetDirty(state);
                AssetDatabase.SaveAssetIfDirty(state);
            }

            UpdateRows();
        }

        public void UnpinObject(UniqueID uniqueID)
        {
            var state = GetObjectTrackerState();

            state.TrackedObjects =
                state.TrackedObjects
                     .Where(to => !to.ID.IsEmpty()
                                  && !to.ID.Equals(uniqueID))
                     .ToList();

            EditorUtility.SetDirty(state);
            AssetDatabase.SaveAssetIfDirty(state);
            UpdateRows();
        }

        public const string ObjectTrackerStatePath = "Assets/Settings/ObjectTrackerState.asset";

        private ObjectTrackerState _state;

        private ObjectTrackerState GetObjectTrackerState()
        {
            if (_state != null)
            {
                return _state;
            }

            var loadedState = AssetDatabase.LoadAssetAtPath<ObjectTrackerState>(ObjectTrackerStatePath);

            if (loadedState != null)
            {
                _state = loadedState;
                return _state;
            }

            var defaultState = ScriptableObject.CreateInstance<ObjectTrackerState>();
            Debug.Log("ObjectTrackerState not found, creating default state.");
            AssetDatabase.CreateAsset(defaultState, ObjectTrackerStatePath);

            _state = defaultState;
            return _state;
        }

        public Texture2D GetPreview(GameObject gameObject, string path)
        {
            var editor = Editor.CreateEditor(gameObject);
            var tex = editor.RenderStaticPreview(path, null, 256, 256);
            Object.DestroyImmediate(editor);
            return tex;
        }
        
        public override void OnCreated()
        {
            Selection.selectionChanged += SelectionChanged;

            base.OnCreated();
        }

        public override void OnWillBeDestroyed()
        {
            Selection.selectionChanged -= SelectionChanged;

            base.OnWillBeDestroyed();
        }

        private void SelectionChanged()
        {
            if (_pinSelectionButton == null)
            {
                return;
            }
            
            var sel = Selection.activeGameObject;

            if (sel != null)
            {
                _pinSelectionButton.text = GetPinButtonText();
                _pinSelectionButton.SetEnabled(!IsTracked(sel));
            }
            else
            {
                _pinSelectionButton.text = "Select a GameObject to pin...";
                _pinSelectionButton.SetEnabled(false);
            }
        }
    }
}