using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotHaveName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("Abstract");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasDifferentName_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("NonExistentClass");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasMatchingName_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("PublicAbstractClass");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "PublicAbstractClass",
					             but it was "PublicAbstractClass"
					             """);
			}

			[Fact]
			public async Task WhenTypeHasMatchingSuffix_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("AbstractClass").AsSuffix();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name not ending with "AbstractClass",
					             but it was "PublicAbstractClass"
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotHaveName("foo");
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeMatchesIgnoringCase_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveName("pUBLICaBSTRACTcLASS").IgnoringCase();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "pUBLICaBSTRACTcLASS" ignoring case,
					             but it was "PublicAbstractClass"
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeHasDifferentName_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveName("NonExistentClass"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "NonExistentClass",
					             but it was "PublicAbstractClass" which differs at index 0:
					                ↓ (actual)
					               "PublicAbstractClass"
					               "NonExistentClass"
					                ↑ (expected)
					             """);
			}

			[Fact]
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
