using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the lines of the <see langword="string" /> subject satisfy the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     Lines are separated by <c>\r\n</c>, <c>\n</c> or <c>\r</c>.<br />
	///     A single trailing line terminator does not start a new line, so <c>"a\nb\n"</c> has two lines.
	/// </remarks>
	public static AndOrResult<string?, IThat<string?>> HasLines(
		this IThat<string?> source,
		Action<IThatSubject<IEnumerable<string?>>> expectations)
		=> new(source.Get().ExpectationBuilder
				.ForMember<string?, IEnumerable<string?>>(
					s => s.GetLines(),
					" which ",
					false)
				.Validate((it, grammars) => new HasLinesConstraint(it, grammars))
				.AddExpectations(e => expectations(
						new ThatSubject<IEnumerable<string?>>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			source);

	internal sealed class HasLinesConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			Outcome = actual is null ? Outcome.Failure : Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Active))
			{
				stringBuilder.Append("with lines");
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("lines are");
			}
			else
			{
				stringBuilder.Append("has lines");
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			// The result is appended by the nested expectations on the lines.
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Active))
			{
				stringBuilder.Append("without lines");
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("lines are not");
			}
			else
			{
				stringBuilder.Append("does not have lines");
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
