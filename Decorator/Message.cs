using System;
using System.Collections.Generic;

namespace Decorator {

	public abstract class BaseMessage {
		public abstract string Type { get; }
		public abstract object[] Arguments { get; }

		public uint Count => (uint)this.Arguments.Length;
		public object this[uint index] => this.Arguments[index];

		public override bool Equals(object obj) {
			if (ReferenceEquals(this, obj)) return true;

			if (obj == null) return false;

			if (obj is BaseMessage bm &&
				this.Type == bm.Type) {

				if (this.Arguments == bm.Arguments) return true;

				if ((this.Arguments == null && bm.Arguments != null) ||
					(this.Arguments != null && bm.Arguments == null)) return false;

				if (bm.Arguments.Length != this.Arguments.Length) return false;

				return ValueEquals(this.Arguments, bm.Arguments);
			}

			return false;
		}

		private static bool ValueEquals(object[] a, object[] b) {
			for (var i = 0; i < a.Length; i++)
				if (!a[i].Equals(b[i]))
					return false;
			return true;
		}

		public override int GetHashCode() {
			var hashCode = 1627454568;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Type);
			hashCode = hashCode * -1521134295 + EqualityComparer<object[]>.Default.GetHashCode(this.Arguments);
			hashCode = hashCode * -1521134295 + this.Count.GetHashCode();
			return hashCode;
		}
	}

	public class BasicMessage : BaseMessage {

		public BasicMessage(string type, params object[] args) {
			this.Type = type;
			this.Arguments = args;
		}

		public override string Type { get; }
		public override object[] Arguments { get; }
	}
}