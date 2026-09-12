namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class DoesNotHaveValue
		{
			public sealed class Tests
			{
				[Theory]
				[InlineData(MyNumbers.One, 2L)]
				[InlineData(MyNumbers.Two, -7L)]
				[InlineData(MyNumbers.Three, 0L)]
				public async Task WhenSubjectDoesNotHaveUnexpectedValue_ShouldSucceed(MyNumbers? subject,
					long? unexpected)
				{
					async Task Act()
						=> await That(subject).DoesNotHaveValue(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(MyNumbers.One, 1L)]
				[InlineData(MyNumbers.Two, 2L)]
				[InlineData(MyNumbers.Three, 3L)]
				public async Task WhenSubjectHasUnexpectedValue_ShouldFail(MyNumbers? subject,
					long? unexpected)
				{
					async Task Act()
						=> await That(subject).DoesNotHaveValue(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not have value {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData(null)]
				[InlineData(0L)]
				[InlineData(2L)]
				public async Task WhenSubjectIsNull_ShouldSucceed(long? unexpected)
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).DoesNotHaveValue(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					MyColors? subject = MyColors.Yellow;

					async Task Act()
						=> await That(subject).DoesNotHaveValue(null);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
