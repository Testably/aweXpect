using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).HasName("Abstract");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "Abstract",
					             but it had name "PublicAbstractClass", which differs at index 0:
					                ↓ (actual)
					               "PublicAbstractClass"
					               "Abstract"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenTypeHasExpectedPrefix_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).HasName("PublicAbstract").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasMatchingName_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).HasName("PublicAbstractClass");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasName("foo");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has name equal to "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeMatchesIgnoringCase_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).HasName("pUBLICaBSTRACTcLASS").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeDoesNotHaveName_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("NonExistentClass"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasName_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("PublicAbstractClass"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "PublicAbstractClass",
					             but it had name "PublicAbstractClass"
					             """);
			}
		}
	}
}
