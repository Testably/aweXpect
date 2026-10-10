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
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsFile(this IThat<Uri?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFileConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is not a file URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsFile" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsNotFile(this IThat<Uri?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFileConstraint(it, grammars).Invert()),
			source);

	private sealed class IsFileConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Uri>(it, grammars),
			IValueConstraint<Uri?>
	{
		public ConstraintResult IsMetBy(Uri? actual)
		{
			Actual = actual;
			Outcome = actual?.IsFile == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is a file URI", "are file URIs"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not a file URI", "are not file URIs"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
