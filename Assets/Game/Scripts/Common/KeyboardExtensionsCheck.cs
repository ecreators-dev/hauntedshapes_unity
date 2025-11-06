using UnityEngine.InputSystem;

namespace Game.Interaction.Input
{
    public static class KeyboardExtensionsCheck
    {
        public static bool WasKeyReleasedThisFrame(this Key code)
        {
            if (Keyboard.current == null)
            {
                return false;
            }

            UnityEngine.InputSystem.Controls.KeyControl keyControl = Keyboard.current[code];
            return keyControl != null && keyControl.wasReleasedThisFrame;
        }
    }
}
