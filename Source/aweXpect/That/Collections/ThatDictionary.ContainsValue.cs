using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatDictionary
{
	private const string ContainsValueSummary =
		"Verifies that the dictionary contains the <paramref name=\"expected\" /> value.";

	private const string DoesNotContainValueSummary =
		"Verifies that the dictionary does not contain the <paramref name=\"unexpected\" /> value.";

	[CreateExpectationFamily("ContainsValue", NegatedName = "DoesNotContainValue", PerSubject = true,
		GuaranteesNotNull = true, Summary = ContainsValueSummary, NegatedSummary = DoesNotContainValueSummary)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>
		ContainsValueCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			TValue expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		OptionsSlot<ObjectMatchType<TValue>> options = new(ObjectToleranceMatchTypeFactory.ForValuesOf<TValue>());
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>(
			expectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars) =>
					new ContainValueConstraint<TCollection, TKey, TValue>(it, grammars, state.Negated ? "unexpected" : "expected",
						state.Expected, state.Options).InvertIf(state.Negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("ContainsValue", NegatedName = "DoesNotContainValue", PerSubject = true,
		Factory = typeof(ObjectToleranceMatchTypeFactory),
		GuaranteesNotNull = true, Summary = ContainsValueSummary, NegatedSummary = DoesNotContainValueSummary)]
	internal static ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection?>, TValue, TTolerance>
		ContainsValueWithToleranceCore<TCollection, TKey, TValue, TTolerance>(
			IThat<TCollection?> subject,
			TValue expected,
			ObjectToleranceMatchType<TValue, TTolerance> initialOptions,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		OptionsSlot<ObjectMatchType<TValue>> options = new(initialOptions);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection?>, TValue, TTolerance>(
			expectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars) =>
					new ContainValueConstraint<TCollection, TKey, TValue>(it, grammars, state.Negated ? "unexpected" : "expected",
						state.Expected, state.Options).InvertIf(state.Negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("ContainsValue", NegatedName = "DoesNotContainValue", PerSubject = true,
		GuaranteesNotNull = true, Summary = ContainsValueSummary, NegatedSummary = DoesNotContainValueSummary)]
	internal static StringEqualityTypeResult<TCollection, IThat<TCollection?>>
		ContainsValueForStringsCore<TCollection, TKey>(
			IThat<TCollection?> subject,
			string? expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, string?>>
	{
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars) =>
					new ContainValueConstraint<TCollection, TKey, string?>(it, grammars, state.Negated ? "unexpected" : "expected",
						state.Expected, state.Options).InvertIf(state.Negated)),
			subject,
			options);
	}

	private sealed class ContainValueConstraint<TDictionary, TKey, TValue>(
		string it,
		ExpectationGrammars grammars,
		string parameterName,
		TValue expected,
		IReadOnlyOptionsSlot<IEqualityOptions<TValue>> options)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IAsyncContextConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private IEqualityEvaluator<TValue>? _evaluationOptions;

		/// <remarks>
		///     The expected value is validated first, so that an unusable pattern is rejected whichever entries the
		///     subject has.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(TDictionary? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			options.ValidateExpectedItems([expected,], parameterName);
			_evaluationOptions = options.ForEvaluation(context, cancellationToken);
			Outcome = actual is not null && await ContainsValue(actual, expected, _evaluationOptions)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			AddDictionaryContext(contexts, Actual);
			contexts.AddEqualityOptionsContexts(_evaluationOptions ?? options.WithoutEvaluation());
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains value ", "contain value "));
			stringBuilder.Append(Formatter.Format(expected).Indent(indentation, false));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain value ");
			stringBuilder.Append(Formatter.Format(expected).Indent(indentation, false));
			stringBuilder.AppendClosestMatchHint(expected, GetValuesForHint(Actual, options));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain value ", "do not contain value "));
			stringBuilder.Append(Formatter.Format(expected).Indent(indentation, false));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
