#if UNITY_EDITOR
using Game.Interaction.Input;

using UnityEditor;

using UnityEngine;

[CustomPropertyDrawer(typeof(MovementTimingSettingsRef))]
public class MovementTimingSettingsRefDrawer : PropertyDrawer
{
    private static float LabelHeight = 20f;
    private static float FieldSpacing = 5f;
    private const float CurveHeight = 55f;

    private static readonly Color AccelerationColor = new Color(0f, 0.8f, 1f, 0.25f);
    private static readonly Color DecelerationColor = new Color(1f, 0f, 0f, 0.25f);

    private readonly AnimationCurve previewCurve = new AnimationCurve();
    private readonly Keyframe keyStart = new Keyframe(0f, 0f);
    private Keyframe keyPeak1 = new Keyframe(0.25f, 1f);
    private Keyframe keyPeak2 = new Keyframe(0.75f, 1f);
    private bool editable;
    private readonly Keyframe keyEnd = new Keyframe(1f, 0f);

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Object assigned = property.FindPropertyRelative(nameof(MovementTimingSettingsRef.asset)).objectReferenceValue;

        if (assigned == null)
            return LabelHeight;

        int anzahl_rows;
        if (editable)
        {
            anzahl_rows = 6;
            return (anzahl_rows * LabelHeight) + CurveHeight + FieldSpacing + ((anzahl_rows - 1) * FieldSpacing);
        }
        else
        {
            anzahl_rows = 2;
            return (anzahl_rows * LabelHeight) + ((anzahl_rows - 1) * FieldSpacing);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty assetProperty = property.FindPropertyRelative(nameof(MovementTimingSettingsRef.asset));

        Rect rect = new Rect(position.x, position.y, position.width, LabelHeight);
        EditorGUI.PropertyField(rect, assetProperty, label); // 1

        if (assetProperty.objectReferenceValue == null)
        {
            EditorGUI.EndProperty();
            return;
        }

        MovementTimingSettings settings = assetProperty.objectReferenceValue as MovementTimingSettings;
        if (settings == null)
        {
            EditorGUI.EndProperty();
            return;
        }

        rect.y += LabelHeight + FieldSpacing;
        editable = EditorGUI.Toggle(rect, "Asset überschreiben", editable);

        SerializedObject serializedSettings = new SerializedObject(settings);
        serializedSettings.Update();

        if (editable)
        {
            GUI.enabled = editable;
            SerializedProperty maxTime = serializedSettings.FindProperty(nameof(MovementTimingSettings.maxTime));
            SerializedProperty accelerationTime = serializedSettings.FindProperty(nameof(MovementTimingSettings.accelerationTime));
            SerializedProperty decelerationTime = serializedSettings.FindProperty(nameof(MovementTimingSettings.decelerationTime));
            SerializedProperty maxSpeed = serializedSettings.FindProperty(nameof(MovementTimingSettings.maxSpeed));

            rect.y += LabelHeight + FieldSpacing;

            float Height = LabelHeight * 2 + FieldSpacing;
            Rect timeSpeedRect = new Rect(rect.x, rect.y, rect.width, Height);

            float oldLabelWidth = EditorGUIUtility.labelWidth;

            // Hälfte für das Label (Max Time)
            float field_width = timeSpeedRect.width * 0.5f - FieldSpacing;
            EditorGUIUtility.labelWidth = field_width / 2;
            EditorGUI.PropertyField(
                new Rect(timeSpeedRect.x, timeSpeedRect.y, field_width, timeSpeedRect.height),
                maxTime,
                new GUIContent("Max Time")
            );

            // andere Hälfte für das Label (Max Speed)
            EditorGUIUtility.labelWidth = field_width / 2;
            EditorGUI.PropertyField(
                new Rect(timeSpeedRect.x + FieldSpacing + field_width, timeSpeedRect.y, field_width + FieldSpacing, timeSpeedRect.height),
                maxSpeed,
                new GUIContent("Max Speed")
            );

            // Reset
            EditorGUIUtility.labelWidth = oldLabelWidth;

            rect.y += timeSpeedRect.height + FieldSpacing;
            accelerationTime.floatValue = EditorGUI.Slider(rect, "Acceleration Time", accelerationTime.floatValue, 0f, maxTime.floatValue);

            rect.y += LabelHeight + FieldSpacing;
            decelerationTime.floatValue = EditorGUI.Slider(rect, "Deceleration Time", decelerationTime.floatValue, 0f, maxTime.floatValue);

            rect.y += LabelHeight + FieldSpacing;
            Rect curveRect = new Rect(rect.x, rect.y, rect.width, CurveHeight);

            GUI.enabled = false;
            DrawTimingPreview(curveRect, accelerationTime.floatValue, decelerationTime.floatValue);
            GUI.enabled = true;

            serializedSettings.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }

    private void DrawTimingPreview(Rect rect, float acceleration, float deceleration)
    {
        float total = Mathf.Max(0.0001f, acceleration + deceleration);
        float accelerationRatio = acceleration / total;
        float decelerationRatio = deceleration / total;

        float accelWidth = rect.width * accelerationRatio;
        float decelWidth = rect.width * decelerationRatio;

        // Hintergrund - farblich Segmentweise
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, accelWidth, rect.height), AccelerationColor);
        EditorGUI.DrawRect(new Rect(rect.x + accelWidth, rect.y, decelWidth, rect.height), DecelerationColor);

        // Kurve vorbereiten
        keyPeak1.time = accelerationRatio;
        keyPeak2.time = 1f - decelerationRatio;

        previewCurve.keys = new[] { keyStart, keyPeak1, keyEnd };

        for (int i = 0; i < previewCurve.keys.Length; i++)
        {
            if (i == 0)
            {
                AnimationUtility.SetKeyLeftTangentMode(previewCurve, i, AnimationUtility.TangentMode.Constant);
                AnimationUtility.SetKeyRightTangentMode(previewCurve, i, AnimationUtility.TangentMode.Auto);
            }
            else if (i == previewCurve.keys.Length - 1)
            {
                AnimationUtility.SetKeyLeftTangentMode(previewCurve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyRightTangentMode(previewCurve, i, AnimationUtility.TangentMode.Constant);
            }
            else
            {
                AnimationUtility.SetKeyLeftTangentMode(previewCurve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyRightTangentMode(previewCurve, i, AnimationUtility.TangentMode.Auto);
            }
        }

        // Kurve drüber als Readonly Rendering
        EditorGUI.CurveField(rect, previewCurve, Color.white, new Rect(0f, 0f, 1f, 1f));
    }
}
#endif