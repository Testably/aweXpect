using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using aweXpect.Reflection.Tests.TestHelpers.Types.ToVerifyingTheNamespaceOfIt;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WithoutNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOutTypesWithNamespace()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithNamespace("aweXpect.Reflection.Tests.TestHelpers.Types").AsPrefix()
					.WithoutNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");

				await That(types).Contains(typeof(SomeClassToVerifyTheNamespaceOfIt))
					.And.DoesNotContain(typeof(PublicClass));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with namespace starting with \"aweXpect.Reflection.Tests.Test…\" without namespace equal to \"aweXpect.Reflection.Tests.Test…\" in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportAsPrefix()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithoutNamespace("aweXpect.Reflection.Tests.TestHelpers.Types").AsPrefix();

				await That(types).DoesNotContain(typeof(SomeClassToVerifyTheNamespaceOfIt))
					.And.DoesNotContain(typeof(PublicClass))
					.And.Contains(typeof(TypeFilters));
				await That(types.GetDescription())
					.IsEqualTo("types without namespace starting with \"aweXpect.Reflection.Tests.Test…\" in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithoutNamespace("AWEXPECT.REFLECTION.TESTS.TESTHELPERS.TYPES").IgnoringCase();

				await That(types).DoesNotContain(typeof(PublicClass))
					.And.Contains(typeof(SomeClassToVerifyTheNamespaceOfIt));
				await That(types.GetDescription())
					.IsEqualTo(
						"types without namespace equal to \"AWEXPECT.REFLECTION.TESTS.TEST…\" ignoring case in assembly")
					.AsPrefix();
			}
		}
	}
}
