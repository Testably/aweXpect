using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed partial class AreOfType
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrOfType_ShouldCountItemsOfEitherType()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.Sub)), typeof(TestClass).GetProperty(nameof(TestClass.Other)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfType<Exception>().OrOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.Sub)), typeof(TestClass).GetProperty(nameof(TestClass.Other)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfType<Exception>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type Exception for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               public int ThatProperties.AreOfType.QuantifierTests.TestClass.Other { get; set; }
					             ]

					             Collection:
					             [
					               public ArgumentException ThatProperties.AreOfType.QuantifierTests.TestClass.Sub { get; set; },
					               public int ThatProperties.AreOfType.QuantifierTests.TestClass.Other { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithType_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.Sub)), typeof(TestClass).GetProperty(nameof(TestClass.Other)),];

				Type type = typeof(Exception);

				async Task Act()
				{
					await That(subject).Exactly(1).AreOfType(type);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemMatches_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.Other)), typeof(TestClass).GetProperty(nameof(TestClass.Sub)),];

				Type type = typeof(Exception);

				async Task Act()
				{
					await That(subject).None().AreOfType(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type Exception for no items,
					             but 1 of 2 were

					             Matching items:
					             [
					               public ArgumentException ThatProperties.AreOfType.QuantifierTests.TestClass.Sub { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.AreOfType.QuantifierTests.TestClass.Other { get; set; },
					               public ArgumentException ThatProperties.AreOfType.QuantifierTests.TestClass.Sub { get; set; }
					             ]
					             """);
			}

			private class TestClass
			{
				public ArgumentException? Sub { get; set; }
				public int Other { get; set; }
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				ImmutableArray<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.Sub)), typeof(TestClass).GetProperty(nameof(TestClass.Other)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfType(typeof(Exception));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type Exception for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               public int ThatProperties.AreOfType.StructCollectionTests.TestClass.Other { get; set; }
					             ]

					             Collection:
					             [
					               public ArgumentException ThatProperties.AreOfType.StructCollectionTests.TestClass.Sub { get; set; },
					               public int ThatProperties.AreOfType.StructCollectionTests.TestClass.Other { get; set; }
					             ]
					             """);
			}

			private class TestClass
			{
				public ArgumentException? Sub { get; set; }
				public int Other { get; set; }
			}
		}

		public sealed class AsyncEnumerableQuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.Sub)), typeof(TestClass).GetProperty(nameof(TestClass.Other)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfType<Exception>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type Exception for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               public int ThatProperties.AreOfType.AsyncEnumerableQuantifierTests.TestClass.Other { get; set; }
					             ]

					             Collection:
					             [
					               public ArgumentException ThatProperties.AreOfType.AsyncEnumerableQuantifierTests.TestClass.Sub { get; set; },
					               public int ThatProperties.AreOfType.AsyncEnumerableQuantifierTests.TestClass.Other { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithType_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.Sub)), typeof(TestClass).GetProperty(nameof(TestClass.Other)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				Type type = typeof(Exception);

				async Task Act()
				{
					await That(subject).Exactly(1).AreOfType(type);
				}

				await That(Act).DoesNotThrow();
			}

			private class TestClass
			{
				public ArgumentException? Sub { get; set; }
				public int Other { get; set; }
			}
		}
#endif
	}
}
