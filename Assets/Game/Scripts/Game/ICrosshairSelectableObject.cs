namespace Game
{
    public interface ICrosshairSelectableObject
    {
        bool CanSelect { get; }

        /// <summary>
        /// Einfluss auf Crosshair-Farbe
        /// </summary>
        bool IsLocked { get; }

        string GetText();
    }
}
