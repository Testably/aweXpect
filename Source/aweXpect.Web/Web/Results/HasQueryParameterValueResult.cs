using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
	public StringEqualityResult<Uri, IThat<Uri?>> WithValue(string? expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		_expectationBuilder.And("").AddConstraint((expected, _name, options),
			static (state, it, grammars) => new WithValueConstraint(
				it, grammars, state.expected, state._name, state.options));
		return new StringEqualityResult<Uri, IThat<Uri?>>(_expectationBuilder, _returnValue, options);
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
		StringEqualityOptions options = new(nameof(expected));
		CollectionMatchOptions matchOptions = new();
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
		Action<IThat<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		_expectationBuilder
			.ForMember(
				MemberAccessor<Uri, string?>.FromFunc(u =>
				{
					string[]? values = u.GetQueryParameterValues(_name);
					if (values?.Length != 1)
					{
						return null;
					}

					return values[0];
				}, "the value "),
				(_, stringBuilder) => stringBuilder.Append(" whose value "))
			.AddExpectations(e => expectations(new ThatSubject<string?>(e)));
		return this;
	}

	/// <summary>
	///     Verifies that the values of the query parameter satisfy the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<Uri, IThat<Uri?>> WhoseValues(
		Action<IThat<string?[]?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		_expectationBuilder
			.ForMember(
				MemberAccessor<Uri, string?[]?>.FromFunc(u => u.GetQueryParameterValues(_name), "the values "),
				(_, stringBuilder) => stringBuilder.Append(" whose values "))
			.AddExpectations(e => expectations(new ThatSubject<string?[]>(e)),
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural);
		return this;
	}

	private sealed class WithValueConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expected,
		string name,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<Uri?>(it, grammars),
			IAsyncConstraint<Uri?>
	{
		private string[]? _values;

		public async ValueTask<ConstraintResult> IsMetBy(Uri? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual?.IsAbsoluteUri != true)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_values = actual.GetQueryParameterValues(name);
			if (_values is null || _values.Length != 1)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = await options.AreConsideredEqual(_values[0], expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <remarks>
		///     The text continues the one of the query parameter, which carries the negation and the number of the
		///     subject, like the member expectations of <c>Whose</c> do.
		/// </remarks>
		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(" whose value ");
			stringBuilder.Append(options.GetExpectation(expected, ExpectationGrammars.Active));
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
			=> stringBuilder.Append(options.GetExtendedMemberFailure(It, "query parameter value", Grammars,
				_values?[0], expected));
	}

	private sealed class WithValuesConstraint(
		string it,
		ExpectationGrammars grammars,
		string[] expected,
		string name,
		StringEqualityOptions options,
		CollectionMatchOptions matchOptions)
		: ConstraintResult.WithNotNullValue<Uri?>(it, grammars),
			IAsyncConstraint<Uri?>
	{
		private string[]? _values;

		public async ValueTask<ConstraintResult> IsMetBy(Uri? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual?.IsAbsoluteUri != true)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_values = actual.GetQueryParameterValues(name);
			Outcome = _values is not null && await matchOptions.Matches(It, _values, expected, options)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <remarks>
		///     The text continues the one of the query parameter, which carries the negation and the number of the
		///     subject, like the member expectations of <c>Whose</c> do.
		/// </remarks>
		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(" whose values ").Append(matchOptions.GetExpectation(
				Formatter.Format(expected, FormattingOptions.SingleLine) + options, ExpectationGrammars.Plural));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had query parameter values ");
			Formatter.Format(stringBuilder, _values);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
