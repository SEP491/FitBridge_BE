namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    internal class HandshakeState : IDisposable
    {
        public int RetryCount { get; set; }

        public CancellationTokenSource? CancellationTokenSource { get; set; }

        // For Redis serialization - storing when the handshake started
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public void Dispose()
        {
            CancellationTokenSource?.Dispose();
        }
    }

    // Redis-serializable version of HandshakeState
    internal class HandshakeStateData
    {
        public int RetryCount { get; set; }

        public DateTime StartedAt { get; set; }
    }
}