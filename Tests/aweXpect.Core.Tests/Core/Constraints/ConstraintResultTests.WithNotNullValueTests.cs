using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Constraints;

public partial class ConstraintResultTests
{
	public sealed class WithNotNullValueTests
	{
		[Theory]
		[InlineData("normal", "negated", Outcome.Success, false, "normal")]
		[InlineData("normal", "negated", Outcome.Failure, false, "normal")]
		[InlineData("normal", "negated", Outcome.Undecided, false, "normal")]
		[InlineData("normal", "negated", Outcome.Success, true, "negated")]
		[InlineData("normal", "negated", Outcome.Failure, true, "negated")]
		[InlineData("normal", "negated", Outcome.Undecided, true, "negated")]
		public async Task AppendExpectation_ShouldUseExpectedText(
			string expectation, string negatedExpectation, Outcome outcome, bool invert, string expectedText)
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(0,
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

		[Theory]
		[InlineData("normal", "negated", "undecided", Outcome.Success, false, "normal")]
		[InlineData("normal", "negated", "undecided", Outcome.Failure, false, "normal")]
		[InlineData("normal", "negated", "undecided", Outcome.Undecided, false, "undecided")]
		[InlineData("normal", "negated", "undecided", Outcome.Success, true, "negated")]
		[InlineData("normal", "negated", "undecided", Outcome.Failure, true, "negated")]
		[InlineData("normal", "negated", "undecided", Outcome.Undecided, true, "undecided")]
		public async Task AppendResult_ShouldUseExpectedText(
			string result, string negatedResult, string undecidedResult, Outcome outcome, bool invert,
			string expectedText)
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(0,
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

		[Theory]
		[InlineData("normal", "negated", "undecided", Outcome.Success, false)]
		[InlineData("normal", "negated", "undecided", Outcome.Failure, false)]
		[InlineData("normal", "negated", "undecided", Outcome.Undecided, false)]
		[InlineData("normal", "negated", "undecided", Outcome.Success, true)]
		[InlineData("normal", "negated", "undecided", Outcome.Failure, true)]
		[InlineData("normal", "negated", "undecided", Outcome.Undecided, true)]
		public async Task AppendResult_WhenActualIsNull_ShouldUseIsNullText(
			string result, string negatedResult, string undecidedResult, Outcome outcome, bool invert)
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(null,
				result: result,
				negatedResult: negatedResult,
				undecidedResult: undecidedResult,
				it: "my dummy",
				outcome: outcome);
			if (invert)
			{
				sut.Invert();
			}

			string resultText = sut.GetResultText();

			await That(resultText).IsEqualTo("my dummy was <null>");
		}

		[Fact]
		public async Task AppendResult_WhenUndecided_ShouldAppendDefaultResultText()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(0);

			string resultText = sut.GetResultText();

			await That(sut.Outcome).IsEqualTo(Outcome.Undecided);
			await That(resultText).IsEqualTo("it could not be verified, because it was already cancelled");
		}

		[Theory]
		[InlineData(Outcome.Success, Outcome.Failure)]
		[InlineData(Outcome.Failure, Outcome.Success)]
		[InlineData(Outcome.Undecided, Outcome.Undecided)]
		public async Task Invert_ShouldFlipSetOutcome(Outcome initialOutcome, Outcome expectedResult)
		{
			MyWithNotNullValueDummy<int?> sut = new(0, outcome: initialOutcome);
			await That(sut.IsNegatedSet).IsFalse();

			sut.Invert();

			await That(sut.Outcome).IsEqualTo(expectedResult);
			await That(sut.IsNegatedSet).IsTrue();
		}

		[Fact]
		public async Task NormalCase_ShouldNotHaveNegatedGrammarsFlagAndSuccessOutcome()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(0, grammars: ExpectationGrammars.Plural);

			await That(sut.Grammars).HasFlag(ExpectationGrammars.Plural);
			await That(sut.Grammars).DoesNotHaveFlag(ExpectationGrammars.Negated);
		}

		[Fact]
		public async Task ShouldInitializeOutcomeToUndecided()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(0);

			await That(sut.Outcome).IsEqualTo(Outcome.Undecided);
		}

		[Fact]
		public async Task TryGetValue_ShouldExtractValueWhenTypeMatches()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(42);

			bool result = sut.TryGetValue(out int value);

			await That(result).IsTrue();
			await That(value).IsEqualTo(42);
		}

		[Fact]
		public async Task TryGetValue_ShouldReturnFalseWhenTypeDoesNotMatch()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(42);

			bool result = sut.TryGetValue(out bool? value);

			await That(result).IsFalse();
			await That(value).IsNull();
		}

		[Fact]
		public async Task TryGetValue_ShouldReturnFalseWhenTypeIsSupertype()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<MyBaseClass?>(new MyBaseClass());

			bool result = sut.TryGetValue(out MyDerivedClass? value);

			await That(result).IsFalse();
			await That(value).IsNull();
		}

		[Fact]
		public async Task TryGetValue_ShouldReturnTrueWhenTypeIsSubtypeAndValueIsNull()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<MyDerivedClass?>(null);

			bool result = sut.TryGetValue(out MyBaseClass? value);

			await That(result).IsTrue();
			await That(value).IsNull();
		}

		[Fact]
		public async Task TryGetValue_ShouldReturnTrueWhenTypeMatchesAndValueIsNull()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<MyDerivedClass?>(null);

			bool result = sut.TryGetValue(out MyDerivedClass? value);

			await That(result).IsTrue();
			await That(value).IsNull();
		}

		[Fact]
		public async Task WhenActualIsNull_ShouldHaveFailureOutcome()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(null);

			await That(sut.Outcome).IsEqualTo(Outcome.Failure);
		}

		[Fact]
		public async Task WhenActualIsNull_WhenInverted_ShouldHaveFailureOutcome()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(null);

			sut.Invert();

			await That(sut.Outcome).IsEqualTo(Outcome.Failure);
		}

		[Fact]
		public async Task WhenInverted_ShouldHaveNegatedGrammarsFlagAndSuccessOutcome()
		{
			ConstraintResult sut = new MyWithNotNullValueDummy<int?>(0, grammars: ExpectationGrammars.Nested);

			sut = sut.Invert();

			await That(sut.Grammars).HasFlag(ExpectationGrammars.Nested);
			await That(sut.Grammars).HasFlag(ExpectationGrammars.Negated);
		}

		private sealed class MyWithNotNullValueDummy<T> : ConstraintResult.WithNotNullValue<T>
		{
			private readonly string _expectation;
			private readonly string _negatedExpectation;
			private readonly string _negatedResult;
			private readonly string _result;
			private readonly string? _undecidedResult;

			public MyWithNotNullValueDummy(T value,
				string expectation = "",
				string negatedExpectation = "",
				string result = "",
				string negatedResult = "",
				string? undecidedResult = null,
				Outcome? outcome = null,
				string it = "it",
				ExpectationGrammars grammars = ExpectationGrammars.None) : base(it, grammars)
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
	}
}
