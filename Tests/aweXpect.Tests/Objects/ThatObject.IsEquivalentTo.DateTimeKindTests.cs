namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class DateTimeKindTests
		{
			private static readonly DateTime Local = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Local);
			private static readonly DateTime Unspecified = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
			private static readonly DateTime Utc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			[Fact]
			public async Task WhenKindsAreIncompatible_ShouldFail()
			{
				Reading subject = new(Utc);
				Reading expected = new(Local);

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               is equivalent to ThatObject.IsEquivalentTo.DateTimeKindTests.Reading {
					                   Timestamp = {{Formatter.Format(Local)}}
					                 },
					               but it was not:
					                 Property Timestamp differed:
					                      Found: {{Formatter.Format(Utc)}}
					                   Expected: {{Formatter.Format(Local)}}

					               Equivalency options:
					                - include public fields and properties
					               """)
					.Because("a Utc and a Local value with the same ticks denote different instants");
			}

			[Fact]
			public async Task WhenKindsAreTheSame_ShouldSucceed()
			{
				Reading subject = new(Utc);
				Reading expected = new(Utc);

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenOneKindIsUnspecified_ShouldSucceed()
			{
				Reading subject = new(Unspecified);
				Reading expected = new(Utc);

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).DoesNotThrow()
					.Because("an Unspecified Kind is compatible with both Utc and Local");
			}

			private sealed class Reading(DateTime timestamp)
			{
				public DateTime Timestamp { get; } = timestamp;
			}
		}
	}
}
