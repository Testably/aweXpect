using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WithName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForTypesWithName()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName(nameof(SomeClassToVerifyTheNameOfIt));

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name equal to \"SomeClassToVerifyTheNameOfIt\" in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsPrefix()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("SomeClassToVerifyTheNameOf").AsPrefix();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name starting with \"SomeClassToVerifyTheNameOf\" in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsRegex()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("[a-zA-Z]*VerifyTheNameOfIt").AsRegex();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name matching regex \"[a-zA-Z]*VerifyTheNameOfIt\" in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("ClassToVerifyTheNameOfIt").AsSuffix();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name ending with \"ClassToVerifyTheNameOfIt\" in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("*ToVerifyTheNameOf*").AsWildcard();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name matching \"*ToVerifyTheNameOf*\" in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportExactly()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName(nameof(SomeClassToVerifyTheNameOfIt)).Exactly();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name equal to \"SomeClassToVerifyTheNameOfIt\" in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName(nameof(SomeClassToVerifyTheNameOfIt).ToLowerInvariant()).IgnoringCase();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with name equal to \"someclasstoverifythenameofit\" ignoring case in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringLeadingWhiteSpace()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("\t " + nameof(SomeClassToVerifyTheNameOfIt))
					.IgnoringLeadingWhiteSpace();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with name equal to \"\\t SomeClassToVerifyTheNameOfIt\" ignoring leading white-space in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringTrailingWhiteSpace()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName(nameof(SomeClassToVerifyTheNameOfIt) + "\t ")
					.IgnoringTrailingWhiteSpace();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with name equal to \"SomeClassToVerifyTheNameOfIt\\t \" ignoring trailing white-space in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportUsingCustomComparer()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("SOmEClAssTOVErIfyThENAmEofit")
					.Using(new IgnoreCaseForVocalsComparer());

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with name equal to \"SOmEClAssTOVErIfyThENAmEofit\" using IgnoreCaseForVocalsComparer in assembly")
					.AsPrefix();
			}

			private class SomeClassToVerifyTheNameOfIt;
		}
	}
}
