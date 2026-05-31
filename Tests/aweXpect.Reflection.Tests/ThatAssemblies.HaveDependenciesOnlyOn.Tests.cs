using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class HaveDependenciesOnlyOn
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
			public async Task WhenAllAssembliesDependOnlyOnAllowed_ShouldSucceed()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).HaveDependenciesOnlyOn("aweXpect.Core");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllowedMatchesAsWildcard_ShouldSucceed()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).HaveDependenciesOnlyOn("aweXpect.*").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAnAssemblyDependsOnDisallowedAssembly_ShouldFail()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveDependenciesOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have dependencies only on assemblies equal to "aweXpect.Core",
					             but it contained assemblies with disallowed dependencies [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null depends on ["aweXpect.Reflection", "aweXpect"]
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllAssembliesDependOnlyOnAllowed_ShouldFail()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveDependenciesOnlyOn("aweXpect.Core"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in the assemblies *
					             not all have dependencies only on assemblies equal to "aweXpect.Core",
					             but it only contained assemblies depending only on the allowed assemblies [
					               aweXpect.Reflection, Version=*, Culture=neutral, PublicKeyToken=*
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAnAssemblyDependsOnDisallowedAssembly_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveDependenciesOnlyOn("aweXpect.Core"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
