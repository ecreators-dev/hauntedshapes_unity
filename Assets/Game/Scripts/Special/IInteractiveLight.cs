namespace Game.Interaction
{
    public interface IInteractiveLight
    {
        bool IsEnabled { get; }
        bool CanBeRepaired { get; }
        bool IsBroken { get; }

        void DisableDanger();
        void EnableDanger();
        void Flicker();
        void Repair();
        void SwitchDefaultLight();
        void SwitchRed();
        void TurnOff();
        void TurnOn();
    }
}