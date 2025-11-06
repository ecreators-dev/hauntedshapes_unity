using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Interaction
{
    public sealed class DangerLightArrayControl : MonoBehaviour, IGameLoadProgress
    {
        [SerializeField] private List<GameLightControl> controlledLights;

        public bool IsDangerActive { get; private set; }

        public void StartDanger()
        {
            IsDangerActive = true;

            foreach (IInteractiveLight light in controlledLights.Cast<IInteractiveLight>())
            {
                if (!light.IsEnabled)
                {
                    continue;
                }

                light.SwitchRed();
                light.EnableDanger();
            }
        }

        public void EndDanger()
        {
            foreach (IInteractiveLight light in controlledLights.Cast<IInteractiveLight>())
            {
                if (!light.IsEnabled)
                {
                    continue;
                }

                light.DisableDanger();
                light.SwitchDefaultLight();
            }

            IsDangerActive = false;
        }
    }
}
