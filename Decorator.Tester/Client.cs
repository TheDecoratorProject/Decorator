using Decorator.Attributes;
using Decorator.Tester.MessageTypes;

using System;
using System.Collections.Generic;

namespace Decorator.Tester
{
	public class Client
	{
		private uint _id { get; set; }
		private Server _server { get; set; }

		public void Join(Server server)
		{
			_server = server;
			_server.Join(this);
		}

		public void Disconnect()
		{
			if (_server != default)
			{
				_server.Disconnect(_id);
				_server = default;
			}
		}

		public void SendChat(string msg)
		{
			_server.HandleMessage(Serializer.SerializeItem(new SendChat {
				ChatMessage = msg,
				ClientId = _id
			}), _id);
		}

		[DeserializedHandler]
		public void SafeDisconnectEvent(SafeDisconnectEvent sde)
			=> Log("This client has disconnected safely.");

		[DeserializedHandler]
		public void ClientEvent(ClientEvent ce)
		{
			var friendlyState = ce.JoinState ? "joined" : "left";

			Log($"{ce.Id} has {friendlyState}! {ce.Username}");
		}

		[DeserializedHandler]
		public void ClientEvent(IEnumerable<ClientExistsEvent> ces)
		{
			foreach (var ce in ces)
			{
				Log($"{ce.Id} was here before I was here! {ce.Username}");
			}
		}

		[DeserializedHandler]
		public void InitHandler(InitEvent init)
		{
			_id = init.MyId;

			Log("Initialized!");
		}

		[DeserializedHandler]
		public void ChatHandler(Chat chat)
			=> Log($"Chat from {chat.PlayerId}: {chat.ChatMessage}");

		[DeserializedHandler]
		public void PingHandler(Ping ping)
			=> Log($"Recieved back ping: {ping.IntegerValue}");

		private void Log(string msg)
			=> Console.WriteLine($"[{_id}] {msg}");
	}
}