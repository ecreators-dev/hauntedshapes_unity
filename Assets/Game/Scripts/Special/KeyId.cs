using UnityEngine;

namespace Game.Interaction
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Schlüssel Id", menuName = "Haunted-Shapes/new Key Id")]
    public sealed class KeyId : ScriptableObject
    {
        [SerializeField] private string id;

        public bool IsKey(KeyId key) => string.Equals(key.id, id);
    }
}
