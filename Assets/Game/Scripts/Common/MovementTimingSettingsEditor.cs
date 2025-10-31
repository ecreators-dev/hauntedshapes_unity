#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

namespace Game.Interaction.Input
{
    [CustomEditor(typeof(MovementTimingSettings))]
    public sealed class MovementTimingSettingsEditor : UnityEditor.Editor
    {
        const float CurveH = 2f;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty pMaxTime = serializedObject.FindProperty(nameof(MovementTimingSettings.maxTime));
            SerializedProperty pAccel = serializedObject.FindProperty(nameof(MovementTimingSettings.accelerationTime));
            SerializedProperty pDecel = serializedObject.FindProperty(nameof(MovementTimingSettings.decelerationTime));
            SerializedProperty pSpeed = serializedObject.FindProperty(nameof(MovementTimingSettings.maxSpeed));

            EditorGUILayout.LabelField("Timing Preset", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(pMaxTime, new GUIContent("Max Time (s)"));
            EditorGUILayout.PropertyField(pSpeed, new GUIContent("Max Speed"));

            pAccel.floatValue = EditorGUILayout.Slider("Accel Time", pAccel.floatValue, 0f, pMaxTime.floatValue);
            pDecel.floatValue = EditorGUILayout.Slider("Decel Time", pDecel.floatValue, 0f, pMaxTime.floatValue);

            // Preview-Balken
            var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(CurveH));
            DrawPreview(rect, pAccel.floatValue, pDecel.floatValue);

            serializedObject.ApplyModifiedProperties();
        }

        void DrawPreview(Rect r, float accel, float decel)
        {
            EditorGUI.DrawRect(r, new Color(0.12f, 0.12f, 0.12f));
            float total = Mathf.Max(0.0001f, accel + decel);
            float aW = r.width * (accel / total);
            float dW = r.width * (decel / total);
            float plateauW = Mathf.Max(0f, r.width - aW - dW);

            EditorGUI.DrawRect(new Rect(r.x, r.y, aW, r.height), new Color(0f, 0.9f, 1f, 0.4f)); // accel
            if (plateauW > 0.5f)
                EditorGUI.DrawRect(new Rect(r.x + aW, r.y, plateauW, r.height), new Color(0f, 1f, 0f, 0.25f)); // plateau
            EditorGUI.DrawRect(new Rect(r.x + aW + plateauW, r.y, dW, r.height), new Color(1f, 0f, 0f, 0.4f)); // decel
        }
    }
}
#endif