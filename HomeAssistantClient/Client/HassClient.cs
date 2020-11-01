using HomeAssistantClient.Config;
using HomeAssistantClient.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Websocket.Client;

namespace HomeAssistantClient
{
    public class HassClient : IHassClient
    {
        private int _id = 0;
        private IWebsocketClient _client;
        private HomeAssistantConfig _config;
        private Subject<HassMessage> Messages = new Subject<HassMessage>();
        private Subject<HassEvent> Events = new Subject<HassEvent>();
        private bool _isAuthed = false;
        private ConcurrentDictionary<HassSubscribeEventType, int> _subscriptions = new ConcurrentDictionary<HassSubscribeEventType, int>();

        /// <summary>
        /// The current sequential ID used by home assistant to correlate messages
        /// </summary>
        public int CurrentID => _id;

        /// <summary>
        /// The list of current subscriptions.
        /// </summary>
        /// <remarks>
        /// On startup this list will always contain a StateChange subscription.
        /// </remarks>
        public List<KeyValuePair<HassSubscribeEventType, int>> Subscriptions => _subscriptions.Where(a => a.Value > 0).ToList();

        /// <summary>
        /// The home assistant service.
        /// </summary>
        /// <exception cref="AuthInvalidException">Thrown when the provided Authentication Token is not valid.</exception>
        /// <param name="config">The home assistant config</param>
        public HassClient(HomeAssistantConfig config)
        {
            _config = config;
            _client = new WebsocketClient(new Uri(config.Host));
            // we need to make sure the handshake has started and finished
            Task.Run(async () => await StartHandshake()).Wait();
        }

        /// <summary>
        /// Starts the handshake with the home assistant websocket server and starts to listen for incoming messages.
        /// </summary>
        private async Task StartHandshake()
        {
            await _client.Start();
            _client.MessageReceived.Subscribe(msg =>
            {
                _ = IncomingMessageRouter(msg.Text);
            });

            await Authenticate();
        }

        /// <summary>
        /// Authenticates with the home assistant instance
        /// </summary>
        /// <returns></returns>
        private async Task Authenticate()
        {
            var newAuth = new AuthenticationMessage() { Token = _config.Token };
            await ClientSend(newAuth);
            await SubscribeToStateChange();
        }

        /// <summary>
        /// Subscribes to the all state changes
        /// </summary>
        /// <returns></returns>
        private async Task SubscribeToStateChange()
        {
            if (_isAuthed)
            {
                int id = GetId();
                _subscriptions.TryAdd(HassSubscribeEventType.StateChange, id);
                var initalSub = new HassSubscribeRequest() { Id = id, EventType = HassSubscribeEventType.StateChange };
                await ClientSend(initalSub);
            }
        }

        /// <summary>
        /// Sends a message to websocket endpoint.
        /// </summary>
        /// <typeparam name="T">A type that inherits from the HassMessage abstract class.</typeparam>
        /// <param name="messgae">The message (usually a command) to send to the home assistant websocket endpoint.</param>
        /// <returns></returns>
        private async Task ClientSend<T>(T messgae)
        {
            await _client.SendInstant(JsonConvert.SerializeObject(messgae));
        }

        /// <summary>
        /// Updates and then returns the next ID
        /// </summary>
        /// <returns>The lastest ID</returns>
        private int GetId()
        {
            ++_id;

            return _id;
        }

        /// <summary>
        /// Routes messages to their correct locations
        /// </summary>
        /// <param name="message">The incoming home assistant websocket message</param>
        /// <returns></returns>
        private async Task IncomingMessageRouter(string message)
        {
            var incomingMessage = JsonConvert.DeserializeObject<HassMessage>(message);

            if (incomingMessage.Type == HassReturnType.AuthOk)
            {
                _isAuthed = true;
                await SubscribeToStateChange();
            }

            if (incomingMessage.Type == HassReturnType.AuthRequired)
            {
                await Authenticate();
            }

            if (incomingMessage.Type == HassReturnType.AuthInvalid)
            {
                throw new AuthInvalidException();
            }

            if (incomingMessage.Type == HassReturnType.Result)
            {
                await ParseIncomingResult(incomingMessage as HassResponse);
            }

            if (incomingMessage.Type == HassReturnType.Event)
            {
                Events.OnNext(incomingMessage as HassEvent);
            }
        }

        /// <summary>
        /// Attempts to parse the incoming response result based on properties of the object.
        /// </summary>
        /// <param name="response">A response message from the home assistant websocket</param>
        /// <returns></returns>
        private async Task ParseIncomingResult(HassResponse response)
        {
            List<string> props = response.Result?.DescendantsAndSelf().OfType<JObject>().ToList().FirstOrDefault().Properties().Select(a => a.Name).ToList();

            if (props != null)
            {
                bool isStates = await PropertiesInObject(props, "entity_id", "state");
                bool isPanels = await PropertiesInObject(props, "hassio", "history");
                bool isConfig = await PropertiesInObject(props, "config_dir", "unit_system");
                bool isThemes = await PropertiesInObject(props, "themes");
                bool isServices = await PropertiesInObject(props, "hassio", "switch");

                if (isStates)
                {
                    Messages.OnNext(new HassStateResponse() { Result = response.Result.ToObject<List<HassState>>(), Id = response.Id });
                }
                else if (isConfig)
                {
                    Messages.OnNext(new HassConfigResponse() { Result = response.Result.ToObject<HassConfig>(), Id = response.Id });
                }
                else if (isPanels)
                {
                    Messages.OnNext(new HassPanelResponse() { Result = response.Result.ToObject<Dictionary<string, HassPanelEntries>>(), Id = response.Id });
                }
                else if (isThemes)
                {
                    Messages.OnNext(new HassThemeResponse() { Result = response.Result.ToObject<HassTheme>(), Id = response.Id });
                }
                else if (isServices)
                {
                    Messages.OnNext(new HassServicesResponse() { Result = response.Result.ToObject<Dictionary<string, dynamic>>(), Id = response.Id });
                }
                else
                {
                    Messages.OnNext(response);
                }
            }
            else
            {
                Messages.OnNext(response);
            }
        }

        /// <summary>
        /// Helper function to check if the parms are in the list of props
        /// </summary>
        /// <param name="props">The list of properties to check</param>
        /// <param name="list">The list of known strings that should be in that prop list</param>
        /// <returns></returns>
        private async Task<bool> PropertiesInObject(List<string> props, params string[] list)
        {
            Task<bool> task = Task.Run(() =>
            {
                foreach (var item in list)
                {
                    if (!props.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            });

            return await task;
        }

        /// <summary>
        /// Subscribes to the provided event type
        /// </summary>
        /// <param name="sub">The type of Hass Subscription</param>
        /// <returns>A HassResponse that the subscription was succesful</returns>
        public async Task<HassResponse> SubscribeToEvents(HassSubscribeEventType sub)
        {
            int id = GetId();
            if (_subscriptions.TryAdd(sub, id))
            {
                var subMsg = new HassSubscribeRequest() { Id = id, EventType = sub };
                await ClientSend(subMsg);
            }
            else
            {
                return null;
            }

            var stream = Messages.Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassResponse>()
                                                .Where(r => r.Id == id)
                                                .Timeout(TimeSpan.FromSeconds(5)).Catch(Observable.Return((HassResponse)null));

            return await stream.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Unsubscribe to the provided event type, if it is currently active
        /// </summary>
        /// <param name="sub">The type of Hass Subscription</param>
        /// <returns>A HassResponse that the subscription was succesful</returns>
        public async Task<HassResponse> UnsubscribeToEvents(HassSubscribeEventType sub)
        {
            if (_subscriptions.TryGetValue(sub, out int id))
            {
                var unsubMsg = new HassUnsubscribeRequest() { Subscription = id, Id = GetId() };
                await ClientSend(unsubMsg);
            }

            return null;
        }

        /// <summary>
        /// Sends a hass command to the websocket and waits for a response
        /// </summary>
        /// <param name="commandToSend">The command to send, see GenericCommand</param>
        /// <param name="timespan">The time to wait for the command to be acknowledged</param>
        /// <returns>The command response</returns>
        public async Task<HassResponse> SendCommandAsync(HassCmdRequest commandToSend, TimeSpan? timespan = null)
        {
            commandToSend.Id = GetId();

            if (!timespan.HasValue)
            {
                timespan = TimeSpan.FromSeconds(5);
            }

            await ClientSend(commandToSend);

            var stream = Messages.Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassResponse>()
                                                .Where(r => r.Id == commandToSend.Id).Take(1)
                                                .Timeout(timespan.Value).Catch(Observable.Return((HassResponse)null));

            return await stream.FirstOrDefaultAsync();
        }


        /// <summary>
        /// Sends a command and sends back an Observable of the command response
        /// </summary>
        /// <param name="commandToSend"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public IObservable<HassResponse> SendCommand(HassCmdRequest commandToSend, TimeSpan? timespan = null)
        {
            commandToSend.Id = GetId();

            if (!timespan.HasValue)
            {
                timespan = TimeSpan.FromSeconds(5);
            }

            ClientSend(commandToSend).Wait();

            var stream = Messages.Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassResponse>()
                                                .Where(r => r.Id == commandToSend.Id).Take(1)
                                                .Timeout(timespan.Value).Catch(Observable.Return((HassResponse)null));

            return stream;
        }

        /// <summary>
        /// Gets the state of an entity
        /// </summary>
        /// <param name="entityId">The entity id to get</param>
        /// <param name="timespan">The timeout interval</param>
        /// <returns></returns>
        public async Task<HassState> GetStateAsync(string entityId, TimeSpan? timespan = null)
        {
            int id = GetId();

            if (!timespan.HasValue)
            {
                timespan = TimeSpan.FromSeconds(5);
            }

            await ClientSend(new HassGenericCmd() { Id = id, CommandType = HassCommandType.GetStates });

            var stream = Messages.AsObservable().Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassStateResponse>()
                                                .Where(a => a.Id == id)
                                                .SelectMany(a => a.Result)
                                                .Where(a => a.EntityId == entityId).Take(1)
                                                .Timeout(timespan.Value).Catch(Observable.Return((HassState)null));

            return await stream.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets that the state of a single entity as an Observable.
        /// </summary>
        /// <param name="entityId">The entityid to get</param>
        /// <param name="timespan">The timeout interval</param>
        /// <returns></returns>
        public IObservable<HassState> GetState(string entityId, TimeSpan? timespan = null)
        {
            int id = GetId();

            if (!timespan.HasValue)
            {
                timespan = TimeSpan.FromSeconds(5);
            }

            ClientSend(new HassGenericCmd() { Id = id, CommandType = HassCommandType.GetStates }).Wait();

            var stream = Messages.Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassStateResponse>()
                                                .Where(a => a.Id == id)
                                                .SelectMany(a => a.Result)
                                                .Where(a => a.EntityId == entityId).Take(1)
                                                .Timeout(timespan.Value).Catch(Observable.Return((HassState)null));

            return stream;
        }

        /// <summary>
        /// Gets the state of the entities provided
        /// </summary>
        /// <param name="entitiyIds">The list of entity ids to filter on</param>
        /// <returns>A list of states for each entity that was found</returns>
        public async Task<List<HassState>> GetStatesAsync(List<string> entitiyIds)
        {
            int id = GetId();

            await ClientSend(new HassGenericCmd() { Id = id, CommandType = HassCommandType.GetStates });

            var stream = Messages.AsObservable().Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassStateResponse>()
                                                .Where(r => r.Id == id)
                                                .SelectMany(r => r.Result)
                                                .Where(r => entitiyIds.Contains(r.EntityId)).ToList();

            return await stream.FirstOrDefaultAsync().Cast<List<HassState>>();
        }

        /// <summary>
        /// Gets the state of the entities provided as an Observable.
        /// </summary>
        /// <param name="entitiyIds"></param>
        /// <returns></returns>
        public IObservable<List<HassState>> GetStates(List<string> entitiyIds)
        {
            int id = GetId();

            ClientSend(new HassGenericCmd() { Id = id, CommandType = HassCommandType.GetStates }).Wait();

            var stream = Messages.AsObservable().Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassStateResponse>()
                                                .Where(r => r.Id == id)
                                                .SelectMany(r => r.Result)
                                                .Where(r => entitiyIds.Contains(r.EntityId)).ToList();

            return stream.Cast<List<HassState>>();
        }

        /// <summary>
        /// Get all states possible for the currently authenticated user.
        /// </summary>
        /// <param name="timespan">The timeout interval</param>
        /// <returns>All states</returns>
        public async Task<HassStateResponse> GetStatesAsync(TimeSpan? timespan = null)
        {
            int id = GetId();

            if (!timespan.HasValue)
            {
                timespan = TimeSpan.FromSeconds(5);
            }

            await ClientSend(new HassGenericCmd() { Id = id, CommandType = HassCommandType.GetStates });

            var stream = Messages.Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassStateResponse>()
                                                .Where(r => r.Id == id)
                                                .Timeout(timespan.Value).Catch(Observable.Return((HassStateResponse)null));

            return await stream.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get all states possible for the currently authenticated user as Observable.
        /// </summary>
        /// <param name="timespan">The timeout interval</param>
        /// <returns>All states</returns>
        public IObservable<HassStateResponse> GetStates(TimeSpan? timespan = null)
        {
            int id = GetId();

            if (!timespan.HasValue)
            {
                timespan = TimeSpan.FromSeconds(5);
            }

            ClientSend(new HassGenericCmd() { Id = id, CommandType = HassCommandType.GetStates }).Wait();

            var stream = Messages.Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassStateResponse>()
                                                .Where(r => r.Id == id)
                                                .Timeout(timespan.Value).Catch(Observable.Return((HassStateResponse)null));
            return stream;
        }

        /// <summary>
        /// Gets a list of all entities
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<string>> GetEntityList(TimeSpan? timeout = null)
        {
            int id = GetId();

            if (!timeout.HasValue)
            {
                timeout = TimeSpan.FromSeconds(5);
            }

            await ClientSend(new HassGenericCmd() { Id = id, CommandType = HassCommandType.ListEntities });

            var stream = Messages.Where(r => r.Type == HassReturnType.Result)
                                                .OfType<HassResponse>()
                                                .Where(r => r.Id == id).Take(1)
                                                .Select(a => a.Result.DescendantsAndSelf().OfType<JObject>().ToList().FirstOrDefault().Properties().Select(a => a.Name).ToList()).Cast<List<string>>().Timeout(timeout.Value).Catch(null);

            return await stream.FirstOrDefaultAsync();
        }

        /// <summary>
        /// A never ending observable of all responses (commands, etc)
        /// </summary>
        /// <returns></returns>
        public IObservable<HassResponse> GetMessages()
        {
            return Messages.OfType<HassResponse>().AsObservable();
        }

        /// <summary>
        /// A never ending observable of events that you've subscribed to.
        /// </summary>
        /// <returns></returns>
        public IObservable<HassEvent> ListenToStateChanges()
        {
            return Events.AsObservable();
        }
    }
}
