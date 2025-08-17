using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class AreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorsContainStaticConstructors_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					                 BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not static,
					             but it contained static constructors [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonStaticConstructors_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorsContainStaticConstructors_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					                 BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonStaticConstructors_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a static constructor,
					             but it only contained non-static constructors [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
