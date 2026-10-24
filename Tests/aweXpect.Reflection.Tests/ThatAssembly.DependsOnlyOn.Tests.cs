using System.Collections.Generic;
using System.Reflection;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class DependsOnlyOn
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllowedMatchesAsWildcard_ShouldSucceed()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("aweXpect.*").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllowedMatchesIgnoringCase_ShouldSucceed()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("AWExPECT.cORE").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyDependsOnDisallowedAssembly_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core",
					             but it had disallowed dependencies

					             Disallowed dependencies:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssemblyDependsOnlyOnAllowed_ShouldSucceed()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("aweXpect.Core");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenCustomizedPrefixEndsWithDot_ShouldExcludeMatchingAssemblies()
			{
				Assembly subject = typeof(In).Assembly;

				// A customized prefix in the natural trailing-dot form ("aweXpect.") must exclude the
				// aweXpect.Core reference at the (then explicit) name-segment boundary.
				using (Customize.aweXpect.Settings().ExcludedAssemblyPrefixes
					       .Set(["System", "netstandard", "mscorlib", "Microsoft", "aweXpect.",]))
				{
					async Task Act()
					{
						await That(subject).DependsOnlyOn();
					}

					await That(Act).DoesNotThrow();
				}
			}

			[Test]
			public async Task WhenEnumerableContainsAllDependencies_ShouldSucceed()
			{
				Assembly subject = typeof(In).Assembly;
				List<string> expected = ["aweXpect.Core",];

				async Task Act()
				{
					await That(subject).DependsOnlyOn(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableDoesNotContainADependency_ShouldFail()
			{
				Assembly subject = typeof(In).Assembly;
				List<string> expected = ["First", "Second",];

				async Task Act()
				{
					await That(subject).DependsOnlyOn(expected);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "First" or equal to "Second",
					             but it had disallowed dependencies

					             Disallowed dependencies:
					             [
					               "aweXpect.Core"
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsNull_ShouldThrowArgumentNullException()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn((IEnumerable<string>)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenMultipleAllowedAssembliesAreGiven_ShouldDescribeAssemblies()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn("First", "Second");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "First" or equal to "Second",
					             but it had disallowed dependencies

					             Disallowed dependencies:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNoAllowedAssembliesAreGiven_ShouldDescribeNoAssemblies()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DependsOnlyOn();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on no assemblies,
					             but it had disallowed dependencies

					             Disallowed dependencies:
					             [
					               "aweXpect.Core"
					             ]
					             """);
			}

			[Test]
			public async Task WhenUsedForItems_ShouldLabelTheDisallowedDependenciesWithTheItem()
			{
				Assembly[] subject = [typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,];

				async Task Act()
				{
					await That(subject).All().ComplyWith(assembly => assembly.DependsOnlyOn("aweXpect.Core"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has dependencies only on assemblies equal to "aweXpect.Core" for all items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests*
					             ]

					             Collection:
					             [
					               aweXpect.Reflection,*
					               aweXpect.Reflection.Tests*
					             ]

					             Disallowed dependencies (item [1]):
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenUsedInThatAll_ShouldListTheDisallowedDependenciesOfEachFailure()
			{
				Assembly reflectionAssembly = typeof(In).Assembly;

				async Task Act()
				{
					await ThatAll(
						That(reflectionAssembly).DependsOnlyOn(),
						That(reflectionAssembly).DependsOnlyOn("aweXpect.Core"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected all of the following to succeed:
					              [01] Expected that reflectionAssembly has dependencies only on no assemblies
					              [02] Expected that reflectionAssembly has dependencies only on assemblies equal to "aweXpect.Core"
					             but
					              [01] it had disallowed dependencies

					             [01] Disallowed dependencies:
					             [
					               "aweXpect.Core"
					             ]
					             """);
			}

			[Test]
			public async Task WhenUsedInWhose_ShouldLabelTheDisallowedDependenciesWithTheMember()
			{
				AssemblyHolder subject = new(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).Whose(holder => holder.Assembly, assembly => assembly.DependsOnlyOn());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Assembly has dependencies only on no assemblies,
					             but Assembly had disallowed dependencies

					             Disallowed dependencies (Assembly):
					             [
					               "aweXpect.Core"
					             ]
					             """);
			}

			private sealed class AssemblyHolder(Assembly assembly)
			{
				public Assembly Assembly { get; } = assembly;
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssemblyDependsOnDisallowedAssembly_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOnlyOn("aweXpect.Core"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyDependsOnlyOnAllowed_ShouldFail()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DependsOnlyOn("aweXpect.Core"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have dependencies only on assemblies equal to "aweXpect.Core",
					             but it only had dependencies on the allowed assemblies
					             """);
			}
		}
	}
}
