using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class DoNotHaveParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task AtIndex_WhenOneConstructorHasParameterAtIndex_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(string), typeof(int),]),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>().AtIndex(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type int at index 0,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text),
					               ThatConstructors.DoNotHaveParameter.TestClass(string text, int value)
					             ]
					             """);
			}

			[Test]
			public async Task ByName_WhenNoConstructorHasParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(string), typeof(int),]),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter("other");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByName_WhenOneConstructorHasParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(Dummy),]),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter("dum").AsPrefix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter with name "dum",
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(ThatConstructors.DoNotHaveParameter.Dummy dummy)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text),
					               ThatConstructors.DoNotHaveParameter.TestClass(ThatConstructors.DoNotHaveParameter.Dummy dummy)
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject = [];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenNoConstructorHasParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(string), typeof(int),]),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<long>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenOneConstructorHasParameterOfSubtype_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(Dummy),]),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type ThatConstructors.DoNotHaveParameter.DummyBase,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(ThatConstructors.DoNotHaveParameter.Dummy dummy)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text),
					               ThatConstructors.DoNotHaveParameter.TestClass(ThatConstructors.DoNotHaveParameter.Dummy dummy)
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenOneConstructorIsNull_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					null,
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<long>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type long,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<ConstructorInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenOneConstructorHasParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(Dummy),]),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<string>("text");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type string with name "text",
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text),
					               ThatConstructors.DoNotHaveParameter.TestClass(ThatConstructors.DoNotHaveParameter.Dummy dummy)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenTypeDiffers_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(string), typeof(int),]),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>("text");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Type_ByType_WhenOneConstructorHasParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(Dummy),]),
				];
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoNotHaveParameter(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type int,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text),
					               ThatConstructors.DoNotHaveParameter.TestClass(ThatConstructors.DoNotHaveParameter.Dummy dummy)
					             ]
					             """);
			}

			[Test]
			public async Task Type_ByTypeAndName_WhenOneConstructorHasParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(Dummy),]),
				];
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoNotHaveParameter(unexpectedType, "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type int with name "value",
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text),
					               ThatConstructors.DoNotHaveParameter.TestClass(ThatConstructors.DoNotHaveParameter.Dummy dummy)
					             ]
					             """);
			}

			[Test]
			public async Task WithDefaultValue_WhenNoConstructorHasParameterWithDefaultValue_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(string), typeof(int),]),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>().WithDefaultValue();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDefaultValue_WhenOneConstructorHasParameterWithDefaultValue_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(bool), typeof(int),]),
				];

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>().WithDefaultValue();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type int,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(bool flag, int value = 42)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text),
					               ThatConstructors.DoNotHaveParameter.TestClass(bool flag, int value = 42)
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoConstructorHasParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveParameter<long>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has parameter of type long,
					             but none of 1 did

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneConstructorHasParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject =
				[
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
					typeof(TestClass).GetConstructor([typeof(Dummy),]),
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
			public async Task ByName_WhenOneConstructorHasParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter with name "value",
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task ByType_WhenNoConstructorHasParameter_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<long>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenOneConstructorHasParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type int,
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenOneConstructorHasParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type int with name "value",
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task Type_ByType_WhenOneConstructorHasParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).DoNotHaveParameter(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type string,
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]
					             """);
			}

			[Test]
			public async Task Type_ByTypeAndName_WhenOneConstructorHasParameter_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).DoNotHaveParameter(unexpectedType, "text");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type string with name "text",
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHaveParameter.TestClass(int value, string text)
					             ]
					             """);
			}
		}
#endif

		private class TestClass
		{
			public TestClass(int value, string text) { }
			public TestClass(string text, int value) { }
			public TestClass(Dummy dummy) { }
			public TestClass(bool flag, int value = 42) { }
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
