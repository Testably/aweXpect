using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class FilteredExtensions
{
	public sealed partial class Assemblies
	{
		public sealed class WithName
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldFilterForAssembliesWithName()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName("aweXpect.Reflection.Tests");

					await That(assemblies).HasSingle().Which.IsEqualTo(typeof(FilteredExtensions).Assembly);
					await That(assemblies.GetDescription())
						.IsEqualTo("in all loaded assemblies with name equal to \"aweXpect.Reflection.Tests\"");
				}

				[Fact]
				public async Task ShouldSupportAsPrefix()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName("aweXpect.Reflection").AsPrefix();

					await That(assemblies).HasCount().AtLeast(2);
					await That(assemblies.GetDescription())
						.IsEqualTo("in all loaded assemblies with name starting with \"aweXpect.Reflection\"");
				}

				[Fact]
				public async Task ShouldSupportAsRegex()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName("aweXpect\\.[a-zA-Z\\.]*").AsRegex();

					await That(assemblies).HasCount().AtLeast(3);
					await That(assemblies.GetDescription())
						.IsEqualTo("in all loaded assemblies with name matching regex \"aweXpect\\.[a-zA-Z\\.]*\"");
				}

				[Fact]
				public async Task ShouldSupportAsSuffix()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName(".Tests").AsSuffix();

					await That(assemblies).HasSingle().Which.IsEqualTo(typeof(FilteredExtensions).Assembly);
					await That(assemblies.GetDescription())
						.IsEqualTo("in all loaded assemblies with name ending with \".Tests\"");
				}

				[Fact]
				public async Task ShouldSupportAsWildcard()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName("awe*pect*").AsWildcard();

					await That(assemblies).HasCount().AtLeast(3);
					await That(assemblies.GetDescription())
						.IsEqualTo("in all loaded assemblies with name matching \"awe*pect*\"");
				}

				[Fact]
				public async Task ShouldSupportExactly()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName("aweXpect.Reflection.Tests").Exactly();

					await That(assemblies).HasSingle().Which.IsEqualTo(typeof(FilteredExtensions).Assembly);
					await That(assemblies.GetDescription())
						.IsEqualTo("in all loaded assemblies with name equal to \"aweXpect.Reflection.Tests\"");
				}

				[Fact]
				public async Task ShouldSupportIgnoringCase()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName("awexpect.reflection.tests").IgnoringCase();

					await That(assemblies).HasSingle().Which.IsEqualTo(typeof(FilteredExtensions).Assembly);
					await That(assemblies.GetDescription())
						.IsEqualTo(
							"in all loaded assemblies with name equal to \"awexpect.reflection.tests\" ignoring case");
				}

				[Fact]
				public async Task ShouldSupportIgnoringLeadingWhiteSpace()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName("\t aweXpect.Reflection.Tests")
						.IgnoringLeadingWhiteSpace();

					await That(assemblies).HasSingle().Which.IsEqualTo(typeof(FilteredExtensions).Assembly);
					await That(assemblies.GetDescription())
						.IsEqualTo(
							"in all loaded assemblies with name equal to \"\\t aweXpect.Reflection.Tests\" ignoring leading white-space");
				}

				[Fact]
				public async Task ShouldSupportIgnoringTrailingWhiteSpace()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName("aweXpect.Reflection.Tests\t ")
						.IgnoringTrailingWhiteSpace();

					await That(assemblies).HasSingle().Which.IsEqualTo(typeof(FilteredExtensions).Assembly);
					await That(assemblies.GetDescription())
						.IsEqualTo(
							"in all loaded assemblies with name equal to \"aweXpect.Reflection.Tests\\t \" ignoring trailing white-space");
				}

				[Fact]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Reflection.Collections.Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
						.WithName("AwEXpEct.REflEctIOn.TEsts")
						.Using(new IgnoreCaseForVocalsComparer());

					await That(assemblies).HasSingle().Which.IsEqualTo(typeof(FilteredExtensions).Assembly);
					await That(assemblies.GetDescription())
						.IsEqualTo(
							"in all loaded assemblies with name equal to \"AwEXpEct.REflEctIOn.TEsts\" using IgnoreCaseForVocalsComparer");
				}
			}
		}
	}
}
