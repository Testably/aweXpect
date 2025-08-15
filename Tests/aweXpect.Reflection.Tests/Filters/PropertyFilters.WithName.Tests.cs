using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WithName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForTypesWithName()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties()
					.WithName(nameof(SomeClassToVerifyThePropertyNameOfIt.SomePropertyToVerifyTheNameOfIt));

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo("properties with name equal to \"SomePropertyToVerifyTheNameOfI…\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsPrefix()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties().WithName("SomePropertyToVerifyTheNameOf").AsPrefix();

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo("properties with name starting with \"SomePropertyToVerifyTheNameOf\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsRegex()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties().WithName("[a-zA-Z]*PropertyToVerifyTheName").AsRegex();

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo("properties with name matching regex \"[a-zA-Z]*PropertyToVerifyTheNa…\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties().WithName("PropertyToVerifyTheNameOfIt").AsSuffix();

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo("properties with name ending with \"PropertyToVerifyTheNameOfIt\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties().WithName("*PropertyToVerifyTheNameOf*").AsWildcard();

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo("properties with name matching \"*PropertyToVerifyTheNameOf*\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportExactly()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties().WithName(nameof(SomeClassToVerifyThePropertyNameOfIt.SomePropertyToVerifyTheNameOfIt))
					.Exactly();

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo("properties with name equal to \"SomePropertyToVerifyTheNameOfI…\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties().WithName(nameof(SomeClassToVerifyThePropertyNameOfIt.SomePropertyToVerifyTheNameOfIt)
						.ToLowerInvariant()).IgnoringCase();

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with name equal to \"somepropertytoverifythenameofi…\" ignoring case in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringLeadingWhiteSpace()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties().WithName(
						"\t " + nameof(SomeClassToVerifyThePropertyNameOfIt.SomePropertyToVerifyTheNameOfIt))
					.IgnoringLeadingWhiteSpace();

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with name equal to \"\\t SomePropertyToVerifyTheNameO…\" ignoring leading white-space in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringTrailingWhiteSpace()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties().WithName(
						nameof(SomeClassToVerifyThePropertyNameOfIt.SomePropertyToVerifyTheNameOfIt) + "\t ")
					.IgnoringTrailingWhiteSpace();

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with name equal to \"SomePropertyToVerifyTheNameOfI…\" ignoring trailing white-space in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportUsingCustomComparer()
			{
				Filtered.Properties properties = In.Type<SomeClassToVerifyThePropertyNameOfIt>()
					.Properties().WithName("SOmEPropertyTOVErIfyThENAmEofit")
					.Using(new IgnoreCaseForVocalsComparer());

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with name equal to \"SOmEPropertyTOVErIfyThENAmEofi…\" using IgnoreCaseForVocalsComparer in")
					.AsPrefix();
			}
		}
	}
}
