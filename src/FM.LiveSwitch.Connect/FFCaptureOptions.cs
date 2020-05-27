﻿using CommandLine;

namespace FM.LiveSwitch.Connect
{
    [Verb("ffcapture", HelpText = "Captures local media from FFmpeg.")]
    class FFCaptureOptions : SendOptions
    {
        [Option("input-args", Required = true, HelpText = "The FFmpeg input arguments.")]
        public string InputArgs { get; set; }

        [Option("audio-mode", Required = false, Default = FFCaptureMode.LSEncode, HelpText = "Where audio is encoded.")]
        public FFCaptureMode AudioMode { get; set; }

        [Option("video-mode", Required = false, Default = FFCaptureMode.LSEncode, HelpText = "Where video is encoded.")]
        public FFCaptureMode VideoMode { get; set; }

        [Option("ffencode-keyframe-interval", Required = false, Default = 30, HelpText = "The keyframe interval for video. Only used if video-mode is ffencode.")]
        public int FFEncodeKeyFrameInterval { get; set; }
    }
}
