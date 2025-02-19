namespace Common.Logging
{
    public interface IGameLogger
    {
        void Log(string message);
        void LogError(string message);
    }
}