using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotRequired
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldsContainRequiredFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

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

			[Test]
			public async Task WhenFilteringOnlyNonRequiredFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(BaseClassWithMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldsContainRequiredFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRequired());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyNonRequiredFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(BaseClassWithMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

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
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenFieldsContainRequiredFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

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

			[Test]
			public async Task WhenFilteringOnlyNonRequiredFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(BaseClassWithMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
