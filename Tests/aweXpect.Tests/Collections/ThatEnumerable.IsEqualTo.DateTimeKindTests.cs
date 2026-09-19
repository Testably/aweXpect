using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
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
				DateTime[] subject = [Utc,];
				IEnumerable<DateTime> expected = [Local,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to collection expected in order,
					              but it
					                contained item {Formatter.Format(Utc)} at index 0 that was not expected and
					                lacked the one expected item

					              Collection:
					              [
					                {Formatter.Format(Utc)}
					              ]

					              Expected:
					              [
					                {Formatter.Format(Local)}
					              ]
					              """)
					.Because("a Utc and a Local value with the same ticks denote different instants");
			}

			[Fact]
			public async Task WhenKindsAreTheSame_ShouldSucceed()
			{
				DateTime[] subject = [Utc,];
				IEnumerable<DateTime> expected = [Utc,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenOneKindIsUnspecified_ShouldSucceed()
			{
				DateTime[] subject = [Unspecified,];
				IEnumerable<DateTime> expected = [Utc,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("an Unspecified Kind is compatible with both Utc and Local");
			}
		}
	}
}
