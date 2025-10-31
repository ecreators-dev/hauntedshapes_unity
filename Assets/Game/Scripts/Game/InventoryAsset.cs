using Game.Interaction;

using UnityEngine;

namespace Game
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Inventory Item", menuName = "Haunted-Shapes/new Inventory Item")]
    public sealed class InventoryAsset : ScriptableObject
    {
        [SerializeField] private string Name;
        [SerializeField] private KeyId UnlockId;

        [Tooltip("Definiert, wie groß der Stapel in einem Inventarplatz für diese Art Inventaritem werden darf")]
        [SerializeField] internal int MaximumStackSize = 1;

        [Tooltip("Größe des Objekts in einem Slot. Platz, den das Objekt im Inventar einnimmt. Standard = 1")]
        [SerializeField] internal int InventoryItemSize = 1;
        
        [field:Tooltip("Verbrauch bei Benutzung")]
        [field: SerializeField] public bool IsConsumable { get; private set; }

        internal bool IsItem(InventoryAsset item) => string.Equals(item.Name, Name);

        internal bool IsKey(KeyId keyFind) => UnlockId != null && UnlockId.IsKey(keyFind);
    }
}
