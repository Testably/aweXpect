using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class Override
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyOverridingProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonOverridingProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all override a base property,
					             but it contained properties which do not override a base property [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyOverridingProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Override());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             do not all override a base property,
					             but it only contained properties which override a base property [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNonOverridingProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Override());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyOverridingProperties_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonOverridingProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all override a base property,
					             but it contained properties which do not override a base property [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
