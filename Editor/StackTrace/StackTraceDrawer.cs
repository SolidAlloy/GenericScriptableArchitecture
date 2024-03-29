﻿namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;
    using SolidUtilities;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class StackTraceDrawer
    {
        private readonly StackCollection<StackTraceEntry> _entries;
        private readonly SerializedProperty _stackTraceProperty;
        private readonly SerializedProperty _enabled;
        private readonly Action _repaint;

        private StackTraceEntry _selectedTrace;

        private Rect _lastContentRect;
        private Vector2 _listScrollPos;
        private Vector2 _selectedContentScrollPos;
        private float _windowsHeightRatio = 0.6f;
        private bool _resizeMouseDown;

        // can't use Object as key in dictionary, have to use instance id instead.
        private static readonly Dictionary<(int instanceId, string propertyPath), (bool enabled, bool expanded)> _stackTraceProperties
            = new Dictionary<(int instanceId, string propertyPath), (bool enabled, bool expanded)>();

        static StackTraceDrawer()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange stateChange)
        {
            if (stateChange != PlayModeStateChange.EnteredEditMode)
                return;

            foreach ((var key, var value) in _stackTraceProperties)
            {
                var targetObject = EditorUtility.InstanceIDToObject(key.instanceId);

                if (targetObject == null)
                    continue;

                var serializedObject = new SerializedObject(targetObject);
                var property = serializedObject.FindProperty(key.propertyPath);
                property.isExpanded = value.expanded;

                var enabledProp = property.FindPropertyRelative(nameof(StackTraceProvider.Enabled));
                enabledProp.boolValue = value.enabled;

                serializedObject.ApplyModifiedProperties();
            }
        }

        public StackTraceDrawer(StackCollection<StackTraceEntry> entries, SerializedProperty stackTraceProperty, [NotNull] Action repaint)
        {
            _entries = entries;
            _stackTraceProperty = stackTraceProperty;
            _enabled = _stackTraceProperty.FindPropertyRelative(nameof(StackTraceProvider.Enabled));
            _repaint = repaint;
        }

        public void Draw()
        {
            EditorGUILayout.Space();

            if (StackTraceSettings.EnabledInProject)
            {
                if (ApplicationUtil.InEditMode)
                {
                    EditorGUILayoutHelper.DrawInfoMessage("Stack traces are enabled for all assets in Project Settings.");
                    return;
                }
            }
            else
            {
                if ( ! _enabled.boolValue)
                {
                    if (GUILayout.Button("Enable stack trace"))
                    {
                        _enabled.boolValue = true;
                        SaveEnabledValue();
                    }

                    return;
                }

                if (ApplicationUtil.InEditMode)
                {
                    if (GUILayout.Button("Disable stack trace"))
                    {
                        _enabled.boolValue = false;
                    }

                    return;
                }
            }

            using var verticalBlock = new GUILayoutHelper.Vertical(null);

            var headerRect = GetHeaderRect();
            DrawHeader(headerRect);

            var contentRect = GetContentRect();
            DrawContent(contentRect);
        }

        private void SaveEnabledValue()
        {
            SaveStackTraceProperty(properties =>
            {
                properties.enabled = _enabled.boolValue;
                return properties;
            });
        }

        private void SaveExpandedValue()
        {
            SaveStackTraceProperty(properties =>
            {
                properties.expanded = _stackTraceProperty.isExpanded;
                return properties;
            });
        }

        private void SaveStackTraceProperty(Func<(bool enabled, bool expanded), (bool enabled, bool expanded)> changeProperties)
        {
            if (!(_stackTraceProperty.serializedObject.targetObject is Component component) || PrefabUtility.IsPartOfPrefabAsset(component))
                return;

            var key = (component.GetInstanceID(), _stackTraceProperty.propertyPath);

            if (_stackTraceProperties.TryGetValue(key, out var properties))
            {
                properties = changeProperties(properties);
            }
            else
            {
                properties = (_enabled.boolValue, _stackTraceProperty.isExpanded);
            }

            _stackTraceProperties[key] = properties;
        }

        private Rect GetHeaderRect()
        {
            const float headerWidth = 0f;
            const float headerHeight = 18;
            return GUILayoutUtility.GetRect(headerWidth, headerHeight, GUILayout.ExpandWidth(true));
        }

        private Rect GetContentRect()
        {
            const float contentRectWidth = 10f;
            const float contentRectHeight = 400f;
            return GUILayoutUtility.GetRect(contentRectWidth, _stackTraceProperty.isExpanded ? contentRectHeight : 0f, GUILayout.ExpandWidth(true));
        }

        private void DrawHeader(Rect headerRect)
        {
            if (Event.current.type == EventType.Repaint)
                ReorderableList.defaultBehaviours.DrawHeaderBackground(headerRect);

            const float sidePadding = 6f;
            const float topPadding = 1f;
            const float bottomPadding = 2f;

            headerRect.xMin += sidePadding;
            headerRect.xMax -= sidePadding;
            headerRect.height -= bottomPadding;
            headerRect.y += topPadding;

            DrawFoldout(headerRect);
        }

        private void DrawFoldout(Rect headerRect)
        {
            const float buttonWidth = 60f;
            const float leftMargin = 10f;

            var shiftedRightHeaderRect = new Rect(headerRect.x + leftMargin, headerRect.y, headerRect.width - leftMargin, headerRect.height);

            DrawDisableButton(shiftedRightHeaderRect, buttonWidth);
            DrawClearButton(shiftedRightHeaderRect, buttonWidth);

            bool expanded = EditorGUI.Foldout(shiftedRightHeaderRect, _stackTraceProperty.isExpanded, "Stack Trace", true);

            if (_stackTraceProperty.isExpanded != expanded)
            {
                _stackTraceProperty.isExpanded = expanded;
                SaveExpandedValue();
            }
        }

        private void DrawDisableButton(Rect foldoutRect, float buttonWidth)
        {
            const float betweenButtons = 10f;

            if (StackTraceSettings.EnabledInProject)
                return;

            var disableButton = GetButtonRect(foldoutRect, buttonWidth, buttonWidth * 2f + betweenButtons);

            if (GUI.Button(disableButton, "Disable"))
            {
                _enabled.boolValue = false;
                SaveEnabledValue();
            }
        }

        private void DrawClearButton(Rect foldoutRect, float buttonWidth)
        {
            var clearButton = GetButtonRect(foldoutRect, buttonWidth, buttonWidth);

            if (GUI.Button(clearButton, "Clear"))
            {
                _entries.Clear();
                _selectedTrace = null;
            }
        }

        private Rect GetButtonRect(Rect foldoutRect, float buttonWidth, float distanceFromRight)
        {
            return new Rect(
                foldoutRect.xMax - distanceFromRight,
                // The button does not appear in the middle because of top and bottom padding in header rect. That's why shift it down by 1 pixel.
                foldoutRect.y + 1f,
                buttonWidth,
                foldoutRect.height);
        }

        private void DrawContent(Rect contentRect)
        {
            if (!_stackTraceProperty.isExpanded)
                return;

            if (Event.current.type == EventType.Repaint)
            {
                _lastContentRect = contentRect;
                Styles.BoxBackground.Draw(_lastContentRect, false, false, false, false);
            }

            using var area = GUILayoutHelper.AreaBlock(_lastContentRect);

            const float resizeMargin = 0.2f;
            _windowsHeightRatio = Mathf.Clamp(_windowsHeightRatio, resizeMargin, 1 - resizeMargin);

            var splitHeight = _lastContentRect.height * _windowsHeightRatio;
            contentRect = new Rect(-1, splitHeight, _lastContentRect.width + 1, _lastContentRect.height - splitHeight);

            DrawList(contentRect);
            HandleSelectedPartResize(splitHeight);
            DrawSelectedContent(contentRect);
        }

        private void DrawList(Rect contentRect)
        {
            const float lineHeight = 18f;
            const float scrollWidth = 20f;

            var scrollRect = new Rect(0f, 0f, _lastContentRect.width, _lastContentRect.height - contentRect.height);
            var listRect = new Rect(0f, 0f, scrollRect.width - scrollWidth, _entries.Count * lineHeight);

            using var scrollViewBlock = GUIHelper.ScrollViewBlock(scrollRect, ref _listScrollPos, listRect);

            int i = 0;

            foreach (var stackTraceEntry in _entries)
            {
                string currentText = GetFirstLine(stackTraceEntry);

                var elementRect = new Rect(scrollRect.x, i * lineHeight, scrollRect.width, lineHeight);

                if (Event.current.type == EventType.MouseDown && elementRect.Contains(Event.current.mousePosition))
                {
                    _selectedTrace = stackTraceEntry;
                    _repaint();
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
            var lineHeights = lines.Select(line => Styles.MessageStyle.CalcHeight(GUIContentHelper.Temp(line), textWidth)).ToList();

            var listRect = new Rect(0f, 0f, textWidth, lineHeights.Sum());
            using var scrollView = GUIHelper.ScrollViewBlock(scrollViewRect, ref _selectedContentScrollPos, listRect);

            var currentYPos = 0f;

            for (int i = 0; i < lines.Length; i++)
            {
                var lineHeight = lineHeights[i];
                var lineRect = new Rect(0f, currentYPos, textWidth, lineHeight);
                currentYPos += lineHeight;

                EditorGUI.SelectableLabel(lineRect, lines[i], Styles.MessageStyle);
                DrawSeparator(lineRect);
            }
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
                _repaint();
            }
        }

        private static string GetFirstLine(string value)
        {
            return Regex.Match(value, @".*?(\r|\n)").Value;
        }

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