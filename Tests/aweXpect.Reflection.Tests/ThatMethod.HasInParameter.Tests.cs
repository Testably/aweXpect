using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasInParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodHasInParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasInParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasNoInParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasInParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has an in parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
				{
					await That(methodInfo).HasInParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has an in parameter,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task Type_WhenMethodHasInParameterOfType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasInParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_WhenMethodHasNoInParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasInParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with in modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Type_WhenMethodHasInParameterOfTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasInParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task TypeExactly_WhenMethodHasInParameterOfExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasInParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task TypeExactly_WhenMethodHasInParameterOfExactTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasInParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodHasInParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasInParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have an in parameter,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenMethodHasNoInParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasInParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithInParameter(in int value)
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
