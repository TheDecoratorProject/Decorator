using Decorator.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Decorator {

	public interface IDeserializer<TClass>
		where TClass : class {
		ITypeManager TypeManager { get; }
		IDeserializableHandlerManager<TClass> DeserializableHandlerManager { get; }

		bool CanDeserialize<TItem>(BaseMessage m);

		bool CanDeserialize(Type t, BaseMessage m);

		bool CanDeserializeRepeats<TItem>(BaseMessage m);

		bool CanDeserializeRepeats(Type t, BaseMessage m);

		IEnumerable<TItem> DeserializeRepeats<TItem>(BaseMessage m) where TItem : new();

		IEnumerable<object> DeserializeRepeats(Type t, BaseMessage m);

		TItem Deserialize<TItem>(BaseMessage m) where TItem : new();

		object Deserialize(Type t, BaseMessage m);

		void DeserializeToMethod(TClass instance, BaseMessage msg);

		void DeserializeToMethod<T>(TClass instance, T item);
	}

	public class Deserializer<TClass> : IDeserializer<TClass>
		where TClass : class {

		public Deserializer() {
			this.TypeManager = new TypeManager();
			this.DeserializableHandlerManager = new DeserializableHandlerManager<TClass>();
		}

		public ITypeManager TypeManager { get; }
		public IDeserializableHandlerManager<TClass> DeserializableHandlerManager { get; }

		public bool CanDeserialize<TItem>(BaseMessage m)
			=> CanDeserialize(typeof(TItem), m);

		public bool CanDeserialize(Type t, BaseMessage m)
			=> this.TypeManager.QualifiesAsType(t, m);

		public TItem Deserialize<TItem>(BaseMessage m)
			where TItem : new()
			=> (TItem)Deserialize(typeof(TItem), m);

		public object Deserialize(Type t, BaseMessage m) {
			if (this.CanDeserialize(t, m))
				return this.TypeManager.DeserializeToType(t, m);

			throw new DecoratorException("no");
		}

		public bool CanDeserializeRepeats<TItem>(BaseMessage m)
			=> CanDeserializeRepeats(typeof(TItem), m);

		public bool CanDeserializeRepeats(Type t, BaseMessage m) {
			var positions = t.GetPositions();

			//NOTE: copied, so may want to not reinvent the wheel here
			var maxPos = positions.Length > 0
				? positions
					.Max(x => x.GetPosition()) + 1
				: 0;

			if (m.Count % maxPos != 0) return false;

			for (uint i = 0; i < m.Count / maxPos; i++) {
				var args = new object[maxPos];

				Array.Copy(m.Arguments, i * (maxPos), args, 0, args.Length);

				var msg = new BasicMessage(m.Type, args);

				if (!CanDeserialize(t, msg)) return false;
			}

			return true;
		}

		public IEnumerable<TItem> DeserializeRepeats<TItem>(BaseMessage m) where TItem : new()
			=> FromObj<TItem>(DeserializeRepeats(typeof(TItem), m)).ToArray();

		public IEnumerable<object> DeserializeRepeats(Type t, BaseMessage m) {
			if (!CanDeserializeRepeats(t, m))
				throw new DecoratorException("noOOO");

			var positions = t.GetPositions();

			//NOTE: copied, so may want to not reinvent the wheel here
			var maxPos = positions.Length > 0
				? positions
					.Max(x => x.GetPosition()) + 1
				: 0;

			if (m.Count % maxPos != 0)

				//TODO: exceptions, or CanDeserialize, or something /shrug
				return default;

			var results = new List<object>();

			for (uint i = 0; i < m.Count / maxPos; i++) {
				var args = new object[maxPos];

				Array.Copy(m.Arguments, i * (maxPos), args, 0, args.Length);

				var msg = new BasicMessage(m.Type, args);

				results.Add(Deserialize(t, msg));
			}

			return results;
		}

		public void DeserializeToMethod(TClass instance, BaseMessage msg) {
			foreach (var i in this.DeserializableHandlerManager.Cache) {
				if (CanDeserialize(i.Key, msg))
					foreach (var k in i.Value)
						this.DeserializableHandlerManager.InvokeMethod(k, instance, Deserialize(i.Key, msg));
			}
		}

		public void DeserializeToMethod<TItem>(TClass instance, TItem item) {
			foreach (var i in this.DeserializableHandlerManager.GetHandlersFor<TItem>()) {
				this.DeserializableHandlerManager.InvokeMethod<TItem>(i, instance, item);
			}
		}

		private IEnumerable<T> FromObj<T>(IEnumerable<object> objs) {
			foreach (var i in objs)
				yield return (T)i;
		}
	}
}