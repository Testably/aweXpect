using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
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

		public static TheoryData<AccessModifiers, Func<Type, bool>> CheckAccessModifiers()
			=> new()
			{
				{
					AccessModifiers.Public, type => type.IsNested ? type.IsNestedPublic : type.IsPublic
				},
				{
					AccessModifiers.Private, type => type.IsNested ? type.IsNestedPrivate : type.IsNotPublic
				},
				{
					AccessModifiers.Internal, type => type.IsNested ? type.IsNestedAssembly : !type.IsVisible
				},
			};
	}
}
