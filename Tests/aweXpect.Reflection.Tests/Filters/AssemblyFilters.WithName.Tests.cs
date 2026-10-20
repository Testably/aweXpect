using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WithName
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForAssembliesWithName()
			{
				Filtered.Assemblies Assemblies = In.AllLoadedAssemblies()
					.WithName("aweXpect.Reflection.Tests");

				await That(Assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(Assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with name equal to \"aweXpect.Reflection.Tests\"");
			}

			[Test]
			public async Task ShouldSupportAsPrefix()
			{
				Filtered.Assemblies Assemblies = In.AllLoadedAssemblies()
					.WithName("aweXpect.Reflection").AsPrefix();

				await That(Assemblies).HasCount().GreaterThanOrEqualTo(2);
				await That(Assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with name starting with \"aweXpect.Reflection\"");
			}

			[Test]
			public async Task ShouldSupportAsRegex()
			{
				Filtered.Assemblies Assemblies = In.AllLoadedAssemblies()
					.WithName("aweXpect\\.[a-zA-Z\\.]*").AsRegex();

				await That(Assemblies).HasCount().GreaterThanOrEqualTo(3);
				await That(Assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with name matching regex \"aweXpect\\\\.[a-zA-Z\\\\.]*\"");
			}

			[Test]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Assemblies Assemblies = In.AllLoadedAssemblies()
					.WithName(".Tests").AsSuffix();

				await That(Assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(Assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with name ending with \".Tests\"");
			}

			[Test]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Assemblies Assemblies = In.AllLoadedAssemblies()
					.WithName("awe*pect*").AsWildcard();

				await That(Assemblies).HasCount().GreaterThanOrEqualTo(3);
				await That(Assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with name matching \"awe*pect*\"");
			}

			[Test]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Assemblies Assemblies = In.AllLoadedAssemblies()
					.WithName("awexpect.reflection.tests").IgnoringCase();

				await That(Assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(Assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with name equal to \"awexpect.reflection.tests\" ignoring case");
			}

			[Test]
			public async Task ShouldSupportIgnoringLeadingWhiteSpace()
			{
				Filtered.Assemblies Assemblies = In.AllLoadedAssemblies()
					.WithName("\t aweXpect.Reflection.Tests")
					.IgnoringLeadingWhiteSpace();

				await That(Assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(Assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with name equal to \"\\t aweXpect.Reflection.Tests\" ignoring leading whitespace");
			}

			[Test]
			public async Task ShouldSupportIgnoringTrailingWhiteSpace()
			{
				Filtered.Assemblies Assemblies = In.AllLoadedAssemblies()
					.WithName("aweXpect.Reflection.Tests\t ")
					.IgnoringTrailingWhiteSpace();

				await That(Assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(Assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with name equal to \"aweXpect.Reflection.Tests\\t \" ignoring trailing whitespace");
			}

			[Test]
			public async Task ShouldSupportUsingCustomComparer()
			{
				Filtered.Assemblies Assemblies = In.AllLoadedAssemblies()
					.WithName("AwEXpEct.REflEctIOn.TEsts")
					.Using(new IgnoreCaseForVocalsComparer());

				await That(Assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(Assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with name equal to \"AwEXpEct.REflEctIOn.TEsts\" using IgnoreCaseForVocalsComparer");
			}
		}
	}
}
