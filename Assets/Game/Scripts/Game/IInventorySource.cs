using Game.Interaction;

namespace Game
{
    public interface IInventorySource
    {
        void Clear();
        bool AddItem(InventoryAsset item);
        void RemoveItem(InventoryAsset item);
        
        void ConsumeKey(KeyId keyValue);
        
        bool HasKey(KeyId keyValue);
        void RemoveAllItems(InventoryAsset item);
    }
}
