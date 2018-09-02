using Decorator.Attributes;
using Decorator.Tester.MessageTypes;

using System;

namespace Decorator.Tester {

	public class Client {
		private uint _id { get; set; }
		private Server _server { get; set; }

		public void Join(Server server) {
			this._server = server;
			this._server.Join(this);
		}

		public void Disconnect() {
			if (this._server != default(Server)) {
				this._server.Disconnect(this._id);
				this._server = default(Server);
			}
		}

		public void SendChat(string msg) {
			this._server.HandleMessage(Serializer.Serialize(new SendChat {
				ChatMessage = msg
			}), this._id);
		}

		[DeserializedHandler]
		public void SafeDisconnectEvent(SafeDisconnectEvent sde)
			=> this.Log("This client has disconnected safely.");

		[DeserializedHandler]
		public void ClientEvent(ClientEvent ce) {
			var friendlyState = ce.JoinState ? "joined" : "left";

			this.Log($"{ce.Id} has {friendlyState}! {ce.Username}");
		}

		[DeserializedHandler]
		public void InitHandler(InitEvent init) {
			this._id = init.MyId;

			this.Log("Initialized!");
		}

		[DeserializedHandler]
		public void ChatHandler(Chat chat)
			=> this.Log($"Chat from {chat.PlayerId}: {chat.ChatMessage}");

		[DeserializedHandler]
		public void PingHandler(Ping ping)
			=> this.Log($"Recieved back ping: {ping.IntegerValue}");

		private void Log(string msg)
			=> Console.WriteLine($"[{this._id}] {msg}");
	}
}