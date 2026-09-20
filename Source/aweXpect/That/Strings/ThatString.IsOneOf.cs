using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsOneOf(
		this IThat<string?> subject,
		params string?[] expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsOneOf(
		this IThat<string?> subject,
		IEnumerable<string?> expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsNotOneOf(
		this IThat<string?> subject,
		params string?[] unexpected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsNotOneOf(
		this IThat<string?> subject,
		IEnumerable<string?> unexpected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOneOfConstraint(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<string?> expectedValues,
		StringEqualityOptions options)
		: ConstraintResult.WithValue<string?>(it, grammars),
			IAsyncConstraint<string?>
	{
		private bool _hasNothingToInspect;

		/// <inheritdoc cref="ConstraintResult.Outcome" />
		/// <remarks>
		///     A match type other than the exact one inspects the content of the subject, which a <see langword="null" />
		///     does not have, so it fails in both polarities. A <see langword="null" /> value inspects nothing and still
		///     matches a <see langword="null" /> subject as a plain equality check.
		/// </remarks>
		public override Outcome Outcome
		{
			get => _hasNothingToInspect ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}

		public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			StringEqualityOptions stringEqualityOptions = options;
			bool hasValues = false;
			foreach (string? value in expectedValues)
			{
				hasValues = true;
				if (await stringEqualityOptions
					    .AreConsideredEqual(actual, value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			_hasNothingToInspect = actual is null && options.InspectsSubject;
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			Formatter.Format(stringBuilder, expectedValues);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			Formatter.Format(stringBuilder, expectedValues);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
