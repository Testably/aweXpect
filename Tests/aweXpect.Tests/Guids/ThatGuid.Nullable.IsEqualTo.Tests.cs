namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed partial class Nullable
	{
		public sealed class IsEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
				{
					Guid? subject = null;
					Guid? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					Guid? subject = FixedGuid();
					Guid? expected = OtherGuid();

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Guid? subject = null;

					async Task Act()
						=> await That(subject).IsEqualTo(FixedGuid());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {FixedGuid()},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsTheSame_ShouldSucceed()
				{
					Guid? subject = FixedGuid();
					Guid? expected = subject;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
