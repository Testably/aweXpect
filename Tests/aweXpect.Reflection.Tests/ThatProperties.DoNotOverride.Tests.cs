using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class DoNotOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonOverridingProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoNotOverride();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainOverridingProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoNotOverride();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all do not override a base property,
					             but it contained properties which override a base property [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonOverridingProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotOverride());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a property which overrides a base property,
					             but it only contained properties which do not override a base property [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainOverridingProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotOverride());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonOverridingProperties_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).DoNotOverride();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainOverridingProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).DoNotOverride();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all do not override a base property,
					             but it contained properties which override a base property [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
