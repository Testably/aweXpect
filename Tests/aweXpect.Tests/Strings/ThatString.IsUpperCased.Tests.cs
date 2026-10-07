using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsUpperCased
	{
		public sealed class IncludingUncasedLettersTests
		{
			[Test]
			[Arguments("STRAßE")]
			[Arguments("ı")]
			[Arguments("ﬁ")]
			[Arguments("𝐚")]
			public async Task WhenActualContainsLowerCaseLetterWithoutUpperCaseForm_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is upper-cased including uncased letters,
					              but it was "{subject}"
					              """);
			}

			[Test]
			[Arguments("ǅ")]
			[Arguments("ᾈ")]
			public async Task WhenActualContainsTitlecaseLetter_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is upper-cased including uncased letters,
					              but it was "{subject}"
					              """)
					.Because("a titlecase letter is not upper-cased, even where the runtime has no upper-case mapping for it");
			}

			[Test]
			public async Task WhenActualIsDigitsAndPunctuation_ShouldSucceed()
			{
				string subject = "1-2, 3!";

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsEmpty_ShouldSucceed()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsLowerCased_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is upper-cased including uncased letters,
					             but it was "abc"
					             """);
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is upper-cased including uncased letters,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldFail()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is upper-cased including uncased letters,
					             but it was "𐐨"
					             """);
			}

			[Test]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldSucceed()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsUpperCased_ShouldSucceed()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsUpperCasedOrCaseless_ShouldSucceed()
			{
				string subject = "A漢字B";

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFalseIsSpecified_ShouldNotMentionUncasedLetters()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters(false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is upper-cased,
					             but it was "abc"
					             """);
			}

			[Test]
			public async Task WhenFalseIsSpecified_ShouldSucceedForLowerCaseLetterWithoutUpperCaseForm()
			{
				string subject = "STRAßE";

				async Task Act()
					=> await That(subject).IsUpperCased().IncludingUncasedLetters(false);

				await That(Act).DoesNotThrow()
					.Because("a letter without an upper-case form counts as upper-cased, as without the option");
			}

			[Test]
			[Arguments(true, true)]
			[Arguments(true, false)]
			[Arguments(false, true)]
			[Arguments(false, false)]
			public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException(bool first, bool second)
			{
				string subject = "ABC";
				CasingResult<string, IThat<string?>> sut = That(subject).IsUpperCased();
				_ = sut.IncludingUncasedLetters(first);

				void Act() => sut.IncludingUncasedLetters(second);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("IncludingUncasedLetters cannot be specified more than once.")
					.Because("the second value would silently replace the first one");
			}
		}

		public sealed class Tests
		{
			[Test]
			[Arguments("STRAßE")]
			[Arguments("ı")]
			[Arguments("ﬁ")]
			[Arguments("𝐚")]
			public async Task WhenActualContainsLowerCaseLetterWithoutUpperCaseForm_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).DoesNotThrow()
					.Because("without IncludingUncasedLetters a letter without an upper-case form counts as upper-cased");
			}

			[Test]
			public async Task WhenActualContainsTitlecaseLetter_ShouldFollowTheRuntimeCaseMapping()
			{
				string subject = "ǅ";

				async Task Act()
					=> await That(subject).IsUpperCased();

#if NETFRAMEWORK
				await That(Act).DoesNotThrow()
					.Because("the invariant culture of .NET Framework has no upper-case mapping for ǅ");
#else
				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is upper-cased,
					             but it was "ǅ"
					             """)
					.Because("the invariant culture of .NET maps ǅ to Ǆ");
#endif
			}

			[Test]
			public async Task WhenActualIsEmpty_ShouldSucceed()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsLowerCased_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is upper-cased,
					             but it was "abc"
					             """);
			}

			[Test]
			public async Task WhenActualIsMixedCased_ShouldFail()
			{
				string subject = "AbC";

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is upper-cased,
					             but it was "AbC"
					             """);
			}

			[Test]
			public async Task WhenActualIsNotUpperCased_ShouldLimitDisplayedStringTo100Characters()
			{
				string subject = StringWithMoreThan100Characters;

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is upper-cased,
					              but it was "{StringWith100Characters}…"
					              """);
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is upper-cased,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldFail()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is upper-cased,
					             but it was "𐐨"
					             """);
			}

			[Test]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldSucceed()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsUpperCased_ShouldSucceed()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsUpperCasedOrCaseless_ShouldSucceed()
			{
				string subject = "A漢字B";

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsUpperCasedOrSpecialCharacters_ShouldSucceed()
			{
				string subject = "A-B-C!";

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsWhitespace_ShouldSucceed()
			{
				string subject = " \t\r\n";

				async Task Act()
					=> await That(subject).IsUpperCased();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
