using System;

namespace Decorator.Tests
{
	public class TestException : Exception
	{
		public TestException(string msg = "") : base($"A faulty test has been detected: {msg}")
		{
		}
	}
}