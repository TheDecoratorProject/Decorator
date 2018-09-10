using Decorator.Attributes;
using Decorator.Tester.MessageTypes;

using System.Collections.Generic;

namespace Decorator.Tester {

	public class Server {
		private uint _counter { get; set; }
		private Dictionary<uint, Client> _clients { get; } = new Dictionary<uint, Client>();

		public void Join(Client cli) {
			var msg = Serializer<ClientEvent>.Serialize(new ClientEvent {
				Id = this._counter,
				Username = "//todo: :)",
				JoinState = true
			});

			this._clients[this._counter] = cli;

			foreach (var i in this._clients)
				if (i.Key != this._counter)
					Deserializer<Client>.DeserializeMessageToMethod(i.Value, msg);

			var initMsg = Serializer<InitEvent>.Serialize(new InitEvent {
				MyId = this._counter
			});

			var clients = new List<ClientExistsEvent>();

			foreach (var i in this._clients)
				if (i.Key != this._counter)
					clients.Add(new ClientExistsEvent {
						Id = i.Key,
						Username = "//todogg: :)"
					});

			var online = Serializer<ClientExistsEvent>.Serialize(clients);

			Deserializer<Client>.DeserializeMessageToMethod(this._clients[this._counter], initMsg);
			Deserializer<Client>.DeserializeMessageToMethod(this._clients[this._counter], online);

			this._counter += 1;
		}

		public void Disconnect(uint clientId) {
			foreach (var i in this._clients)
				if (i.Key != clientId)
					Deserializer<Client>.DeserializeMessageToMethod(i.Value, Serializer<ClientEvent>.Serialize(new ClientEvent {
						Id = clientId,
						JoinState = false,
						Username = null
					}));

			Deserializer<Client>.DeserializeMessageToMethod(this._clients[clientId], Serializer<SafeDisconnectEvent>.Serialize(new SafeDisconnectEvent()));

			this._clients.Remove(clientId);
		}

		public void HandleMessage(BaseMessage msg, uint clientId) {
			Deserializer<Server>.DeserializeMessageToMethod(this, msg);
		}

		[DeserializedHandler]
		public void ChatMessage(SendChat schat) {
			var chat = new Chat {
				PlayerId = schat.ClientId,
				ChatMessage = schat.ChatMessage
			};

			var msg = Serializer<Chat>.Serialize(chat);

			foreach (var i in this._clients) {
				Deserializer<Client>.DeserializeMessageToMethod(i.Value, msg);
			}
		}
	}
}