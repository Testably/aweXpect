using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed partial class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAConstructorHasAttribute_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenAConstructorHasMatchingAttribute_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenNoConstructorHasMatchingAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes),
					typeof(TestClass).GetConstructor([typeof(string),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task Negated_WhenNoConstructorHasAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but none of 1 did

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass()
					             ]
					             """);
			}

			[Test]
			public async Task WhenAConstructorHasAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),]),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor(Type.EmptyTypes), null,];

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(),
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatConstructors.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(),
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoConstructorHasAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenAConstructorHasMatchingAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes),
					typeof(TestClass).GetConstructor([typeof(string),]),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(),
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenNoConstructorHasMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes),
					typeof(TestClass).GetConstructor([typeof(string),]),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow()
					.Because("neither a missing attribute nor one that does not match the predicate counts");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),]),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatConstructors.DoNotHave.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom

					             Collection:
					             [
					               ThatConstructors.DoNotHave.AttributeTests.TestClass(string value)
					             ]
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass()
				{
				}

				[Foo(Value = 2)]
				// ReSharper disable once UnusedParameter.Local
				public TestClass(string value)
				{
				}
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
