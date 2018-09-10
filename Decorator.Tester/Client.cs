using Decorator.Attributes;
using Decorator.Tester.MessageTypes;

using System;
using System.Collections.Generic;

namespace Decorator.Tester {

	public class Client {
		private uint _id { get; set; }
		private Server _server { get; set; }

		public void Join(Server server) {
			this._server = server;
			this._server.Join(this);
		}

		public void Disconnect() {
			if (this._server != default) {
				this._server.Disconnect(this._id);
				this._server = default;
			}
		}

		public void SendChat(string msg) {
			this._server.HandleMessage(Serializer<SendChat>.Serialize(new SendChat {
				ChatMessage = msg,
				ClientId = this._id
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
		public void ClientEvent(IEnumerable<ClientExistsEvent> ces) {
			foreach (var ce in ces) {
				this.Log($"{ce.Id} was here before I was here! {ce.Username}");
			}
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