using System.Text;

namespace Decorator {

	/// <summary>The message class that is used for all serialization/deserialization transfers</summary>
	public class Message {

		public Message(string type, params object[] args) {
			this.Type = type;
			this.Args = args;
		}

		public string Type { get; }
		public object[] Args { get; }

		internal int MatchAmount { get; set; }

		public override string ToString() {
			var strb = new StringBuilder();

			strb.AppendLine($"Type: {this.Type ?? "null"}");

			for (int i = 0; i < this.Args.Length; i++)
				strb.AppendLine($"\t[{i}] {this.Args[i].GetType()}: {this.Args[i]}");

			return strb.ToString();
		}
	}
}