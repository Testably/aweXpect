namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class DateTimeKindTests
		{
			private static readonly DateTime Local = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Local);
			private static readonly DateTime Unspecified = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
			private static readonly DateTime Utc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			[Fact]
			public async Task WhenKindsAreIncompatible_ShouldFail()
			{
				DateTime[] subject = [Utc,];

				async Task Act()
					=> await That(subject).Contains(Local);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains an item equal to {Formatter.Format(Local)} at least once,
					              but it did not contain it

					              Collection:
					              [
					                {Formatter.Format(Utc)}
					              ]
					              """)
					.Because("a Utc and a Local value with the same ticks denote different instants");
			}

			[Fact]
			public async Task WhenKindsAreIncompatible_ShouldSucceedForDoesNotContain()
			{
				DateTime[] subject = [Utc,];

				async Task Act()
					=> await That(subject).DoesNotContain(Local);

				await That(Act).DoesNotThrow()
					.Because("an incomparable value is not contained, unlike an ordering check it stays decidable");
			}

			[Fact]
			public async Task WhenOneKindIsUnspecified_ShouldSucceed()
			{
				DateTime[] subject = [Unspecified,];

				async Task Act()
					=> await That(subject).Contains(Utc);

				await That(Act).DoesNotThrow()
					.Because("an Unspecified Kind is compatible with both Utc and Local");
			}
		}
	}
}
