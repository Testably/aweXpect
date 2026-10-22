using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotHaveNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenNamespaceIsOnlyParentNamespace_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveNamespace("aweXpect.Reflection.Tests");
				}

				await That(Act).DoesNotThrow()
					.Because("unlike IsNotWithinNamespace, the whole namespace is compared as a string");
			}

			[Test]
			public async Task WhenTypeHasMatchingNamespace_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has namespace not equal to "aweXpect.Reflection.Tests.Test…",
					             but it had namespace "aweXpect.Reflection.Tests.TestHelpers.Types"
					             """);
			}

			[Test]
			public async Task WhenTypeHasMatchingPrefix_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveNamespace("aweXpect.Reflection").AsPrefix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has namespace not starting with "aweXpect.Reflection",
					             but it had namespace "aweXpect.Reflection.Tests.TestHelpers.Types"
					             """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotHaveNamespace("aweXpect");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has namespace not equal to "aweXpect",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeMatchesIgnoringCase_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveNamespace("AWEXPECT.REFLECTION.TESTS.TESTHELPERS.TYPES")
						.IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has namespace not equal to "AWEXPECT.REFLECTION.TESTS.TEST…" ignoring case,
					             but it had namespace "aweXpect.Reflection.Tests.TestHelpers.Types"
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeHasDifferentNamespace_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveNamespace("aweXpect"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has namespace equal to "aweXpect",
					             but it had namespace "aweXpect.Reflection.Tests.Test…" with a length of 43, which is longer than the expected length of 8 and has superfluous:
					               ".Reflection.Tests.TestHelpers.Types"
					             """);
			}

			[Test]
			public async Task WhenTypeHasNamespace_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.DoesNotHaveNamespace("aweXpect.Reflection.Tests.TestHelpers.Types"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
