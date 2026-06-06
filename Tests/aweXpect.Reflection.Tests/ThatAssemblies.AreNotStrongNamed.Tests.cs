using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class AreNotStrongNamed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllAssembliesAreNotStrongNamed_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).AreNotStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssembliesContainStrongNamedAssemblies_ShouldFail()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).AreNotStrongNamed();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not strong named,
					             but it contained strong named assemblies [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllAssembliesAreNotStrongNamed_ShouldFail()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStrongNamed());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a strong named assembly,
					             but it only contained not strong named assemblies [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssembliesContainStrongNamedAssemblies_ShouldSucceed()
			{
				IEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStrongNamed());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenAllAssembliesAreNotStrongNamed_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(PublicAbstractClass).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AreNotStrongNamed();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssembliesContainStrongNamedAssemblies_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subject = new[]
				{
					typeof(In).Assembly, typeof(PublicAbstractClass).Assembly,
				}.ToTestAsyncEnumerable<Assembly?>();

				async Task Act()
				{
					await That(subject).AreNotStrongNamed();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not strong named,
					             but it contained strong named assemblies [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
