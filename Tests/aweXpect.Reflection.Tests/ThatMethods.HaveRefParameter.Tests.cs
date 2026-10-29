using System.Collections.Generic;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

// The System.Type overloads are deliberately exercised here alongside the generic ones.
#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatMethods
{
	public sealed class HaveRefParameter
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
			public async Task WhenAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNotAllHaveRefParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have a ref parameter,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithType_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeAndName_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeAndName_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with name "value" with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeExactly_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeExactly_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeExactlyAndName_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WithType_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!);

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!);

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllHaveRefParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveRefParameter());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             not all have a ref parameter,
					             but all 2 did

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.AnotherMethodWithRefParameter(ref string text)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveRefParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ModifierMessageTests
		{
			[Test]
			public async Task Name_WhenNotAllHaveRefParameter_ShouldFailWithModifierDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter with name "value" with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task SystemType_WhenNotAllHaveRefParameter_ShouldFailWithModifierDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task SystemTypeAndName_WhenNotAllHaveRefParameter_ShouldFailWithModifierDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with name "value" with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task SystemTypeExactly_WhenNotAllHaveRefParameter_ShouldFailWithModifierDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task SystemTypeExactlyAndName_WhenNotAllHaveRefParameter_ShouldFailWithModifierDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with name "value" with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task Type_WhenNotAllHaveRefParameter_ShouldFailWithModifierDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task TypeAndName_WhenNotAllHaveRefParameter_ShouldFailWithModifierDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with name "value" with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task TypeExactly_WhenNotAllHaveRefParameter_ShouldFailWithModifierDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task TypeExactlyAndName_WhenNotAllHaveRefParameter_ShouldFailWithModifierDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type int with name "value" with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllHaveRefParameter_NegatedShouldListMatchingMethod()
			{
				// Kills the Formatter.Format(Matching) statement mutant in the negated result.
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveRefParameter());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             not all have a ref parameter,
					             but all 1 did

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveRefParameter_ShouldListOffendingMethod()
			{
				// Kills the Formatter.Format(NotMatching) statement mutant: the offending
				// method must appear in the result message.
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have a ref parameter,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeButNotAllParametersAreRef_ShouldStillSucceed()
			{
				// Kills the Any()->All() mutant: this method has a ref parameter and a normal one,
				// so Any(IsRefParameter) is true but All(IsRefParameter) is false.
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefAndNormalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter();
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAllHaveRefParameter_ShouldSucceed()
			{
				// Kills the async equality (== true -> != true) and boolean (true -> false) mutants.
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithRefParameter))!);

				async Task Act()
				{
					await That(methods).HaveRefParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenNotAllHaveRefParameter_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!);

				async Task Act()
				{
					await That(methods).HaveRefParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have a ref parameter,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithRefParameter(ref int value),
					               void ThatMethods.HaveRefParameter.TestClass.MethodWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WhenSomeButNotAllParametersAreRef_ShouldStillSucceed()
			{
				// Kills the async Any()->All() mutant.
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefAndNormalParameter))!);

				async Task Act()
				{
					await That(methods).HaveRefParameter();
				}

				await That(Act).DoesNotThrow();
			}
#endif
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithRefParameter(ref int value) => value = 0;

			public void AnotherMethodWithRefParameter(ref string text) => text = string.Empty;

			public void MethodWithoutModifiers(int value)
			{
			}

			public void MethodWithRefAndNormalParameter(ref int value, string text) => value = 0;
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
