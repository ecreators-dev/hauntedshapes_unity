using Game.Interaction;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game
{
    /// <summary>
    /// Inventar des Spielers (Verwaltung)
    /// </summary>
    public sealed class InventoryBehaviour : MonoBehaviour, IInventorySource, IGameLoadProgress
    {
#if DEV
        [SerializeField]
#endif
        private List<InventoryEntry> entries = new List<InventoryEntry>();

        [Tooltip("Anzahl Slots im Inventar")]
        [Min(1)]
        [SerializeField] private int size = 7;

#if UNITY_EDITOR
        private int sizeOld = 0;

        private void OnValidate()
        {
            size = Mathf.Min(size, 20);

            if (size != sizeOld)
            {
                // rebuild inventory:
                Clear();

                if (size > sizeOld)
                {
                    int missing = size - sizeOld;
                    for (int i = 0; i < missing; i++)
                    {
                        CreateEntryEmpty();
                    }
                }

                if (sizeOld > size)
                {
                    int tooMuch = sizeOld - size;
                    for (int i = 0; i < tooMuch; i++)
                    {
                        entries.RemoveAt(sizeOld);
                    }
                }

                sizeOld = size;
            }
        }
#endif

        /// <summary>
        /// Erstellt einen neuen Sloteintrag
        /// </summary>
        private void CreateEntryEmpty()
        {
            this.entries.Add(new InventoryEntry());
        }

        /// <summary>
        /// Sofern (return true) Platz im Inventar in einem Slot ist, wird der Gegenstand im ersten davon eingefügt
        /// </summary>
        public bool AddItem(InventoryAsset item)
        {
            int maxCount = item.MaximumStackSize;
            List<InventoryEntry> fillableSlots = FindFillableSlots(item, maxCount, item.InventoryItemSize);
            if (fillableSlots.Count > 0)
            {
                // erster slot
                InventoryEntry slot = fillableSlots.First();
                slot.Insert(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Findet alle Slots, in die das Asset passt
        /// </summary>
        private List<InventoryEntry> FindFillableSlots(InventoryAsset item, int slotSizeType, int inventoryItemSize)
        {
            if (item == null)
                return new List<InventoryEntry>();

            List<InventoryEntry> free = new List<InventoryEntry>();

            foreach (InventoryEntry entrySlot in entries)
            {
                if (entrySlot.IsEmpty)
                {
                    free.Add(entrySlot);
                }
                else if (entrySlot.Asset.IsItem(item))
                {
                    // ist noch platz?
                    int spaceForItems = slotSizeType;
                    int itemsInSlot = entrySlot.Count * inventoryItemSize;
                    if (spaceForItems - itemsInSlot >= inventoryItemSize)
                        free.Add(entrySlot);
                }
            }

            return free;
        }

        /// <summary>
        /// Entfernt genau dieses Asset aus dem Inventar
        /// </summary>
        public void RemoveItem(InventoryAsset item)
        {
            if (item == null)
                return;

            InventoryEntry? foundEntry = entries.FirstOrDefault(e => e.Asset == item);
            if (foundEntry != null)
            {
                foundEntry.RemoveOne();
            }
        }

        /// <summary>
        /// Entfernt den Type des Items aus allen Slots
        /// </summary>
        public void RemoveAllItems(InventoryAsset item)
        {
            foreach (InventoryEntry entry in entries)
            {
                if (!entry.IsEmpty && entry.Asset.IsItem(item))
                {
                    entry.RemoveAll();
                }
            }
        }

        /// <summary>
        /// Macht alle Slots leer
        /// </summary>
        public void Clear()
        {
            entries.ForEach(e => e.RemoveAll());
        }

        /// <summary>
        /// Prüft, ob der Schlüssel im Inventar ist
        /// </summary>
        public bool HasKey(KeyId keyValue) => entries.Any(e => e.IsKey(keyValue));

        /// <summary>
        /// Sofern ein Schlüsselgegenstand aufgebraucht wird, wird eines davon entfernt
        /// </summary>
        public void ConsumeKey(KeyId keyValue)
        {
            if (keyValue == null)
                return;

            foreach (InventoryEntry slot in entries)
            {
                if (slot.IsKey(keyValue) && !slot.IsEmpty && slot.IsConsumable)
                {
                    slot.RemoveOne();
                    break;
                }
            }
        }
    }
}
