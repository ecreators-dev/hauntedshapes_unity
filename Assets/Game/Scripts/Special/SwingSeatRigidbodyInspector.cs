#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

namespace Game.Interaction
{
    [CustomEditor(typeof(SwingSeatRigidbody))]
    public class SwingSeatRigidbodyInspector : UnityEditor.Editor
    {
        private readonly string[] modeNames = { "Low", "Med", "Strong", "Extreme" };
        private int selectedIndex = 0;
        private SwingSeatRigidbody swing;

        private void OnEnable()
        {
            swing = (SwingSeatRigidbody)target;
        }

        public override bool HasPreviewGUI() => true;

        public override GUIContent GetPreviewTitle()
        {
            if (swing != null)
                return new GUIContent($"{ObjectNames.NicifyVariableName(swing.GetType().Name)} (Script)");

            return base.GetPreviewTitle();
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            // WICHTIG: Zeichne den Hintergrund → Drag-to-Resize bleibt!
            GUI.Box(rect, GUIContent.none, background);

            // Nutze Handles für deine Grafik
            Handles.BeginGUI();

            SwingEffect selected = GetSelectedEffect();
            if (selected != null)
            {
                // --- Verfügbarer Platz für Grafik ---
                float availableHeight = rect.height - 20 + 20; // 20px Titel + 20px Abstand
                float availableWidth = rect.width - 40f;   // Margin links/rechts

                // --- Skalierter Radius ---
                float maxRadius = Mathf.Min(availableWidth * 0.5f, availableHeight * 0.7f);
                float radius = Mathf.Min(maxRadius, 40f, maxRadius);

                SwingEffectInspector.DrawIllustrationByMode(selected, radius, rect);
            }
            Handles.EndGUI();
        }

        public override void OnPreviewSettings()
        {
            // selectedIndex ist der preview selectedIndex der Popupbox
            int newIndex = EditorGUILayout.Popup(selectedIndex, modeNames, GUILayout.Width(100));
            if (newIndex != selectedIndex)
            {
                selectedIndex = newIndex;
                Repaint(); // Sofortiges Update
            }
        }

        private SwingEffect GetSelectedEffect()
        {
            return selectedIndex switch
            {
                0 => swing.lowEffect,
                1 => swing.mediumEffect,
                2 => swing.strongEffect,
                3 => swing.extremeEffect,
                _ => swing.lowEffect
            };
        }
    }
}
#endif
