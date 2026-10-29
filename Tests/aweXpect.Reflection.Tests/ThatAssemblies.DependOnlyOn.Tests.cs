using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed partial class DependOnlyOn
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllAssembliesDependOnlyOnAllowed_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).DependOnlyOn("aweXpect.Core");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllowedAssembliesAreGiven_ShouldDescribeAssemblies()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOnlyOn("First", "Second");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have dependencies only on assemblies equal to "First" or equal to "Second",
					             but none of 1 did*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAllowedMatchesAsWildcard_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).DependOnlyOn("aweXpect.*").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAnAssemblyDependsOnDisallowedAssembly_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOnlyOn("aweXpect.Core");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have dependencies only on assemblies equal to "aweXpect.Core",
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null depends on ["aweXpect.Reflection", "Mono.Cecil", "AutoFixture", "aweXpect"]
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null depends on ["aweXpect.Reflection", "Mono.Cecil", "AutoFixture", "aweXpect"]
					             ]
					             """).AsWildcard();
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have dependencies only on assemblies equal to "aweXpect.Core",
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableContainsAllDependencies_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);
				List<string> expected = ["aweXpect.Core",];

				async Task Act()
				{
					await That(subject).DependOnlyOn(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableDoesNotContainADependency_ShouldFail()
			{
				IEnumerable<Assembly?> subject = [typeof(In).Assembly,];
				List<string> expected = ["First", "Second",];

				async Task Act()
				{
					await That(subject).DependOnlyOn(expected);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have dependencies only on assemblies equal to "First" or equal to "Second",
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection, Version=* depends on ["aweXpect.Core"]
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=* depends on ["aweXpect.Core"]
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<Assembly?> subject = [typeof(In).Assembly,];

				async Task Act()
				{
					await That(subject).DependOnlyOn((IEnumerable<string>)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenMultipleAssembliesHaveDisallowedDependencies_ShouldSeparateWithComma()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly, typeof(ThatAssemblies).Assembly);

				async Task Act()
				{
					await That(subject).DependOnlyOn("NonExistentAssembly");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in the assemblies *
					             all have dependencies only on assemblies equal to "NonExistentAssembly",
					             but none of 2 did

					             Not matching items:
					             [
					               aweXpect.Reflection, Version=* depends on *,
					               aweXpect.Reflection.Tests, Version=* depends on *
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=* depends on *,
					               aweXpect.Reflection.Tests, Version=* depends on *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNoAllowedAssembliesAreGiven_ShouldDescribeNoAssemblies()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOnlyOn();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have dependencies only on no assemblies,
					             but none of 1 did*
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllAssembliesDependOnlyOnAllowed_ShouldFail()
			{
				Filtered.Assemblies subject = In.Assemblies(typeof(In).Assembly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DependOnlyOn("aweXpect.Core"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in the assemblies *
					             not all have dependencies only on assemblies equal to "aweXpect.Core",
					             but all 1 did

					             Collection:
					             [
					               aweXpect.Reflection, Version=*, Culture=neutral, PublicKeyToken=*
					             ]
					             """).AsWildcard();
			}

			[Test]
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

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenEnumerableContainsAllDependencies_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();
				List<string> expected = ["aweXpect.Core",];

				async Task Act()
				{
					await That(subject).DependOnlyOn(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableDoesNotContainADependency_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();
				List<string> expected = ["First", "Second",];

				async Task Act()
				{
					await That(subject).DependOnlyOn(expected);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have dependencies only on assemblies equal to "First" or equal to "Second",
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection, Version=* depends on ["aweXpect.Core"]
					             ]

					             Collection:
					             [
					               aweXpect.Reflection, Version=* depends on ["aweXpect.Core"]
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).DependOnlyOn((IEnumerable<string>)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}
		}
#endif
	}
}
