using System.Collections.Generic;

using UnityEngine;

namespace Game.Common.Story
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private List<StoryAsset> storyAsset;
    }

    [CreateAssetMenu(fileName = "New Story Asset", menuName = "Game/Story Asset")]
    public class StoryAsset : ScriptableObject
    {
        [SerializeField] private string[] scenes;

        [SerializeField] private Vector3 playerStartPosition;

        [SerializeField] private Quaternion playerStartRotation;

#if UNITY_EDITOR
        [ContextMenu("Take Selected As Playerposition/rotation")]
        public void SetPlayerStartTransform()
        {
            Transform activeTransform = UnityEditor.Selection.activeTransform;
            if (activeTransform != null)
            {
                playerStartPosition = activeTransform.position;
                playerStartRotation = activeTransform.rotation;
            }
        }
#endif
    }
}
