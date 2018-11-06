using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public interface IConverterContainer
	{
		IConverter<T> Request<T>()
			where T : IDecorable, new();
	}
}
