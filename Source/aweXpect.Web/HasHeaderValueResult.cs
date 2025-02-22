using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The result on a header value.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class HasHeaderValueResult<TType, TThat>
	: AndOrResult<TType, TThat>
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly Func<TType, string?[]?> _headerValueAccessor;
	private readonly TThat _returnValue;

	internal HasHeaderValueResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		Func<TType, string?[]?> headerValueAccessor)
		: base(expectationBuilder, returnValue)
	{
		_expectationBuilder = expectationBuilder;
		_returnValue = returnValue;
		_headerValueAccessor = headerValueAccessor;
	}

	/// <summary>
	///     Verifies that the header has the <paramref name="expected" /> value.
	/// </summary>
	public StringEqualityResult<TType, TThat> WithValue(string? expected)
	{
		StringEqualityOptions options = new();
		_expectationBuilder.And("").AddConstraint((_, _) =>
			new WithHeaderValueConstraint("the value", expected, _headerValueAccessor, options));
		return new StringEqualityResult<TType, TThat>(_expectationBuilder, _returnValue, options);
	}

	/// <summary>
	///     Verifies that the header value satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TType, TThat> WhoseValue(
		Action<IThat<string?>> expectations)
	{
		_expectationBuilder
			.ForMember(
				MemberAccessor<TType, string?>.FromFunc(t =>
				{
					string?[]? values = _headerValueAccessor(t);
					if (values?.Length != 1)
					{
						return null;
					}

					return values[0];
				}, "the value "),
				(member, stringBuilder) => stringBuilder.Append(" whose value "))
			.AddExpectations(e => expectations(new ThatSubject<string?>(e)));
		return this;
	}

	/// <summary>
	///     Verifies that the header values satisfy the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TType, TThat> WhoseValues(
		Action<IThat<string?[]?>> expectations)
	{
		_expectationBuilder
			.ForMember(
				MemberAccessor<TType, string?[]?>.FromFunc(t => _headerValueAccessor(t), "the values "),
				(member, stringBuilder) => stringBuilder.Append(" whose values "))
			.AddExpectations(e => expectations(new ThatSubject<string?[]>(e)), ExpectationGrammars.Nested);
		return this;
	}

	private readonly struct WithHeaderValueConstraint(
		string it,
		string? expected,
		Func<TType, string?[]?> headerValueAccessor,
		StringEqualityOptions options)
		: IValueConstraint<TType?>
	{
		public ConstraintResult IsMetBy(TType? actual)
		{
			if (actual is null)
			{
				return new ConstraintResult.Failure<TType?>(actual, ToString(), "");
			}

			string?[]? headerValues = headerValueAccessor(actual);
			if (headerValues is null)
			{
				return new ConstraintResult.Failure<TType?>(actual, ToString(),
					$"{it} did not contain the expected header");
			}

			if (headerValues.Length != 1)
			{
				return new ConstraintResult.Failure<TType?>(actual, ToString(),
					$"the header contained {headerValues.Length} values");
			}

			string? headerValue = headerValues[0];
			if (options.AreConsideredEqual(headerValue, expected))
			{
				return new ConstraintResult.Success<TType?>(actual, ToString());
			}

			return new ConstraintResult.Failure(ToString(),
				options.GetExtendedFailure(it, headerValue, expected));
		}

		public override string ToString()
			=> $" whose value {options.GetExpectation(expected, ExpectationGrammars.Active)}";
	}
}
