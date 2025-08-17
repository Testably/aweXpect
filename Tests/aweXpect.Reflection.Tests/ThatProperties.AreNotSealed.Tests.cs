using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonSealedProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotSealed();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainSealedProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not sealed,
					             but it contained sealed properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonSealedProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotSealed());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a sealed property,
					             but it only contained non-sealed properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainSealedProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotSealed());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
