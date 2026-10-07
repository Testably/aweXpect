using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsLowerCased
	{
		public sealed class IncludingUncasedLettersTests
		{
			[Test]
			[Arguments("ǅ")]
			[Arguments("ᾈ")]
			public async Task WhenActualContainsTitlecaseLetter_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is lower-cased including uncased letters,
					              but it was "{subject}"
					              """)
					.Because("a titlecase letter is not lower-cased, even where the runtime has no lower-case mapping for it");
			}

			[Test]
			[Arguments("İstanbul")]
			[Arguments("ℂ")]
			[Arguments("ϒ")]
			[Arguments("𝐀")]
			public async Task WhenActualContainsUpperCaseLetterWithoutLowerCaseForm_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is lower-cased including uncased letters,
					              but it was "{subject}"
					              """);
			}

			[Test]
			public async Task WhenActualIsDigitsAndPunctuation_ShouldSucceed()
			{
				string subject = "1-2, 3!";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsEmpty_ShouldSucceed()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsLowerCased_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsLowerCasedOrCaseless_ShouldSucceed()
			{
				string subject = "a漢字b";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased including uncased letters,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldSucceed()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldFail()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased including uncased letters,
					             but it was "𐐀"
					             """);
			}

			[Test]
			public async Task WhenActualIsUpperCased_ShouldFail()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased including uncased letters,
					             but it was "ABC"
					             """);
			}

			[Test]
			public async Task WhenFalseIsSpecified_ShouldNotMentionUncasedLetters()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters(false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was "ABC"
					             """);
			}

			[Test]
			public async Task WhenFalseIsSpecified_ShouldSucceedForUpperCaseLetterWithoutLowerCaseForm()
			{
				string subject = "𝐀";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters(false);

				await That(Act).DoesNotThrow()
					.Because("a letter without a lower-case form counts as lower-cased, as without the option");
			}

			[Test]
			[Arguments(true, true)]
			[Arguments(true, false)]
			[Arguments(false, true)]
			[Arguments(false, false)]
			public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException(bool first, bool second)
			{
				string subject = "abc";
				CasingResult<string, IThat<string?>> sut = That(subject).IsLowerCased();
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
			public async Task WhenActualContainsTitlecaseLetter_ShouldFollowTheRuntimeCaseMapping()
			{
				string subject = "ǅ";

				async Task Act()
					=> await That(subject).IsLowerCased();

#if NETFRAMEWORK
				await That(Act).DoesNotThrow()
					.Because("the invariant culture of .NET Framework has no lower-case mapping for ǅ");
#else
				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was "ǅ"
					             """)
					.Because("the invariant culture of .NET maps ǅ to ǆ");
#endif
			}

			[Test]
			[Arguments("İstanbul")]
			[Arguments("ℂ")]
			[Arguments("ϒ")]
			[Arguments("𝐀")]
			public async Task WhenActualContainsUpperCaseLetterWithoutLowerCaseForm_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow()
					.Because("without IncludingUncasedLetters a letter without a lower-case form counts as lower-cased");
			}

			[Test]
			public async Task WhenActualIsEmpty_ShouldSucceed()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsLowerCased_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsLowerCasedOrCaseless_ShouldSucceed()
			{
				string subject = "a漢字b";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsLowerCasedOrSpecialCharacters_ShouldSucceed()
			{
				string subject = "a-b-c!";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsMixedCased_ShouldFail()
			{
				string subject = "aBc";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was "aBc"
					             """);
			}

			[Test]
			public async Task WhenActualIsNotLowerCased_ShouldLimitDisplayedStringTo100Characters()
			{
				string subject = StringWithMoreThan100Characters;

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is lower-cased,
					              but it was "{StringWith100Characters}…"
					              """);
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldSucceed()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldFail()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was "𐐀"
					             """);
			}

			[Test]
			public async Task WhenActualIsUpperCased_ShouldFail()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was "ABC"
					             """);
			}

			[Test]
			public async Task WhenActualIsWhitespace_ShouldSucceed()
			{
				string subject = " \t\r\n";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
