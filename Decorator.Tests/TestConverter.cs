using Decorator.Compiler;
using Decorator.Converter;
using Decorator.ModuleAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Decorator.Tests
{
	internal static class StaticProvider
	{
		public static object Lock = new object();
		public static bool UseIL { get => Instantiator.UseIL; set => Instantiator.UseIL = value; }

		public static ChooseILInstantiator Instantiator = new ChooseILInstantiator();

		public static IConverterContainer Container = new TestContainer(Instantiator);
	}

	public class TestContainer : IConverterContainer
	{
		public TestContainer(IConverterInstanceCreator instantiator)
		{
			_instantiator = instantiator;
			_genContainer = (memberInfo) =>
			{
				return new ConverterContainerContainer(memberInfo.GetMemberFrom(), this);
			};
		}

		private Func<MemberInfo, BaseContainer> _genContainer;
		private IConverterInstanceCreator _instantiator;

		public ICompiler<T> RequestCompiler<T>() where T : new() => _instantiator.CreateCompiler<T>();

		public IConverter<T> RequestConverter<T>()
			where T : new()
		{
			var compiler = RequestCompiler<T>();

			var compiled = compiler.Compile(_genContainer);

			ILSerialize<T> ilSer = null;
			ILDeserialize<T> ilDes = null;

			if (compiler.SupportsIL(compiled))
			{
				ilSer = compiler.CompileILSerialize(compiled);
				ilDes = compiler.CompileILDeserialize(compiled);
			}

			return _instantiator.Create<T>(compiled, ilSer, ilDes);
		}
	}

	public class ChooseILInstantiator : IConverterInstanceCreator
	{
		public bool UseIL = false;

		IConverter<T> IConverterInstanceCreator.Create<T>(BaseModule[] members, ILSerialize<T> ilSer, ILDeserialize<T> ilDes)
		{
			if (!UseIL || ilSer == null || ilDes == null)
			{
				return new Converter<T>(members);
			}
			else
			{
				return new ILConverter<T>(members, ilSer, ilDes);
			}
		}

		ICompiler<T> IConverterInstanceCreator.CreateCompiler<T>()
			=> new Compiler<T>();
	}

	public static class TestConverter<T>
		where T : new()
	{
		private static readonly IConverter<T> _converter;
		private static readonly IConverter<T> _ilconverter;

		static TestConverter()
		{
			lock (StaticProvider.Lock)
			{
				StaticProvider.UseIL = false;
				_converter = StaticProvider.Container.RequestConverter<T>();

				StaticProvider.UseIL = true;
				_ilconverter = StaticProvider.Container.RequestConverter<T>();

				Members = _converter.Members;
				ILMembers = _ilconverter.Members;
			}
		}

		public static ReadOnlyCollection<BaseModule> Members { get; }
		public static ReadOnlyCollection<BaseModule> ILMembers { get; }

		public static bool TryDeserialize(bool ilConverter, object[] array, out T result)
			=> ilConverter ?
				_ilconverter.TryDeserialize(array, out result)
				: _converter.TryDeserialize(array, out result);

		public static bool TryDeserialize(bool ilConverter, object[] array, ref int arrayIndex, out T result)
			=> ilConverter ?
				_ilconverter.TryDeserialize(array, ref arrayIndex, out result)
				: _converter.TryDeserialize(array, ref arrayIndex, out result);

		public static object[] Serialize(bool ilConverter, T item)
			=> ilConverter ?
				_ilconverter.Serialize(item)
				: _converter.Serialize(item);

		/*
		public static bool TryDeserialize(object[] array, out T result)
			=> _ilconverter.TryDeserialize(array, out result);

		public static bool TryDeserialize(object[] array, ref int arrayIndex, out T result)
			=> _ilconverter.TryDeserialize(array, ref arrayIndex, out result);

		public static object[] Serialize(T item)
			=> _ilconverter.Serialize(item);
		*/
	}
}
