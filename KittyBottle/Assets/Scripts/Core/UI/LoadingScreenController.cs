using Zenject;

namespace Core.UI
{
    public class LoadingScreenController : ILoadingScreenController
    {
        private readonly LoadingScreen loadingScreen;
        private readonly FadingPanel fadingPanel;
        private readonly LoadingScreenConfig config;

        [Inject]
        public LoadingScreenController(LoadingScreen loadingScreen, FadingPanel fadingPanel, LoadingScreenConfig config)
        {
            this.loadingScreen = loadingScreen;
            this.fadingPanel = fadingPanel;
            this.config = config;
        }

        public void ShowLoadingScreen()
        {
            fadingPanel.FadeIn(config.FadeinTime, config.FadingEase, config.FadeinDelay);
            loadingScreen.ResetValues();
        }

        public void HideLoadingScreen()
        {
            fadingPanel.FadeOut(config.FadeOutTime, config.FadingEase, config.FadeOutDelay);
        }

        public void UpdateProgress(float progress)
        {
            loadingScreen.UpdateTargetProgress(progress);
        }
    }
}