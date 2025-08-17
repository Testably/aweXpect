using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class HasNamespace
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).HasNamespace("Reflection.Tests.TestHelpers");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has namespace equal to "Reflection.Tests.TestHelpers",
					             but it was "aweXpect.Reflection.Tests.Test…" which differs at index 0:
					                ↓ (actual)
					               "aweXpect.Reflection.Tests.TestHelpers.Types"
					               "Reflection.Tests.TestHelpers"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenTypeHasExpectedPrefix_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).HasNamespace("aweXpect.Reflection.Tests").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasMatchingNamespace_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).HasNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).HasNamespace("foo");

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has namespace equal to "foo",
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeDoesNotHaveNamespace_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasNamespace("SomeOtherNamespace"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasNamespace_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasNamespace("aweXpect.Reflection.Tests.TestHelpers.Types"));

				await That(Act).Throws<XunitException>()
					.WithMessage("*does not have namespace*aweXpect.Reflection.Tests.TestHelpers.Types*").AsWildcard();
			}
		}
	}
}
