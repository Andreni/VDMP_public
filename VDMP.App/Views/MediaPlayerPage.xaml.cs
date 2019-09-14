using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using VDMP.App.Helpers;
using VDMP.App.ViewModels;

namespace VDMP.App.Views
{
    public sealed partial class MediaPlayerPage : Page
    {
        // For more on the MediaPlayer and adjusting controls and behavior see https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/media-playback
        // The DisplayRequest is used to stop the screen dimming while watching for extended periods
        private readonly DisplayRequest _displayRequest = new DisplayRequest();
        private bool _isRequestActive;

        public MediaPlayerPage()
        {
            InitializeComponent();
        }

        public MediaPlayerViewModel ViewModel { get; } = new MediaPlayerViewModel();
        private string SessionName { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            mpe.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            mpe.MediaPlayer.RealTimePlayback = true;

            if (e != null)
            {
                var mediaMediaSource = (MediaSource) e.Parameter;
                ViewModel.Source = mediaMediaSource;
                // Is resuming requested?
                if (mediaMediaSource != null)
                {
                    var resume = false;
                    if (mediaMediaSource.CustomProperties.Keys != null)
                    {
                        var startPlayback = (TimeSpan) mediaMediaSource.CustomProperties["startPosition"];
                        SessionName = (string) mediaMediaSource.CustomProperties["name"];
                        if (startPlayback > new TimeSpan(0, 0, 0, 1))
                            resume = true;
                        mpe.MediaPlayer.PlaybackSession.Position = startPlayback;
                    }


                    mpe.MediaPlayer.Play();
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            mpe.MediaPlayer.Pause();
            HandelPlaybackState(mpe.MediaPlayer.PlaybackSession);
            mpe.MediaPlayer.PlaybackSession.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;
            ViewModel.DisposeSource();
        }

        private async void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            if (sender is MediaPlaybackSession playbackSession && playbackSession.NaturalVideoHeight != 0)
            {
                if (playbackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    if (!_isRequestActive)
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            _displayRequest.RequestActive();
                            _isRequestActive = true;
                        });
                }
                else
                {
                    await HandelPlaybackState(sender).ConfigureAwait(true);

                    if (_isRequestActive)
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            _displayRequest.RequestRelease();
                            _isRequestActive = false;
                        });
                }
            }
        }

        private async Task HandelPlaybackState(MediaPlaybackSession sender)
        {
            // Entered change in playback state:
            var timePlayedInPercentage = sender.MediaPlayer.PlaybackSession.Position.Ticks /
                                         (double) sender.MediaPlayer.PlaybackSession.NaturalDuration.Ticks;

            if (timePlayedInPercentage > 0.92)
                await UserSettings.DeletePlaybackState(SessionName).ConfigureAwait(true);
            else
                await UserSettings.WritePlaybackState(sender.MediaPlayer.PlaybackSession.Position, SessionName)
                    .ConfigureAwait(true);
        }
    }
}
