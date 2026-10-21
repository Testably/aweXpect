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
	///     Verifies that the subject has the default port for the used scheme.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsDefaultPort" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> HasDefaultPort(this IThat<Uri?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasDefaultPortConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject does not have the default port for the used scheme.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsDefaultPort" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> DoesNotHaveDefaultPort(this IThat<Uri?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasDefaultPortConstraint(it, grammars).Invert()),
			subject);

	private sealed class HasDefaultPortConstraint(string it, ExpectationGrammars grammars)
		: AbsoluteUriConstraint(it, grammars)
	{
		protected override bool IsSatisfiedBy(Uri? actual)
			=> actual?.IsDefaultPort == true;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has ", "have ")).Append("the default port for the used scheme");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have ", "do not have ")).Append("the default port for the used scheme");
	}
}
