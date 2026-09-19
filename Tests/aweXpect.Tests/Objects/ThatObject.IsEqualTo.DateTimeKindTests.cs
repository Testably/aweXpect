namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEqualTo
	{
		public sealed class DateTimeKindTests
		{
			private static readonly DateTime Local = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Local);
			private static readonly DateTime Unspecified = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
			private static readonly DateTime Utc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			[Fact]
			public async Task WhenKindsAreIncompatible_ShouldFail()
			{
				object subject = Utc;

				async Task Act()
					=> await That(subject).IsEqualTo(Local);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(Local)},
					              but it was {Formatter.Format(Utc)}
					              """)
					.Because("a Utc and a Local value with the same ticks denote different instants");
			}

			[Fact]
			public async Task WhenKindsAreTheSame_ShouldSucceed()
			{
				object subject = Utc;

				async Task Act()
					=> await That(subject).IsEqualTo(Utc);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenOneKindIsUnspecified_ShouldSucceed()
			{
				object subject = Unspecified;

				async Task Act()
					=> await That(subject).IsEqualTo(Utc);

				await That(Act).DoesNotThrow()
					.Because("an Unspecified Kind is compatible with both Utc and Local");
			}
		}
	}
}
