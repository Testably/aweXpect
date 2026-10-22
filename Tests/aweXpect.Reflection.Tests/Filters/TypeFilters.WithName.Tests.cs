using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WithName
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesWithName()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName(nameof(SomeClassToVerifyTheNameOfIt));

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name equal to \"SomeClassToVerifyTheNameOfIt\" in assembly").AsPrefix();
			}

			[Test]
			public async Task ShouldSupportAsPrefix()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("SomeClassToVerifyTheNameOf").AsPrefix();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name starting with \"SomeClassToVerifyTheNameOf\" in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportAsRegex()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("[a-zA-Z]*VerifyTheNameOfIt").AsRegex();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name matching regex \"[a-zA-Z]*VerifyTheNameOfIt\" in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("ClassToVerifyTheNameOfIt").AsSuffix();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name ending with \"ClassToVerifyTheNameOfIt\" in assembly").AsPrefix();
			}

			[Test]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("*ToVerifyTheNameOf*").AsWildcard();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo("types with name matching \"*ToVerifyTheNameOf*\" in assembly").AsPrefix();
			}

			[Test]
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

			[Test]
			public async Task ShouldSupportIgnoringCaseBeforeAsPrefix()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("someclasstoverifythenameof").IgnoringCase().AsPrefix();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with name starting with \"someclasstoverifythenameof\" ignoring case in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportIgnoringLeadingWhiteSpace()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName("\t " + nameof(SomeClassToVerifyTheNameOfIt))
					.IgnoringLeadingWhiteSpace();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with name equal to \"\\t SomeClassToVerifyTheNameOfIt\" ignoring leading whitespace in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportIgnoringTrailingWhiteSpace()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName(nameof(SomeClassToVerifyTheNameOfIt) + "\t ")
					.IgnoringTrailingWhiteSpace();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with name equal to \"SomeClassToVerifyTheNameOfIt\\t \" ignoring trailing whitespace in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportNormalizingWhiteSpace()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithName(" \t" + nameof(SomeClassToVerifyTheNameOfIt))
					.NormalizingWhiteSpace();

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with name equal to \" \\tSomeClassToVerifyTheNameOfIt\" normalizing whitespace in assembly")
					.AsPrefix();
			}

			[Test]
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
