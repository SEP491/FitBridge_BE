namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    internal class HandshakeState
    {
        public int RetryCount { get; set; }

        public CancellationTokenSource? CancellationTokenSource { get; set; }
    }
}