using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasOptionalParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task Generic_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with optional modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task GenericExactly_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int with optional modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task GenericExactlyWithName_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int with name "value" with optional modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task GenericWithName_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with name "value" with optional modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task Name_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter with name "value" with optional modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task Type_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with optional modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task Type_WhenMethodHasOptionalParameterOfType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Type_WhenMethodHasOptionalParameterOfTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task TypeExactly_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int with optional modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task TypeExactly_WhenMethodHasOptionalParameterOfExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task TypeExactly_WhenMethodHasOptionalParameterOfExactTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task TypeExactlyWithName_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int with name "value" with optional modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task TypeWithName_WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with name "value" with optional modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenMethodHasNoOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has an optional parameter,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenMethodHasOptionalParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has an optional parameter,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSomeButNotAllParametersAreOptionalParameters_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMixedParameters))!;

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodHasNoOptionalParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasOptionalParameter());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodHasOptionalParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasOptionalParameter());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have an optional parameter,
					             but it did
					             """);
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

			public void MethodWithMixedParameters(int required, int value = 0)
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
