using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the subject is a file URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsFile" />
	/// </remarks>
	public static AndOrResult<Uri, IThat<Uri>> IsFile(this IThat<Uri> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFileConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is not a file URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsFile" />
	/// </remarks>
	public static AndOrResult<Uri, IThat<Uri>> IsNotFile(this IThat<Uri> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFileConstraint(it, grammars).Invert()),
			source);

	private sealed class IsFileConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<Uri>(grammars),
			IValueConstraint<Uri>
	{
		public ConstraintResult IsMetBy(Uri actual)
		{
			Actual = actual;
			Outcome = actual.IsFile ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is a file URI");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not a file URI");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
