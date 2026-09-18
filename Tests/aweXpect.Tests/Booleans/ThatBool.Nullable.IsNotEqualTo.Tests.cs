namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed partial class Nullable
	{
		public sealed class IsNotEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenSubjectAndExpectedAreNull_ShouldFail()
				{
					bool? subject = null;
					bool? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not <null>,
						             but it was <null>
						             """);
				}

				[Theory]
				[InlineData(true, false)]
				[InlineData(true, null)]
				[InlineData(false, true)]
				[InlineData(false, null)]
				[InlineData(null, true)]
				[InlineData(null, false)]
				public async Task WhenSubjectIsDifferent_ShouldSucceed(bool? subject, bool? unexpected)
				{
					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WhenSubjectIsTheSame_ShouldFail(bool? subject)
				{
					bool? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}
		}
	}
}
