using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveParameterCount
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveExpectedCount_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStringAndInt))!,
				};

				async Task Act()
					=> await That(methods).HaveParameterCount(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveExpectedCount_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
					=> await That(methods).HaveParameterCount(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have 2 parameters,
					             but it contained methods with a different number of parameters *
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveExpectedCount_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStringAndInt))!,
				};

				async Task Act()
					=> await That(methods).DoesNotComplyWith(they => they.HaveParameterCount(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have 2 parameters,
					             but it only contained methods with 2 parameters *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveExpectedCount_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
					=> await That(methods).DoesNotComplyWith(they => they.HaveParameterCount(2));

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithInt(int value) { }
			public void MethodWithIntAndString(int value, string name) { }
			public void MethodWithStringAndInt(string name, int value) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
