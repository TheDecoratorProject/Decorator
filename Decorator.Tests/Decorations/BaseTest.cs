using FluentAssertions;

using System;
using System.Reflection;

namespace Decorator.Tests.Decorations
{
	public class BaseTest<TClass, TMemberType, TFactory, TType>
			where TClass : new()
			where TMemberType : MemberInfo
			where TFactory : IDecorationFactory
	{
		private readonly IMemberTest<TMemberType, TFactory> _tester;
		private readonly TMemberType _memberInfo;
		private readonly Func<TClass, TType> _getValue;
		private readonly Action<TClass, TType> _setValue;
		private readonly int _expectedEstimateSize;
		private readonly TType _value;

		public BaseTest(TType value, IMemberTest<TMemberType, TFactory> tester, TMemberType memberInfo, Func<TClass, TType> getValue, Action<TClass, TType> setValue, int expectedEstimateSize)
		{
			_value = value;
			_tester = tester;
			_memberInfo = memberInfo;
			_getValue = getValue;
			_setValue = setValue;
			_expectedEstimateSize = expectedEstimateSize;
		}

		public void Deserialize()
		{
			var objArray = new ObjectArray();
			var decoration = _tester.GetDecoration<TType>(_memberInfo);
			var instance = new TClass();

			objArray.Data[objArray.Index] = _value;

			decoration.Deserialize(ref objArray.Data, instance, ref objArray.Index)
				.Should()
				.BeTrue();

			_getValue(instance)
				.Should()
				.Be(_value);
		}

		public void Serialize()
		{
			var objArray = new ObjectArray();
			var decoration = _tester.GetDecoration<TType>(_memberInfo);
			var instance = new TClass();

			_setValue(instance, _value);

			decoration.Serialize(ref objArray.Data, instance, ref objArray.Index);

			objArray.Data[1]
				.Should()
				.Be(_value);
		}

		public void EstimateSize()
		{
			var instance = new TClass();
			var size = 0;

			_tester.GetDecoration<TType>(_memberInfo).EstimateSize(instance, ref size);

			size
				.Should()
				.Be(_expectedEstimateSize);
		}
	}
}