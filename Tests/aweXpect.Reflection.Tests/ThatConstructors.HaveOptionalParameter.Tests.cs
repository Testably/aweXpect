using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveOptionalParameter
	{
#if NET8_0_OR_GREATER
		private static async IAsyncEnumerable<ConstructorInfo> ToAsyncEnumerable(params ConstructorInfo[] items)
		{
			foreach (ConstructorInfo item in items)
			{
				yield return item;
			}

			await Task.CompletedTask;
		}
#endif
		public sealed class Tests
		{
			[Test]
			public async Task ByGenericType_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task ByGenericTypeAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task ByGenericTypeExactly_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task ByGenericTypeExactlyAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task ByName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithOptionalParameter(int value = 0),
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByTypeAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeExactly_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByTypeExactly_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeExactlyAndName_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByTypeExactlyAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(), typeof(AnotherClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have an optional parameter,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithOptionalParameter(int value = 0),
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeButNotAllParametersAreOptional_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithMixedOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeAndName_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeAndName_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value" with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithOptionalParameter(int value = 0),
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeExactly_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeExactly_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithOptionalParameter(int value = 0),
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeExactlyAndName_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WithType_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOptionalParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with optional modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithOptionalParameter(int value = 0),
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(AnotherClassWithOptionalParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenSomeButNotAllParametersAreOptional_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithMixedOptionalParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithOptionalParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have an optional parameter,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithOptionalParameter(int value = 0),
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByGenericType_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByGenericTypeAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByTypeAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByGenericTypeExactly_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByTypeExactly_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByGenericTypeExactlyAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByTypeExactlyAndName_WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int with name "value" with optional modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithoutModifiers(int value)
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
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(), typeof(AnotherClassWithOptionalParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveOptionalParameter());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             not all have an optional parameter,
					             but all 2 did

					             Collection:
					             [
					               ThatConstructors.HaveOptionalParameter.ClassWithOptionalParameter(int value = 0),
					               ThatConstructors.HaveOptionalParameter.AnotherClassWithOptionalParameter(string text = "")
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithOptionalParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveOptionalParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithOptionalParameter
		{
			public ClassWithOptionalParameter(int value = 0)
			{
			}
		}

		private class AnotherClassWithOptionalParameter
		{
			public AnotherClassWithOptionalParameter(string text = "")
			{
			}
		}

		private class ClassWithMixedOptionalParameter
		{
			public ClassWithMixedOptionalParameter(int required, int value = 0)
			{
			}
		}

		private class ClassWithoutModifiers
		{
			public ClassWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
