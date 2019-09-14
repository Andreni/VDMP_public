using System;
using Windows.Media.Core;
using Windows.Media.Playback;
using VDMP.App.Helpers;

namespace VDMP.App.ViewModels
{
    public class MediaPlayerViewModel : Observable
    {
        // TODO WTS: Set your default video and image URIs
        private const string DefaultSource =
            "https://sec.ch9.ms/ch9/db15/43c9fbed-535e-4013-8a4a-a74cc00adb15/C9L12WinTemplateStudio_high.mp4";

        // The poster image is displayed until the video is started
        private const string DefaultPoster = "ms-appx:///Assets/splashwide.png";

        private string _posterSource;

        private IMediaPlaybackSource _source;

        public MediaPlayerViewModel()
        {
            Source = MediaSource.CreateFromUri(new Uri(DefaultSource));
            PosterSource = DefaultPoster;
        }

        public IMediaPlaybackSource Source
        {
            get => _source;
            set => Set(ref _source, value);
        }

        public string PosterSource
        {
            get => _posterSource;
            set => Set(ref _posterSource, value);
        }

        public void DisposeSource()
        {
            var mediaSource = Source as MediaSource;
            mediaSource?.Dispose();
            Source = null;
        }
    }
}