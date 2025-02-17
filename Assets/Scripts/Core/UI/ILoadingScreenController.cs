namespace Core.UI
{
    public interface ILoadingScreenController
    {
        void ShowLoadingScreen();
        void HideLoadingScreen();
        void UpdateProgress(float progress);
    }
}