using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class AreStrongNamed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllAssembliesAreStrongNamed_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(object).Assembly,
				};

				async Task Act()
				{
					await That(subject).AreStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssembliesContainNotStrongNamedAssemblies_ShouldFail()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).AreStrongNamed();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all strong named,
					             but it contained not strong named assemblies [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllAssembliesAreStrongNamed_ShouldFail()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(object).Assembly,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStrongNamed());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all strong named,
					             but it only contained strong named assemblies [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssembliesContainNotStrongNamedAssemblies_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStrongNamed());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenAllAssembliesAreStrongNamed_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(object).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AreStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssembliesContainNotStrongNamedAssemblies_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AreStrongNamed();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all strong named,
					             but it contained not strong named assemblies [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
