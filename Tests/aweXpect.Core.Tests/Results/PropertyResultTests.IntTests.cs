using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class IntTests
	{
		[Fact]
		public async Task Between_ShouldTriggerValidationForMaximum()
		{
			Signaler<int?> signal = new();
			PropertyResult.Int<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Between(42).And(43);

			await That(signal).Signaled().With(e => e == 43);
		}

		[Fact]
		public async Task Between_ShouldTriggerValidationForMinimum()
		{
			Signaler<int?> signal = new();
			PropertyResult.Int<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Between(42).And(43);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task Between_ShouldVerifyThatActualIsBetweenMinimumAndMaximum()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValue(42);

			MyClass? result = await sut.Between(41).And(43);

			await That(result?.IntValue).IsEqualTo(42);
		}

		[Fact]
		public async Task Between_WhenActualIsOutsideTheRange_ShouldFail()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValue(42);

			async Task Act()
				=> await sut.Between(43).And(44);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has int value between 43 and 44,
				             but it had int value 42
				             """);
		}

		[Fact]
		public async Task Between_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValueOfNullSubject();

			async Task Act()
				=> await sut.Between(41).And(43);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has int value between 41 and 43,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task EqualTo_ShouldTriggerValidation()
		{
			Signaler<int?> signal = new();
			PropertyResult.Int<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.EqualTo(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task EqualTo_ShouldVerifyThatActualIsEqualToExpected()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValue(42);

			MyClass? result = await sut.EqualTo(42);

			await That(result?.IntValue).IsEqualTo(42);
		}

		[Fact]
		public async Task EqualTo_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValueOfNullSubject();

			async Task Act()
				=> await sut.EqualTo(42);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has int value equal to 42,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task GreaterThan_ShouldTriggerValidation()
		{
			Signaler<int?> signal = new();
			PropertyResult.Int<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.GreaterThan(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task GreaterThan_ShouldVerifyThatActualIsGreaterThanExpected()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValue(42);

			MyClass? result = await sut.GreaterThan(41);

			await That(result?.IntValue).IsEqualTo(42);
		}

		[Fact]
		public async Task GreaterThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<int?> signal = new();
			PropertyResult.Int<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.GreaterThanOrEqualTo(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task GreaterThanOrEqualTo_ShouldVerifyThatActualIsGreaterThanOrEqualToExpected()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValue(42);

			MyClass? result = await sut.GreaterThanOrEqualTo(42);

			await That(result?.IntValue).IsEqualTo(42);
		}

		[Fact]
		public async Task LessThan_ShouldTriggerValidation()
		{
			Signaler<int?> signal = new();
			PropertyResult.Int<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.LessThan(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task LessThan_ShouldVerifyThatActualIsLessThanExpected()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValue(42);

			MyClass? result = await sut.LessThan(43);

			await That(result?.IntValue).IsEqualTo(42);
		}

		[Fact]
		public async Task LessThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<int?> signal = new();
			PropertyResult.Int<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.LessThanOrEqualTo(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task LessThanOrEqualTo_ShouldVerifyThatActualIsLessThanOrEqualToExpected()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValue(42);

			MyClass? result = await sut.LessThanOrEqualTo(42);

			await That(result?.IntValue).IsEqualTo(42);
		}

		[Fact]
		public async Task NotEqualTo_ShouldTriggerValidation()
		{
			Signaler<int?> signal = new();
			PropertyResult.Int<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotEqualTo(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task NotEqualTo_ShouldVerifyThatActualIsNotEqualToExpected()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValue(42);

			MyClass? result = await sut.NotEqualTo(43);

			await That(result?.IntValue).IsEqualTo(42);
		}

		[Fact]
		public async Task NotEqualTo_WhenSubjectIsNullAndUnexpectedIsNull_ShouldFail()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValueOfNullSubject();

			async Task Act()
				=> await sut.NotEqualTo(null);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has int value not equal to <null>,
				             but it was <null>
				             """)
				.Because("a null subject has no int value to compare, whatever the unexpected value is");
		}

		[Fact]
		public async Task NotEqualTo_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Int<MyClass?> sut = MyClass.HasIntValueOfNullSubject();

			async Task Act()
				=> await sut.NotEqualTo(42);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has int value not equal to 42,
				             but it was <null>
				             """);
		}

		public sealed class GrammarTests
		{
			[Fact]
			public async Task WhenActive_ShouldUseTheActiveVoice()
			{
				PropertyResult.Int<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasIntValue(42, ExpectationGrammars.Active);

				async Task Act()
					=> await sut.GreaterThan(43);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             with int value greater than 43,
					             but it had int value 42
					             """);
			}

			[Fact]
			public async Task WhenActiveAndNegated_ShouldNegateTheComparison()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.IntValueOf(s, ExpectationGrammars.Active)
						.EqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             with int value not equal to 0,
					             but it had int value 0
					             """);
			}

			[Fact]
			public async Task WhenActiveAndNegated_WithNotEqualTo_ShouldExpectEquality()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.IntValueOf(s, ExpectationGrammars.Active)
						.NotEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             with int value equal to 1,
					             but it had int value 0
					             """);
			}

			[Fact]
			public async Task WhenNegated_ShouldUseDoesNotHave()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.IntValueOf(s, ExpectationGrammars.None)
						.EqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have int value equal to 0,
					             but it had int value 0
					             """);
			}

			[Fact]
			public async Task WhenNegated_WithNotEqualTo_ShouldExpectEquality()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.IntValueOf(s, ExpectationGrammars.None)
						.NotEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has int value equal to 1,
					             but it had int value 0
					             """);
			}

			[Fact]
			public async Task WhenNested_ShouldReadAsAStatementAboutTheProperty()
			{
				PropertyResult.Int<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasIntValue(42, ExpectationGrammars.Nested);

				async Task Act()
					=> await sut.GreaterThan(43);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose int value is greater than 43,
					             but it had int value 42
					             """);
			}

			[Fact]
			public async Task WhenNestedAndNegated_ShouldNegateTheComparison()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.IntValueOf(s, ExpectationGrammars.Nested)
						.EqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose int value is not equal to 0,
					             but it had int value 0
					             """);
			}

			[Fact]
			public async Task WhenNestedAndNegated_WithNotEqualTo_ShouldExpectEquality()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.IntValueOf(s, ExpectationGrammars.Nested)
						.NotEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose int value is equal to 1,
					             but it had int value 0
					             """);
			}

			[Fact]
			public async Task WhenPlural_ShouldUseThePluralVerb()
			{
				PropertyResult.Int<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasIntValue(42, ExpectationGrammars.Plural);

				async Task Act()
					=> await sut.GreaterThan(43);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             have int value greater than 43,
					             but it had int value 42
					             """);
			}

			[Fact]
			public async Task WhenPluralAndNegated_ShouldUseThePluralVerb()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.IntValueOf(s, ExpectationGrammars.Plural)
						.EqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             do not have int value equal to 0,
					             but it had int value 0
					             """);
			}

			[Fact]
			public async Task WhenPluralAndNegated_WithNotEqualTo_ShouldExpectEquality()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.IntValueOf(s, ExpectationGrammars.Plural)
						.NotEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             have int value equal to 1,
					             but it had int value 0
					             """);
			}
		}

		public sealed class NarrowedTypeTests
		{
			[Fact]
			public async Task WhenTheMapperIsTypedAtTheBaseType_ShouldFailForAMismatch()
			{
				async Task Act()
					=> await MyClass.HasIntValueOfNarrowedSubject(42).EqualTo(43);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has int value equal to 43,
					             but it had int value 42
					             """)
					.Because("a constraint typed at the narrowed type would silently never be matched");
			}

			[Fact]
			public async Task WhenTheMapperIsTypedAtTheBaseType_ShouldReturnTheNarrowedType()
			{
				MyDerivedClass? result = await MyClass.HasIntValueOfNarrowedSubject(42).EqualTo(42);

				await That(result?.IntValue).IsEqualTo(42);
			}
		}
	}
}
