using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the subject is an absolute URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsAbsoluteUri" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsAbsolute(this IThat<Uri?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsAbsoluteConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is not an absolute URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsAbsoluteUri" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsNotAbsolute(this IThat<Uri?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsAbsoluteConstraint(it, grammars).Invert()),
			source);

	private sealed class IsAbsoluteConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Uri>(it, grammars),
			IValueConstraint<Uri?>
	{
		public ConstraintResult IsMetBy(Uri? actual)
		{
			Actual = actual;
			Outcome = actual?.IsAbsoluteUri == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is an absolute URI", "are absolute URIs"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not an absolute URI", "are not absolute URIs"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
