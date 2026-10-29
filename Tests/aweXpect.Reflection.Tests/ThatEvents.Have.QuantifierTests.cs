using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed partial class Have
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrHave_ShouldCountItemsWithEitherAttribute()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)), typeof(TestClass).GetEvent(nameof(TestClass.WithBar)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>().OrHave<BarAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)), typeof(TestClass).GetEvent(nameof(TestClass.WithoutFoo)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatEvents.Have.QuantifierTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               event EventHandler ThatEvents.Have.QuantifierTests.TestClass.WithoutFoo
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.Have.QuantifierTests.TestClass.WithFoo1,
					               event EventHandler ThatEvents.Have.QuantifierTests.TestClass.WithoutFoo
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithPredicate_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)), typeof(TestClass).GetEvent(nameof(TestClass.WithFoo2)),];

				async Task Act()
				{
					await That(subject).Exactly(1).Have<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemHasAttribute_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).None().Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatEvents.Have.QuantifierTests.FooAttribute for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               event EventHandler ThatEvents.Have.QuantifierTests.TestClass.WithFoo1
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.Have.QuantifierTests.TestClass.WithoutFoo,
					               event EventHandler ThatEvents.Have.QuantifierTests.TestClass.WithFoo1
					             ]
					             """);
			}

			[Test]
			public async Task WhenAttributeTypeIsNoAttribute_ShouldThrowArgumentException()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)),];

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
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)), typeof(TestClass).GetEvent(nameof(TestClass.WithFoo2)),];

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
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)), typeof(TestClass).GetEvent(nameof(TestClass.WithoutFoo)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatEvents.Have.QuantifierTests.FooAttribute,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               event EventHandler ThatEvents.Have.QuantifierTests.TestClass.WithoutFoo
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.Have.QuantifierTests.TestClass.WithFoo1,
					               event EventHandler ThatEvents.Have.QuantifierTests.TestClass.WithoutFoo
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

#pragma warning disable CS0067
			private class TestClass
			{
				[Foo(Value = 1)]
				public event EventHandler? WithFoo1;

				[Foo(Value = 2)]
				public event EventHandler? WithFoo2;

				[Bar]
				public event EventHandler? WithBar;

				public event EventHandler? WithoutFoo;
			}
#pragma warning restore CS0067
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				ImmutableArray<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)), typeof(TestClass).GetEvent(nameof(TestClass.WithoutFoo)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have(typeof(FooAttribute));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatEvents.Have.StructCollectionTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               event EventHandler ThatEvents.Have.StructCollectionTests.TestClass.WithoutFoo
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.Have.StructCollectionTests.TestClass.WithFoo1,
					               event EventHandler ThatEvents.Have.StructCollectionTests.TestClass.WithoutFoo
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

#pragma warning disable CS0067
			private class TestClass
			{
				[Foo(Value = 1)]
				public event EventHandler? WithFoo1;

				[Foo(Value = 2)]
				public event EventHandler? WithFoo2;

				[Bar]
				public event EventHandler? WithBar;

				public event EventHandler? WithoutFoo;
			}
#pragma warning restore CS0067
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)), typeof(TestClass).GetEvent(nameof(TestClass.WithoutFoo)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatEvents.Have.AsyncEnumerableTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               event EventHandler ThatEvents.Have.AsyncEnumerableTests.TestClass.WithoutFoo
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.Have.AsyncEnumerableTests.TestClass.WithFoo1,
					               event EventHandler ThatEvents.Have.AsyncEnumerableTests.TestClass.WithoutFoo
					             ]
					             """);
			}

			[Test]
			public async Task WhenTheItemsMatch_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.WithFoo1)), typeof(TestClass).GetEvent(nameof(TestClass.WithFoo2)),
				}.ToTestAsyncEnumerable<EventInfo?>();

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

#pragma warning disable CS0067
			private class TestClass
			{
				[Foo(Value = 1)]
				public event EventHandler? WithFoo1;

				[Foo(Value = 2)]
				public event EventHandler? WithFoo2;

				[Bar]
				public event EventHandler? WithBar;

				public event EventHandler? WithoutFoo;
			}
#pragma warning restore CS0067
		}
#endif
	}
}
