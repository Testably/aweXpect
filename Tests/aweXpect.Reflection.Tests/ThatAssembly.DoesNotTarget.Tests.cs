using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class DoesNotTarget
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
			public async Task WhenAssemblyDoesNotTargetTheFramework_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotTarget("net99.0");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotTarget("net8.0");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             targets not equal to "net8.0",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenAssemblyMatchesAsPrefix_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotTarget("net").AsPrefix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              targets not starting with "net",
					              but it had target framework "{CurrentTarget}"
					              """);
			}

			[Test]
			public async Task WhenAssemblyMatchesIgnoringCase_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotTarget(CurrentTarget.ToUpperInvariant()).IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              targets not equal to "{CurrentTarget.ToUpperInvariant()}" ignoring case,
					              but it had target framework "{CurrentTarget}"
					              """);
			}

			[Test]
			public async Task WhenAssemblyTargetsTheFramework_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotTarget(CurrentTarget);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              targets not equal to "{CurrentTarget}",
					              but it had target framework "{CurrentTarget}"
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssemblyDoesNotTargetTheFramework_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotTarget("net99.0"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              targets equal to "net99.0",
					              but it had target framework "{CurrentTarget}", which differs *
					              """).AsWildcard();
			}

			[Test]
			public async Task WhenAssemblyTargetsTheFramework_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotTarget(CurrentTarget));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
