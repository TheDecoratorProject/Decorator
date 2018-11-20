using Decorator.Attributes;
using Decorator.Compiler;
using Decorator.ModuleAPI;
using Decorator.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestIL
{
	class Program
	{
		public class TestClass
		{
			[Position(0), Required]
			public string MyProperty { get; set; }
		}

		static void Main(string[] args)
		{
			var c = new Compiler<TestClass>();

			Compiler<TestClass>.SaveWrap((i) => new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i)));
		}
	}
}
