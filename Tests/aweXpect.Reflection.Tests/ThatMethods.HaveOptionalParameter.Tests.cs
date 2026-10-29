using System.Collections.Generic;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveOptionalParameter
	{
#if NET8_0_OR_GREATER
		private static async IAsyncEnumerable<MethodInfo> ToAsyncEnumerable(params MethodInfo[] items)
		{
			foreach (MethodInfo item in items)
			{
				yield return item;
			}

			await Task.CompletedTask;
		}
#endif
		public sealed class Tests
		{
			[Test]
			public async Task Generic_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task GenericExactly_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task GenericExactlyWithName_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with name "value" with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task GenericWithName_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with name "value" with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodHasOneRequiredAndOneOptionalParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRequiredAndOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have an optional parameter,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithName_WhenNotAllHaveOptionalParameterWithName_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter with name "value" with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithType_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeAndName_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeAndName_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with name "value" with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeExactly_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeExactly_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeExactlyAndName_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeExactlyAndName_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with name "value" with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithOptionalParameter))!);

				async Task Act()
				{
					await That(methods).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenMethodHasOneRequiredAndOneOptionalParameter_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRequiredAndOptionalParameter))!);

				async Task Act()
				{
					await That(methods).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!);

				async Task Act()
				{
					await That(methods).HaveOptionalParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have an optional parameter,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WithType_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!);

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!);

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveOptionalParameter());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             not all have an optional parameter,
					             but all 2 did

					             Collection:
					             [
					               void ThatMethods.HaveOptionalParameter.TestClass.MethodWithOptionalParameter(int value = 0),
					               void ThatMethods.HaveOptionalParameter.TestClass.AnotherMethodWithOptionalParameter(string text = "")
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveOptionalParameter());
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

			public void AnotherMethodWithOptionalParameter(string text = "")
			{
			}

			public void MethodWithRequiredAndOptionalParameter(int required, int value = 0)
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
