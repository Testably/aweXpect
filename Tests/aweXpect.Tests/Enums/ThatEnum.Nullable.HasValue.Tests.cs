namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class HasValue
		{
			public sealed class ContinuationTests
			{
				[Fact]
				public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldRenderLikeTheShorthand()
				{
					MyNumbers? subject = MyNumbers.One;

					async Task Act()
						=> await That(subject).HasValue().EqualTo(2L);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has value equal to 2,
						             but it had value 1
						             """)
						.Because("the continuation renders exactly like the HasValue(expected) shorthand");
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					MyNumbers? subject = null;

					async Task Act()
						=> await That(subject).HasValue().GreaterThan(0L);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has value greater than 0,
						             but it was <null>
						             """);
				}
			}

			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					MyColors? subject = MyColors.Yellow;

					async Task Act()
						=> await That(subject).HasValue(null);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has value equal to <null>,
						              but it had value {Formatter.Format((long?)subject)}
						              """);
				}

				[Theory]
				[InlineData(MyNumbers.One, 2L)]
				[InlineData(MyNumbers.Two, -7L)]
				[InlineData(MyNumbers.Three, 0L)]
				public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldFail(MyNumbers? subject,
					long? expected)
				{
					async Task Act()
						=> await That(subject).HasValue(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has value equal to {Formatter.Format(expected)},
						              but it had value {Formatter.Format((long?)subject)}
						              """);
				}

				[Theory]
				[InlineData(MyNumbers.One, 1L)]
				[InlineData(MyNumbers.Two, 2L)]
				[InlineData(MyNumbers.Three, 3L)]
				public async Task WhenSubjectHasExpectedValue_ShouldSucceed(MyNumbers? subject,
					long? expected)
				{
					async Task Act()
						=> await That(subject).HasValue(expected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(null)]
				[InlineData(0L)]
				[InlineData(1L)]
				public async Task WhenSubjectIsNull_ShouldFail(long? expected)
				{
					MyNumbers? subject = null;

					async Task Act()
						=> await That(subject).HasValue(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has value equal to {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}
			}
		}
	}
}
