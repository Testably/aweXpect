using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has no dependency on the <paramref name="unexpected" /> assembly.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Assembly, IThat<Assembly?>> DoesNotDependOn(
		this IThat<Assembly?> subject, string unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<Assembly, IThat<Assembly?>>(subject.Get().ExpectationBuilder
				.AddConstraint((Unexpected: unexpected, Options: options),
					static (s, it, grammars)
					=> new DependsOnConstraint(it, grammars, s.Unexpected, s.Options).Invert()),
			subject,
			options);
	}
}
