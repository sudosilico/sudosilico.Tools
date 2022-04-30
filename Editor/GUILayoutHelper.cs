using UnityEngine;

namespace sudosilico.Tools
{
    public static class GUILayoutHelper
    {
        private static int _buttonWidth = 20;
        private static GUIStyle _iconButtonStyle;
        
        public static Rect OneButtonFieldLayout(Rect position, out Rect buttonPos)
        {
            var textFieldPos = position;
            textFieldPos.width -= _buttonWidth;

            buttonPos = position;
            buttonPos.x += textFieldPos.width;
            buttonPos.width = _buttonWidth;
            
            return textFieldPos;
        }
        
        public static Rect TwoButtonFieldLayout(Rect position, out Rect button1Pos, out Rect button2Pos)
        {
            var textFieldPosition = position;
            textFieldPosition.width -= (_buttonWidth * 2);

            button1Pos = position;
            button1Pos.x += textFieldPosition.width;
            button1Pos.width = _buttonWidth;
                    
            button2Pos = button1Pos;
            button2Pos.x += _buttonWidth;   
            
            return textFieldPosition;
        }

        public static bool IconButton(Rect position, GUIContent content)
        {
            _iconButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(0, 0, 0, 0)
            };
            
            return GUI.Button(position, content, _iconButtonStyle);
        }
    }
}