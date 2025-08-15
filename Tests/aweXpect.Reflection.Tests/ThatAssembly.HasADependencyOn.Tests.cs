using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class HasADependencyOn
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssemblyDoesNotHaveADependency_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
					=> await That(subject).HasADependencyOn("NonExistentAssembly");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a dependency on assembly equal to "NonExistentAssembly",
					             but it did not have the required dependency in [*]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssemblyHasADependency_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
					=> await That(subject).HasADependencyOn("aweXpect.Core");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
					=> await That(subject).HasADependencyOn("aweXpect.Core");

				await That(Act).ThrowsException()
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
					=> await That(subject).HasADependencyOn("System").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDependencyMatchesIgnoringCase_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
					=> await That(subject).HasADependencyOn("AWExPECT.cORE").IgnoringCase();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssemblyDoesNotHaveADependency_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasADependencyOn("NonExistentAssembly"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyHasADependency_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasADependencyOn("aweXpect.Core"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no dependency on assembly equal to "aweXpect.Core",
					             but it had the unexpected dependency in [*]
					             """)
					.AsWildcard();
			}
		}
	}
}
