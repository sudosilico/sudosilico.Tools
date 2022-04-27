using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace sudosilico.Tools.Elements
{
    public class UniqueIDManagerListView : ListView
    {
        private UniqueIDManagerWindow _window;

        private List<string> _trackedGameObjectNames = new();
        private List<GameObject> _trackedGameObjects = new();
        
        public void UpdateTrackedGameObjects()
        {
            _trackedGameObjects.Clear();
            _trackedGameObjectNames.Clear();
            
            foreach (var component in UniqueIDManager.Components.Values)
            {
                // null check to prevent errors when switching scenes with the window open
                if (component != null)
                {
                    _trackedGameObjects.Add(component.gameObject);
                    _trackedGameObjectNames.Add(component.gameObject.name);
                }
            }
            
            itemsSource = _trackedGameObjects.ToList();
            Rebuild();
            MarkDirtyRepaint();
        }
        
        public void SetWindow(UniqueIDManagerWindow window)
        {
            _window = window;
            
            makeItem = MakeItem;
            bindItem = BindItem;
            itemsSource = _trackedGameObjects;
            selectionType = SelectionType.Single;
            onSelectionChange += OnSelectionChange;
            fixedItemHeight = 16;
        }
        
        private void OnSelectionChange(IEnumerable<object> changedObjects)
        {
            var go = changedObjects.FirstOrDefault() as GameObject;

            if (go != null && _window != null)
            {
                _window.SelectGameObject(go);
            }
        }

        private void BindItem(VisualElement visualElement, int i)
        {
            if (visualElement is Label label)
            {
                label.text = _trackedGameObjectNames[i];
            }
        }
            
        private VisualElement MakeItem()
        {
            var label = new Label();
            return label;
        }
        
        public new class UxmlFactory : UxmlFactory<UniqueIDManagerListView, VisualElement.UxmlTraits>
        {
        }
    }
}