using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlySealedProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(p => p.Name == nameof(ClassWithSealedMembers.VirtualProperty));

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonSealedProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all sealed,
					             but it contained non-sealed properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlySealedProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithSealedMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(p => p.Name == nameof(ClassWithSealedMembers.VirtualProperty));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreSealed());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all sealed,
					             but it only contained sealed properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNonSealedProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreSealed());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
