using System;
using Xels.Bitcoin.EventBus;

namespace Xels.Bitcoin.Features.SignalR
{
    public interface IClientEvent
    {
        Type NodeEventType { get; }

        void BuildFrom(EventBase @event);
    }
}