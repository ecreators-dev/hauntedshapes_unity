using UnityEngine;

namespace Game.Interaction
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Bedingung", menuName = "Haunted-Shapes/new Interaction Condition")]
    public sealed class InteractionConditionAsset : ScriptableObject
    {
        [Tooltip("Schlüssel Id, wenn nötig, dann nicht leer lassen")]
        [SerializeField] public KeyId KeyValue;
        [SerializeField] public int PlayerLevel;
        [SerializeField] public bool MustBeGhost;
    }
}
