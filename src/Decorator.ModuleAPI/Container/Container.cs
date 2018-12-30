using System;

namespace Decorator.ModuleAPI
{
	public class Container : BaseContainer
	{
		public Container(Member member)
		{
			Member = member;
		}

		public override Member Member { get; }
	}
}