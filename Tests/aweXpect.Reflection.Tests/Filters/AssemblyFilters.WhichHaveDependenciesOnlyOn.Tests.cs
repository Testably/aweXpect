using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichHaveDependenciesOnlyOn
	{
		/// <summary>
		///     Excludes the framework facade assemblies that differ between target frameworks (e.g. <c>netstandard</c> on
		///     <c>net48</c>), so that the tests only have to whitelist the actual non-framework dependencies.
		/// </summary>
		private static CustomizationLifetime ExcludeFrameworkAssemblies()
			=> Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes
				.Set([
					.. Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get(), "netstandard", "WindowsBase",
				]);

		public sealed class Tests
		{
			[Fact]
			public async Task ShouldDescribeTheFilterForASingleAllowedAssembly()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveDependenciesOnlyOn("aweXpect.Core");

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have dependencies only on assemblies equal to \"aweXpect.Core\"");
			}

			[Fact]
			public async Task ShouldDescribeTheFilterForMultipleAllowedAssemblies()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveDependenciesOnlyOn("aweXpect.Core", "aweXpect");

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have dependencies only on assemblies equal to \"aweXpect.Core\" or equal to \"aweXpect\"");
			}

			[Fact]
			public async Task ShouldFilterForAssembliesDependingOnlyOnAllowed()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly)
					.WhichHaveDependenciesOnlyOn("aweXpect.Core");

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Fact]
			public async Task ShouldSupportAsWildcard()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly)
					.WhichHaveDependenciesOnlyOn("aweXpect.*").AsWildcard();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly)
					.WhichHaveDependenciesOnlyOn("awexpect.core").IgnoringCase();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Fact]
			public async Task WhenAssemblyDependsOnDisallowedAssembly_ShouldBeFilteredOut()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveDependenciesOnlyOn("aweXpect.Core");

				await That(assemblies).IsEmpty();
			}
		}
	}
}
