using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForTypesWithName()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName(nameof(SomeClassToVerifyTheMethodNameOfIt.SomeMethodToVerifyTheNameOfIt));

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo("methods with name equal to \"SomeMethodToVerifyTheNameOfIt\" in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsPrefix()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName("SomeMethodToVerifyTheNameOf").AsPrefix();

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo("methods with name starting with \"SomeMethodToVerifyTheNameOf\" in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsRegex()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName("[a-zA-Z]*MethodToVerifyTheName").AsRegex();

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo("methods with name matching regex \"[a-zA-Z]*MethodToVerifyTheName\" in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName("MethodToVerifyTheNameOfIt").AsSuffix();

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo("methods with name ending with \"MethodToVerifyTheNameOfIt\" in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName("*MethodToVerifyTheNameOf*").AsWildcard();

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo("methods with name matching \"*MethodToVerifyTheNameOf*\" in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportExactly()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName(nameof(SomeClassToVerifyTheMethodNameOfIt.SomeMethodToVerifyTheNameOfIt))
					.Exactly();

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo("methods with name equal to \"SomeMethodToVerifyTheNameOfIt\" in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName(nameof(SomeClassToVerifyTheMethodNameOfIt.SomeMethodToVerifyTheNameOfIt)
						.ToLowerInvariant()).IgnoringCase();

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with name equal to \"somemethodtoverifythenameofit\" ignoring case in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringLeadingWhiteSpace()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName(
						"\t " + nameof(SomeClassToVerifyTheMethodNameOfIt.SomeMethodToVerifyTheNameOfIt))
					.IgnoringLeadingWhiteSpace();

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with name equal to \"\\t SomeMethodToVerifyTheNameOfI…\" ignoring leading white-space in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringTrailingWhiteSpace()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName(
						nameof(SomeClassToVerifyTheMethodNameOfIt.SomeMethodToVerifyTheNameOfIt) + "\t ")
					.IgnoringTrailingWhiteSpace();

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with name equal to \"SomeMethodToVerifyTheNameOfIt\\t…\" ignoring trailing white-space in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportUsingCustomComparer()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithName("SOmEMEthOdTOVErIfyThENAmEofit")
					.Using(new IgnoreCaseForVocalsComparer());

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with name equal to \"SOmEMEthOdTOVErIfyThENAmEofit\" using IgnoreCaseForVocalsComparer in assembly")
					.AsPrefix();
			}
		}
	}
}
