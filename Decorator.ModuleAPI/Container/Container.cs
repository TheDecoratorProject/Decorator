using System;

namespace Decorator.ModuleAPI
{
	public class Container : BaseContainer
	{
		public Container(Type modifiedType, Member member)
		{
			ModifiedType = modifiedType ?? throw new ArgumentNullException(nameof(modifiedType));
			Member = member;
		}

		public Type ModifiedType { get; }
		public override Member Member { get; }
	}
}