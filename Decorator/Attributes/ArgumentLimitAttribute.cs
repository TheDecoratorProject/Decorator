using System;

namespace Decorator.Attributes {

	/// <summary>Limits the amount of arguments a message can have</summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ArgumentLimitAttribute : Attribute, IMessageRequirementAttribute {

		public ArgumentLimitAttribute(int argLimit) {
			this.ArgLimit = argLimit;
		}

		public int ArgLimit { get; set; }

		public bool MeetsRequirements(Message msg) {
			if (msg.Args.Length > this.ArgLimit) throw new ArgumentOutOfRangeException($"Too many arguments - exceeds the ArgLimit amount set.");
			return true;
		}
	}
}