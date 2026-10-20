using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).HasName("Reflection");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "Reflection",
					             but it had name "aweXpect.Reflection.Tests", which differs at index 0:
					                ↓ (actual)
					               "aweXpect.Reflection.Tests"
					               "Reflection"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenTypeHasExpectedPrefix_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).HasName("aweXpect.Reflection").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasMatchingName_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).HasName("aweXpect.Reflection.Tests");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Assembly? subject = null;

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
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).HasName("AWExPECT.rEFLECTION.tESTS").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssemblyDoesNotHaveName_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("NonExistentAssembly"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyHasName_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("aweXpect.Reflection.Tests"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "aweXpect.Reflection.Tests",
					             but it had name "aweXpect.Reflection.Tests"
					             """);
			}
		}
	}
}
