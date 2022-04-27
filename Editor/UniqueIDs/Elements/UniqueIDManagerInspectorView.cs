using UnityEngine;
using UnityEngine.UIElements;

namespace sudosilico.Tools.Elements
{
    public class UniqueIDManagerInspectorView : VisualElement
    {
        public UniqueIDManagerInspectorView()
        {
        }

        public new class UxmlFactory : UxmlFactory<UniqueIDManagerInspectorView, VisualElement.UxmlTraits>
        {
        }
    }
}