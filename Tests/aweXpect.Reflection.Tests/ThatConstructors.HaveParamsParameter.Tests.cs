using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveParamsParameter
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
			public async Task ByGenericType_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter<int[]>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task ByGenericTypeAndName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter<int[]>("values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task ByGenericTypeExactly_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly<int[]>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int[] with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task ByGenericTypeExactlyAndName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly<int[]>("values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int[] with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task ByName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter("values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with params modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithParamsParameter(params int[] values),
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByTypeAndName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeExactly_WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByTypeExactly_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int[] with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeExactlyAndName_WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByTypeExactlyAndName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int[] with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(), typeof(AnotherClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have a params parameter,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithParamsParameter(params int[] values),
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeButNotAllParametersAreParams_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithMixedParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeAndName_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeAndName_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with name "values" with params modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithParamsParameter(params int[] values),
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeExactly_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithTypeExactly_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int[] with params modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithParamsParameter(params int[] values),
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task WithTypeExactlyAndName_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WithType_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithParamsParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with params modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithParamsParameter(params int[] values),
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(AnotherClassWithParamsParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenSomeButNotAllParametersAreParams_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithMixedParamsParameter).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have a params parameter,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithParamsParameter(params int[] values),
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByGenericType_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter<int[]>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByGenericTypeAndName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter<int[]>("values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter("values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByTypeAndName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int[] with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByGenericTypeExactly_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly<int[]>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int[] with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByTypeExactly_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int[] with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByGenericTypeExactlyAndName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly<int[]>("values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int[] with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_ByTypeExactlyAndName_WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(ClassWithoutModifiers).GetConstructors().Single());

				async Task Act()
				{
					await That(constructors).HaveParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type int[] with name "values" with params modifier,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithoutModifiers(int[] values)
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(), typeof(AnotherClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParamsParameter());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             not all have a params parameter,
					             but all 2 did

					             Collection:
					             [
					               ThatConstructors.HaveParamsParameter.ClassWithParamsParameter(params int[] values),
					               ThatConstructors.HaveParamsParameter.AnotherClassWithParamsParameter(params string[] texts)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(), typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParamsParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithParamsParameter
		{
			public ClassWithParamsParameter(params int[] values)
			{
			}
		}

		private class AnotherClassWithParamsParameter
		{
			public AnotherClassWithParamsParameter(params string[] texts)
			{
			}
		}

		private class ClassWithMixedParamsParameter
		{
			public ClassWithMixedParamsParameter(int first, params int[] values)
			{
			}
		}

		private class ClassWithoutModifiers
		{
			public ClassWithoutModifiers(int[] values)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
