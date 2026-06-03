using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonVirtualProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainVirtualProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not virtual,
					             but it contained virtual properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonVirtualProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotVirtual());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a virtual property,
					             but it only contained non-virtual properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainVirtualProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotVirtual());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonVirtualProperties_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainVirtualProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not virtual,
					             but it contained virtual properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
