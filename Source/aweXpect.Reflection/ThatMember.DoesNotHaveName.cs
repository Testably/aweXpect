using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMember
{
	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> does not have the <paramref name="unexpected" /> name.
	/// </summary>
	public static StringEqualityTypeResult<TMember, IThat<TMember>> DoesNotHaveName<TMember>(
		this IThat<TMember> subject, string unexpected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<TMember, IThat<TMember>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasNameConstraint<TMember>(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}
}
