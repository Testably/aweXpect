using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="Uri" /> values.
/// </summary>
public static partial class ThatUri
{
	/// <summary>
	///     Fails both ways for a relative URI, whose components cannot be accessed, and stops further processing.
	/// </summary>
	private static IThat<Uri?> FailIfRelative(this IThat<Uri?> source)
	{
		source.Get().ExpectationBuilder
			.AddConstraint((it, grammars) => new FailIfRelativeConstraint(it, grammars))
			.And("");
		return source;
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
