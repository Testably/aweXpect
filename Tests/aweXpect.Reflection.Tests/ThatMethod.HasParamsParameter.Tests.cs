using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasParamsParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodHasParamsParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has a params parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has a params parameter,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodHasParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParamsParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have a params parameter,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenMethodHasNoParamsParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParamsParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithParamsParameter(params int[] values)
			{
			}

			public void MethodWithoutModifiers(int[] values)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
