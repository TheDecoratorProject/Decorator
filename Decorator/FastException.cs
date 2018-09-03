using System;

namespace Decorator {

	internal interface IFastException {
		bool ThrownException { get; }
		object Result { get; }
		Exception Exception { get; }

		object GetResult();
	}

	internal class FastException<TResult> : IFastException {

		public FastException(Exception err) {
			this._state = false;
			this.Exception = err;
		}

		public FastException(TResult result) {
			this._state = true;
			this.Result = result;
		}

		private readonly bool _state;

		public bool ThrownException => !this._state;
		public TResult Result { get; }
		public Exception Exception { get; }

		public TResult GetResult() {
			if (this._state) return this.Result;
			else throw this.Exception;
		}

		object IFastException.GetResult() => this.GetResult();

		object IFastException.Result => this.Result;
	}
}