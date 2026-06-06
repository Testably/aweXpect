using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreRequired
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldsContainNonRequiredFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all required,
					             but it contained non-required fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyRequiredFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreRequired();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldsContainNonRequiredFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRequired());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyRequiredFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRequired());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all required,
					             but it only contained required fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFieldsContainNonRequiredFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(ClassWithRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all required,
					             but it contained non-required fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyRequiredFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreRequired();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
