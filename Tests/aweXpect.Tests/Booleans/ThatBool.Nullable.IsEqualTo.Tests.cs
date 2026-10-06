namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed partial class Nullable
	{
		public sealed class IsEqualTo
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
				{
					bool? subject = null;
					bool? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(true, false)]
				[Arguments(true, null)]
				[Arguments(false, true)]
				[Arguments(false, null)]
				[Arguments(null, true)]
				[Arguments(null, false)]
				public async Task WhenSubjectIsDifferent_ShouldFail(bool? subject, bool? expected)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				[Arguments(true)]
				[Arguments(false)]
				[Arguments(null)]
				public async Task WhenSubjectIsTheSame_ShouldSucceed(bool? subject)
				{
					bool? expected = subject;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
