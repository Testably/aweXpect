using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonAbstractProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(p => p.GetGetMethod()?.IsAbstract != true && p.GetSetMethod()?.IsAbstract != true);

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainAbstractProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not abstract,
					             but it contained abstract properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonAbstractProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(p => p.GetGetMethod()?.IsAbstract != true && p.GetSetMethod()?.IsAbstract != true);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an abstract property,
					             but it only contained non-abstract properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainAbstractProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
