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
	private static IThat<Uri?> FailIfRelative(this IThat<Uri?> subject)
	{
		subject.Get().ExpectationBuilder
			.AddConstraint((it, grammars) => new FailIfRelativeConstraint(it, grammars))
			.And("");
		return subject;
	}

	private static void ThrowIfNotAbsolute(Uri value, string paramName)
	{
		ThrowHelper.ThrowIfNull(value, paramName);
		if (!value.IsAbsoluteUri)
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{paramName}' must be a valid absolute URI.", paramName));
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
