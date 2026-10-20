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
	///     Verifies that the subject is a file URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsFile" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsFile(this IThat<Uri?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFileConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not a file URI.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsFile" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsNotFile(this IThat<Uri?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsFileConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsFileConstraint(string it, ExpectationGrammars grammars)
		: AbsoluteUriConstraint(it, grammars)
	{
		protected override bool IsSatisfiedBy(Uri? actual)
			=> actual?.IsFile == true;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is a file URI", "are file URIs"));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not a file URI", "are not file URIs"));
	}
}
