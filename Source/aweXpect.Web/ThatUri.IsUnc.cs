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
	///     Verifies that the subject is a UNC path.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsUnc" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsUnc(this IThat<Uri?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsUncConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not a UNC path.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.IsUnc" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> IsNotUnc(this IThat<Uri?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsUncConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsUncConstraint(string it, ExpectationGrammars grammars)
		: AbsoluteUriConstraint(it, grammars)
	{
		protected override bool IsSatisfiedBy(Uri? actual)
			=> actual?.IsUnc == true;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is a UNC path", "are UNC paths"));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not a UNC path", "are not UNC paths"));
	}
}
