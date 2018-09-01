using System;

namespace Decorator.Attributes {

	/// <summary>Limits the amount of arguments a message can have</summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ArgumentLimitAttribute : Attribute {

		public ArgumentLimitAttribute(int argLimit) => this.ArgLimit = argLimit;

		public int ArgLimit { get; set; }
	}
}