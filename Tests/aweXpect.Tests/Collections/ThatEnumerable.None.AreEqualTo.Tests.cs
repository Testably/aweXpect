using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class None
	{
		public sealed partial class AreEqualTo
		{
			public sealed class Tests
			{
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(6, cts);

					async Task Act()
						=> await That(subject).None().AreEqualTo(8)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 8 for no items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [0, 1, 2, 3, 4, 5, 6, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceEnumerable subject = new();

					async Task Act()
						=> await That(subject).None().AreEqualTo(15)
							.And.None().AreEqualTo(81);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<int> subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).None().AreEqualTo(5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 5 for no items,
						             but at least 1 of at least 5 were

						             Matching items:
						             [5, (… and maybe more)]

						             Collection:
						             [1, 1, 2, 3, 5, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsEqualValues_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

					async Task Act()
						=> await That(subject).None().AreEqualTo(1);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for no items,
						             but at least 1 of at least 1 were

						             Matching items:
						             [1, (… and maybe more)]

						             Collection:
						             [1, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable((int[]) []);

					async Task Act()
						=> await That(subject).None().AreEqualTo(0);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableOnlyContainsDifferentValues_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

					async Task Act()
						=> await That(subject).None().AreEqualTo(42);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).None().AreEqualTo(0);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 0 for no items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringTests
			{
				[Test]
				public async Task AsPrefix_WhenAnItemMatchesThePattern_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["text", "# Title", "## Intro",]);

					async Task Act()
						=> await That(subject).None().AreEqualTo("#").AsPrefix();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             starts with "#" for no items,
						             but at least 1 of at least 2 were

						             Matching items:
						             [
						               "# Title",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "text",
						               "# Title",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task AsRegex_WhenAnItemMatchesThePattern_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["text", "# Title", "## Intro",]);

					async Task Act()
						=> await That(subject).None().AreEqualTo("^#").AsRegex();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             matches regex "^#" for no items,
						             but at least 1 of at least 2 were

						             Matching items:
						             [
						               "# Title",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "text",
						               "# Title",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task AsSuffix_WhenAnItemMatchesThePattern_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["text", "# Title", "## Intro",]);

					async Task Act()
						=> await That(subject).None().AreEqualTo("le").AsSuffix();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             ends with "le" for no items,
						             but at least 1 of at least 2 were

						             Matching items:
						             [
						               "# Title",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "text",
						               "# Title",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task AsWildcard_WhenAnItemMatchesThePattern_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["text", "# Title", "## Intro",]);

					async Task Act()
						=> await That(subject).None().AreEqualTo("#*").AsWildcard();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             matches "#*" for no items,
						             but at least 1 of at least 2 were

						             Matching items:
						             [
						               "# Title",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "text",
						               "# Title",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task ShouldSupportIgnoringCase()
				{
					IEnumerable<string> subject = ToEnumerable(["FOO", "BAR", "BAZ",]);

					async Task Act()
						=> await That(subject).None().AreEqualTo("bar").IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "bar" ignoring case for no items,
						             but at least 1 of at least 2 were

						             Matching items:
						             [
						               "BAR",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "FOO",
						               "BAR",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsEqualValues_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).None().AreEqualTo("bar");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "bar" for no items,
						             but at least 1 of at least 2 were

						             Matching items:
						             [
						               "bar",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "foo",
						               "bar",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IEnumerable<string> subject = [];

					async Task Act()
						=> await That(subject).None().AreEqualTo("foo");

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableOnlyContainsDifferentValues_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["FOO", "BAR", "BAZ",]);

					async Task Act()
						=> await That(subject).None().AreEqualTo("bar");

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).None().AreEqualTo("");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "" for no items,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
