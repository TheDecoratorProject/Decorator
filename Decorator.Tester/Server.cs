using Decorator.Attributes;
using Decorator.Tester.MessageTypes;

using System.Collections.Generic;

namespace Decorator.Tester
{
	public class Server
	{
		private uint _counter { get; set; }
		private Dictionary<uint, Client> _clients { get; } = new Dictionary<uint, Client>();

		public void Join(Client cli)
		{
			var msg = Serializer.SerializeItem(new ClientEvent {
				Id = _counter,
				Username = "//todo: :)",
				JoinState = true
			});

			_clients[_counter] = cli;

			foreach (var i in _clients)
				if (i.Key != _counter)
					Deserializer<Client>.InvokeMethodFromMessage(i.Value, msg);

			var initMsg = Serializer.SerializeItem(new InitEvent {
				MyId = _counter
			});

			var clients = new List<ClientExistsEvent>();

			foreach (var i in _clients)
				if (i.Key != _counter)
					clients.Add(new ClientExistsEvent {
						Id = i.Key,
						Username = "//todogg: :)"
					});

			var online = Serializer.SerializeItems(clients);

			Deserializer<Client>.InvokeMethodFromMessage(_clients[_counter], initMsg);
			Deserializer<Client>.InvokeMethodFromMessage(_clients[_counter], online);

			_counter += 1;
		}

		public void Disconnect(uint clientId)
		{
			foreach (var i in _clients)
				if (i.Key != clientId)
					Deserializer<Client>.InvokeMethodFromMessage(i.Value, Serializer.SerializeItem(new ClientEvent {
						Id = clientId,
						JoinState = false,
						Username = null
					}));

			Deserializer<Client>.InvokeMethodFromMessage(_clients[clientId], Serializer.SerializeItem(new SafeDisconnectEvent()));

			_clients.Remove(clientId);
		}

		public void HandleMessage(BaseMessage msg, uint clientId)
			=> Deserializer<Server>.InvokeMethodFromMessage(this, msg);

		[DeserializedHandler]
		public void ChatMessage(SendChat schat)
		{
			var chat = new Chat {
				PlayerId = schat.ClientId,
				ChatMessage = schat.ChatMessage
			};

			var msg = Serializer.SerializeItem(chat);

			foreach (var i in _clients)
			{
				Deserializer<Client>.InvokeMethodFromMessage(i.Value, msg);
			}
		}
	}
}