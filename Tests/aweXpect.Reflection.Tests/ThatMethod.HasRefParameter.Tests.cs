using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasRefParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task Name_WhenMethodHasNoRefParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter with name "value" with ref modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task Type_WhenMethodHasNoRefParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with ref modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task Type_WhenMethodHasRefParameterOfType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Type_WhenMethodHasRefParameterOfTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task TypeExactly_WhenMethodHasNoRefParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int with ref modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task TypeExactly_WhenMethodHasRefParameterOfExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task TypeExactly_WhenMethodHasRefParameterOfExactTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task TypeExactlyWithName_WhenMethodHasNoRefParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int with name "value" with ref modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task TypeWithName_WhenMethodHasNoRefParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with name "value" with ref modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenMethodHasNoRefParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has a ref parameter,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenMethodHasRefParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has a ref parameter,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSomeButNotAllParametersAreRefParameters_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMixedParameters))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodHasNoRefParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasRefParameter());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodHasRefParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasRefParameter());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have a ref parameter,
					             but it did
					             """);
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithRefParameter(ref int value) => value = 0;

			public void MethodWithMixedParameters(ref int value, string other) => value = 0;

			public void MethodWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
