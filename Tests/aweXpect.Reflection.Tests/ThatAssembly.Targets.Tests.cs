using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class Targets
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
			[Test]
			public async Task WhenAssemblyDoesNotTargetTheFramework_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).Targets("net99.0");
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              targets equal to "net99.0",
					              but it had target framework "{CurrentTarget}", which differs *
					              """).AsWildcard();
			}

			[Test]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
				{
					await That(subject).Targets("net8.0");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             targets equal to "net8.0",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenAssemblyMatchesIgnoringCase_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).Targets(CurrentTarget.ToUpperInvariant()).IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyTargetsTheFramework_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).Targets(CurrentTarget);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssemblyDoesNotTargetTheFramework_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Targets("net99.0"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyTargetsTheFramework_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Targets(CurrentTarget));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              targets not equal to "{CurrentTarget}",
					              but it had target framework "{CurrentTarget}"
					              """);
			}
		}
	}
}
