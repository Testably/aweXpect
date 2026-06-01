using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> does not have the <paramref name="unexpected" /> name.
	/// </summary>
	public static StringEqualityTypeResult<Assembly?, IThat<Assembly?>> DoesNotHaveName(
		this IThat<Assembly?> subject, string unexpected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<Assembly?, IThat<Assembly?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasNameConstraint(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}
}
