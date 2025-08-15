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
	///     Verifies that the <see cref="Assembly" /> has no dependency on the <paramref name="unexpected" /> assembly.
	/// </summary>
	public static StringEqualityTypeResult<Assembly?, IThat<Assembly?>> HasNoDependencyOn(
		this IThat<Assembly?> subject, string unexpected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<Assembly?, IThat<Assembly?>>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new HasADependencyOnConstraint(it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}
}
