using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed partial class AreOfType
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_OrOfType_ShouldCountItemsOfEitherType()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.Sub)), typeof(TestClass).GetEvent(nameof(TestClass.Other)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfType<EventHandler>().OrOfType<Action>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsMatch_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.Sub)), typeof(TestClass).GetEvent(nameof(TestClass.Other)),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreOfType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type EventHandler for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               event Action ThatEvents.AreOfType.QuantifierTests.TestClass.Other
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreOfType.QuantifierTests.TestClass.Sub,
					               event Action ThatEvents.AreOfType.QuantifierTests.TestClass.Other
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
					await That(subject).Exactly(1).AreOfType(type);
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
					await That(subject).None().AreOfType(type);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type EventHandler for no items,
					             but 1 of 2 were

					             Matching items:
					             [
					               event EventHandler ThatEvents.AreOfType.QuantifierTests.TestClass.Sub
					             ]

					             Collection:
					             [
					               event Action ThatEvents.AreOfType.QuantifierTests.TestClass.Other,
					               event EventHandler ThatEvents.AreOfType.QuantifierTests.TestClass.Sub
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
					await That(subject).AtLeast(2).AreOfType(typeof(EventHandler));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type EventHandler for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               event Action ThatEvents.AreOfType.StructCollectionTests.TestClass.Other
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreOfType.StructCollectionTests.TestClass.Sub,
					               event Action ThatEvents.AreOfType.StructCollectionTests.TestClass.Other
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
					await That(subject).AtLeast(2).AreOfType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type EventHandler for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               event Action ThatEvents.AreOfType.AsyncEnumerableQuantifierTests.TestClass.Other
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreOfType.AsyncEnumerableQuantifierTests.TestClass.Sub,
					               event Action ThatEvents.AreOfType.AsyncEnumerableQuantifierTests.TestClass.Other
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
					await That(subject).Exactly(1).AreOfType(type);
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
