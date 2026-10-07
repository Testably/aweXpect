using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsEqualTo(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to <null>,
					             but it was 'a'
					             """);
			}

			[Test]
			[Arguments(true, true)]
			[Arguments(true, false)]
			[Arguments(false, true)]
			[Arguments(false, false)]
			public async Task WhenIgnoringCaseIsSpecifiedTwice_ShouldThrowInvalidOperationException(
				bool first, bool second)
			{
				char subject = 'a';
				CharEqualityResult<char, IThat<char>> sut = That(subject).IsEqualTo('A');
				_ = sut.IgnoringCase(first);

				void Act() => sut.IgnoringCase(second);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("IgnoringCase cannot be specified more than once.")
					.Because("the second value would silently replace the first one");
			}

			[Test]
			[Arguments('a', 'A')]
			[Arguments('B', 'b')]
			[Arguments('ä', 'Ä')]
			public async Task WhenSubjectDiffersOnlyInCase_AndIgnoringCase_ShouldSucceed(char subject, char expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectDiffersOnlyInCase_AndNotIgnoringCase_ShouldFail()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsEqualTo('A').IgnoringCase(false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 'A',
					             but it was 'a'
					             """);
			}

			[Test]
			public async Task WhenSubjectIsADottedCapitalI_AndIgnoringCase_ShouldFail()
			{
				char subject = 'İ';

				async Task Act()
					=> await That(subject).IsEqualTo('i').IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 'i' ignoring case,
					             but it was 'İ'
					             """)
					.Because("the comparison does not depend on a culture like Turkish");
			}

			[Test]
			public async Task WhenSubjectIsDifferent_AndIgnoringCase_ShouldFail()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsEqualTo('B').IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 'B' ignoring case,
					             but it was 'a'
					             """);
			}

			[Test]
			[Arguments('a', 'b')]
			[Arguments('B', 'b')]
			public async Task WhenSubjectIsDifferent_ShouldFail(char subject, char expected)
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
			[Arguments('a')]
			[Arguments('X')]
			[Arguments('5')]
			[Arguments('\t')]
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
