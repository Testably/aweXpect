using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasParameterCount
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodHasDifferentCount_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterCount(3);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has 3 parameters,
					             but it had 2 parameters
					             """);
			}

			[Fact]
			public async Task WhenMethodHasExpectedCount_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterCount(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasSingleParameter_ShouldDescribeWithSingular()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!;

				async Task Act()
				{
					await That(methodInfo).HasParameterCount(2);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has 2 parameters,
					             but it had one parameter
					             """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
				{
					await That(methodInfo).HasParameterCount(0);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has no parameters,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodHasDifferentCount_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParameterCount(3));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasExpectedCount_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParameterCount(2));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have 2 parameters,
					             but it did
					             """);
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithInt(int value) { }
			public void MethodWithIntAndString(int value, string name) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
