using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public class ConverterInstanceCreator : IConverterInstanceCreator
	{
		IConverter<T> IConverterInstanceCreator.Create<T>()
			=> new Converter<T>();
	}
}
