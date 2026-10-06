using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Constraints;

public partial class ConstraintResultTests
{
	public sealed class WithValueTests
	{
		[Test]
		[Arguments("normal", "negated", Outcome.Success, false, "normal")]
		[Arguments("normal", "negated", Outcome.Failure, false, "normal")]
		[Arguments("normal", "negated", Outcome.Undecided, false, "normal")]
		[Arguments("normal", "negated", Outcome.Success, true, "negated")]
		[Arguments("normal", "negated", Outcome.Failure, true, "negated")]
		[Arguments("normal", "negated", Outcome.Undecided, true, "negated")]
		public async Task AppendExpectation_ShouldUseExpectedText(
			string expectation, string negatedExpectation, Outcome outcome, bool invert, string expectedText)
		{
			ConstraintResult sut = new MyWithValueDummy<int>(0,
				expectation,
				negatedExpectation,
				outcome: outcome);
			if (invert)
			{
				sut.Invert();
			}

			string expectationText = sut.GetExpectationText();

			await That(expectationText).IsEqualTo(expectedText);
		}

		[Test]
		[Arguments("normal", "negated", "undecided", Outcome.Success, false, "normal")]
		[Arguments("normal", "negated", "undecided", Outcome.Failure, false, "normal")]
		[Arguments("normal", "negated", "undecided", Outcome.Undecided, false, "undecided")]
		[Arguments("normal", "negated", "undecided", Outcome.Success, true, "negated")]
		[Arguments("normal", "negated", "undecided", Outcome.Failure, true, "negated")]
		[Arguments("normal", "negated", "undecided", Outcome.Undecided, true, "undecided")]
		public async Task AppendResult_ShouldUseExpectedText(
			string result, string negatedResult, string undecidedResult, Outcome outcome, bool invert,
			string expectedText)
		{
			ConstraintResult sut = new MyWithValueDummy<int>(0,
				result: result,
				negatedResult: negatedResult,
				undecidedResult: undecidedResult,
				outcome: outcome);
			if (invert)
			{
				sut.Invert();
			}

			string resultText = sut.GetResultText();

			await That(resultText).IsEqualTo(expectedText);
		}

		[Test]
		public async Task AppendResult_WhenUndecided_ShouldAppendDefaultResultText()
		{
			ConstraintResult sut = new MyWithValueDummy<int>(0);

			string resultText = sut.GetResultText();

			await That(sut.Outcome).IsEqualTo(Outcome.Undecided);
			await That(resultText).IsEqualTo("it could not be verified, because the evaluation was already canceled");
		}

		[Test]
		public async Task AppendResult_WhenUndecided_ShouldStartWithIt()
		{
			ConstraintResult sut = new MyWithValueWithItDummy("Items");

			string resultText = sut.GetResultText();

			await That(sut.Outcome).IsEqualTo(Outcome.Undecided);
			await That(resultText).IsEqualTo("Items could not be verified, because the evaluation was already canceled");
		}

		[Test]
		[Arguments(Outcome.Success, Outcome.Failure)]
		[Arguments(Outcome.Failure, Outcome.Success)]
		[Arguments(Outcome.Undecided, Outcome.Undecided)]
		public async Task Invert_ShouldFlipSetOutcome(Outcome initialOutcome, Outcome expectedResult)
		{
			MyWithValueDummy<int> sut = new(0, outcome: initialOutcome);
			await That(sut.IsNegatedSet).IsFalse();

			sut.Invert();

			await That(sut.Outcome).IsEqualTo(expectedResult);
			await That(sut.IsNegatedSet).IsTrue();
		}

		[Test]
		public async Task IsNegated_ShouldFollowTheNegatedGrammars()
		{
			MyWithValueDummy<int> sut = new(0, outcome: Outcome.Success, grammars: ExpectationGrammars.Negated);

			await That(sut.IsNegatedSet).IsTrue();
			await That(sut.Outcome).IsEqualTo(Outcome.Failure)
				.Because("the outcome and the expectation text agree about the negation");
		}

		[Test]
		public async Task NormalCase_ShouldNotHaveNegatedGrammarsFlagAndSuccessOutcome()
		{
			ConstraintResult sut = new MyWithValueDummy<int>(0, grammars: ExpectationGrammars.Plural);

			await That(sut.Grammars).HasFlag(ExpectationGrammars.Plural);
			await That(sut.Grammars).DoesNotHaveFlag(ExpectationGrammars.Negated);
		}

		[Test]
		public async Task Outcome_WhenActualIsNull_ShouldKeepTheSetOutcome()
		{
			ConstraintResult sut = new MyWithValueDummy<int?>(null, outcome: Outcome.Success);

			await That(sut.Outcome).IsEqualTo(Outcome.Success)
				.Because("WithValue applies no null policy of its own");
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task Outcome_WhenFailureBothWays_ShouldBeKeptUnderNegation(bool invert)
		{
			ConstraintResult sut = new MyWithValueDummy<int>(0, outcome: Outcome.FailureBothWays);
			if (invert)
			{
				sut.Invert();
			}

			await That(sut.Outcome).IsEqualTo(Outcome.FailureBothWays);
		}

		[Test]
		public async Task ShouldInitializeOutcomeToUndecided()
		{
			ConstraintResult sut = new MyWithValueDummy<int>(0);

			await That(sut.Outcome).IsEqualTo(Outcome.Undecided);
		}

		[Test]
		public async Task TryGetStoredValue_ShouldReturnTrueWhenTypeIsSubtypeAndValueIsNull()
		{
			ConstraintResult sut = new MyWithValueDummy<MyDerivedClass?>(null);

			bool result = sut.TryGetStoredValue(out MyBaseClass? value);

			await That(result).IsTrue();
			await That(value).IsNull();
		}

		[Test]
		public async Task TryGetStoredValue_ShouldReturnTrueWhenTypeMatchesAndValueIsNull()
		{
			ConstraintResult sut = new MyWithValueDummy<MyDerivedClass?>(null);

			bool result = sut.TryGetStoredValue(out MyDerivedClass? value);

			await That(result).IsTrue();
			await That(value).IsNull();
		}

		[Test]
		public async Task TryGetValue_ShouldExtractValueWhenTypeMatches()
		{
			ConstraintResult sut = new MyWithValueDummy<int>(42);

			bool result = sut.TryGetValue(out int value);

			await That(result).IsTrue();
			await That(value).IsEqualTo(42);
		}

		[Test]
		public async Task TryGetValue_ShouldReturnFalseWhenTypeDoesNotMatch()
		{
			ConstraintResult sut = new MyWithValueDummy<int>(42);

			bool result = sut.TryGetValue(out bool? value);

			await That(result).IsFalse();
			await That(value).IsNull();
		}

		[Test]
		public async Task TryGetValue_ShouldReturnFalseWhenTypeIsSupertype()
		{
			ConstraintResult sut = new MyWithValueDummy<MyBaseClass?>(new MyBaseClass());

			bool result = sut.TryGetValue(out MyDerivedClass? value);

			await That(result).IsFalse();
			await That(value).IsNull();
		}

		[Test]
		public async Task TryGetValue_ShouldReturnFalseWhenValueIsNull()
		{
			ConstraintResult sut = new MyWithValueDummy<MyDerivedClass?>(null);

			bool result = sut.TryGetValue(out MyDerivedClass? value);

			await That(result).IsFalse().Because("TryGetValue only returns true for a value that is not null");
			await That(value).IsNull();
		}

		[Test]
		public async Task WhenInverted_ShouldHaveNegatedGrammarsFlagAndSuccessOutcome()
		{
			ConstraintResult sut = new MyWithValueDummy<int>(0, grammars: ExpectationGrammars.Nested);

			sut = sut.Invert();

			await That(sut.Grammars).HasFlag(ExpectationGrammars.Nested);
			await That(sut.Grammars).HasFlag(ExpectationGrammars.Negated);
		}

		private sealed class MyWithValueDummy<T> : ConstraintResult.WithValue<T>
		{
			private readonly string _expectation;
			private readonly string _negatedExpectation;
			private readonly string _negatedResult;
			private readonly string _result;
			private readonly string? _undecidedResult;

			public MyWithValueDummy(T value,
				string expectation = "",
				string negatedExpectation = "",
				string result = "",
				string negatedResult = "",
				string? undecidedResult = null,
				Outcome? outcome = null,
				ExpectationGrammars grammars = ExpectationGrammars.None) : base("it", grammars)
			{
				_expectation = expectation;
				_negatedExpectation = negatedExpectation;
				_result = result;
				_negatedResult = negatedResult;
				_undecidedResult = undecidedResult;
				if (outcome.HasValue)
				{
					Outcome = outcome.Value;
				}

				Actual = value;
			}

			public bool IsNegatedSet => IsNegated;

			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> stringBuilder.Append(_expectation);

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
				=> stringBuilder.Append(_result);

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> stringBuilder.Append(_negatedExpectation);

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
				=> stringBuilder.Append(_negatedResult);

			protected override void AppendUndecidedResult(StringBuilder stringBuilder, string? indentation = null)
			{
				if (_undecidedResult is not null)
				{
					stringBuilder.Append(_undecidedResult);
					return;
				}

				base.AppendUndecidedResult(stringBuilder, indentation);
			}
		}

		private sealed class MyWithValueWithItDummy(string it)
			: ConstraintResult.WithValue<int>(it, ExpectationGrammars.None)
		{
			protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
			}

			protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			{
			}

			protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			{
			}

			protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			{
			}
		}
	}
}
