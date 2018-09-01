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
					Deserializer.DeserializeToEvent<Client>(i.Value, msg);

			var initMsg = Serializer.Serialize(new InitEvent {
				MyId = this._counter
			});

			Deserializer.DeserializeToEvent<Client>(this._clients[this._counter], initMsg);

			this._counter++;
		}

		public void Disconnect(uint clientId) {
			foreach (var i in this._clients)
				if (i.Key != clientId)
					Deserializer.DeserializeToEvent<Client>(i.Value, Serializer.Serialize(new ClientEvent {
						Id = clientId,
						JoinState = false,
						Username = null
					}));

			Deserializer.DeserializeToEvent<Client>(this._clients[clientId], Serializer.Serialize(new SafeDisconnectEvent()));

			this._clients.Remove(clientId);
		}

		public void HandleMessage(Message msg, uint clientId) {
			if (this._clients.TryGetValue(clientId, out var client))
				Deserializer.DeserializeToEvent<Server>(this, msg, clientId, client);
		}

		[DeserializedHandler]
		public void ChatMessage(SendChat schat, uint clientId) {
			var chat = new Chat {
				PlayerId = clientId,
				ChatMessage = schat.ChatMessage
			};

			var msg = Serializer.Serialize(chat);

			foreach (var i in this._clients) {
				Deserializer.DeserializeToEvent<Client>(i.Value, msg);
			}
		}
	}
}