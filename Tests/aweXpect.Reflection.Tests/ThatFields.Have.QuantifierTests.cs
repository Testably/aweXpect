using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed partial class Have
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrHave_ShouldCountItemsWithEitherAttribute()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.WithFoo1)), typeof(TestClass).GetField(nameof(TestClass.WithBar)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>().OrHave<BarAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.WithFoo1)), typeof(TestClass).GetField(nameof(TestClass.WithoutFoo)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatFields.Have.QuantifierTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               int ThatFields.Have.QuantifierTests.TestClass.WithoutFoo
					             ]

					             Collection:
					             [
					               int ThatFields.Have.QuantifierTests.TestClass.WithFoo1,
					               int ThatFields.Have.QuantifierTests.TestClass.WithoutFoo
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithPredicate_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.WithFoo1)), typeof(TestClass).GetField(nameof(TestClass.WithFoo2)),];

				async Task Act()
				{
					await That(subject).Exactly(1).Have<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemHasAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetField(nameof(TestClass.WithFoo1)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).None().Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatFields.Have.QuantifierTests.FooAttribute for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               int ThatFields.Have.QuantifierTests.TestClass.WithFoo1
					             ]

					             Collection:
					             [
					               int ThatFields.Have.QuantifierTests.TestClass.WithoutFoo,
					               int ThatFields.Have.QuantifierTests.TestClass.WithFoo1
					             ]
					             """);
			}

			[Test]
			public async Task WhenAttributeTypeIsNoAttribute_ShouldThrowArgumentException()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.WithFoo1)),];

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
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.WithFoo1)), typeof(TestClass).GetField(nameof(TestClass.WithFoo2)),];

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
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.WithFoo1)), typeof(TestClass).GetField(nameof(TestClass.WithoutFoo)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatFields.Have.QuantifierTests.FooAttribute,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               int ThatFields.Have.QuantifierTests.TestClass.WithoutFoo
					             ]

					             Collection:
					             [
					               int ThatFields.Have.QuantifierTests.TestClass.WithFoo1,
					               int ThatFields.Have.QuantifierTests.TestClass.WithoutFoo
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

#pragma warning disable CS0649
			private class TestClass
			{
				[Foo(Value = 1)]
				public int WithFoo1;

				[Foo(Value = 2)]
				public int WithFoo2;

				[Bar]
				public int WithBar;

				public int WithoutFoo;
			}
#pragma warning restore CS0649
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				ImmutableArray<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.WithFoo1)), typeof(TestClass).GetField(nameof(TestClass.WithoutFoo)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have(typeof(FooAttribute));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatFields.Have.StructCollectionTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               int ThatFields.Have.StructCollectionTests.TestClass.WithoutFoo
					             ]

					             Collection:
					             [
					               int ThatFields.Have.StructCollectionTests.TestClass.WithFoo1,
					               int ThatFields.Have.StructCollectionTests.TestClass.WithoutFoo
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

#pragma warning disable CS0649
			private class TestClass
			{
				[Foo(Value = 1)]
				public int WithFoo1;

				[Foo(Value = 2)]
				public int WithFoo2;

				[Bar]
				public int WithBar;

				public int WithoutFoo;
			}
#pragma warning restore CS0649
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.WithFoo1)), typeof(TestClass).GetField(nameof(TestClass.WithoutFoo)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatFields.Have.AsyncEnumerableTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               int ThatFields.Have.AsyncEnumerableTests.TestClass.WithoutFoo
					             ]

					             Collection:
					             [
					               int ThatFields.Have.AsyncEnumerableTests.TestClass.WithFoo1,
					               int ThatFields.Have.AsyncEnumerableTests.TestClass.WithoutFoo
					             ]
					             """);
			}

			[Test]
			public async Task WhenTheItemsMatch_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.WithFoo1)), typeof(TestClass).GetField(nameof(TestClass.WithFoo2)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

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

#pragma warning disable CS0649
			private class TestClass
			{
				[Foo(Value = 1)]
				public int WithFoo1;

				[Foo(Value = 2)]
				public int WithFoo2;

				[Bar]
				public int WithBar;

				public int WithoutFoo;
			}
#pragma warning restore CS0649
		}
#endif
	}
}
