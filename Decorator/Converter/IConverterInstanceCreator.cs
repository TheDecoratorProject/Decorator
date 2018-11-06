using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public interface IConverterInstanceCreator
	{
		IConverter<T> Create<T>()
			where T : IDecorable, new();
	}
}
