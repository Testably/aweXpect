using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class DependsOn
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssemblyDoesNotHaveADependency_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DependsOn("NonExistentAssembly");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a dependency on assembly equal to "NonExistentAssembly",
					             but it did not

					             Dependencies:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssemblyHasADependency_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DependsOn("aweXpect.Core");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
				{
					await That(subject).DependsOn("aweXpect.Core");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has a dependency on assembly equal to "aweXpect.Core",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenDependencyMatchesAsPrefix_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DependsOn("System").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependencyMatchesIgnoringCase_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DependsOn("AWExPECT.cORE").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedInWhose_ShouldLabelTheDependenciesWithTheMember()
			{
				AssemblyHolder subject = new(typeof(PublicAbstractClass).Assembly);

				async Task Act()
				{
					await That(subject).Whose(holder => holder.Assembly,
						assembly => assembly.DependsOn("NonExistentAssembly"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Assembly has a dependency on assembly equal to "NonExistentAssembly",
					             but Assembly did not

					             Dependencies (Assembly):
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			private sealed class AssemblyHolder(Assembly assembly)
			{
				public Assembly Assembly { get; } = assembly;
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssemblyDoesNotHaveADependency_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOn("NonExistentAssembly"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyHasADependency_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOn("aweXpect.Core"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no dependency on assembly equal to "aweXpect.Core",
					             but it did

					             Dependencies:
					             [
					               *
					             ]
					             """)
					.AsWildcard();
			}
		}
	}
}
