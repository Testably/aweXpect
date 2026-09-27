namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsEqualTo(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to <null>,
					             but it was 'a'
					             """);
			}

			[Theory]
			[InlineData('a', 'A')]
			[InlineData('B', 'b')]
			[InlineData('ä', 'Ä')]
			public async Task WhenSubjectDiffersOnlyInCase_AndIgnoringCase_ShouldSucceed(char subject, char expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectDiffersOnlyInCase_AndNotIgnoringCase_ShouldFail()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsEqualTo('A').IgnoringCase(false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 'A',
					             but it was 'a'
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsADottedCapitalI_AndIgnoringCase_ShouldFail()
			{
				char subject = 'İ';

				async Task Act()
					=> await That(subject).IsEqualTo('i').IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 'i' ignoring case,
					             but it was 'İ'
					             """)
					.Because("the comparison does not depend on a culture like Turkish");
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_AndIgnoringCase_ShouldFail()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsEqualTo('B').IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 'B' ignoring case,
					             but it was 'a'
					             """);
			}

			[Theory]
			[InlineData('a', 'b')]
			[InlineData('B', 'b')]
			public async Task WhenSubjectIsDifferent_ShouldFail(char subject, char expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData('a')]
			[InlineData('X')]
			[InlineData('5')]
			[InlineData('\t')]
			public async Task WhenSubjectIsTheSame_ShouldSucceed(char subject)
			{
				char expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
