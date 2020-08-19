﻿using CommandLine;
using CommandLine.Text;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FM.LiveSwitch.Connect
{
    partial class Program
    {
        static void Main(string[] args)
        {
            using var parser = new Parser((settings) =>
            {
                settings.CaseInsensitiveEnumValues = true;
                settings.HelpWriter = null;
            });

            var result = parser.ParseArguments<
                ShellOptions,
                CaptureOptions, 
                FFCaptureOptions,
                FakeOptions,
                PlayOptions, 
                RenderOptions,
                FFRenderOptions,
                LogOptions,
                RecordOptions,
                InterceptOptions
            >(args);

            result.MapResult(
                (ShellOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new ShellRunner(options).Run();
                    }).GetAwaiter().GetResult();
                },
                (CaptureOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new Capturer(options).Capture();
                    }).GetAwaiter().GetResult();
                },
                (FFCaptureOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new FFCapturer(options).Capture();
                    }).GetAwaiter().GetResult();
                },
                (FakeOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new Faker(options).Fake();
                    }).GetAwaiter().GetResult();
                },
                (PlayOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new Player(options).Play();
                    }).GetAwaiter().GetResult();
                },
                (RenderOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new Renderer(options).Render();
                    }).GetAwaiter().GetResult();
                },
                (FFRenderOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new FFRenderer(options).Render();
                    }).GetAwaiter().GetResult();
                },
                (LogOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new Logger(options).Log();
                    }).GetAwaiter().GetResult();
                },
                (RecordOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new Recorder(options).Record();
                    }).GetAwaiter().GetResult();
                },
                (InterceptOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        Initialize(options);
                        return await new Interceptor(options).Intercept();
                    }).GetAwaiter().GetResult();
                },
                errors =>
                {
                    var helpText = HelpText.AutoBuild(result);
                    helpText.Copyright = "Copyright (C) 2020 Frozen Mountain Software Ltd.";
                    helpText.AddEnumValuesToHelpText = true;
                    Console.Error.Write(helpText);
                    return 1;
                });
        }

        private static void Initialize(Options options)
        {
            Log.AddProvider(new ErrorLogProvider(options.LogLevel));

            Console.Error.WriteLine("Checking for OpenH264...");
            OpenH264.Utility.DownloadOpenH264(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).GetAwaiter().GetResult();
            options.DisableOpenH264 = true;
            try
            {
                options.DisableOpenH264 = !OpenH264.Utility.Initialize();
                if (options.DisableOpenH264)
                {
                    Console.Error.WriteLine("OpenH264 failed to initialize.");
                }
                else
                {
                    Console.Error.WriteLine("OpenH264 initialized.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"OpenH264 failed to initialize. {ex}");
            }

            Console.Error.WriteLine("Checking for Nvidia hardware acceleration...");
            options.DisableNvidia = !Nvidia.Utility.NvencSupported;
            if (options.DisableNvidia)
            {
                Console.Error.WriteLine("Nvidia hardware acceleration failed to initialize.");
            }
            else
            {
                Console.Error.WriteLine("Nvidia hardware acceleration initialized.");
            }
        }
    }
}
