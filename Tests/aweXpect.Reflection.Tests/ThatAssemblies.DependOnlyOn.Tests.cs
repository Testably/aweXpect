using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class DependOnlyOn
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllAssembliesDependOnlyOnAllowed_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).DependOnlyOn("aweXpect.Core");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllowedAssembliesAreGiven_ShouldDescribeAssemblies()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOnlyOn("First", "Second");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have dependencies only on assemblies equal to "First" or equal to "Second",
					             but it contained assemblies with disallowed dependencies *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAllowedMatchesAsWildcard_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).DependOnlyOn("aweXpect.*").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAnAssemblyDependsOnDisallowedAssembly_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOnlyOn("aweXpect.Core");
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

			[Fact]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				IEnumerable<Assembly?> subject = new Assembly?[]
				{
					null,
				};

				async Task Act()
				{
					await That(subject).DependOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have dependencies only on assemblies equal to "aweXpect.Core",
					             but it contained assemblies with disallowed dependencies [
					               <null>
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMultipleAssembliesHaveDisallowedDependencies_ShouldSeparateWithComma()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly, typeof(ThatAssemblies).Assembly);

				async Task Act()
				{
					await That(subject).DependOnlyOn("NonExistentAssembly");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in the assemblies *
					             all have dependencies only on assemblies equal to "NonExistentAssembly",
					             but it contained assemblies with disallowed dependencies [
					               aweXpect.Reflection, Version=* depends on *,
					               aweXpect.Reflection.Tests, Version=* depends on *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNoAllowedAssembliesAreGiven_ShouldDescribeNoAssemblies()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOnlyOn();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have dependencies only on no assemblies,
					             but it contained assemblies with disallowed dependencies *
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllAssembliesDependOnlyOnAllowed_ShouldFail()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DependOnlyOn("aweXpect.Core"));
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
					await That(subject).DoesNotComplyWith(they => they.DependOnlyOn("aweXpect.Core"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
