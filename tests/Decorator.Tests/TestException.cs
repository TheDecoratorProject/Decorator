using System;

namespace Decorator.Tests
{
	public class TestException : Exception
	{
		public TestException() : base("")
		{
		}

		public TestException(string msg) : base($"A faulty test has been detected: {msg}")
		{
		}
	}
}