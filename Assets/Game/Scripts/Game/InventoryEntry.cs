using Game.Interaction;

using UnityEngine;

namespace Game
{
    [System.Serializable]
    public sealed class InventoryEntry
    {
        public InventoryEntry()
        { }

#if DEV
        [field:SerializeField]
#endif
        public InventoryAsset Asset { get; private set; }

#if DEV
        [field: SerializeField]
        [Min(0)]
#endif
        public int Count { get; private set; }


        public void Insert(InventoryAsset item)
        {
            if (Asset != null && !Asset.IsItem(item))
            {
                return;
            }

            Asset = item;
            Add(1);
        }

        public void RemoveAll()
        {
            Asset = null;
            Count = 0;
        }

        private void Add(uint count)
        {
            if (count == 0 || Asset == null)
            {
                return;
            }

            this.Count += (int)count;
        }

        public void RemoveOne()
        {
            if (Asset == null || Count == 0)
            {
                return;
            }

            Remove(1);
        }

        private void Remove(uint count)
        {
            if (count == 0)
            {
                return;
            }

            if (IsEmpty)
            {
                this.Count = 0;
                return;
            }

            this.Count -= (int)count;
            if (Count == 0)
            {
                Asset = null;
            }
        }

        internal bool IsKey(KeyId keyValue) => Asset != null && Count > 0 && Asset.IsKey(keyValue);

        public bool IsEmpty => Asset == null || Count == 0;

        public bool IsConsumable => Asset != null && Asset.IsConsumable;
    }
}
