using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class Target
	{
#if NET8_0
		private const string CurrentTarget = "net8.0";
#elif NET10_0
		private const string CurrentTarget = "net10.0";
#else
		private const string CurrentTarget = "net48";
#endif

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllAssembliesTargetTheFramework_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).Target(CurrentTarget);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssembliesDoNotTargetTheFramework_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).Target("net99.0");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all target equal to "net99.0",
					             but it contained not matching assemblies *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMatchingIgnoringCase_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).Target(CurrentTarget.ToUpperInvariant()).IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllAssembliesTargetTheFramework_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Target(CurrentTarget));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that in assembly containing type PublicAbstractClass
					              not all target equal to "{CurrentTarget}",
					              but it only contained matching assemblies *
					              """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssembliesDoNotTargetTheFramework_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Target("net99.0"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
