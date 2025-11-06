namespace Game.Interaction
{
    internal interface ISwingPush
    {
        DurationSeconds Push(SwingEffect option);
        float StartPushRandom();
    }
}