using HomeAssistantApi.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeAssistantApi
{
    public interface IHassClient
    {
        IObservable<HassState> GetState(string entityId, TimeSpan? timespan = null);
        Task<HassState> GetStateAsync(string entityId, TimeSpan? timespan = null);
        IObservable<HassResponse> GetMessages();
        IObservable<HassStateResponse> GetStates(TimeSpan? timespan = null);
        IObservable<List<HassState>> GetStates(List<string> entitiyIds);
        Task<HassStateResponse> GetStatesAsync(TimeSpan? timespan = null);
        Task<List<HassState>> GetStatesAsync(List<string> entitiyIds);
        IObservable<HassEvent> ListenToStateChanges();
        IObservable<HassResponse> SendCommand(CommandRequest commandToSend, TimeSpan? timespan = null);
        Task<HassResponse> SendCommandAsync(CommandRequest commandToSend, TimeSpan? timespan = null);
        Task<HassResponse> SubscribeToEvents(SubscribeEventType sub);
        Task<HassResponse> UnsubscribeToEvents(SubscribeEventType sub);
    }
}