using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class HasNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).HasNamespace("Reflection.Tests.TestHelpers");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has namespace equal to "Reflection.Tests.TestHelpers",
					             but it had namespace "aweXpect.Reflection.Tests.Test…", which differs at index 0:
					                ↓ (actual)
					               "aweXpect.Reflection.Tests.TestHelpers.Types"
					               "Reflection.Tests.TestHelpers"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenTypeHasExpectedPrefix_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).HasNamespace("aweXpect.Reflection.Tests").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasMatchingNamespace_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).HasNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasNamespace("foo");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has namespace equal to "foo",
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeDoesNotHaveNamespace_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasNamespace("SomeOtherNamespace"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasNamespace_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.HasNamespace("aweXpect.Reflection.Tests.TestHelpers.Types"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has namespace not equal to "aweXpect.Reflection.Tests.Test…",
					             but it had namespace "aweXpect.Reflection.Tests.TestHelpers.Types"
					             """)
					.Because("the negated match type already says \"not\", so the verb must not be negated again");
			}
		}
	}
}
