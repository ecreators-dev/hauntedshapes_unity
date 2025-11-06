#nullable enable
using Game.Interaction;

namespace Game
{
    public interface IInteractiveObject : ICrosshairSelectableObject
    {
        IInteractionZone? GetInteractionArea();
    }
}
#nullable restore