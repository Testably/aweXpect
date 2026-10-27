using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatMember
{
	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> does not have the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TMember, IThat<TMember?>> DoesNotHaveName<TMember>(
		this IThat<TMember?> subject, string unexpected)
		where TMember : MemberInfo
	{
		return new StringEqualityTypeResult<TMember, IThat<TMember?>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected,
				static (unexpected, it, grammars)
				=> new HasNameConstraint<TMember?>(it, grammars, unexpected, "unexpected").Invert(),
				out HasNameConstraint<TMember?> constraint),
			subject,
			constraint);
	}
}
