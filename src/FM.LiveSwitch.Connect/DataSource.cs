﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace FM.LiveSwitch.Connect
{
    class DataSource : IDisposable
    {
        public Action<DataSource, string> OnMessage;

        public bool IsStarted { get; private set; }
        public bool IsStopped { get; private set; } = true;

        private readonly object StateLock = new object();

        private CancellationTokenSource CancellationTokenSource;

        public Task Start()
        {
            lock (StateLock)
            {
                if (IsStarted)
                {
                    return Task.CompletedTask;
                }

                IsStarted = true;
                IsStopped = false;
                CancellationTokenSource = new CancellationTokenSource();
            }

            Task.Run(async () =>
            {
                while (IsStarted)
                {
                    try
                    {
                        await Task.Run(() =>
                        {
                            var message = Console.ReadLine();
                            if (message != null && IsStarted)
                            {
                                OnMessage?.Invoke(this, message);
                            }
                        }, CancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        // Do nothing. The operation was canceled.
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("An exception was encountered while reading from stdin. {0}", ex);
                    }
                }
            });

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            lock (StateLock)
            {
                if (IsStopped)
                {
                    return Task.CompletedTask;
                }

                IsStarted = false;
                IsStopped = true;
                CancellationTokenSource.Cancel();
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            CancellationTokenSource.Dispose();
        }
    }
}
