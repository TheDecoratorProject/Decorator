using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator {

	public class MessageException : Exception {
		internal MessageException(string msg) : base(msg) {

		}
	}

	public class InvalidMessageCountException : MessageException {
		internal InvalidMessageCountException(string msg) : base(msg) {

		}
	}

	public class BaseMessageInequalityException : MessageException {
		internal BaseMessageInequalityException(string msg) : base(msg) {

		}
	}

	public class InabilityToSetValueException : MessageException {
		internal InabilityToSetValueException(string msg) : base(msg) {

		}
	}
}