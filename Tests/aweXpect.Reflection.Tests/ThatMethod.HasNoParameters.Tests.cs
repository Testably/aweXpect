using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasNoParameters
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodHasNoParameters_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!;

				async Task Act()
					=> await That(methodInfo).HasNoParameters();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasParameters_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasNoParameters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has no parameters,
					             but it had 2 parameters
					             """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
					=> await That(methodInfo).HasNoParameters();

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
			public async Task WhenMethodHasParameters_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasNoParameters());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasNoParameters_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasNoParameters());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have no parameters,
					             but it did
					             """);
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithoutParameters() { }
			public void MethodWithIntAndString(int value, string name) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
