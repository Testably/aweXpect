using System.Collections.Generic;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public static IEnumerable<(AccessModifiers, string)> GetAccessModifiers()
		=> new List<(AccessModifiers, string)>
		{
			(AccessModifiers.Any, ""),
			(AccessModifiers.Public, "public "),
			(AccessModifiers.Private, "private "),
			(AccessModifiers.Protected, "protected "),
			(AccessModifiers.Internal, "internal "),
			(AccessModifiers.Public | AccessModifiers.Internal, "public or internal "),
			(AccessModifiers.Public | AccessModifiers.Protected | AccessModifiers.Internal, "public, protected or internal "),
		};
}
