using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldsContainStaticFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not static,
					             but it contained static fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonStaticFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldsContainStaticFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonStaticFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a static field,
					             but it only contained non-static fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
