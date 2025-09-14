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
	///     Verifies that the subject is an UNC path.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsUnc" />
	/// </remarks>
	public static AndOrResult<Uri, IThat<Uri>> IsUnc(this IThat<Uri> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsUncConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is not an UNC path.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsUnc" />
	/// </remarks>
	public static AndOrResult<Uri, IThat<Uri>> IsNotUnc(this IThat<Uri> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsUncConstraint(it, grammars).Invert()),
			source);

	private sealed class IsUncConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<Uri>(grammars),
			IValueConstraint<Uri>
	{
		public ConstraintResult IsMetBy(Uri actual)
		{
			Actual = actual;
			Outcome = actual.IsUnc ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is an UNC path");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not an UNC path");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
