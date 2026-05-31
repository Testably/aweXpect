using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasOutParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodHasOutParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOutParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasNoOutParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOutParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has an out parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
				{
					await That(methodInfo).HasOutParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has an out parameter,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task Type_WhenMethodHasOutParameterOfType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOutParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_WhenMethodHasNoOutParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOutParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with out modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Type_WhenMethodHasOutParameterOfTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOutParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task TypeExactly_WhenMethodHasOutParameterOfExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOutParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task TypeExactly_WhenMethodHasOutParameterOfExactTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOutParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodHasOutParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasOutParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have an out parameter,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenMethodHasNoOutParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasOutParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithOutParameter(out int value)
			{
				value = 0;
			}

			public void MethodWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
