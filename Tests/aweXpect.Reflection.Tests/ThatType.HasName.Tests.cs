using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).HasName("Abstract");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "Abstract",
					             but it was "PublicAbstractClass" which differs at index 0:
					                ↓ (actual)
					               "PublicAbstractClass"
					               "Abstract"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenTypeHasExpectedPrefix_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).HasName("PublicAbstract").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasMatchingName_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).HasName("PublicAbstractClass");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).HasName("foo");

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has name equal to "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeMatchesIgnoringCase_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).HasName("pUBLICaBSTRACTcLASS").IgnoringCase();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
