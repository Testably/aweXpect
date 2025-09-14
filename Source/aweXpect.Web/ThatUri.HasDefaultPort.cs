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
	///     Verifies that the subject has the default port for the used scheme.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsDefaultPort" />
	/// </remarks>
	public static AndOrResult<Uri, IThat<Uri>> HasDefaultPort(this IThat<Uri> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasDefaultPortConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject does not have the default port for the used scheme.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsDefaultPort" />
	/// </remarks>
	public static AndOrResult<Uri, IThat<Uri>> DoesNotHaveDefaultPort(this IThat<Uri> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasDefaultPortConstraint(it, grammars).Invert()),
			source);

	private sealed class HasDefaultPortConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<Uri>(grammars),
			IValueConstraint<Uri>
	{
		public ConstraintResult IsMetBy(Uri actual)
		{
			Actual = actual;
			Outcome = actual.IsDefaultPort ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has the default port for the used scheme");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have the default port for the used scheme");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
