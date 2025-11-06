using UnityEngine;

namespace Game
{
    public sealed class PlayerBehaviour : GameActorBehaviour, IPlayerActor, IGameLoadProgress
    {
        internal readonly PlayerData Data = new PlayerData();

        [SerializeField] private InventoryBehaviour inventory;

        public IInventorySource GetInventory() => inventory;
    }
}
