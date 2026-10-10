using System;
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
		StringEqualityOptions options = new(nameof(expected));
		_expectationBuilder.And("").AddConstraint((expected, _headerValueAccessor, options),
			static (state, it, grammars) => new WithHeaderValueConstraint(
				it, grammars, state.expected, state._headerValueAccessor, state.options));
		return new StringEqualityResult<TType, TThat>(_expectationBuilder, _returnValue, options);
	}

	/// <summary>
	///     Verifies that the header value satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TType, TThat> WhoseValue(
		Action<IThat<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
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
				(_, stringBuilder) => stringBuilder.Append(" whose value "))
			.AddExpectations(e => expectations(new ThatSubject<string?>(e)));
		return this;
	}

	/// <summary>
	///     Verifies that the header values satisfy the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TType, TThat> WhoseValues(
		Action<IThat<string?[]?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		_expectationBuilder
			.ForMember(
				MemberAccessor<TType, string?[]?>.FromFunc(t => _headerValueAccessor(t), "the values "),
				(_, stringBuilder) => stringBuilder.Append(" whose values "))
			.AddExpectations(e => expectations(new ThatSubject<string?[]>(e)),
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural);
		return this;
	}

	private sealed class WithHeaderValueConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expected,
		Func<TType, string?[]?> headerValueAccessor,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<TType?>(it, grammars),
			IAsyncConstraint<TType?>
	{
		private string?[]? _headerValues;

		public async ValueTask<ConstraintResult> IsMetBy(TType? actual, CancellationToken cancellationToken)
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
			Outcome = await options.AreConsideredEqual(headerValue, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <remarks>
		///     The text continues the one of the header, which carries the negation and the number of the subject, like
		///     the member expectations of <c>Whose</c> do.
		/// </remarks>
		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(" whose value ");
			stringBuilder.Append(options.GetExpectation(expected, ExpectationGrammars.Active));
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
			=> stringBuilder.Append(options.GetExtendedMemberFailure(It, "header value", Grammars, _headerValues?[0],
				expected));
	}
}
