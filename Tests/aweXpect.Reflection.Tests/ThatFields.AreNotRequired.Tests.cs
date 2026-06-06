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
			[Fact]
			public async Task WhenFieldsContainRequiredFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not required,
					             but it contained required fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

			[Fact]
			public async Task WhenFilteringOnlyNonRequiredFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(BaseClassWithMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRequired());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a required field,
					             but it only contained non-required fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFieldsContainRequiredFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not required,
					             but it contained required fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
