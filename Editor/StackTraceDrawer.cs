namespace GenericScriptableArchitecture.Editor
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.Helpers;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class StackTraceDrawer
    {
        private static readonly ContentCache _contentCache = new ContentCache();

        private readonly IStackTraceProvider _target;
        private readonly IRepaintable _editor;

        private StackTraceEntry _selectedTrace;

        private Rect _lastContentRect;
        private Vector2 _listScrollPos;
        private Vector2 _selectedContentScrollPos;
        private float _windowsHeightRatio = 0.6f;
        private bool _resizeMouseDown;

        public StackTraceDrawer([NotNull] IStackTraceProvider target, [NotNull] IRepaintable editor)
        {
            _target = target;
            _editor = editor;
        }

        public void Draw()
        {
            const float contentRectHeight = 400;
            const float headerHeight = 18;

            EditorGUILayout.Space();
            using var verticalBlock = new DrawHelper.VerticalBlock(null);
            Rect headerRect = GUILayoutUtility.GetRect(0f, headerHeight, GUILayout.ExpandWidth(true));
            Rect contentRect = GUILayoutUtility.GetRect(10f, _target.Expanded ? contentRectHeight : 0f, GUILayout.ExpandWidth(true));
            DrawHeader(headerRect);
            DrawContent(contentRect);
        }

        private void DrawHeader(Rect headerRect)
        {
            if (Event.current.type == EventType.Repaint)
                ReorderableList.defaultBehaviours.DrawHeaderBackground(headerRect);

            const float padding = 6f;

            headerRect.xMin += padding;
            headerRect.xMax -= padding;
            headerRect.height -= 2f;
            headerRect.y += 1;

            DrawFoldout(headerRect);
        }

        private void DrawFoldout(Rect headerRect)
        {
            const float buttonWidth = 60f;
            const float leftMargin = 10f;
            var shiftedRightHeaderRect = new Rect(headerRect.x + leftMargin, headerRect.y, headerRect.width - leftMargin, headerRect.height);

            var buttonRect = new Rect(
                shiftedRightHeaderRect.xMax - buttonWidth,
                shiftedRightHeaderRect.y + 1f,
                buttonWidth,
                shiftedRightHeaderRect.height);

            if (GUI.Button(buttonRect, "Clear"))
            {
                _target.Entries.Clear();
                _selectedTrace = null;
            }

            _target.Expanded = EditorGUI.Foldout(shiftedRightHeaderRect, _target.Expanded, "Stack Trace", true);
        }

        private void DrawContent(Rect contentRect)
        {
            if (!_target.Expanded)
                return;

            if (Event.current.type == EventType.Repaint)
            {
                _lastContentRect = contentRect;
                Styles.BoxBackground.Draw(_lastContentRect, false, false, false, false);
            }

            GUILayout.BeginArea(_lastContentRect);

            const float resizeMargin = 0.2f;

            _windowsHeightRatio = Mathf.Clamp(_windowsHeightRatio, resizeMargin, 1 - resizeMargin);

            var splitHeight = _lastContentRect.height * _windowsHeightRatio;
            contentRect = new Rect(-1, splitHeight, _lastContentRect.width + 1, _lastContentRect.height - splitHeight);

            DrawList(contentRect);
            HandleSelectedPartResize(splitHeight);
            DrawSelectedContent(contentRect);

            GUILayout.EndArea();
        }

        private void DrawList(Rect contentRect)
        {
            const float lineHeight = 18f;

            var listRect = new Rect
            {
                height = _lastContentRect.height - contentRect.height,
                width = _lastContentRect.width
            };

            var scrollViewRect = new Rect
            {
                y = listRect.y,
                height = listRect.height,
                width = listRect.width,
            };

            var position = new Rect
            {
                height = _target.Entries.Count * lineHeight,
                width = scrollViewRect.width - 20,
            };

            _listScrollPos = GUI.BeginScrollView(scrollViewRect, _listScrollPos, position);

            int i = 0;

            foreach (var stackTraceEntry in _target.Entries)
            {
                string currentText = GetFirstLine(stackTraceEntry);

                var elementRect = new Rect
                {
                    width = listRect.width,
                    height = lineHeight,
                    y = i * lineHeight,
                    x = listRect.x
                };

                if (Event.current.type == EventType.MouseDown && elementRect.Contains(Event.current.mousePosition))
                {
                    _selectedTrace = stackTraceEntry;
                    Repaint();
                }
                else if (Event.current.type == EventType.Repaint)
                {
                    bool isSelected = _selectedTrace == stackTraceEntry;
                    Styles.Background.Draw(elementRect, false, false, isSelected, false);
                    Styles.Text.Draw(elementRect, new GUIContent(currentText), 0);
                    DrawSeparator(elementRect);
                }

                i++;
            }

            GUI.EndScrollView();
        }

        private void DrawSeparator(Rect rect)
        {
            var lineRect = new Rect(rect.x, rect.y - 1f, rect.width, 1f);
            EditorGUI.DrawRect(lineRect, Styles.DarkSeparatorLine);
            ++lineRect.y;
            EditorGUI.DrawRect(lineRect, Styles.LightSeparatorLine);
        }

        private void DrawSelectedContent(Rect contentRect)
        {
            if (_selectedTrace == null)
                return;

            var scrollViewRect = new Rect
            {
                y = contentRect.y,
                height = contentRect.height,
                width = contentRect.width - 1,
            };

            var textWidth = scrollViewRect.width - 14;

            var lines = _selectedTrace.ToString().Split('\n');
            var lineHeights = lines.Select(line => Styles.MessageStyle.CalcHeight(_contentCache.GetItem(line), textWidth)).ToList();

            var listRect = new Rect(0f, 0f, textWidth, lineHeights.Sum());
            _selectedContentScrollPos = GUI.BeginScrollView(scrollViewRect, _selectedContentScrollPos, listRect);

            var currentYPos = 0f;

            for (int i = 0; i < lines.Length; i++)
            {
                var lineHeight = lineHeights[i];
                var lineRect = new Rect(0f, currentYPos, textWidth, lineHeight);
                currentYPos += lineHeight;

                EditorGUI.SelectableLabel(lineRect, lines[i], Styles.MessageStyle);
                DrawSeparator(lineRect);
            }

            GUI.EndScrollView();
        }

        private void HandleSelectedPartResize(float splitHeight)
        {
            const float resizeHeight = 4f;

            var cursorRect = new Rect(0, splitHeight - resizeHeight / 2, _lastContentRect.width, resizeHeight);
            Event currentEvent = Event.current;

            EditorGUIUtility.AddCursorRect(cursorRect, MouseCursor.ResizeVertical);

            var lineRect = new Rect(0, splitHeight - resizeHeight / 4, _lastContentRect.width, resizeHeight / 2);
            EditorGUI.DrawRect(lineRect, Styles.DarkSeparatorLine);

            if (currentEvent.type == EventType.MouseDown && cursorRect.Contains(currentEvent.mousePosition))
            {
                _resizeMouseDown = true;
            }
            else if (currentEvent.type == EventType.MouseUp && _resizeMouseDown)
            {
                _resizeMouseDown = false;
            }
            else if (_resizeMouseDown)
            {
                _windowsHeightRatio = currentEvent.mousePosition.y / _lastContentRect.height;
                Repaint();
            }
        }

        private static string GetFirstLine(string value)
        {
            return Regex.Match(value, @".*?(\r|\n)").Value;
        }

        private void Repaint() => _editor.Repaint();

        private static class Styles
        {
            public static readonly GUIStyle Text = new GUIStyle("CN EntryInfo") { padding = new RectOffset(2, 2, 3, 0) };
            public static readonly GUIStyle Background = new GUIStyle("CN EntryBackodd");
            public static readonly GUIStyle MessageStyle = "CN Message";
            public static readonly GUIStyle BoxBackground = "RL Background";

            private static readonly Color _darkSeparatorLineDarkSkin = new Color(0.11f, 0.11f, 0.11f, 0.258f);
            private static readonly Color _darkSeparatorLineLightSkin = new Color(0.0f, 0.0f, 0.0f, 0.065f);

            private static readonly Color _lightSeparatorLineDarkSkin = new Color(1f, 1f, 1f, 0.033f);
            private static readonly Color _lightSeparatorLineLightSkin = new Color(1f, 1f, 1f, 0.323f);

            public static Color DarkSeparatorLine => DarkSkin ? _darkSeparatorLineDarkSkin : _darkSeparatorLineLightSkin;

            public static Color LightSeparatorLine => DarkSkin ? _lightSeparatorLineDarkSkin : _lightSeparatorLineLightSkin;

            private static bool DarkSkin => EditorGUIUtility.isProSkin;
        }
    }
}