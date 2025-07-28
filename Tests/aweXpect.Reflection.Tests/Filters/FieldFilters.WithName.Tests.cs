using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WithName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForTypesWithName()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName(nameof(SomeClassToVerifyTheFieldNameOfIt.SomeFieldToVerifyTheNameOfIt));

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo("fields with name equal to \"SomeFieldToVerifyTheNameOfIt\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsPrefix()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName("SomeFieldToVerifyTheNameOf").AsPrefix();

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo("fields with name starting with \"SomeFieldToVerifyTheNameOf\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsRegex()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName("[a-zA-Z]*FieldToVerifyTheName").AsRegex();

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo("fields with name matching regex \"[a-zA-Z]*FieldToVerifyTheName\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName("FieldToVerifyTheNameOfIt").AsSuffix();

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo("fields with name ending with \"FieldToVerifyTheNameOfIt\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName("*FieldToVerifyTheNameOf*").AsWildcard();

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo("fields with name matching \"*FieldToVerifyTheNameOf*\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportExactly()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName(nameof(SomeClassToVerifyTheFieldNameOfIt.SomeFieldToVerifyTheNameOfIt))
					.Exactly();

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo("fields with name equal to \"SomeFieldToVerifyTheNameOfIt\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName(nameof(SomeClassToVerifyTheFieldNameOfIt.SomeFieldToVerifyTheNameOfIt)
						.ToLowerInvariant()).IgnoringCase();

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields with name equal to \"somefieldtoverifythenameofit\" ignoring case in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringLeadingWhiteSpace()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName(
						"\t " + nameof(SomeClassToVerifyTheFieldNameOfIt.SomeFieldToVerifyTheNameOfIt))
					.IgnoringLeadingWhiteSpace();

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields with name equal to \"\\t SomeFieldToVerifyTheNameOfIt\" ignoring leading white-space in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringTrailingWhiteSpace()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName(
						nameof(SomeClassToVerifyTheFieldNameOfIt.SomeFieldToVerifyTheNameOfIt) + "\t ")
					.IgnoringTrailingWhiteSpace();

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields with name equal to \"SomeFieldToVerifyTheNameOfIt\\t \" ignoring trailing white-space in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportUsingCustomComparer()
			{
				Filtered.Fields fields = In.Type<SomeClassToVerifyTheFieldNameOfIt>()
					.Fields().WithName("SOmEFieldTOVErIfyThENAmEofit")
					.Using(new IgnoreCaseForVocalsComparer());

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields with name equal to \"SOmEFieldTOVErIfyThENAmEofit\" using IgnoreCaseForVocalsComparer in")
					.AsPrefix();
			}
		}
	}
}
