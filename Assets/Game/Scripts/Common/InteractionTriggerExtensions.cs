using UnityEngine;

namespace Game.Interaction
{
    public static class InteractionTriggerExtensions
    {
        public static IGameController GetGameController(this Component any)
            => GameController.Instance;

        public static bool? GameController_OnInteractionTriggerEnter(this IInteractionZone receiver, Collider enter)
        {
            GameController? game = GameObject.FindFirstObjectByType<GameController>();
            return game?.InteractEnter(receiver, enter);
        }

        public static bool? GameController_OnInteractionTriggerExit(this IInteractionZone receiver, Collider enter)
        {
            GameController? game = GameObject.FindFirstObjectByType<GameController>();
            return game?.InteractExit(receiver, enter);
        }
    }
}
