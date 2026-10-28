using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class DoNotHaveParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task AtIndex_WhenOneMethodHasParameterAtIndex_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>().AtIndex(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type int at index 0,
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task AtIndex_WhenParameterOnlyExistsBeforeIndex_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>().AtIndex(1);
				}

				await That(Act).DoesNotThrow()
					.Because("the int parameter is at index 0, not at index 1");
			}

			[Test]
			public async Task ByName_WhenNoMethodHasParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter("other");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByName_WhenOneMethodHasParameter_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDerivedParameter)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter("dum").AsPrefix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter with name "dum",
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithDerivedParameter(ThatMethods.DoNotHaveParameter.Dummy dummy)
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = [];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenNoMethodHasParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<long>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenOneMethodHasParameterOfSubtype_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDerivedParameter)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type ThatMethods.DummyBase,
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithDerivedParameter(ThatMethods.DoNotHaveParameter.Dummy dummy)
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenOneMethodIsNull_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					null,
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<long>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type long,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<MethodInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenOneMethodHasParameter_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDerivedParameter)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<string>("text");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type string with name "text",
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenTypeDiffers_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>("text");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Type_ByType_WhenOneMethodHasParameter_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDerivedParameter)),
				];
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoNotHaveParameter(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type int,
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task Type_ByTypeAndName_WhenOneMethodHasParameter_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDerivedParameter)),
				];
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoNotHaveParameter(unexpectedType, "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type int with name "value",
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task WithDefaultValue_WhenNoMethodHasParameterWithDefaultValue_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>().WithDefaultValue();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDefaultValue_WhenOneMethodHasParameterWithDefaultValue_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter)),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>().WithDefaultValue();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type int,
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithOptionalParameter(int value = 42)
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoMethodHasParameter_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveParameter<long>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             also contain a method with parameter of type long,
					             but it only contained methods without a matching parameter

					             Collection:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneMethodHasParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDerivedParameter)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveParameter<int>());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task ByName_WhenOneMethodHasParameter_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter with name "value",
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenNoMethodHasParameter_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<long>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenOneMethodHasParameter_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type int,
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenOneMethodHasParameter_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type int with name "value",
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task Type_ByType_WhenOneMethodHasParameter_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
				}.ToTestAsyncEnumerable<MethodInfo?>();
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).DoNotHaveParameter(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type string,
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task Type_ByTypeAndName_WhenOneMethodHasParameter_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters)),
				}.ToTestAsyncEnumerable<MethodInfo?>();
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).DoNotHaveParameter(unexpectedType, "text");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not have parameter of type string with name "text",
					             but it contained methods with a matching parameter

					             Not matching items:
					             [
					               void ThatMethods.DoNotHaveParameter.TestClass.MethodWithParameters(int value, string text)
					             ]
					             """);
			}
		}
#endif

		private abstract class TestClass
		{
			public abstract void MethodWithParameters(int value, string text);
			public abstract void MethodWithReversedParameters(string text, int value);
			public abstract void MethodWithDerivedParameter(Dummy dummy);
			public abstract void MethodWithOptionalParameter(int value = 42);
		}

		private class Dummy : DummyBase
		{
		}
	}
}
