using HomeAssistantClient.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeAssistantClient
{
    public interface IHassClient
    {

        /// <summary>
        /// Gets that the state of a single entity as an Observable.
        /// </summary>
        /// <param name="entityId">The entityid to get</param>
        /// <param name="timespan">The timeout interval</param>
        /// <returns></returns>
        IObservable<HassState> GetState(string entityId, TimeSpan? timespan = null);

        /// <summary>
        /// Gets the state of an entity
        /// </summary>
        /// <param name="entityId">The entity id to get</param>
        /// <param name="timespan">The timeout interval</param>
        /// <returns></returns>
        Task<HassState> GetStateAsync(string entityId, TimeSpan? timespan = null);

        /// <summary>
        /// A never ending observable of all responses
        /// </summary>
        /// <returns></returns>
        IObservable<HassResponse> GetMessages();

        /// <summary>
        /// Get all states possible for the currently authenticated user as Observable.
        /// </summary>
        /// <param name="timespan">The timeout interval</param>
        /// <returns>All states</returns>
        IObservable<HassStateResponse> GetStates(TimeSpan? timespan = null);

        /// <summary>
        /// Gets the state of the entities provided as an Observable.
        /// </summary>
        /// <param name="entitiyIds"></param>
        /// <returns></returns>
        IObservable<List<HassState>> GetStates(List<string> entitiyIds);

        /// <summary>
        /// Get all states possible for the currently authenticated user.
        /// </summary>
        /// <param name="timespan">The timeout interval</param>
        /// <returns>All states</returns>
        Task<HassStateResponse> GetStatesAsync(TimeSpan? timespan = null);

        /// <summary>
        /// Gets the state of the entities provided
        /// </summary>
        /// <param name="entitiyIds">The list of entity ids to filter on</param>
        /// <returns>A list of states for each entity that was found</returns>
        Task<List<HassState>> GetStatesAsync(List<string> entitiyIds);

        /// <summary>
        /// A never ending observable of events that you've subscribed to.
        /// </summary>
        /// <returns></returns>
        IObservable<HassEvent> ListenToStateChanges();

        /// <summary>
        /// Sends a command and sends back an Observable of the command response
        /// </summary>
        /// <param name="commandToSend"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        IObservable<HassResponse> SendCommand(HassCmdRequest commandToSend, TimeSpan? timespan = null);

        /// <summary>
        /// Sends a hass command to the websocket and waits for a response
        /// </summary>
        /// <param name="commandToSend">The command to send, see GenericCommand</param>
        /// <param name="timespan">The time to wait for the command to be acknowledged</param>
        /// <returns>The command response</returns>
        Task<HassResponse> SendCommandAsync(HassCmdRequest commandToSend, TimeSpan? timespan = null);

        /// <summary>
        /// Subscribes to the provided event type
        /// </summary>
        /// <param name="sub">The type of Hass Subscription</param>
        /// <returns>A HassResponse that the subscription was succesful</returns>
        Task<HassResponse> SubscribeToEvents(HassSubscribeEventType sub);

        /// <summary>
        /// Gets a list of all entities
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<List<string>> GetEntityList(TimeSpan? timeout = null);

        /// <summary>
        /// Unsubscribe to the provided event type, if it is currently active
        /// </summary>
        /// <param name="sub">The type of Hass Subscription</param>
        /// <returns>A HassResponse that the subscription was succesful</returns>
        Task<HassResponse> UnsubscribeToEvents(HassSubscribeEventType sub);
    }
}