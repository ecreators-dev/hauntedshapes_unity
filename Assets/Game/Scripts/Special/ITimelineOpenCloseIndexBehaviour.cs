namespace Game.Interaction
{
    public interface ITimelineOpenCloseIndexBehaviour
    {
        void Close(int playableIndex);
        
        void Open(int playableIndex);
    }
}
