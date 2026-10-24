using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotRequired
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyNonRequiredProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainRequiredProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are required,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlyNonRequiredProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRequired());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is required,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainRequiredProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRequired());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenFilteringOnlyNonRequiredProperties_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainRequiredProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are required,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
