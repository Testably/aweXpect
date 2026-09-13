namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed class IsNotEmpty
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenActualIsNotEmpty_ShouldSucceed(string? subject)
			{
				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenActualIsWhitespace_ShouldSucceed()
			{
				string subject = " \t ";

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
