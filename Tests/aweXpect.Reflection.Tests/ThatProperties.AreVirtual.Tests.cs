using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyVirtualProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonVirtualProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all virtual,
					             but it contained non-virtual properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyVirtualProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreVirtual());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all virtual,
					             but it only contained virtual properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNonVirtualProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreVirtual());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyVirtualProperties_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonVirtualProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all virtual,
					             but it contained non-virtual properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
