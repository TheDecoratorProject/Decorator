using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public class ConverterContainer : IConverterContainer
	{
		public ConverterContainer() : this(new ConverterInstanceCreator())
		{
		}

		public ConverterContainer(IConverterInstanceCreator instantiator)
		{
			_instantiator = instantiator;
			_genContainer = (memberInfo) =>
			{
				return new ConverterContainerContainer(memberInfo.GetMemberFrom(), this);
			};
		}

		private readonly IConverterInstanceCreator _instantiator;

		private readonly Cache _converters = new Cache();
		private readonly Cache _compilers = new Cache();

		private Func<MemberInfo, BaseContainer> _genContainer;

		public IConverter<T> RequestConverter<T>()
			where T : new()
		{
			IConverter<T> requestNew()
			{
				var compiler = RequestCompiler<T>();

				var compiled = compiler.Compile(_genContainer);

				return _instantiator.Create<T>(compiled);
			}

			return (IConverter<T>)_converters.Request(requestNew);
		}

		public ICompiler<T> RequestCompiler<T>()
			where T : new()
			=> (ICompiler<T>)_compilers.Request(() => _instantiator.CreateCompiler<T>());
	}
}