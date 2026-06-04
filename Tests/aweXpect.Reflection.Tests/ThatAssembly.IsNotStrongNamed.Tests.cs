using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class IsNotStrongNamed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssemblyIsNotStrongNamed_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).IsNotStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
				{
					await That(subject).IsNotStrongNamed();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not strong named,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenAssemblyIsStrongNamed_ShouldFail()
			{
				Assembly subject = typeof(In).Assembly;

				async Task Act()
				{
					await That(subject).IsNotStrongNamed();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not strong named,
					              but it was strong named {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssemblyIsNotStrongNamed_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotStrongNamed());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is strong named,
					              but it was not strong named {Formatter.Format(subject)}
					              """);
			}

			[Fact]
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
