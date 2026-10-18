using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class IsNotStrongNamed
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssemblyIsNotStrongNamed_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).IsNotStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
				{
					await That(subject).IsNotStrongNamed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not strong named,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenAssemblyIsStrongNamed_ShouldFail()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).IsNotStrongNamed();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is not strong named,
					              but it was strong named {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssemblyIsNotStrongNamed_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotStrongNamed());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is strong named,
					              but it was not strong named {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenAssemblyIsStrongNamed_ShouldSucceed()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotStrongNamed());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
