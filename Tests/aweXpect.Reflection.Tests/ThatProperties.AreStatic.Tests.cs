using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyStaticProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithStaticMembers)
					.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreStatic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonStaticProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithStaticMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					               BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all static,
					             but it contained non-static properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyStaticProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithStaticMembers)
					.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreStatic());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all static,
					             but it only contained static properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNonStaticProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithStaticMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					               BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreStatic());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
