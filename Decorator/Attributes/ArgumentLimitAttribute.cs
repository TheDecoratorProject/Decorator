using System;

namespace Decorator.Attributes {

	/// <summary>
	/// Prevents the message from having any more arguments then the amount specified.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ArgumentLimitAttribute : Attribute {

		public ArgumentLimitAttribute(int argLimit) => this.ArgLimit = argLimit;

		public int ArgLimit { get; set; }
	}
}