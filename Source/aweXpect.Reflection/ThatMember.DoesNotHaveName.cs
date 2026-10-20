using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

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
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<TMember, IThat<TMember?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (s, it, grammars)
				=> new HasNameConstraint<TMember?>(it, grammars, s.Unexpected, s.Options).Invert()),
			subject,
			options);
	}
}
