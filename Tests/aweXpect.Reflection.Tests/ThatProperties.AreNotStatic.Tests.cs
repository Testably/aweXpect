using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonStaticProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithStaticMembers)
					.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainStaticProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithStaticMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					               BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not static,
					             but it contained static properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonStaticProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithStaticMembers)
					.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a static property,
					             but it only contained non-static properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainStaticProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithStaticMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					               BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
