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
	public class Program
	{
		public class TestClass
		{
			[Position(0), Optional]
			public string MyReferenceType { get; set; }

			[Position(1), Optional]
			public int MyValueType { get; set; }
		}


		static void Main(string[] args)
		{
			var c = new Compiler<TestClass>();

			int i2 = 0;
			TestClass val = new TestClass();

			Console.WriteLine(SomeClass.Test(new object[] { null, 0 }, ref i2, ref val));

			Console.ReadLine();
			// Compiler<TestClass>.SaveWrap(new Compiler<TestClass>().Compile((i) => new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i))));
		}
	}

	public static class SomeClass
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002060 File Offset: 0x00000260
		public static bool Test(object[] array, ref int ptr, ref Program.TestClass ptr2)
		{
			ptr2 = new Program.TestClass();
			if (array.Length <= ptr)
			{
				return false;
			}
			object obj = array[ptr];
			string myReferenceType;
			if ((myReferenceType = (obj as string)) != null)
			{
				ptr2.MyReferenceType = myReferenceType;
			}
			if (obj == null)
			{
				ptr2.MyReferenceType = null;
			}
			ptr++;
			if (array.Length <= ptr)
			{
				return false;
			}
			object obj2 = array[ptr];
			if (obj2 is int)
			{
				int num = (int)obj2;
				ptr2.MyValueType = (int)num;
			}
			ptr++;
			return true;
		}
	}

}
