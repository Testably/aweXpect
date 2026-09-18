using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class DateTimeKindTests
	{
		public sealed class GrammarTests
		{
			[Fact]
			public async Task WhenActive_ShouldUseTheActiveVoice()
			{
				PropertyResult.DateTimeKind<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasDateTimeKindValue(DateTimeKind.Utc, ExpectationGrammars.Active);

				async Task Act()
					=> await sut.EqualTo(DateTimeKind.Local);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             with kind value equal to Local,
					             but it had kind value Utc
					             """);
			}

			[Fact]
			public async Task WhenNegated_ShouldUseDoesNotHave()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.DateTimeKindValueOf(s, ExpectationGrammars.None)
						.EqualTo(DateTimeKind.Unspecified));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have kind value equal to Unspecified,
					             but it had kind value Unspecified
					             """);
			}

			[Fact]
			public async Task WhenPluralAndNegated_ShouldUseThePluralVerb()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.DateTimeKindValueOf(s, ExpectationGrammars.Plural)
						.EqualTo(DateTimeKind.Unspecified));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             do not have kind value equal to Unspecified,
					             but it had kind value Unspecified
					             """);
			}
		}
	}
}
