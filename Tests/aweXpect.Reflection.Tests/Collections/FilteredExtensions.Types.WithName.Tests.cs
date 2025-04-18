using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class FilteredExtensions
{
	public sealed partial class Types
	{
		public sealed class WithName
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldFilterForTypesWithName()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithName(nameof(SomeClassToVerifyTheNameOfIt));

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with name equal to \"SomeClassToVerifyTheNameOfIt\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportAsPrefix()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithName("SomeClassToVerifyTheNameOf").AsPrefix();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with name starting with \"SomeClassToVerifyTheNameOf\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportAsRegex()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithName("[a-zA-Z]*VerifyTheNameOfIt").AsRegex();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with name matching regex \"[a-zA-Z]*VerifyTheNameOfIt\" in assembly")
						.AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportAsSuffix()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithName("ClassToVerifyTheNameOfIt").AsSuffix();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with name ending with \"ClassToVerifyTheNameOfIt\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportAsWildcard()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithName("*ToVerifyTheNameOf*").AsWildcard();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with name matching \"*ToVerifyTheNameOf*\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportExactly()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithName(nameof(SomeClassToVerifyTheNameOfIt)).Exactly();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNameOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with name equal to \"SomeClassToVerifyTheNameOfIt\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportIgnoringCase()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
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
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
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
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
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
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
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
}
