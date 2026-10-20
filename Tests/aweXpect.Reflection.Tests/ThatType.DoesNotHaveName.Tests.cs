using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotHaveName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("Abstract");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasDifferentName_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("NonExistentClass");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasMatchingName_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("PublicAbstractClass");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "PublicAbstractClass",
					             but it had name "PublicAbstractClass"
					             """);
			}

			[Test]
			public async Task WhenTypeHasMatchingSuffix_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("AbstractClass").AsSuffix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not ending with "AbstractClass",
					             but it had name "PublicAbstractClass"
					             """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotHaveName("foo");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeMatchesIgnoringCase_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("pUBLICaBSTRACTcLASS").IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "pUBLICaBSTRACTcLASS" ignoring case,
					             but it had name "PublicAbstractClass"
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeHasDifferentName_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveName("NonExistentClass"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "NonExistentClass",
					             but it had name "PublicAbstractClass", which differs at index 0:
					                ↓ (actual)
					               "PublicAbstractClass"
					               "NonExistentClass"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenTypeHasName_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveName("PublicAbstractClass"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
