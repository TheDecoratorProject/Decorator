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
			public string MyReferenceType { get; set; }

			[Position(1), Required]
			public int MyValueType { get; set; }
		}


		static void Main(string[] args)
		{
			var c = new Compiler<TestClass>();
			
			Console.ReadLine();
			// Compiler<TestClass>.SaveWrap(new Compiler<TestClass>().Compile((i) => new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i))));
		}
	}
}
