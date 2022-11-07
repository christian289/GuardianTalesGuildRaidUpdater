using GuardianTalesGuildRaidUpdater.Models;
using GuardianTalesGuildRaidUpdater.Options;
using GuardianTalesGuildRaidUpdater.Services;

namespace GuardianTalesGuildRaidUpdater.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly PeriodicTimer periodicTimer;

        private readonly GoogleOAuthClientOption googleOpt;
        private readonly GoogleApiEndPointsOption endpointOpt;
        private readonly GoogleOAuthService authService;

        [ObservableProperty]
        public ContentImageIndex imageContainer;
        [ObservableProperty]
        public Stream currentImageStream;

        public LoginViewModel(
            IOptionsMonitor<GoogleOAuthClientOption> googleOpt,
            IOptionsMonitor<GoogleApiEndPointsOption> endpointOpt,
            GoogleOAuthService authService,
            ContentImageIndex imageContainer)
        {
            this.googleOpt = googleOpt.CurrentValue;
            this.endpointOpt = endpointOpt.CurrentValue;
            this.authService = authService;

            this.ImageContainer = imageContainer;

            CurrentImageStream = ImageContainer.Images[imageContainer.CurrentIdx].ContentInfo.Stream;
            periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            _ = Task.Factory.StartNew(async () =>
            {
                while (await periodicTimer.WaitForNextTickAsync())
                {
                    if (ImageContainer.IdxQueue.TryDequeue(out int result))
                    {
                        ImageContainer.CurrentIdx = result;
                        ImageContainer.NewImageInput = true;
                        CurrentImageStream = ImageContainer.Images[result].ContentInfo.Stream;
                    }
                    else
                    {
                        ImageContainer.NewImageInput = false;
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        [RelayCommand]
        private async Task GoogleOAuthLoginCommandCore()
        {
            await authService.StartGoogleOAuthAsync();
        }
    }
}
