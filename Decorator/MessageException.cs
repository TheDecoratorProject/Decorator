using System;

namespace Decorator {

	public class MessageException : Exception {

		public MessageException(string msg) : base(msg) {
		}
	}
}