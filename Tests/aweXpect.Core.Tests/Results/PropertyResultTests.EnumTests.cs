using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class EnumTests
	{
		[Test]
		public async Task EqualTo_WhenValueIsNull_ShouldFail()
		{
			PropertyResult.Enum<MyClass?, DateTimeKind> sut = MyClass.HasNullDateTimeKindValue();

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
			PropertyResult.Enum<MyClass?, DateTimeKind> sut = MyClass.HasNullDateTimeKindValue();

			async Task Act()
				=> await sut.EqualTo(null);

			await That(Act).DoesNotThrow()
				.Because("null is equal to null");
		}

		[Test]
		public async Task NotEqualTo_WhenValueIsNull_ShouldSucceed()
		{
			PropertyResult.Enum<MyClass?, DateTimeKind> sut = MyClass.HasNullDateTimeKindValue();

			async Task Act()
				=> await sut.NotEqualTo(DateTimeKind.Utc);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task NotEqualTo_WhenValueIsNullAndUnexpectedIsNull_ShouldFail()
		{
			PropertyResult.Enum<MyClass?, DateTimeKind> sut = MyClass.HasNullDateTimeKindValue();

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

		[Test]
		public async Task Satisfying_WhenNegatedAndPredicateHolds_ShouldFail()
		{
			PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind> sut =
				MyClass.HasDateTimeKindValue(DateTimeKind.Utc, ExpectationGrammars.None);

			async Task Act()
				=> await Specified(sut, true);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have kind value specified,
				             but it had kind value Utc
				             """);
		}

		[Test]
		public async Task Satisfying_WhenPredicateDoesNotHold_ShouldFail()
		{
			PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind> sut =
				MyClass.HasDateTimeKindValue(DateTimeKind.Unspecified, ExpectationGrammars.None);

			async Task Act()
				=> await Specified(sut);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has kind value specified,
				             but it had kind value Unspecified
				             """);
		}

		[Test]
		public async Task Satisfying_WhenPredicateHolds_ShouldSucceed()
		{
			PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind> sut =
				MyClass.HasDateTimeKindValue(DateTimeKind.Local, ExpectationGrammars.None);

			async Task Act()
				=> await Specified(sut);

			await That(Act).DoesNotThrow();
		}

		private static TResult Specified<TResult>(IEnumPropertyResult<TResult, DateTimeKind> result,
			bool isNegated = false)
			=> result.Compare(PropertyComparison<DateTimeKind>.Satisfying(
				kind => kind is DateTimeKind.Utc or DateTimeKind.Local, "specified", isNegated));

		public sealed class GrammarTests
		{
			[Test]
			public async Task WhenActive_ShouldUseTheActiveVoice()
			{
				PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind> sut =
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
			public async Task WhenActive_WithNegatedSatisfying_ShouldPlaceTheNegationBeforeTheExpectation()
			{
				PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind> sut =
					MyClass.HasDateTimeKindValue(DateTimeKind.Utc, ExpectationGrammars.Active);

				async Task Act()
					=> await Specified(sut, true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             with kind value not specified,
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
			public async Task WhenNegated_WithNegatedSatisfying_ShouldExpectThePredicate()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s
						=> Specified(MyClass.DateTimeKindValueOf(s, ExpectationGrammars.None), true));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has kind value specified,
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
				PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind> sut =
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
			public async Task WhenNested_WithNegatedSatisfying_ShouldPlaceTheNegationBeforeTheExpectation()
			{
				PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind> sut =
					MyClass.HasDateTimeKindValue(DateTimeKind.Utc, ExpectationGrammars.Nested);

				async Task Act()
					=> await Specified(sut, true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose kind value is not specified,
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
