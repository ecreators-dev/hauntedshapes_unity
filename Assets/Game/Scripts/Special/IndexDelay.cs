using UnityEngine;

namespace Game.Interaction
{
    [System.Serializable]
    public sealed class IndexDelay
    {
        [SerializeField] internal int ObjectIndex;
        [SerializeField] internal float DelaySeconds;
    }
}
