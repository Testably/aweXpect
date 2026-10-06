namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class IsNotEqualTo
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
				{
					MyColors? subject = null;
					MyColors? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to <null>,
						             but it was <null>
						             """);
				}

				[Test]
				[Arguments(MyColors.Blue, MyColors.Green)]
				[Arguments(MyColors.Blue, null)]
				[Arguments(MyColors.Green, MyColors.Blue)]
				[Arguments(MyColors.Green, null)]
				[Arguments(null, MyColors.Blue)]
				[Arguments(null, MyColors.Green)]
				public async Task WhenSubjectIsDifferent_ShouldSucceed(MyColors? subject,
					MyColors? unexpected)
				{
					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(MyColors.Blue)]
				[Arguments(MyColors.Green)]
				[Arguments(null)]
				public async Task WhenSubjectIsTheSame_ShouldFail(MyColors? subject)
				{
					MyColors? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					MyColors? subject = MyColors.Yellow;

					async Task Act()
						=> await That(subject).IsNotEqualTo(null);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class LongTests
			{
				[Test]
				[Arguments(EnumLong.Int64Max, EnumLong.Int64LessOne)]
				public async Task WhenSubjectIsDifferent_ShouldSucceed(EnumLong? subject,
					EnumLong? unexpected)
				{
					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(EnumLong.Int64Max)]
				[Arguments(EnumLong.Int64LessOne)]
				public async Task WhenSubjectTheSame_ShouldFail(EnumLong? subject)
				{
					EnumLong? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}

			public sealed class UlongTests
			{
				[Test]
				[Arguments(EnumULong.UInt64Max, EnumULong.UInt64LessOne)]
				[Arguments(EnumULong.UInt64Max, EnumULong.Int64Max)]
				public async Task WhenSubjectIsDifferent_ShouldSucceed(EnumULong? subject,
					EnumULong? unexpected)
				{
					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(EnumULong.Int64Max)]
				[Arguments(EnumULong.UInt64LessOne)]
				[Arguments(EnumULong.UInt64Max)]
				public async Task WhenSubjectTheSame_ShouldFail(EnumULong? subject)
				{
					EnumULong? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}
		}
	}
}
