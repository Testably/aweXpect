using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class HaveADependencyOn
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesDoNotHaveADependency_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
					=> await That(subject).HaveADependencyOn("NonExistentAssembly");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have dependency on assembly equal to "NonExistentAssembly",
					             but it contained assemblies without the required dependency [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssembliesHaveADependency_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
					=> await That(subject).HaveADependencyOn("aweXpect.Core");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependencyMatchesAsPrefix_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
					=> await That(subject).HaveADependencyOn("aweX").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependencyMatchesIgnoringCase_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
					=> await That(subject).HaveADependencyOn("aweXpect.Core").IgnoringCase();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesDoNotHaveADependency_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.HaveADependencyOn("NonExistentAssembly"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssembliesHaveADependency_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.HaveADependencyOn("aweXpect.Core"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have dependency on assembly not equal to "aweXpect.Core",
					             but it only contained assemblies with the unexpected dependency [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """)
					.AsWildcard();
			}
		}
	}
}
