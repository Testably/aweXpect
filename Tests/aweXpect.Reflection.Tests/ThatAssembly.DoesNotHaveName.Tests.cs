using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class DoesNotHaveName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssemblyHasDifferentName_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotHaveName("NonExistentAssembly");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssemblyHasMatchingName_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotHaveName("aweXpect.Reflection.Tests");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "aweXpect.Reflection.Tests",
					             but it had name "aweXpect.Reflection.Tests"
					             """);
			}

			[Test]
			public async Task WhenAssemblyHasMatchingSuffix_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotHaveName(".Tests").AsSuffix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not ending with ".Tests",
					             but it had name "aweXpect.Reflection.Tests"
					             """);
			}

			[Test]
			public async Task WhenAssemblyIsNull_ShouldFail()
			{
				Assembly? subject = null;

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
			public async Task WhenAssemblyMatchesIgnoringCase_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotHaveName("AWExPECT.rEFLECTION.tESTS").IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "AWExPECT.rEFLECTION.tESTS" ignoring case,
					             but it had name "aweXpect.Reflection.Tests"
					             """);
			}

			[Test]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotHaveName("Reflection");
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssemblyHasDifferentName_ShouldFail()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveName("NonExistentAssembly"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "NonExistentAssembly",
					             but it had name "aweXpect.Reflection.Tests", which differs at index 0:
					                ↓ (actual)
					               "aweXpect.Reflection.Tests"
					               "NonExistentAssembly"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenAssemblyHasName_ShouldSucceed()
			{
				Assembly subject = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveName("aweXpect.Reflection.Tests"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
