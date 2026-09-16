namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class HasValue
	{
		public sealed class ContinuationTests
		{
			[Theory]
			[InlineData(MyNumbers.One, 2L)]
			[InlineData(MyNumbers.Two, 3L)]
			public async Task ShouldSupportTheComparisonVocabulary(MyNumbers subject, long maximum)
			{
				async Task Act()
					=> await That(subject).HasValue().LessThan(maximum);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldRenderLikeTheShorthand()
			{
				MyNumbers subject = MyNumbers.One;

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
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).HasValue(null);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has value equal to <null>,
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}

			[Theory]
			[InlineData(MyNumbers.One, 2L)]
			[InlineData(MyNumbers.Two, -7)]
			[InlineData(MyNumbers.Three, 0)]
			public async Task WhenSubjectDoesNotHaveExpectedValue_ShouldFail(MyNumbers subject,
				long expected)
			{
				async Task Act()
					=> await That(subject).HasValue(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has value equal to {Formatter.Format(expected)},
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}

			[Theory]
			[InlineData(MyNumbers.One, 1)]
			[InlineData(MyNumbers.Two, 2)]
			[InlineData(MyNumbers.Three, 3)]
			public async Task WhenSubjectHasExpectedValue_ShouldSucceed(MyNumbers subject,
				long expected)
			{
				async Task Act()
					=> await That(subject).HasValue(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
