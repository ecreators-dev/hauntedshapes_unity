#if UNITY_EDITOR
using Game.Interaction;

using UnityEditor;

using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(DangerLightArrayControl))]
    public class DangerLightArrayControlEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DangerLightArrayControl control = (DangerLightArrayControl)target;

            // start danger on lights
            GUILayout.BeginHorizontal();
            GUI.enabled = !control.IsDangerActive;
            if (GUILayout.Button(new GUIContent("Start Danger")))
            {
                control.StartDanger();
            }
            GUI.enabled = control.IsDangerActive;
            if (GUILayout.Button(new GUIContent("End Danger")))
            {
                control.EndDanger();
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }
    }
}
#endif
