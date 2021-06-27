namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Linq;
    using JetBrains.Annotations;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditorInternal;
    using UnityEngine;

    internal class StackTraceDrawer
    {
        private const float Height = 400;
        private const float HeaderHeight = 18;
        private const float ResizeMargin = 0.2f;
        private const float ResizeHeight = 4;
        private const float ClearLeftPadding = 6;
        private const float ClearWidth = 45;
        private const float CollapseWidth = 55;
        private const float LineHeight = 18;

        private readonly IStackTraceProvider _target;
        private readonly IRepaintable _editor;

        private StackTraceEntry _selectedTrace;

        private Rect _lastContentRect;
        private Vector2 _listScrollPosition;
        private Vector2 _contentScrollPosition;
        private float _subWindowValue = 0.6f;
        private bool _resizeMouseDown;

        public StackTraceDrawer([NotNull] IStackTraceProvider target, [NotNull] IRepaintable editor, bool startCollapsed = false)
        {
            _target = target;
            _editor = editor;
        }

        public void Draw()
        {
            EditorGUILayout.Space();

            GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());

            Rect headerRect = GUILayoutUtility.GetRect(0f, HeaderHeight, GUILayout.ExpandWidth(true));
            Rect contentRect = GUILayoutUtility.GetRect(10f, _target.Expanded ? Height : 0f, GUILayout.ExpandWidth(true));
            DrawHeader(headerRect);
            DrawContent(contentRect);

            GUILayout.EndVertical();
        }

        #region ReorederableList

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

        private const string HeaderTitle = "Stack Trace";

        private void DrawFoldout(Rect headerRect)
        {
            const float leftMargin = 10f;
            var shiftedRight = new Rect(headerRect.x + leftMargin, headerRect.y, headerRect.width - leftMargin, headerRect.height);
            _target.Expanded = EditorGUI.Foldout(shiftedRight, _target.Expanded, HeaderTitle, true);
        }

        #endregion

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

            _subWindowValue = Mathf.Clamp(_subWindowValue, ResizeMargin, 1 - ResizeMargin);

            var splitHeight = _lastContentRect.height * _subWindowValue;
            contentRect = new Rect(-1, splitHeight, _lastContentRect.width + 1, _lastContentRect.height - splitHeight);

            DrawList(contentRect);
            HandleSelectedPartResize(splitHeight);
            DrawSelectedContent(contentRect);

            GUILayout.EndArea();
        }

        private void DrawList(Rect contentRect)
        {
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
                height = _target.Entries.Count * LineHeight,
                width = scrollViewRect.width - 20,
            };

            _listScrollPosition = GUI.BeginScrollView(scrollViewRect, _listScrollPosition, position);

            int i = 0;

            foreach (var stackTraceEntry in _target.Entries)
            {
                string currentText = GetFirstLine(stackTraceEntry);

                var elementRect = new Rect
                {
                    width = listRect.width,
                    height = LineHeight,
                    y = i * LineHeight,
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

            Vector2 textSize = Styles.MessageStyle.CalcSize(new GUIContent(_selectedTrace));

            var position = new Rect(Vector2.zero, textSize);

            _contentScrollPosition = GUI.BeginScrollView(scrollViewRect, _contentScrollPosition, position);

            // TODO: Try using ConsoleWindow.StackTraceWithHyperLinks(string stackTrace)
            // If successful, embed it into StackTraceEntry
            EditorGUI.SelectableLabel(position, _selectedTrace, Styles.MessageStyle);

            GUI.EndScrollView();
        }

        private void HandleSelectedPartResize(float splitHeight)
        {
            var cursorRect = new Rect(0, splitHeight - ResizeHeight / 2, _lastContentRect.width, ResizeHeight);
            Event currentEvent = Event.current;

            EditorGUIUtility.AddCursorRect(cursorRect, MouseCursor.ResizeVertical);

            var lineRect = new Rect(0, splitHeight - ResizeHeight / 4, _lastContentRect.width, ResizeHeight / 2);
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
                _subWindowValue = currentEvent.mousePosition.y / _lastContentRect.height;
                Repaint();
            }
        }

        // private void DrawStackTraceHeader()
        // {
        //     var rect = new Rect
        //     {
        //         width = _stackTraceRect.width,
        //         height = HeaderHeight,
        //     };
        //
        //     if (Event.current.type == EventType.Repaint)
        //     {
        //         Styles.Header.Draw(rect, new GUIContent("Stack Trace"), 0);
        //     }
        //
        //     rect.x += ClearLeftPadding;
        //     rect.width = ClearWidth;
        //
        //     if (GUI.Button(rect, new GUIContent("Clear"), Styles.HeaderButton))
        //     {
        //         _target.Entries.Clear();
        //         Deselect();
        //     }
        //
        //     rect.x += ClearWidth;
        //     rect.width = CollapseWidth;
        //
        //     _collapseAnimation.target = !GUI.Toggle(rect, !_collapseAnimation.target, new GUIContent("Collapse"), Styles.HeaderButton);
        // }

        private void Repaint() => _editor.Repaint();

        private static string GetFirstLine(string value)
        {
            return value.Split('\r', '\n').FirstOrDefault();
        }
        private void Deselect()
        {
            _selectedTrace = null;
        }

        private static class Styles
        {
            public static readonly GUIStyle Text = new GUIStyle("CN EntryInfo") { padding = new RectOffset(2, 2, 3, 0)};
            public static readonly GUIStyle Background = new GUIStyle("CN EntryBackodd");
            public static readonly GUIStyle MessageStyle = "CN Message";
            public static readonly GUIStyle BoxBackground = "RL Background";

            private static readonly Color DarkSeparatorLineDarkSkin = new Color(0.11f, 0.11f, 0.11f, 0.258f);
            private static readonly Color DarkSeparatorLineLightSkin = new Color(0.0f, 0.0f, 0.0f, 0.065f);

            private static readonly Color LightSeparatorLineDarkSkin = new Color(1f, 1f, 1f, 0.033f);
            private static readonly Color LightSeparatorLineLightSkin = new Color(1f, 1f, 1f, 0.323f);

            public static Color DarkSeparatorLine => DarkSkin ? DarkSeparatorLineDarkSkin : DarkSeparatorLineLightSkin;

            public static Color LightSeparatorLine => DarkSkin ? LightSeparatorLineDarkSkin : LightSeparatorLineLightSkin;

            private static bool DarkSkin => EditorGUIUtility.isProSkin;
        }
    }
}