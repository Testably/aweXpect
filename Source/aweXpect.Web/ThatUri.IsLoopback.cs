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
	///     Verifies that the subject references the local host.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsLoopback" />
	/// </remarks>
	public static AndOrResult<Uri, IThat<Uri>> IsLoopback(this IThat<Uri> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLoopbackConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject does not reference the local host.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsLoopback" />
	/// </remarks>
	public static AndOrResult<Uri, IThat<Uri>> IsNotLoopback(this IThat<Uri> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLoopbackConstraint(it, grammars).Invert()),
			source);

	private sealed class IsLoopbackConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<Uri>(grammars),
			IValueConstraint<Uri>
	{
		public ConstraintResult IsMetBy(Uri actual)
		{
			Actual = actual;
			Outcome = actual.IsLoopback ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("references the local host");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not reference the local host");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
