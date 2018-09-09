using Decorator.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {

	public interface IDeserializer<TClass>
		where TClass : class {
		IMessageManager TypeManager { get; }
		IDeserializableHandlerManager<TClass> DeserializableHandlerManager { get; }

		bool CanDeserialize<TItem>(BaseMessage m);

		bool CanDeserialize(Type t, BaseMessage m);

		bool CanDeserializeRepeats<TItem>(BaseMessage m);

		bool CanDeserializeRepeats(Type t, BaseMessage m);

		IEnumerable<TItem> DeserializeRepeats<TItem>(BaseMessage m) where TItem : new();

		IEnumerable<object> DeserializeRepeats(Type t, BaseMessage m);

		TItem Deserialize<TItem>(BaseMessage m) where TItem : new();

		object Deserialize(Type t, BaseMessage m);

		void DeserializeMessageToMethod(TClass instance, BaseMessage msg);

		void DeserializeItemToMethod<T>(TClass instance, T item);
	}

	public class Deserializer<TClass> : IDeserializer<TClass>
		where TClass : class {

		public Deserializer() {
			this.TypeManager = new MessageManager();
			this.DeserializableHandlerManager = new DeserializableHandlerManager<TClass>();
		}

		public IMessageManager TypeManager { get; }
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

		public bool CanDeserializeRepeats(Type t, BaseMessage m)
			=> this.TypeManager.QualifiesAsRepeatableType(t, m);

		public IEnumerable<TItem> DeserializeRepeats<TItem>(BaseMessage m) where TItem : new()
			=> FromObj<TItem>(DeserializeRepeats(typeof(TItem), m)).ToArray();

		public IEnumerable<object> DeserializeRepeats(Type t, BaseMessage m) {
			if (this.TypeManager.QualifiesAsRepeatableType(t, m))
				return this.TypeManager.DeserializeRepeatableToType(t, m).ToArray();

			throw new DecoratorException("nooo");
		}

		public void DeserializeMessageToMethod(TClass instance, BaseMessage msg) {
			foreach (var i in this.DeserializableHandlerManager.Cache) {
				if (CanDeserialize(i.Key, msg)) {
					var des = Deserialize(i.Key, msg);

					foreach (var k in i.Value)
						this.DeserializableHandlerManager.InvokeMethod(k, instance, des);
				}

				if(i.Key.GenericTypeArguments.Length > 0)
					if (i.Key == typeof(IEnumerable<>).MakeGenericType(i.Key.GenericTypeArguments[0]))
						if (CanDeserializeRepeats(i.Key.GenericTypeArguments[0], msg)) {
							var des = this.DeserializeRepeats(i.Key.GenericTypeArguments[0], msg);

							var result = this.GetType()
											.GetMethod(nameof(FromObjToArray), BindingFlags.Static | BindingFlags.NonPublic)
											.MakeGenericMethod(i.Key.GenericTypeArguments[0])
											.Invoke(null, new [] { des });

							foreach (var k in i.Value)
								this.DeserializableHandlerManager.InvokeMethod(k, (object)instance, result);
						}
			}
		}

		public void DeserializeItemToMethod<TItem>(TClass instance, TItem item) {
			foreach (var i in this.DeserializableHandlerManager.GetHandlersFor<TItem>()) {
				this.DeserializableHandlerManager.InvokeMethod<TItem>(i, instance, item);
			}
		}

		private static IEnumerable<T> FromObjToArray<T>(IEnumerable<object> objs)
			=> FromObj<T>(objs).ToArray();

		private static IEnumerable<T> FromObj<T>(IEnumerable<object> objs) {
			foreach (var i in objs)
				yield return (T)i;
		}
	}
}