using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class FilteredExtensions
{
	public sealed partial class Types
	{
		public sealed class WithNamespace
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldFilterForTypesWithNamespace()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt");

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with namespace equal to \"aweXpect.Reflection.Tests.Test…\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportAsPrefix()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOf").AsPrefix();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with namespace starting with \"aweXpect.Reflection.Tests.Test…\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportAsRegex()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("[a-zA-Z\\.]*VerifyingTheNamespaceOfIt").AsRegex();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with namespace matching regex \"[a-zA-Z\\.]*VerifyingTheNamespa…\" in assembly")
						.AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportAsSuffix()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("VerifyingTheNamespaceOfIt").AsSuffix();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with namespace ending with \"VerifyingTheNamespaceOfIt\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportAsWildcard()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("*ToVerifyingTheNamespaceOf*").AsWildcard();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with namespace matching \"*ToVerifyingTheNamespaceOf*\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportExactly()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt").Exactly();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo("types with namespace equal to \"aweXpect.Reflection.Tests.Test…\" in assembly").AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportIgnoringCase()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt".ToLowerInvariant()).IgnoringCase();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo(
							"types with namespace equal to \"awexpect.reflection.tests.test…\" ignoring case in assembly")
						.AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportIgnoringLeadingWhiteSpace()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("\t aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt")
						.IgnoringLeadingWhiteSpace();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo(
							"types with namespace equal to \"\\t aweXpect.Reflection.Tests.Te…\" ignoring leading white-space in assembly")
						.AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportIgnoringTrailingWhiteSpace()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt\t ")
						.IgnoringTrailingWhiteSpace();

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo(
							"types with namespace equal to \"aweXpect.Reflection.Tests.Test…\" ignoring trailing white-space in assembly")
						.AsPrefix();
				}

				[Fact]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().WithNamespace("AwEXpEct.REflEctIOn.TEsts.TEstHelpers.Types.ToVerifyingTheNamespaceOfIt")
						.Using(new IgnoreCaseForVocalsComparer());

					await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyTheNamespaceOfIt));
					await That(types.GetDescription())
						.IsEqualTo(
							"types with namespace equal to \"AwEXpEct.REflEctIOn.TEsts.TEst…\" using IgnoreCaseForVocalsComparer in assembly")
						.AsPrefix();
				}
			}
		}
	}
}
