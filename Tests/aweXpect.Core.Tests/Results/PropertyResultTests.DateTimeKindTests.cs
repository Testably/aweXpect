using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class DateTimeKindTests
	{
		[Test]
		public async Task EqualTo_WhenValueIsNull_ShouldFail()
		{
			PropertyResult.DateTimeKind<MyClass?> sut = MyClass.HasNullDateTimeKindValue();

			async Task Act()
				=> await sut.EqualTo(DateTimeKind.Utc);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has kind value equal to Utc,
				             but it had kind value <null>
				             """);
		}

		[Test]
		public async Task EqualTo_WhenValueIsNullAndExpectedIsNull_ShouldSucceed()
		{
			PropertyResult.DateTimeKind<MyClass?> sut = MyClass.HasNullDateTimeKindValue();

			async Task Act()
				=> await sut.EqualTo(null);

			await That(Act).DoesNotThrow()
				.Because("null is equal to null");
		}

		[Test]
		public async Task NotEqualTo_WhenValueIsNull_ShouldSucceed()
		{
			PropertyResult.DateTimeKind<MyClass?> sut = MyClass.HasNullDateTimeKindValue();

			async Task Act()
				=> await sut.NotEqualTo(DateTimeKind.Utc);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task NotEqualTo_WhenValueIsNullAndUnexpectedIsNull_ShouldFail()
		{
			PropertyResult.DateTimeKind<MyClass?> sut = MyClass.HasNullDateTimeKindValue();

			async Task Act()
				=> await sut.NotEqualTo(null);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have kind value equal to <null>,
				             but it had kind value <null>
				             """)
				.Because("null is equal to null");
		}

		public sealed class GrammarTests
		{
			[Test]
			public async Task WhenActive_ShouldUseTheActiveVoice()
			{
				PropertyResult.DateTimeKind<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasDateTimeKindValue(DateTimeKind.Utc, ExpectationGrammars.Active);

				async Task Act()
					=> await sut.EqualTo(DateTimeKind.Local);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             with kind value equal to Local,
					             but it had kind value Utc
					             """);
			}

			[Test]
			public async Task WhenNegated_ShouldUseDoesNotHave()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.DateTimeKindValueOf(s, ExpectationGrammars.None)
						.EqualTo(DateTimeKind.Unspecified));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have kind value equal to Unspecified,
					             but it had kind value Unspecified
					             """);
			}

			[Test]
			public async Task WhenNegated_WithNotEqualTo_ShouldExpectEquality()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.DateTimeKindValueOf(s, ExpectationGrammars.None)
						.NotEqualTo(DateTimeKind.Utc));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has kind value equal to Utc,
					             but it had kind value Unspecified
					             """);
			}

			[Test]
			public async Task WhenNested_ShouldNameTheProperty()
			{
				PropertyResult.DateTimeKind<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasDateTimeKindValue(DateTimeKind.Utc, ExpectationGrammars.Nested);

				async Task Act()
					=> await sut.EqualTo(DateTimeKind.Local);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose kind value is equal to Local,
					             but kind value was Utc
					             """);
			}

			[Test]
			public async Task WhenPluralAndNegated_ShouldUseThePluralVerb()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.DateTimeKindValueOf(s, ExpectationGrammars.Plural)
						.EqualTo(DateTimeKind.Unspecified));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             do not have kind value equal to Unspecified,
					             but it had kind value Unspecified
					             """);
			}
		}
	}
}
