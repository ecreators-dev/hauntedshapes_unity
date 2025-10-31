using System;

using UnityEngine;

namespace Game
{
    [System.Serializable]
    public sealed class PlayerData
    {
        public int Id { get; internal set; } = Guid.NewGuid().GetHashCode();
        
        [field: SerializeField] public int PlayerLevel { get; internal set; } = 1;
    }
}
