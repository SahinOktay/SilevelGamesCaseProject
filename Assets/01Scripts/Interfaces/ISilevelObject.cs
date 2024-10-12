namespace SilevelGames
{
    public interface ISilevelObject
    {
        public void Initialize();
        public void OnGameStart();
        public void OnGameStop();
        public void Reset();
    }
}