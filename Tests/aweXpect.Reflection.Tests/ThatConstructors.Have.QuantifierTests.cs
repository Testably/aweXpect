using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed partial class Have
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrHave_ShouldCountItemsWithEitherAttribute()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor(Type.EmptyTypes), typeof(TestClass).GetConstructor([typeof(string),]),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>().OrHave<BarAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor(Type.EmptyTypes), typeof(TestClass).GetConstructor([typeof(long),]),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructors.Have.QuantifierTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.Have.QuantifierTests.TestClass(long value)
					             ]

					             Collection:
					             [
					               ThatConstructors.Have.QuantifierTests.TestClass(),
					               ThatConstructors.Have.QuantifierTests.TestClass(long value)
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithPredicate_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor(Type.EmptyTypes), typeof(TestClass).GetConstructor([typeof(int),]),];

				async Task Act()
				{
					await That(subject).Exactly(1).Have<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemHasAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor([typeof(long),]), typeof(TestClass).GetConstructor(Type.EmptyTypes),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).None().Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructors.Have.QuantifierTests.FooAttribute for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatConstructors.Have.QuantifierTests.TestClass()
					             ]

					             Collection:
					             [
					               ThatConstructors.Have.QuantifierTests.TestClass(long value),
					               ThatConstructors.Have.QuantifierTests.TestClass()
					             ]
					             """);
			}

			[Test]
			public async Task WhenAttributeTypeIsNoAttribute_ShouldThrowArgumentException()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor(Type.EmptyTypes),];

				Type attributeType = typeof(string);

				async Task Act()
				{
					await That(subject).Have(attributeType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The type must be an attribute, but it was string.");
			}

			[Test]
			public async Task WithType_WhenAllItemsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor(Type.EmptyTypes), typeof(TestClass).GetConstructor([typeof(int),]),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenAnItemDoesNotHaveAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor(Type.EmptyTypes), typeof(TestClass).GetConstructor([typeof(long),]),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatConstructors.Have.QuantifierTests.FooAttribute,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.Have.QuantifierTests.TestClass(long value)
					             ]

					             Collection:
					             [
					               ThatConstructors.Have.QuantifierTests.TestClass(),
					               ThatConstructors.Have.QuantifierTests.TestClass(long value)
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class BarAttribute : Attribute
			{
			}

			private class TestClass
			{
				[Foo(Value = 1)]
				public TestClass() { }

				[Foo(Value = 2)]
				public TestClass(int value) { }

				[Bar]
				public TestClass(string value) { }

				public TestClass(long value) { }
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				ImmutableArray<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor(Type.EmptyTypes), typeof(TestClass).GetConstructor([typeof(long),]),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have(typeof(FooAttribute));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructors.Have.StructCollectionTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.Have.StructCollectionTests.TestClass(long value)
					             ]

					             Collection:
					             [
					               ThatConstructors.Have.StructCollectionTests.TestClass(),
					               ThatConstructors.Have.StructCollectionTests.TestClass(long value)
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class BarAttribute : Attribute
			{
			}

			private class TestClass
			{
				[Foo(Value = 1)]
				public TestClass() { }

				[Foo(Value = 2)]
				public TestClass(int value) { }

				[Bar]
				public TestClass(string value) { }

				public TestClass(long value) { }
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes), typeof(TestClass).GetConstructor([typeof(long),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructors.Have.AsyncEnumerableTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.Have.AsyncEnumerableTests.TestClass(long value)
					             ]

					             Collection:
					             [
					               ThatConstructors.Have.AsyncEnumerableTests.TestClass(),
					               ThatConstructors.Have.AsyncEnumerableTests.TestClass(long value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenTheItemsMatch_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
				{
					typeof(TestClass).GetConstructor(Type.EmptyTypes), typeof(TestClass).GetConstructor([typeof(int),]),
				}.ToTestAsyncEnumerable<ConstructorInfo?>();

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
					await That(subject).Exactly(1).Have<FooAttribute>(foo => foo.Value == 2);
					await That(subject).AtLeast(2).Have(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.All)]
			private sealed class BarAttribute : Attribute
			{
			}

			private class TestClass
			{
				[Foo(Value = 1)]
				public TestClass() { }

				[Foo(Value = 2)]
				public TestClass(int value) { }

				[Bar]
				public TestClass(string value) { }

				public TestClass(long value) { }
			}
		}
#endif
	}
}
