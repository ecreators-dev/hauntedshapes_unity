using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Game.Interaction
{
#if UNITY_EDITOR

    [CustomEditor(typeof(SwingEffect))]
    public sealed class SwingEffectInspector : UnityEditor.Editor
    {
        private SerializedProperty modeProperty;
        private SerializedProperty angleProperty;
        private SerializedProperty dragTimeProperty;
        private SerializedProperty holdTimeProperty;
        private SerializedProperty pushProperty;

        private void OnEnable()
        {
            this.modeProperty = serializedObject.FindProperty(nameof(SwingEffect.Mode));
            // mode A
            this.angleProperty = serializedObject.FindProperty(nameof(SwingEffect.StartAngle));
            this.dragTimeProperty = serializedObject.FindProperty(nameof(SwingEffect.DragSeconds));
            this.holdTimeProperty = serializedObject.FindProperty(nameof(SwingEffect.HoldSeconds));
            // mode B
            this.pushProperty = serializedObject.FindProperty(nameof(SwingEffect.PushForceAngular));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SwingEffect dc = (SwingEffect)target;

            if (dc == null)
            {
                return;
            }

            if (modeProperty == null)
            {
                OnEnable();

                if (modeProperty == null)
                {
                    return;
                }
            }

            // === PrefixLabel + EnumPopup ===
            EditorGUI.BeginChangeCheck();

            // Label links
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(nameof(SwingEffect.Mode));

            // Dropdown rechts – mit aktueller Enum-Wert
            SwingEffect.PushTypeEnum currentMode = (SwingEffect.PushTypeEnum)modeProperty.intValue;
            SwingEffect.PushTypeEnum newMode = (SwingEffect.PushTypeEnum)EditorGUILayout.EnumPopup(currentMode);
            EditorGUILayout.EndHorizontal();

            // Änderung erkennen
            if (EditorGUI.EndChangeCheck())
            {
                // Speichere in SerializedProperty (für Undo!)
                modeProperty.intValue = (int)newMode;

                // Live-Update im Spielobjekt
                dc.Mode = newMode;

                // Änderungen übernehmen
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(dc);
            }

            switch (dc.Mode)
            {
                case SwingEffect.PushTypeEnum.AngleAndDrop:
                    OnInspectorGUI_AngleAndDrop(dc);
                    break;
                case SwingEffect.PushTypeEnum.Push:
                default:
                    OnInspectorGUI_Push(dc);
                    break;
            }
            EditorGUILayout.Space(20);

            // Mindestgröße: 160px hoch, 200px breit (für Kurve + Labels)
            DrawIllustrationByMode(dc);
        }

        /// <summary>
        /// Erstellt eine Grafik im Inspector
        /// </summary>
        public static void DrawIllustrationByMode(SwingEffect effect)
        {
            float value = GetIllustrationAngleValue(effect);
            Rect rect = EditorGUILayout.GetControlRect(
                    GUILayout.MinWidth(100),
                    GUILayout.Height(100),
                    GUILayout.ExpandHeight(true));

            DrawIllustration(rect, 40, effect.DragSeconds, effect.HoldSeconds, value, effect.Mode == SwingEffect.PushTypeEnum.AngleAndDrop);
        }

        /// <summary>
        /// Erstellt eine Grafik im Inspector
        /// </summary>
        public static void DrawIllustrationByMode(SwingEffect effect, float radius, Rect space)
        {
            float value = GetIllustrationAngleValue(effect);
            // --- graphRect: nutzt gesamten Platz, aber Grafik skaliert ---
            float margin = 5;

            float width = space.width - 2 * margin;
            float height = space.height - 2 * margin - 2 * 16;

            if (width < height)
            {
                height = width;
            }
            else
            {
                width = height;
            }

            radius = Mathf.Max(radius, width) - 2 * margin - 2 * 16;

            Rect graphRect = new Rect(space.x + margin, space.y + margin, width, height);
            DrawIllustration(graphRect, radius, effect.DragSeconds, effect.HoldSeconds, value, effect.Mode == SwingEffect.PushTypeEnum.AngleAndDrop);
        }

        public static float GetIllustrationAngleValue(SwingEffect effect)
        {
            return effect.Mode == SwingEffect.PushTypeEnum.AngleAndDrop ? effect.StartAngle : -effect.PushForceAngular;
        }

        private void OnInspectorGUI_Push(SwingEffect dc)
        {
            EditorGUILayout.PropertyField(pushProperty);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                dc.PushForceAngular = pushProperty.floatValue;
            }
        }

        private void OnInspectorGUI_AngleAndDrop(SwingEffect dc)
        {
            EditorGUILayout.PropertyField(angleProperty);
            EditorGUILayout.PropertyField(dragTimeProperty);
            EditorGUILayout.PropertyField(holdTimeProperty);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                dc.StartAngle = angleProperty.floatValue;
                dc.DragSeconds = dragTimeProperty.floatValue;
                dc.HoldSeconds = holdTimeProperty.floatValue;
            }
        }

        private static void DrawIllustration(Rect rect, float radius, float drag, float hold, float valueAny, bool dragAndDropMode)
        {
            float margin = 15f;
            float ROWHEIGHT = 16f;

            // --- Dynamische Größe ---
            float labelMaxWidthRight = 40;
            float totalWidth = radius + margin * 2 + labelMaxWidthRight;
            float totalHeight = ROWHEIGHT * 2 + radius + margin * 2;
            totalWidth = Mathf.Max(totalWidth, totalHeight);
            totalHeight = totalWidth;

            // --- Oben links ---
            Rect bgRect = new Rect(
                rect.x,
                rect.y,
                totalWidth,
                totalHeight
            );

            //Rect bgRect = new Rect(rect.x, rect.yMax - totalHeight, totalWidth, totalHeight);
            DrawBackground(bgRect);
            DrawBorder(bgRect);

            // --- Zentrum: linksbündig (0px + margin + radius) ---
            Vector2 center = new Vector2(bgRect.x + margin + radius, bgRect.y + margin + ROWHEIGHT);

            DrawOutline(radius, center);
            DrawFilledAngle(valueAny, radius, center);
            DrawValueLabels(drag, hold, dragAndDropMode, bgRect, center);
            DrawAngleLabel(valueAny, dragAndDropMode, center);
        }

        private static void DrawValueLabels(float drag, float hold, bool dragAndDropMode, Rect bgRect, Vector2 center)
        {
            int fontSize = GUI.skin.label.fontSize;
            GUIStyle leftStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = Color.white },
                fontSize = fontSize,
                alignment = TextAnchor.MiddleLeft
            };

            if (dragAndDropMode)
            {
                // Hold: oben links über dem Viertelkreis
                EditorGUI.LabelField(new Rect(bgRect.x + 10, bgRect.y + 5, 120, 20), $"Hold: {hold:F1} s", leftStyle);

                // Drag: unter der Grafik, zentriert unter Kreismitte
                EditorGUI.LabelField(new Rect(center.x - 50, bgRect.yMax - 25, 100, 20), $"Drag {drag:F1} s", leftStyle);
            }
            else
            {
                EditorGUI.LabelField(new Rect(bgRect.x + 10, bgRect.y + 5, 120, 20), $"Hold: - s", leftStyle);

                // Drag: unter der Grafik, zentriert unter Kreismitte
                EditorGUI.LabelField(new Rect(center.x - 50, bgRect.yMax - 25, 100, 20), $"Drag - s", leftStyle);
            }
        }

        private static void DrawAngleLabel(float valueAny, bool dragAndDropMode, Vector2 center)
        {
            const string arrowCharToRight = " >";
            const string arrowCharToLeft = "< ";
            string arrowLeft = "";
            string arrowRight = "";

            // angle oder force!
            if (valueAny < 0)
            {
                arrowRight = arrowCharToRight;
            }
            else
            {
                arrowLeft = arrowCharToLeft;
            }

            int fontSize = GUI.skin.label.fontSize;
            GUIStyle angleStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = Color.white },
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            valueAny = Mathf.Abs(valueAny);
            Vector2 anglePos = center + new Vector2(5, -8);

            if (dragAndDropMode)
            {
                EditorGUI.LabelField(new Rect(anglePos.x, anglePos.y, 80, 20), $"{arrowLeft}{valueAny:F0}°{arrowRight}", angleStyle);
            }
            else
            {
                EditorGUI.LabelField(new Rect(anglePos.x, anglePos.y, 80, 20), $"{arrowLeft}F:{valueAny:F0}{arrowRight}", angleStyle);
            }
        }

        private static void DrawFilledAngle(float valueAny, float radius, Vector2 center)
        {
            Vector2 fromVector = Quaternion.Euler(0, 0, 180f) * Vector2.down;
            Handles.color = new Color(0f, 0.6f, 1f, 0.35f);
            Handles.DrawSolidArc(center, Vector3.forward, fromVector * radius, -valueAny, radius);
        }

        private static void DrawOutline(float radius, Vector2 center)
        {
            Handles.color = Color.yellow;
            Handles.DrawAAPolyLine(2f, GetQuarterCircleRotated(center, radius, 30, 90f));

            static Vector3[] GetQuarterCircleRotated(Vector2 center, float radius, int segments, float rotationOffsetDeg)
            {
                Vector3[] points = new Vector3[segments + 1];
                float offsetRad = rotationOffsetDeg * Mathf.Deg2Rad;

                for (int i = 0; i <= segments; i++)
                {
                    float t = i / (float)segments;
                    float angle = Mathf.Lerp(0, Mathf.PI / 2, t) + offsetRad;
                    points[i] = center + new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
                }
                return points;
            }
        }

        private static void DrawBackground(Rect bgRect)
        {
            EditorGUI.DrawRect(bgRect, new Color(0.2f, 0.2f, 0.2f)); // Dunkelgrauer Hintergrund
        }

        private static void DrawBorder(Rect bgRect)
        {
            EditorGUI.DrawRect(new Rect(bgRect.x, bgRect.y, bgRect.width, 1), Color.gray); // Oben
            EditorGUI.DrawRect(new Rect(bgRect.x, bgRect.yMax - 1, bgRect.width, 1), Color.gray); // Unten
            EditorGUI.DrawRect(new Rect(bgRect.x, bgRect.y, 1, bgRect.height), Color.gray); // Links
            EditorGUI.DrawRect(new Rect(bgRect.xMax - 1, bgRect.y, 1, bgRect.height), Color.gray); // Rechts
        }
    }

#endif
}
