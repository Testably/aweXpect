using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasOptionalParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodHasOptionalParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has an optional parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has an optional parameter,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task Type_WhenMethodHasOptionalParameterOfType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with optional modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Type_WhenMethodHasOptionalParameterOfTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task TypeExactly_WhenMethodHasOptionalParameterOfExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task TypeExactly_WhenMethodHasOptionalParameterOfExactTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodHasOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasOptionalParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have an optional parameter,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenMethodHasNoOptionalParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasOptionalParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithOptionalParameter(int value = 0)
			{
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
