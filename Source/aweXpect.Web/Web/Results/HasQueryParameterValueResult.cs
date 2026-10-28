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
///     The result on a query parameter value.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class HasQueryParameterValueResult
	: AndOrResult<Uri, IThat<Uri?>>
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly string _name;
	private readonly IThat<Uri?> _returnValue;

	internal HasQueryParameterValueResult(
		ExpectationBuilder expectationBuilder,
		IThat<Uri?> returnValue,
		string name)
		: base(expectationBuilder, returnValue)
	{
		_expectationBuilder = expectationBuilder;
		_returnValue = returnValue;
		_name = name;
	}

	/// <summary>
	///     Verifies that the query parameter has exactly one value which is equal to the <paramref name="expected" />
	///     value.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expected" /> value can also be matched as a pattern or as a prefix or suffix, e.g. via
	///     <c>AsWildcard()</c> or <c>AsPrefix()</c>.
	/// </remarks>
	public StringEqualityTypeResult<Uri, IThat<Uri?>> WithValue(string? expected)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		_expectationBuilder.And("").AddConstraint((expected, _name, options),
			static (state, it, grammars) => new WithValueConstraint(
				it, grammars, state.expected, state._name, state.options));
		return new StringEqualityTypeResult<Uri, IThat<Uri?>>(_expectationBuilder, _returnValue, options);
	}

	/// <summary>
	///     Verifies that the values of the query parameter are equal to the <paramref name="expected" /> values in
	///     the order in which they appear in the URI.
	/// </summary>
	public StringCollectionMatchResult<Uri, IThat<Uri?>> WithValues(params string[] expected)
		=> WithValues((IEnumerable<string>)expected);

	/// <summary>
	///     Verifies that the values of the query parameter are equal to the <paramref name="expected" /> values in
	///     the order in which they appear in the URI.
	/// </summary>
	public StringCollectionMatchResult<Uri, IThat<Uri?>> WithValues(IEnumerable<string> expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		string[] expectedValues = expected.ToArray();
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		OptionsHolder<CollectionMatchOptions> matchOptions = new(CollectionMatchOptions.Default);
		_expectationBuilder.And("").AddConstraint((expectedValues, _name, options, matchOptions),
			static (state, it, grammars) => new WithValuesConstraint(
				it, grammars, state.expectedValues, state._name, state.options, state.matchOptions));
		return new StringCollectionMatchResult<Uri, IThat<Uri?>>(_expectationBuilder, _returnValue, options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the query parameter has exactly one value which satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<Uri, IThat<Uri?>> WhoseValue(
		Action<IThatSubject<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		_expectationBuilder
			.ForMember(
				MemberAccessor<Uri, string?>.FromFuncWithName(u =>
				{
					string[]? values = u.GetQueryParameterValues(_name);
					if (values?.Length != 1)
					{
						return null;
					}

					return values[0];
				}, "the value "),
				(_, stringBuilder) => stringBuilder.Append(" whose value "))
			.AddExpectations(expectations);
		return this;
	}

	/// <summary>
	///     Verifies that the values of the query parameter satisfy the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<Uri, IThat<Uri?>> WhoseValues(
		Action<IThatSubject<string[]>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		_expectationBuilder
			.ForMember(
				// The values are also accessed when the query parameter is missing, but then the failed check of the
				// query parameter ignores the result on them, so the empty array cannot change the outcome.
				MemberAccessor<Uri, string[]>.FromFuncWithName(u => u.GetQueryParameterValues(_name) ?? [],
					"the values "),
				(_, stringBuilder) => stringBuilder.Append(" whose values "))
			.AddExpectations(expectations,
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural);
		return this;
	}

	private sealed class WithValueConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expected,
		string name,
		OptionsHolder<StringEqualityOptions> options)
		: ConstraintResult.WithNotNullValue<Uri?>(it, grammars),
			IAsyncContextConstraint<Uri?>
	{
		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;
		private string? _value;
		private string[]? _values;

		public async ValueTask<ConstraintResult> IsMetBy(Uri? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_value = null;
			_values = null;
			if (actual?.IsAbsoluteUri != true)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (!actual.TryGetSingleQueryParameterValue(name, out _value))
			{
				_values = actual.GetQueryParameterValues(name);
				Outcome = Outcome.Failure;
				return this;
			}

			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(_value, expected, nameof(expected));
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options.Value).AppendContexts(contexts);

		/// <remarks>
		///     The text continues the one of the query parameter, which carries the negation and the number of the
		///     subject, like the member expectations of <c>Whose</c> do.
		/// </remarks>
		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(" whose value ");
			stringBuilder.Append(options.Value.GetExpectation(expected, ExpectationGrammars.Active));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_values is { Length: not 1, })
			{
				stringBuilder.Append(It).Append(" had ").Append(_values.Length).Append(" query parameter values ");
				Formatter.Format(stringBuilder, _values);
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
			=> stringBuilder.Append((_evaluationOptions ?? options.Value).GetExtendedMemberFailure(It,
				"query parameter value", Grammars, _value, expected, _matchResult));
	}

	private sealed class WithValuesConstraint(
		string it,
		ExpectationGrammars grammars,
		string[] expected,
		string name,
		OptionsHolder<StringEqualityOptions> options,
		OptionsHolder<CollectionMatchOptions> matchOptions)
		: ConstraintResult.WithNotNullValue<Uri?>(it, grammars),
			IAsyncContextConstraint<Uri?>
	{
		private StringEqualityOptions? _evaluationOptions;
		private string? _failure;
		private string[]? _values;

		public async ValueTask<ConstraintResult> IsMetBy(Uri? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_failure = null;
			if (actual?.IsAbsoluteUri != true)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_values = actual.GetQueryParameterValues(name);
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			if (_values is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			(Outcome, _failure) = await matchOptions.Value.GetCollectionMatcher<string?, string?>(expected)
				.VerifyItems(CollectionMatchFailure.Subject, _values, _evaluationOptions, cancellationToken);
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options.Value).AppendContexts(contexts);

		/// <remarks>
		///     The text continues the one of the query parameter, which carries the negation and the number of the
		///     subject, like the member expectations of <c>Whose</c> do.
		/// </remarks>
		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(" whose values ").Append(matchOptions.Value.GetExpectation(
				Formatter.Format(expected, FormattingOptions.SingleLine) + options, ExpectationGrammars.Plural));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had query parameter values ");
			Formatter.Format(stringBuilder, _values);
			CollectionMatchFailure.AppendTo(stringBuilder, _failure, indentation);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);
	}
}
