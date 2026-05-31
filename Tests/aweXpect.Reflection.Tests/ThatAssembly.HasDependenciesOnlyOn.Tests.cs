using System.Reflection;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class HasDependenciesOnlyOn
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
			public async Task WhenAllowedMatchesAsWildcard_ShouldSucceed()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).HasDependenciesOnlyOn("aweXpect.*").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllowedMatchesIgnoringCase_ShouldSucceed()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).HasDependenciesOnlyOn("AWExPECT.cORE").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyDependsOnDisallowedAssembly_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).HasDependenciesOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core",
					             but it also had dependencies on [*]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssemblyDependsOnlyOnAllowed_ShouldSucceed()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).HasDependenciesOnlyOn("aweXpect.Core");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
				{
					await That(subject).HasDependenciesOnlyOn("aweXpect.Core");
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core",
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssemblyDependsOnDisallowedAssembly_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasDependenciesOnlyOn("aweXpect.Core"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyDependsOnlyOnAllowed_ShouldFail()
			{
				using CustomizationLifetime _ = ExcludeFrameworkAssemblies();

				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasDependenciesOnlyOn("aweXpect.Core"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have dependencies only on assemblies equal to "aweXpect.Core",
					             but it only had dependencies on the allowed assemblies
					             """);
			}
		}
	}
}
