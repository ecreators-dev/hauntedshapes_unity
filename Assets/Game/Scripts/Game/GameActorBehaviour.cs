using UnityEngine;

namespace Game
{
    public abstract class GameActorBehaviour : MonoBehaviour, IActorBehaviour, IGameLoadProgress
    {
        [SerializeField] private bool isGhost;

        public bool IsGhost() => isGhost;
    }
}
