using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed partial class AreOfExactType
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrOfExactType_ShouldCountItemsOfEitherType()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.Sub)), typeof(TestClass).GetEvent(nameof(TestClass.Other)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfExactType<EventHandler>().OrOfExactType<Action>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.Sub)), typeof(TestClass).GetEvent(nameof(TestClass.Other)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfExactType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type EventHandler for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               event Action ThatEvents.AreOfExactType.QuantifierTests.TestClass.Other
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreOfExactType.QuantifierTests.TestClass.Sub,
					               event Action ThatEvents.AreOfExactType.QuantifierTests.TestClass.Other
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithType_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.Sub)), typeof(TestClass).GetEvent(nameof(TestClass.Other)),];

				Type type = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).Exactly(1).AreOfExactType(type);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemMatches_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.Other)), typeof(TestClass).GetEvent(nameof(TestClass.Sub)),];

				Type type = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).None().AreOfExactType(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type EventHandler for no items,
					             but 1 of 2 were

					             Matching items:
					             [
					               event EventHandler ThatEvents.AreOfExactType.QuantifierTests.TestClass.Sub
					             ]

					             Collection:
					             [
					               event Action ThatEvents.AreOfExactType.QuantifierTests.TestClass.Other,
					               event EventHandler ThatEvents.AreOfExactType.QuantifierTests.TestClass.Sub
					             ]
					             """);
			}

#pragma warning disable CS0067
			private class TestClass
			{
				public event EventHandler? Sub;
				public event Action? Other;
			}
#pragma warning restore CS0067
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				ImmutableArray<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.Sub)), typeof(TestClass).GetEvent(nameof(TestClass.Other)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfExactType(typeof(EventHandler));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type EventHandler for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               event Action ThatEvents.AreOfExactType.StructCollectionTests.TestClass.Other
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreOfExactType.StructCollectionTests.TestClass.Sub,
					               event Action ThatEvents.AreOfExactType.StructCollectionTests.TestClass.Other
					             ]
					             """);
			}

#pragma warning disable CS0067
			private class TestClass
			{
				public event EventHandler? Sub;
				public event Action? Other;
			}
#pragma warning restore CS0067
		}

		public sealed class AsyncEnumerableQuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.Sub)), typeof(TestClass).GetEvent(nameof(TestClass.Other)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfExactType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type EventHandler for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               event Action ThatEvents.AreOfExactType.AsyncEnumerableQuantifierTests.TestClass.Other
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreOfExactType.AsyncEnumerableQuantifierTests.TestClass.Sub,
					               event Action ThatEvents.AreOfExactType.AsyncEnumerableQuantifierTests.TestClass.Other
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WithType_WhenTheNumberOfMatchingItemsMatches_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.Sub)), typeof(TestClass).GetEvent(nameof(TestClass.Other)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				Type type = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).Exactly(1).AreOfExactType(type);
				}

				await That(Act).DoesNotThrow();
			}

#pragma warning disable CS0067
			private class TestClass
			{
				public event EventHandler? Sub;
				public event Action? Other;
			}
#pragma warning restore CS0067
		}
#endif
	}
}
