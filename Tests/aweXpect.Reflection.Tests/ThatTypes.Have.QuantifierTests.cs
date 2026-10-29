using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class Have
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_Directly_ShouldOnlyCountItemsWithADirectAttribute()
			{
				IEnumerable<Type?> subject = [typeof(WithFoo1), typeof(DerivedFromWithFoo1),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has direct ThatTypes.Have.QuantifierTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.Have.QuantifierTests.DerivedFromWithFoo1
					             ]

					             Collection:
					             [
					               ThatTypes.Have.QuantifierTests.WithFoo1,
					               ThatTypes.Have.QuantifierTests.DerivedFromWithFoo1
					             ]
					             """);
			}

			[Test]
			public async Task AtLeast_OrHave_ShouldCountItemsWithEitherAttribute()
			{
				IEnumerable<Type?> subject = [typeof(WithFoo1), typeof(WithBar),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>().OrHave<BarAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(WithFoo1), typeof(WithoutFoo),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatTypes.Have.QuantifierTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.Have.QuantifierTests.WithoutFoo
					             ]

					             Collection:
					             [
					               ThatTypes.Have.QuantifierTests.WithFoo1,
					               ThatTypes.Have.QuantifierTests.WithoutFoo
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithPredicate_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(WithFoo1), typeof(WithFoo2),];

				async Task Act()
				{
					await That(subject).Exactly(1).Have<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemHasAttribute_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(WithoutFoo), typeof(WithFoo1),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).None().Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatTypes.Have.QuantifierTests.FooAttribute for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatTypes.Have.QuantifierTests.WithFoo1
					             ]

					             Collection:
					             [
					               ThatTypes.Have.QuantifierTests.WithoutFoo,
					               ThatTypes.Have.QuantifierTests.WithFoo1
					             ]
					             """);
			}

			[Test]
			public async Task WhenAttributeTypeIsNoAttribute_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = [typeof(WithFoo1),];

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
				IEnumerable<Type?> subject = [typeof(WithFoo1), typeof(WithFoo2),];

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
				IEnumerable<Type?> subject = [typeof(WithFoo1), typeof(WithoutFoo),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatTypes.Have.QuantifierTests.FooAttribute,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.Have.QuantifierTests.WithoutFoo
					             ]

					             Collection:
					             [
					               ThatTypes.Have.QuantifierTests.WithFoo1,
					               ThatTypes.Have.QuantifierTests.WithoutFoo
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

			[Foo(Value = 1)]
			private class WithFoo1
			{
			}

			[Foo(Value = 2)]
			private class WithFoo2
			{
			}

			[Bar]
			private class WithBar
			{
			}

			private class WithoutFoo
			{
			}

			private class DerivedFromWithFoo1 : WithFoo1
			{
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(WithFoo1), typeof(WithoutFoo),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have(typeof(FooAttribute));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatTypes.Have.StructCollectionTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.Have.StructCollectionTests.WithoutFoo
					             ]

					             Collection:
					             [
					               ThatTypes.Have.StructCollectionTests.WithFoo1,
					               ThatTypes.Have.StructCollectionTests.WithoutFoo
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

			[Foo(Value = 1)]
			private class WithFoo1
			{
			}

			[Foo(Value = 2)]
			private class WithFoo2
			{
			}

			[Bar]
			private class WithBar
			{
			}

			private class WithoutFoo
			{
			}

			private class DerivedFromWithFoo1 : WithFoo1
			{
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(WithFoo1), typeof(WithoutFoo),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatTypes.Have.AsyncEnumerableTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.Have.AsyncEnumerableTests.WithoutFoo
					             ]

					             Collection:
					             [
					               ThatTypes.Have.AsyncEnumerableTests.WithFoo1,
					               ThatTypes.Have.AsyncEnumerableTests.WithoutFoo
					             ]
					             """);
			}

			[Test]
			public async Task WhenTheItemsMatch_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(WithFoo1), typeof(WithFoo2),
				}.ToTestAsyncEnumerable<Type?>();

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

			[Foo(Value = 1)]
			private class WithFoo1
			{
			}

			[Foo(Value = 2)]
			private class WithFoo2
			{
			}

			[Bar]
			private class WithBar
			{
			}

			private class WithoutFoo
			{
			}

			private class DerivedFromWithFoo1 : WithFoo1
			{
			}
		}
#endif
	}
}
