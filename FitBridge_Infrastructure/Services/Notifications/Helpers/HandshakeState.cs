namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    internal class HandshakeState : IDisposable
    {
        public int RetryCount { get; set; }

        public CancellationTokenSource? CancellationTokenSource { get; set; }

        public void Dispose()
        {
            CancellationTokenSource?.Dispose();
        }
    }
}