using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Web.Results;

/// <summary>
///     The result on a header value.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class HasHeaderValueResult<TType, TThat>
	: AndOrResult<TType, TThat>
	where TType : class
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly Func<TType, string[]?> _headerValueAccessor;
	private readonly TThat _returnValue;

	internal HasHeaderValueResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		Func<TType, string[]?> headerValueAccessor)
		: base(expectationBuilder, returnValue)
	{
		_expectationBuilder = expectationBuilder;
		_returnValue = returnValue;
		_headerValueAccessor = headerValueAccessor;
	}

	/// <summary>
	///     Verifies that the header has the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expected" /> value can also be matched as a pattern or as a prefix or suffix, e.g. via
	///     <c>AsWildcard()</c> or <c>AsPrefix()</c>.
	/// </remarks>
	public StringEqualityTypeResult<TType, TThat> WithValue(string? expected)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		_expectationBuilder.And("").AddConstraint((expected, _headerValueAccessor, options),
			static (state, it, grammars) => new WithHeaderValueConstraint(
				it, grammars, state.expected, state._headerValueAccessor, state.options));
		return new StringEqualityTypeResult<TType, TThat>(_expectationBuilder, _returnValue, options);
	}

	/// <summary>
	///     Verifies that the header has exactly the <paramref name="expected" /> values in the same order.
	/// </summary>
	public StringCollectionMatchResult<TType, TThat> WithValues(params string[] expected)
		=> WithValues((IEnumerable<string>)expected);

	/// <summary>
	///     Verifies that the header has exactly the <paramref name="expected" /> values in the same order.
	/// </summary>
	public StringCollectionMatchResult<TType, TThat> WithValues(IEnumerable<string> expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionMatchOptions> matchOptions = new(CollectionMatchOptions.Default);
		_expectationBuilder.And("").AddConstraint(
			(expected: expected.ToArray(), _headerValueAccessor, options, matchOptions),
			static (state, it, grammars) => new WithHeaderValuesConstraint(
				it, grammars, state.expected, state._headerValueAccessor, state.options, state.matchOptions));
		return new StringCollectionMatchResult<TType, TThat>(_expectationBuilder, _returnValue, options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the header value satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TType, TThat> WhoseValue(
		Action<IThatSubject<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		_expectationBuilder.AddSingleValueExpectations(_headerValueAccessor, "header values", expectations);
		return this;
	}

	/// <summary>
	///     Verifies that the header values satisfy the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TType, TThat> WhoseValues(
		Action<IThatSubject<string[]>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		_expectationBuilder
			.ForMember(
				// The values are also accessed when the header is missing, but then the failed header check ignores
				// the result on them, so the empty array cannot change the outcome.
				MemberAccessor<TType, string[]>.FromFuncWithName(t => _headerValueAccessor(t) ?? [], "the values "),
				(_, stringBuilder) => stringBuilder.Append(" whose values "))
			.AddExpectations(expectations,
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural);
		return this;
	}

	private sealed class WithHeaderValueConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expected,
		Func<TType, string?[]?> headerValueAccessor,
		OptionsHolder<StringEqualityOptions> options)
		: ConstraintResult.WithNotNullValue<TType?>(it, grammars),
			IAsyncContextConstraint<TType?>
	{
		private StringEqualityOptions? _evaluationOptions;
		private string?[]? _headerValues;
		private MatchResult _matchResult;

		public async ValueTask<ConstraintResult> IsMetBy(TType? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_headerValues = headerValueAccessor(actual);
			if (_headerValues is null || _headerValues.Length != 1)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			string? headerValue = _headerValues[0];
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(headerValue, expected, nameof(expected));
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options.Value).AppendContexts(contexts);

		/// <remarks>
		///     The text continues the one of the header, which carries the negation and the number of the subject, like
		///     the member expectations of <c>Whose</c> do.
		/// </remarks>
		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(" whose value ");
			stringBuilder.Append(options.Value.GetExpectation(expected, ExpectationGrammars.Active));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_headerValues is null)
			{
				stringBuilder.Append(It).Append(" did not contain the expected header");
			}
			else if (_headerValues.Length != 1)
			{
				stringBuilder.Append(It).Append(" had ").Append(_headerValues.Length).Append(" header values ");
				Formatter.Format(stringBuilder, _headerValues);
			}
			else
			{
				AppendValueFailure(stringBuilder);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendValueFailure(stringBuilder);

		private void AppendValueFailure(StringBuilder stringBuilder)
			=> stringBuilder.Append((_evaluationOptions ?? options.Value).GetExtendedMemberFailure(It, "header value",
				Grammars, _headerValues?[0], expected, _matchResult));
	}

	private sealed class WithHeaderValuesConstraint(
		string it,
		ExpectationGrammars grammars,
		string[] expected,
		Func<TType, string?[]?> headerValueAccessor,
		OptionsHolder<StringEqualityOptions> options,
		OptionsHolder<CollectionMatchOptions> matchOptions)
		: ConstraintResult.WithNotNullValue<TType?>(it, grammars),
			IAsyncContextConstraint<TType?>
	{
		private StringEqualityOptions? _evaluationOptions;
		private string? _failure;
		private string?[]? _headerValues;

		public async ValueTask<ConstraintResult> IsMetBy(TType? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_failure = null;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_headerValues = headerValueAccessor(actual);
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			if (_headerValues is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			(Outcome, _failure) = await matchOptions.Value.GetCollectionMatcher<string?, string?>(expected)
				.VerifyItems(CollectionMatchFailure.Subject, _headerValues, _evaluationOptions, cancellationToken);
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options.Value).AppendContexts(contexts);

		/// <remarks>
		///     The text continues the one of the header, which carries the negation, like <c>WithValue</c> does.
		/// </remarks>
		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(" whose values ").Append(matchOptions.Value.GetExpectation(
				Formatter.Format(expected, FormattingOptions.SingleLine) + options, ExpectationGrammars.Plural));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_headerValues is null)
			{
				stringBuilder.Append(It).Append(" did not contain the expected header");
			}
			else
			{
				AppendValuesFailure(stringBuilder);
				CollectionMatchFailure.AppendTo(stringBuilder, _failure, indentation);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendValuesFailure(stringBuilder);

		private void AppendValuesFailure(StringBuilder stringBuilder)
		{
			stringBuilder.Append(It).Append(" had header values ");
			Formatter.Format(stringBuilder, _headerValues);
		}
	}
}
