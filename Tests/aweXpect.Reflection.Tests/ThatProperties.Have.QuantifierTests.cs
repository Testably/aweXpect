using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed partial class Have
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrHave_ShouldCountItemsWithEitherAttribute()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)), typeof(TestClass).GetProperty(nameof(TestClass.WithBar)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>().OrHave<BarAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)), typeof(TestClass).GetProperty(nameof(TestClass.WithoutFoo)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatProperties.Have.QuantifierTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               public int ThatProperties.Have.QuantifierTests.TestClass.WithoutFoo { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.Have.QuantifierTests.TestClass.WithFoo1 { get; set; },
					               public int ThatProperties.Have.QuantifierTests.TestClass.WithoutFoo { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithPredicate_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)), typeof(TestClass).GetProperty(nameof(TestClass.WithFoo2)),];

				async Task Act()
				{
					await That(subject).Exactly(1).Have<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemHasAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).None().Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatProperties.Have.QuantifierTests.FooAttribute for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               public int ThatProperties.Have.QuantifierTests.TestClass.WithFoo1 { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.Have.QuantifierTests.TestClass.WithoutFoo { get; set; },
					               public int ThatProperties.Have.QuantifierTests.TestClass.WithFoo1 { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenAttributeTypeIsNoAttribute_ShouldThrowArgumentException()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)),];

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
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)), typeof(TestClass).GetProperty(nameof(TestClass.WithFoo2)),];

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
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)), typeof(TestClass).GetProperty(nameof(TestClass.WithoutFoo)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatProperties.Have.QuantifierTests.FooAttribute,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               public int ThatProperties.Have.QuantifierTests.TestClass.WithoutFoo { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.Have.QuantifierTests.TestClass.WithFoo1 { get; set; },
					               public int ThatProperties.Have.QuantifierTests.TestClass.WithoutFoo { get; set; }
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
				public int WithFoo1 { get; set; }

				[Foo(Value = 2)]
				public int WithFoo2 { get; set; }

				[Bar]
				public int WithBar { get; set; }

				public int WithoutFoo { get; set; }
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				ImmutableArray<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)), typeof(TestClass).GetProperty(nameof(TestClass.WithoutFoo)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have(typeof(FooAttribute));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatProperties.Have.StructCollectionTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               public int ThatProperties.Have.StructCollectionTests.TestClass.WithoutFoo { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.Have.StructCollectionTests.TestClass.WithFoo1 { get; set; },
					               public int ThatProperties.Have.StructCollectionTests.TestClass.WithoutFoo { get; set; }
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
				public int WithFoo1 { get; set; }

				[Foo(Value = 2)]
				public int WithFoo2 { get; set; }

				[Bar]
				public int WithBar { get; set; }

				public int WithoutFoo { get; set; }
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)), typeof(TestClass).GetProperty(nameof(TestClass.WithoutFoo)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatProperties.Have.AsyncEnumerableTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               public int ThatProperties.Have.AsyncEnumerableTests.TestClass.WithoutFoo { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.Have.AsyncEnumerableTests.TestClass.WithFoo1 { get; set; },
					               public int ThatProperties.Have.AsyncEnumerableTests.TestClass.WithoutFoo { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenTheItemsMatch_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.WithFoo1)), typeof(TestClass).GetProperty(nameof(TestClass.WithFoo2)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

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
				public int WithFoo1 { get; set; }

				[Foo(Value = 2)]
				public int WithFoo2 { get; set; }

				[Bar]
				public int WithBar { get; set; }

				public int WithoutFoo { get; set; }
			}
		}
#endif
	}
}
