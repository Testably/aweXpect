namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).IsEqualTo(null);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(MyColors.Blue, MyColors.Green)]
			[Arguments(MyColors.Green, MyColors.Blue)]
			public async Task WhenSubjectIsDifferent_ShouldFail(MyColors subject, MyColors expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(MyColors.Blue)]
			[Arguments(MyColors.Green)]
			public async Task WhenSubjectIsTheSame_ShouldSucceed(MyColors subject)
			{
				MyColors expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LongTests
		{
			[Test]
			[Arguments(EnumLong.Int64Max, EnumLong.Int64LessOne)]
			public async Task WhenSubjectIsDifferent_ShouldFail(EnumLong subject,
				EnumLong expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(EnumLong.Int64Max)]
			[Arguments(EnumLong.Int64LessOne)]
			public async Task WhenSubjectTheSame_ShouldSucceed(EnumLong subject)
			{
				EnumLong expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class UlongTests
		{
			[Test]
			[Arguments(EnumULong.UInt64Max, EnumULong.UInt64LessOne)]
			[Arguments(EnumULong.UInt64Max, EnumULong.Int64Max)]
			public async Task WhenSubjectIsDifferent_ShouldFail(EnumULong subject,
				EnumULong expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(EnumULong.Int64Max)]
			[Arguments(EnumULong.UInt64LessOne)]
			[Arguments(EnumULong.UInt64Max)]
			public async Task WhenSubjectTheSame_ShouldSucceed(EnumULong subject)
			{
				EnumULong expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
