﻿using GigaChat.Contracts.Hubs.Chat;
using GigaChat.Contracts.Hubs.Chat.Models.Output;
using GigaChat.Core.ChatRooms.Events;
using GigaChat.Server.SignalR.Hubs.Chat;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.SignalR;

namespace GigaChat.Server.SignalR.EventHandlers.Chat;

public class JoinToChatRoomEventHandler : IRequestHandler<JoinToChatRoomEvent>
{
    private readonly IHubContext<ChatHub, IChatClientHub> _hubContext;
    private readonly IMapper _mapper;

    public JoinToChatRoomEventHandler(IHubContext<ChatHub, IChatClientHub> hubContext, IMapper mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task Handle(JoinToChatRoomEvent request, CancellationToken cancellationToken)
    {
        var outputModel = _mapper.Map<JoinToChatRoomOutputModel>(request);
        var chatRoom = request.ChatRoom;
        var userId = request.UserId;
        
        if (ChatHub.ConnectionIds.TryGetValue(userId, out var connectionId))
            await _hubContext.Groups.AddToGroupAsync(connectionId, chatRoom.Id.ToString(), cancellationToken);

        await _hubContext.Clients
            .Users(userId.ToString())
            .SendJoinToChatRoom(outputModel);
    }
}