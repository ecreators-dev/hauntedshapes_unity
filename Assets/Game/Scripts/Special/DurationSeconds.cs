namespace Game.Interaction
{
    public readonly struct DurationSeconds
    {
        private DurationSeconds(float s)
        {
            this.Seconds = s;
        }

        public float Seconds { get; }

        public static implicit operator float(DurationSeconds seconds) => seconds.Seconds;
        public static implicit operator DurationSeconds(float seconds) => new DurationSeconds(seconds);

        public override string ToString()
        {
            return $"{Seconds}s [{nameof(DurationSeconds)}]";
        }
    }
}