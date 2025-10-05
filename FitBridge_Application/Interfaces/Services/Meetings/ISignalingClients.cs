namespace FitBridge_Application.Interfaces.Services.Meetings
{
    public interface ISignalingClients
    {
        Task StopMeeting();

        Task ShowExpirationAlert();

        /// <summary>
        /// Receives both offer & answer from a peer
        /// </summary>
        /// <param name="message">The SDP message</param>
        Task ReceiveMessage(object message);

        /// <summary>
        /// Receives an ICE candidate from a peer
        /// </summary>
        /// <param name="candidate">The ICE candidate</param>
        ///// <param name="fromConnectionId">The connection ID of the sender</param>
        Task ReceiveICECandidate(object candidate);

        /// <summary>
        /// Notifies when a user joins the WebRTC session
        /// </summary>
        /// <param name="username">The username of the user</param>
        Task UserJoined(string username);

        /// <summary>
        /// Notifies when a user leaves the WebRTC session
        /// </summary>
        /// <param name="username">The username of the user</param>
        Task UserLeft(string username);

        /// <summary>
        /// Notifies when a room does not exist
        /// </summary>
        /// <param name="roomId">The ID of the room</param>
        Task RoomDoesNotExist(string roomId);

        /// <summary>
        /// Notifies when a user is not authorized to join the WebRTC session
        /// </summary>
        /// <param name="roomId">The ID of the room</param>
        Task NotAuthorizedToJoin(string roomId);
    }
}