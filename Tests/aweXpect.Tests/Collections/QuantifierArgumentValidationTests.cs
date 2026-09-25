namespace aweXpect.Tests;

public sealed class QuantifierArgumentValidation
{
	public sealed class Tests
	{
		[Fact]
		public async Task AtLeast_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).AtLeast(-1).AreEqualTo(1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("minimum").And
				.WithMessage("The minimum must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task AtMost_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).AtMost(-1).AreEqualTo(1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task Between_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).Between(3).And(1).AreEqualTo(1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
		}

		[Fact]
		public async Task Between_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).Between(0).And(-1).AreEqualTo(1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task Between_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).Between(-1).And(2).AreEqualTo(1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("minimum").And
				.WithMessage("The minimum must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task Exactly_WhenExpectedIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).Exactly(-1).AreEqualTo(1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("expected").And
				.WithMessage("The expected count must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task HasCountBetween_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).HasCount().Between(3).And(1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
		}

		[Fact]
		public async Task HasCountEqualTo_WhenExpectedIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).HasCount().EqualTo(-1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("expected").And
				.WithMessage("The expected count must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task HasCountNotEqualTo_WhenUnexpectedIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).HasCount().NotEqualTo(-1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("expected").And
				.WithMessage("The expected count must not be negative.").AsPrefix()
				.Because("a count is never negative, so the negated expectation would hold for every collection");
		}

		[Fact]
		public async Task HasCountShorthand_WhenExpectedIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).HasCount(-1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("expected").And
				.WithMessage("The expected count must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task LessThan_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).LessThan(-1).AreEqualTo(1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task MoreThan_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).MoreThan(-1).AreEqualTo(1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("minimum").And
				.WithMessage("The minimum must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task NegatedAtMost_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.AtMost(-1).AreEqualTo(1));

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must not be negative.").AsPrefix()
				.Because("the negated form is where a nonsensical count would otherwise pass unnoticed");
		}

		[Fact]
		public async Task NegatedBetween_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.Between(3).And(1).AreEqualTo(1));

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
				.Because("an inverted range would let the negated expectation succeed for every collection");
		}
	}
}
