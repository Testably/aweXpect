using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static ObjectEqualityResult<object?, IThat<object?>, object?> IsOneOf(
		this IThat<object?> subject,
		params object?[] expected)
	{
		ObjectEqualityOptions<object?> options = new();
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint<object?, object?>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static ObjectEqualityResult<object?, IThat<object?>, object?> IsOneOf(
		this IThat<object?> subject,
		IEnumerable<object?> expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<object?> options = new();
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint<object?, object?>(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static ObjectEqualityResult<object?, IThat<object?>, object?> IsNotOneOf(
		this IThat<object?> subject,
		params object?[] unexpected)
	{
		ObjectEqualityOptions<object?> options = new();
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint<object?, object?>(it, grammars, unexpected, options)
					.Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static ObjectEqualityResult<object?, IThat<object?>, object?> IsNotOneOf(
		this IThat<object?> subject,
		IEnumerable<object?> unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<object?> options = new();
		return new ObjectEqualityResult<object?, IThat<object?>, object?>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint<object?, object?>(it, grammars, unexpected, options)
					.Invert()),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint<TSubject, TExpected>(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TExpected?> expected,
		ObjectEqualityOptions<TSubject> options)
		: ConstraintResult.WithValue<TSubject>(grammars),
			IAsyncConstraint<TSubject>
	{
		public async Task<ConstraintResult> IsMetBy(TSubject actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TExpected? value in expected)
			{
				hasValues = true;
				if (await options.AreConsideredEqual(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				"one of " + Formatter.Format(expected).TrimCommonWhiteSpace(),
				Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(it, Grammars, Actual, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(
				"one of " + Formatter.Format(expected).TrimCommonWhiteSpace(),
				Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
