using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class DoNotDependOn
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesHaveADependency_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotDependOn("aweXpect.Core");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             none have dependency on assembly equal to "aweXpect.Core",
					             but 1 of 1 did

					             Matching items:
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
			public async Task WhenAssembliesHaveNoDependency_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotDependOn("NonExistentAssembly");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Assembly?> subject = [Assembly.GetExecutingAssembly(), null,];

				async Task Act()
				{
					await That(subject).DoNotDependOn("NonExistentAssembly");
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotDependOn("NonExistentAssembly"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have dependency on assembly equal to "NonExistentAssembly",
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
					             at least one has dependency on assembly equal to "NonExistentAssembly",
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard().Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoDependencyMatchesAsPrefix_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotDependOn("System").AsPrefix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             none have dependency on assembly starting with "System",
					             but 1 of 1 did

					             Matching items:
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
			public async Task WhenNoDependencyMatchesIgnoringCase_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoNotDependOn("AWExPECT.cORE").IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             none have dependency on assembly equal to "AWExPECT.cORE" ignoring case,
					             but 1 of 1 did

					             Matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesHaveADependency_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotDependOn("aweXpect.Core"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesHaveNoDependency_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotDependOn("NonExistentAssembly"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             at least one has dependency on assembly equal to "NonExistentAssembly",
					             but none of 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """)
					.AsWildcard();
			}
		}
	}
}
