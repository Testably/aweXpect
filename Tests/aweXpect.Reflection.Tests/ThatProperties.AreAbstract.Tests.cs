using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyAbstractProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(p => p.GetGetMethod()?.IsAbstract == true || p.GetSetMethod()?.IsAbstract == true);

				async Task Act()
					=> await That(subject).AreAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonAbstractProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all abstract,
					             but it contained non-abstract properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyAbstractProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(p => p.GetGetMethod()?.IsAbstract == true || p.GetSetMethod()?.IsAbstract == true);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreAbstract());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all abstract,
					             but it only contained abstract properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNonAbstractProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(AbstractClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreAbstract());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
