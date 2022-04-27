using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace sudosilico.Tools
{
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField]
        private SceneAsset _sceneAsset;
#endif
        
        [SerializeField]
        private string _sceneAssetPath;
        
        [SerializeField]
        private string _sceneName;

        [SerializeField]
        private int _sceneIndex = -1;

        [SerializeField]
        private bool _sceneEnabled = false;

        public static SceneReference FromAssetPath(string assetPath)
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);

            if (sceneAsset == null) 
                return null;
            
            string sceneAssetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            var sceneEnabled = false;
            var sceneIndex = -1;
            
            for (int i = 0; i < scenes.Length; i++)
            {
                var scene = scenes[i];
                    
                if (scene.guid.ToString() == sceneAssetGUID)
                {   
                    sceneIndex = i;
                    sceneEnabled = scene.enabled;
                    break;
                }
            }
            
            var sceneReference = new SceneReference
            {
                _sceneAsset = sceneAsset,
                _sceneAssetPath = assetPath,
                _sceneName = sceneAsset.name,
                _sceneIndex = sceneIndex,
                _sceneEnabled = sceneEnabled,
            };

            return sceneReference;
        }
        
#if UNITY_EDITOR
        public string GetSceneAssetPath() => _sceneAssetPath;
#endif

        public string GetSceneName() => _sceneName;

        private void ValidateScene()
        {
            if (string.IsNullOrEmpty(_sceneName))
                throw new SceneLoadException("No scene specified.");
            
            if (_sceneIndex < 0)
                throw new SceneLoadException($"Scene \'{_sceneName}\' is not in the build settings");
            
            if (!_sceneEnabled)
                throw new SceneLoadException($"Scene \'{_sceneName}\' is not enabled in the build settings");
        }
        
        public void LoadScene(LoadSceneMode mode = LoadSceneMode.Single)
        {
            ValidateScene();
            SceneManager.LoadScene(_sceneName, mode);
        }

        public AsyncOperation LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Single)
        {
            ValidateScene();
            return SceneManager.LoadSceneAsync(_sceneName, mode);
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (_sceneAsset != null)
            {
                _sceneAssetPath = AssetDatabase.GetAssetPath(_sceneAsset);
                string sceneAssetGUID = AssetDatabase.AssetPathToGUID(_sceneAssetPath);
                
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

                _sceneIndex = -1;

                int i = 0;
                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (scene.guid.ToString() == sceneAssetGUID)
                    {   
                        _sceneIndex = i;
                        _sceneEnabled = scene.enabled;

                        if (scene.enabled)
                            _sceneName = _sceneAsset.name;
                            
                        break;
                    }

                    i++;
                }
            }
            else
            {
                _sceneName = "";
                _sceneAssetPath = "";
            }
#endif
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR 
            EditorApplication.update += HandleAfterDeserialize;
#endif
        }

#if UNITY_EDITOR
        private void HandleAfterDeserialize()
        {
            EditorApplication.update -= HandleAfterDeserialize;

            if (_sceneAsset == null)
            {
                if (!string.IsNullOrEmpty(_sceneAssetPath))
                {
                    var sceneAsset = GetSceneAssetFromPath();
            
                    if (sceneAsset == null)
                    {
                        _sceneAssetPath = "";
                    }
            
                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkAllScenesDirty();
                    }
                }
            }
        }

        private SceneAsset GetSceneAssetFromPath()
        {
            return string.IsNullOrEmpty(_sceneAssetPath)
                       ? null
                       : AssetDatabase.LoadAssetAtPath<SceneAsset>(_sceneAssetPath);
        }

        private string GetScenePathFromAsset()
        {
            return (_sceneAsset == null) ? "" : AssetDatabase.GetAssetPath(_sceneAsset);
        }
#endif

        /// <summary>
        /// Exception that is raised when there is an issue resolving and
        /// loading a scene reference.
        /// </summary>
        public class SceneLoadException : Exception
        {
            public SceneLoadException(string message) 
                : base(message)
            {
            }
        }
    }
}