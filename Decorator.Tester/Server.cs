using Decorator.Attributes;
using Decorator.Tester.MessageTypes;

using System.Collections.Generic;

namespace Decorator.Tester {

	public class Server {
		private uint _counter { get; set; }
		private Dictionary<uint, Client> _clients { get; } = new Dictionary<uint, Client>();

		public void Join(Client cli) {
			var msg = Serializer.Serialize(new ClientEvent {
				Id = this._counter,
				Username = "//todo: :)",
				JoinState = true
			});

			this._clients[this._counter] = cli;

			foreach (var i in this._clients)
				if (i.Key != this._counter)
					Deserializer.TryDeserializeToEvent<Client>(i.Value, msg);

			var initMsg = Serializer.Serialize(new InitEvent {
				MyId = this._counter
			});

			var clients = new List<ClientExistsEvent>();

			foreach (var i in this._clients)
				if (i.Key != this._counter)
					clients.Add(new ClientExistsEvent {
						Id = i.Key,
						Username = "//todogg: :)"
					});

			var online = Serializer.SerializeEnumerable(clients);

			Deserializer.TryDeserializeToEvent<Client>(this._clients[this._counter], initMsg);
			Deserializer.TryDeserializeToEvent<Client>(this._clients[this._counter], online);

			this._counter += 1;
		}

		public void Disconnect(uint clientId) {
			foreach (var i in this._clients)
				if (i.Key != clientId)
					Deserializer.TryDeserializeToEvent<Client>(i.Value, Serializer.Serialize(new ClientEvent {
						Id = clientId,
						JoinState = false,
						Username = null
					}));

			Deserializer.TryDeserializeToEvent<Client>(this._clients[clientId], Serializer.Serialize(new SafeDisconnectEvent()));

			this._clients.Remove(clientId);
		}

		public void HandleMessage(Message msg, uint clientId) {
			if (this._clients.TryGetValue(clientId, out var client))
				Deserializer.TryDeserializeToEvent<Server>(this, msg, clientId);
		}

		[DeserializedHandler]
		public void ChatMessage(SendChat schat, uint clientId) {
			var chat = new Chat {
				PlayerId = clientId,
				ChatMessage = schat.ChatMessage
			};

			var msg = Serializer.Serialize(chat);

			foreach (var i in this._clients) {
				Deserializer.TryDeserializeToEvent<Client>(i.Value, msg);
			}
		}
	}
}