using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class DependOn
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesDoNotHaveADependency_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOn("NonExistentAssembly");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have dependency on assembly equal to "NonExistentAssembly",
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAssembliesHaveADependency_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOn("aweXpect.Core");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDependencyMatchesAsPrefix_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOn("aweX").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDependencyMatchesIgnoringCase_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DependOn("aweXpect.Core").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Assembly?> subject = [Assembly.GetExecutingAssembly(), null,];

				async Task Act()
				{
					await That(subject).DependOn("aweXpect.Core");
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DependOn("aweXpect.Core"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have dependency on assembly equal to "aweXpect.Core",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard();
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have dependency on assembly equal to "aweXpect.Core",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard().Because("a null item cannot be inspected, like a null subject");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesDoNotHaveADependency_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DependOn("NonExistentAssembly"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesHaveADependency_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DependOn("aweXpect.Core"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have dependency on assembly equal to "aweXpect.Core",
					             but all 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """)
					.AsWildcard()
					.Because("\"not all\" already negates the expectation, so the dependency must not be negated again");
			}
		}
	}
}
