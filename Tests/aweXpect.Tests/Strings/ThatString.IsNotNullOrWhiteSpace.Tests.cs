namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed class IsNotNullOrWhiteSpace
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null or whitespace,
					             but it was ""
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenActualIsNotEmpty_ShouldSucceed(string? subject)
			{
				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldSucceed()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null or whitespace,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenActualIsWhiteSpace_ShouldLimitDisplayedStringTo100Characters()
			{
				string subject = new(' ', 101);

				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not null or whitespace,
					              but it was "{new string(' ', 100)}…"
					              """);
			}

			[Fact]
			public async Task WhenActualIsWhitespace_ShouldSucceed()
			{
				string subject = " \t ";

				async Task Act()
					=> await That(subject).IsNotNullOrWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null or whitespace,
					             but it was " \t "
					             """);
			}
		}
	}
}
