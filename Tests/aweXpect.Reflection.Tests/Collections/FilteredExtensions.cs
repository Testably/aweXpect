using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class FilteredExtensions
{
	public static TheoryData<AccessModifiers, string> GetAccessModifiers()
		=> new()
		{
			{
				AccessModifiers.Any, ""
			},
			{
				AccessModifiers.Public, "public "
			},
			{
				AccessModifiers.Private, "private "
			},
			{
				AccessModifiers.Protected, "protected "
			},
			{
				AccessModifiers.Internal, "internal "
			},
			{
				AccessModifiers.Public | AccessModifiers.Internal, "public or internal "
			},
			{
				AccessModifiers.Public | AccessModifiers.Protected | AccessModifiers.Internal,
				"public, protected or internal "
			},
		};
}
