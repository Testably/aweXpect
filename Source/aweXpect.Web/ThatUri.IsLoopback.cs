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
	///     Verifies that the subject references the local host.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsLoopback" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsLoopback(this IThat<Uri?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLoopbackConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject does not reference the local host.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsLoopback" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsNotLoopback(this IThat<Uri?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsLoopbackConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsLoopbackConstraint(string it, ExpectationGrammars grammars)
		: AbsoluteUriConstraint(it, grammars)
	{
		protected override bool IsSatisfiedBy(Uri? actual)
			=> actual?.IsLoopback == true;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("references ", "reference ")).Append("the local host");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not ", "do not ")).Append("reference the local host");
	}
}
