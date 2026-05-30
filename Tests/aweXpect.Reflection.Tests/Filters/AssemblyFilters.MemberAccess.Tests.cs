using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class MemberAccess
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldStillSupportPublicBeforeInterfaces()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>().Public.Interfaces();

				await That(types).All().Satisfy(t => t.IsInterface && t.IsPublic).And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("public interfaces in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldStillSupportPublicBeforeMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>().Public.Methods();

				await That(methods).All().Satisfy(m => m.IsPublic).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("public methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportStaticBeforeMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>().Static.Methods();

				await That(methods).All().Satisfy(m => m.IsStatic).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("static methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportStaticBeforeConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>().Static.Constructors();

				await That(constructors).All().Satisfy(c => c.IsStatic).And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("static constructors in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAbstractBeforeMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>().Abstract.Methods();

				await That(methods).All().Satisfy(m => m.IsAbstract).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("abstract methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportSealedBeforeMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>().Sealed.Methods();

				await That(methods).All().Satisfy(m => m is { IsVirtual: true, IsFinal: true })
					.And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("sealed methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldCombinePublicAndStaticBeforeMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>().Public.Static.Methods();

				await That(methods).All().Satisfy(m => m.IsStatic && m.IsPublic).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("public static methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldResetModifierStateAfterSelector()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>();

				Filtered.Methods publicMethods = assemblies.Public.Methods();
				Filtered.Methods anyMethods = assemblies.Methods();

				await That(publicMethods).All().Satisfy(m => m.IsPublic).And.IsNotEmpty();
				await That(anyMethods).Any().Satisfy(m => !m.IsPublic);
				await That(publicMethods.GetDescription())
					.IsEqualTo("public methods in assembly").AsPrefix();
				await That(anyMethods.GetDescription())
					.IsEqualTo("methods in assembly").AsPrefix();
			}
		}
	}
}
