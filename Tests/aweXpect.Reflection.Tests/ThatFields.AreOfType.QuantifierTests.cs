using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed partial class AreOfType
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrOfType_ShouldCountItemsOfEitherType()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.Sub)), typeof(TestClass).GetField(nameof(TestClass.Other)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfType<Exception>().OrOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.Sub)), typeof(TestClass).GetField(nameof(TestClass.Other)),];

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
					               int ThatFields.AreOfType.QuantifierTests.TestClass.Other
					             ]

					             Collection:
					             [
					               ArgumentException ThatFields.AreOfType.QuantifierTests.TestClass.Sub,
					               int ThatFields.AreOfType.QuantifierTests.TestClass.Other
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithType_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.Sub)), typeof(TestClass).GetField(nameof(TestClass.Other)),];

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
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.Other)), typeof(TestClass).GetField(nameof(TestClass.Sub)),];

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
					               ArgumentException ThatFields.AreOfType.QuantifierTests.TestClass.Sub
					             ]

					             Collection:
					             [
					               int ThatFields.AreOfType.QuantifierTests.TestClass.Other,
					               ArgumentException ThatFields.AreOfType.QuantifierTests.TestClass.Sub
					             ]
					             """);
			}

#pragma warning disable CS0649
			private class TestClass
			{
				public ArgumentException? Sub;
				public int Other;
			}
#pragma warning restore CS0649
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				ImmutableArray<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.Sub)), typeof(TestClass).GetField(nameof(TestClass.Other)),];

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
					               int ThatFields.AreOfType.StructCollectionTests.TestClass.Other
					             ]

					             Collection:
					             [
					               ArgumentException ThatFields.AreOfType.StructCollectionTests.TestClass.Sub,
					               int ThatFields.AreOfType.StructCollectionTests.TestClass.Other
					             ]
					             """);
			}

#pragma warning disable CS0649
			private class TestClass
			{
				public ArgumentException? Sub;
				public int Other;
			}
#pragma warning restore CS0649
		}

		public sealed class AsyncEnumerableQuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.Sub)), typeof(TestClass).GetField(nameof(TestClass.Other)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

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
					               int ThatFields.AreOfType.AsyncEnumerableQuantifierTests.TestClass.Other
					             ]

					             Collection:
					             [
					               ArgumentException ThatFields.AreOfType.AsyncEnumerableQuantifierTests.TestClass.Sub,
					               int ThatFields.AreOfType.AsyncEnumerableQuantifierTests.TestClass.Other
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithType_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.Sub)), typeof(TestClass).GetField(nameof(TestClass.Other)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

				Type type = typeof(Exception);

				async Task Act()
				{
					await That(subject).Exactly(1).AreOfType(type);
				}

				await That(Act).DoesNotThrow();
			}

#pragma warning disable CS0649
			private class TestClass
			{
				public ArgumentException? Sub;
				public int Other;
			}
#pragma warning restore CS0649
		}
#endif
	}
}
