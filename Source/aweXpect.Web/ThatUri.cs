using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="Uri" /> values.
/// </summary>
public static partial class ThatUri
{
	/// <summary>
	///     Fails both ways for a relative URI, whose components cannot be accessed, and stops further processing.
	/// </summary>
	/// <remarks>
	///     Used in front of the property results, whose constraints come from aweXpect.Core, and of
	///     <see cref="HasQueryParameter" />; the generated expectations return <see cref="WasRelative" /> and the other
	///     constraints derive from <see cref="AbsoluteUriConstraint" />.
	/// </remarks>
	private static IThat<Uri?> FailIfRelative(this IThat<Uri?> subject)
	{
		subject.Get().ExpectationBuilder
			.AddConstraint((it, grammars) => new FailIfRelativeConstraint(it, grammars))
			.And("");
		return subject;
	}

	/// <summary>
	///     The result of a generated expectation for a relative URI, whose components cannot be accessed, so that it
	///     fails both ways.
	/// </summary>
	private static PredicateResult WasRelative => PredicateResult.FailureBothWays("was relative");

	internal static void ThrowIfNotAbsolute(Uri value, string paramName)
	{
		ThrowHelper.ThrowIfNull(value, paramName);
		if (!value.IsAbsoluteUri)
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{paramName}' must be a valid absolute URI.", paramName));
		}
	}

	/// <summary>
	///     A constraint on an absolute <see cref="Uri" />, which fails both ways for a relative URI, whose components
	///     cannot be accessed, and stops further processing, like <see cref="FailIfRelative" />.
	/// </summary>
	/// <remarks>
	///     It checks the URI itself, as a separate constraint for the check costs a node and a combined result for every
	///     expectation.
	/// </remarks>
	private abstract class AbsoluteUriConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Uri>(it, grammars),
			IValueConstraint<Uri?>
	{
		private bool IsRelative => Actual is { IsAbsoluteUri: false, };

		/// <remarks>
		///     A relative URI has no subject, like the result of <see cref="FailIfRelative" />, so that a combined result
		///     keeps the subject of the following result.
		/// </remarks>
		public override string? LeadingSubject => IsRelative ? null : base.LeadingSubject;

		/// <inheritdoc cref="LeadingSubject" />
		public override string? TrailingSubject => IsRelative ? null : base.TrailingSubject;

		public ConstraintResult IsMetBy(Uri? actual)
		{
			Actual = actual;
			if (IsRelative)
			{
				Outcome = Outcome.FailureBothWays;
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			}
			else
			{
				Outcome = IsSatisfiedBy(actual) ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		/// <summary>
		///     Whether the <paramref name="actual" /> URI, which is absolute or <see langword="null" />, satisfies the
		///     expectation.
		/// </summary>
		protected abstract bool IsSatisfiedBy(Uri? actual);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			if (IsRelative)
			{
				stringBuilder.Append("relative");
			}
			else
			{
				Formatter.Format(stringBuilder, Actual);
			}
		}
	}

	private sealed class FailIfRelativeConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.ExpectationOnly<Uri?>(grammars, null, null),
			IValueConstraint<Uri?>
	{
		ConstraintResult IValueConstraint<Uri?>.IsMetBy(Uri? actual)
		{
			if (actual?.IsAbsoluteUri == false)
			{
				Outcome = Outcome.FailureBothWays;
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			}

			return this;
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" was relative");
	}
}
