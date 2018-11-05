using Decorator.ModuleAPI;

using FluentAssertions;

using System;
using System.Reflection;

using Xunit;

namespace Decorator.Tests
{
	public class MemberTests
	{
		[Fact]
		public void Throws_AccessViolationException_When_AccessingNullPropertyAndField()
		{
			((Action)(() =>
			{
				default(Member).GetMemberType();
			})).Should().ThrowExactly<AccessViolationException>();
		}

		// i'm testing this stuff to make sure it works, but i really don't appreciate if you do what i'm doing :(

		[Fact]
		public void Throws_ArgumentNullException_When_ConstructingNullProperty()
		{
			((Action)(() =>
			{
				try
				{
					typeof(Member)
						.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
										Type.DefaultBinder,
										new[] { typeof(PropertyInfo) },
										null)
						.Invoke(new object[] { null });
				}
				catch (TargetInvocationException tie)
				{
					throw tie.InnerException;
				}
			})).Should().ThrowExactly<ArgumentNullException>();
		}

		[Fact]
		public void Throws_ArgumentNullException_When_ConstructingNullField()
		{
			((Action)(() =>
			{
				try
				{
					typeof(Member)
					.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
									Type.DefaultBinder,
									new[] { typeof(FieldInfo) },
									null)
					.Invoke(new object[] { null });
				}
				catch (TargetInvocationException tie)
				{
					throw tie.InnerException;
				}
			})).Should().ThrowExactly<ArgumentNullException>();
		}
	}
}