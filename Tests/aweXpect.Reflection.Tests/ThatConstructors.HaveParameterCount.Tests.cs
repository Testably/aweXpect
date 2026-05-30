using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveParameterCount
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveExpectedCount_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(string), typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).HaveParameterCount(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveExpectedCount_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).HaveParameterCount(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have 2 parameters,
					             but it contained constructors with a different number of parameters *
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveExpectedCount_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(string), typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).DoesNotComplyWith(they => they.HaveParameterCount(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have 2 parameters,
					             but it only contained constructors with 2 parameters *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveExpectedCount_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).DoesNotComplyWith(they => they.HaveParameterCount(2));

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public TestClass(int value) { }
			public TestClass(int value, string name) { }
			public TestClass(string name, int value) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
