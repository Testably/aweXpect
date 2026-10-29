using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class Have
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrHave_ShouldCountItemsWithEitherAttribute()
			{
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)), typeof(TestClass).GetMethod(nameof(TestClass.WithBar)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>().OrHave<BarAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)), typeof(TestClass).GetMethod(nameof(TestClass.WithoutFoo)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatMethods.Have.QuantifierTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.Have.QuantifierTests.TestClass.WithoutFoo()
					             ]

					             Collection:
					             [
					               void ThatMethods.Have.QuantifierTests.TestClass.WithFoo1(),
					               void ThatMethods.Have.QuantifierTests.TestClass.WithoutFoo()
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithPredicate_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)), typeof(TestClass).GetMethod(nameof(TestClass.WithFoo2)),];

				async Task Act()
				{
					await That(subject).Exactly(1).Have<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemHasAttribute_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.WithoutFoo)), typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).None().Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatMethods.Have.QuantifierTests.FooAttribute for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               void ThatMethods.Have.QuantifierTests.TestClass.WithFoo1()
					             ]

					             Collection:
					             [
					               void ThatMethods.Have.QuantifierTests.TestClass.WithoutFoo(),
					               void ThatMethods.Have.QuantifierTests.TestClass.WithFoo1()
					             ]
					             """);
			}

			[Test]
			public async Task WhenAttributeTypeIsNoAttribute_ShouldThrowArgumentException()
			{
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)),];

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
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)), typeof(TestClass).GetMethod(nameof(TestClass.WithFoo2)),];

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
				IEnumerable<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)), typeof(TestClass).GetMethod(nameof(TestClass.WithoutFoo)),];

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).Have(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatMethods.Have.QuantifierTests.FooAttribute,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.Have.QuantifierTests.TestClass.WithoutFoo()
					             ]

					             Collection:
					             [
					               void ThatMethods.Have.QuantifierTests.TestClass.WithFoo1(),
					               void ThatMethods.Have.QuantifierTests.TestClass.WithoutFoo()
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
				public void WithFoo1() { }

				[Foo(Value = 2)]
				public void WithFoo2() { }

				[Bar]
				public void WithBar() { }

				public void WithoutFoo() { }
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				ImmutableArray<MethodInfo?> subject = [typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)), typeof(TestClass).GetMethod(nameof(TestClass.WithoutFoo)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Have(typeof(FooAttribute));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatMethods.Have.StructCollectionTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.Have.StructCollectionTests.TestClass.WithoutFoo()
					             ]

					             Collection:
					             [
					               void ThatMethods.Have.StructCollectionTests.TestClass.WithFoo1(),
					               void ThatMethods.Have.StructCollectionTests.TestClass.WithoutFoo()
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
				public void WithFoo1() { }

				[Foo(Value = 2)]
				public void WithFoo2() { }

				[Bar]
				public void WithBar() { }

				public void WithoutFoo() { }
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveAttribute_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)), typeof(TestClass).GetMethod(nameof(TestClass.WithoutFoo)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).Have<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatMethods.Have.AsyncEnumerableTests.FooAttribute for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.Have.AsyncEnumerableTests.TestClass.WithoutFoo()
					             ]

					             Collection:
					             [
					               void ThatMethods.Have.AsyncEnumerableTests.TestClass.WithFoo1(),
					               void ThatMethods.Have.AsyncEnumerableTests.TestClass.WithoutFoo()
					             ]
					             """);
			}

			[Test]
			public async Task WhenTheItemsMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.WithFoo1)), typeof(TestClass).GetMethod(nameof(TestClass.WithFoo2)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

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
				public void WithFoo1() { }

				[Foo(Value = 2)]
				public void WithFoo2() { }

				[Bar]
				public void WithBar() { }

				public void WithoutFoo() { }
			}
		}
#endif
	}
}
