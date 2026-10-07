#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEqualTo
		{
			public sealed class ImmutableTests
			{
				[Test]
				[Arguments(double.NaN, false)]
				[Arguments(1.0, true)]
				public async Task DoubleNaNValues_ShouldBeConsideredEqual(double additionalValue, bool expectFailure)
				{
					ImmutableArray<double> subject = [double.NaN, double.NaN, additionalValue,];

					async Task Act()
						=> await That(subject).All().AreEqualTo(double.NaN);

					await That(Act).Throws<FailException>().OnlyIf(expectFailure)
						.WithMessage("""
						             Expected that subject
						             is equal to NaN for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [1.0]

						             Collection:
						             [NaN, NaN, 1.0]
						             """);
				}

				[Test]
				[Arguments(float.NaN, false)]
				[Arguments(1.0F, true)]
				public async Task FloatNaNValues_ShouldBeConsideredEqual(float additionalValue, bool expectFailure)
				{
					ImmutableArray<float> subject = [float.NaN, float.NaN, additionalValue,];

					async Task Act()
						=> await That(subject).All().AreEqualTo(float.NaN);

					await That(Act).Throws<FailException>().OnlyIf(expectFailure)
						.WithMessage("""
						             Expected that subject
						             is equal to NaN for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [1.0]

						             Collection:
						             [NaN, NaN, 1.0]
						             """);
				}
			}

			public sealed class ImmutableItemTests
			{
				[Test]
				public async Task ShouldSupportNullableValues()
				{
					ImmutableArray<int?> subject = [..Factory.GetConstantValueEnumerable<int?>(null, 20),];

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20).ToArray(),];

					async Task Act()
						=> await That(subject).All().AreEqualTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20).ToArray(),];

					async Task Act()
						=> await That(subject).All().AreEqualTo(5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 5 for all items,
						             but only 1 of 20 were

						             Not matching items:
						             [
						               1,
						               1,
						               2,
						               3,
						               8,
						               13,
						               21,
						               34,
						               55,
						               89,
						               (… and 9 more)
						             ]

						             Collection:
						             [
						               1,
						               1,
						               2,
						               3,
						               5,
						               8,
						               13,
						               21,
						               34,
						               55,
						               (… and 10 more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					ImmutableArray<int> subject = [..Factory.GetConstantValueEnumerable(constantValue, 20).ToArray(),];

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ImmutableStringItemTests
			{
				[Test]
				public async Task AsPrefix_WhenAnItemDoesNotMatchThePattern_ShouldFail()
				{
					ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("#").AsPrefix();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             starts with "#" for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [
						               "text"
						             ]

						             Collection:
						             [
						               "# Title",
						               "## Intro",
						               "text"
						             ]
						             """);
				}

				[Test]
				public async Task AsRegex_WhenAnItemDoesNotMatchThePattern_ShouldFail()
				{
					ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("^#").AsRegex();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             matches regex "^#" for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [
						               "text"
						             ]

						             Collection:
						             [
						               "# Title",
						               "## Intro",
						               "text"
						             ]
						             """);
				}

				[Test]
				public async Task AsRegex_WhenExpectedIsAnEmptyPattern_ShouldThrowArgumentException()
				{
					ImmutableArray<string?> subject = ["foo",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("").AsRegexThroughOptions();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
						.WithParamName("expected");
				}

				[Test]
				public async Task AsSuffix_WhenAnItemDoesNotMatchThePattern_ShouldFail()
				{
					ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("o").AsSuffix();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             ends with "o" for all items,
						             but only 1 of 3 were

						             Not matching items:
						             [
						               "# Title",
						               "text"
						             ]

						             Collection:
						             [
						               "# Title",
						               "## Intro",
						               "text"
						             ]
						             """);
				}

				[Test]
				public async Task AsWildcard_WhenAnItemDoesNotMatchThePattern_ShouldFail()
				{
					ImmutableArray<string?> subject = ["# Title", "## Intro", "text",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("#*").AsWildcard();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             matches "#*" for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [
						               "text"
						             ]

						             Collection:
						             [
						               "# Title",
						               "## Intro",
						               "text"
						             ]
						             """);
				}

				[Test]
				public async Task ShouldSupportNullableValues()
				{
					ImmutableArray<string?> subject = [..Factory.GetConstantValueEnumerable<string?>(null, 20),];

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<string?> subject = [..Factory.GetFibonacciNumbers(i => $"item-{i}", 20),];

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-5").Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					ImmutableArray<string> subject = [..Factory.GetFibonacciNumbers(i => $"item-{i}", 10),];

					async Task Act()
						=> await That(subject)!.All().AreEqualTo("item-5");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "item-5" for all items,
						             but only 1 of 10 were

						             Not matching items:
						             [
						               "item-1",
						               "item-1",
						               "item-2",
						               "item-3",
						               "item-8",
						               "item-13",
						               "item-21",
						               "item-34",
						               "item-55"
						             ]

						             Collection:
						             [
						               "item-1",
						               "item-1",
						               "item-2",
						               "item-3",
						               "item-5",
						               "item-8",
						               "item-13",
						               "item-21",
						               "item-34",
						               "item-55"
						             ]
						             """);
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldShowAllConfigurationsInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo")
							.IgnoringCase()
							.IgnoringNewlineStyle()
							.IgnoringLeadingWhiteSpace()
							.IgnoringTrailingWhiteSpace();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" ignoring case, whitespace and newline style for all items,
						             but none of 1 were

						             Not matching items:
						             [
						               "bar"
						             ]

						             Collection:
						             [
						               "bar"
						             ]
						             """);
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldShowIgnoringCaseInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" ignoring case for all items,
						             but none of 1 were

						             Not matching items:
						             [
						               "bar"
						             ]

						             Collection:
						             [
						               "bar"
						             ]
						             """);
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldShowIgnoringLeadingWhiteSpaceInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringLeadingWhiteSpace();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" ignoring leading whitespace for all items,
						             but none of 1 were

						             Not matching items:
						             [
						               "bar"
						             ]

						             Collection:
						             [
						               "bar"
						             ]
						             """);
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldShowIgnoringNewlineStyleInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringNewlineStyle();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" ignoring newline style for all items,
						             but none of 1 were

						             Not matching items:
						             [
						               "bar"
						             ]

						             Collection:
						             [
						               "bar"
						             ]
						             """);
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldShowIgnoringTrailingWhiteSpaceInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringTrailingWhiteSpace();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" ignoring trailing whitespace for all items,
						             but none of 1 were

						             Not matching items:
						             [
						               "bar"
						             ]

						             Collection:
						             [
						               "bar"
						             ]
						             """);
				}

				[Test]
				[Arguments(true)]
				[Arguments(false)]
				public async Task WhenItemsDifferInCase_ShouldSucceedWhenIgnoringCase(bool ignoreCase)
				{
					ImmutableArray<string?> subject = ["foo", "FOO",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringCase(ignoreCase);

					await That(Act).Throws<FailException>().OnlyIf(!ignoreCase)
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [
						               "FOO"
						             ]

						             Collection:
						             [
						               "foo",
						               "FOO"
						             ]
						             """);
				}

				[Test]
				[Arguments(true)]
				[Arguments(false)]
				public async Task WhenItemsDifferInLeadingWhiteSpace_ShouldSucceedWhenIgnoringLeadingWhiteSpace(
					bool ignoreLeadingWhiteSpace)
				{
					ImmutableArray<string?> subject = [" foo", "foo", "\tfoo",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo")
							.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);

					await That(Act).Throws<FailException>().OnlyIf(!ignoreLeadingWhiteSpace)
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" for all items,
						             but only 1 of 3 were

						             Not matching items:
						             [
						               " foo",
						               "\tfoo"
						             ]

						             Collection:
						             [
						               " foo",
						               "foo",
						               "\tfoo"
						             ]
						             """);
				}

				[Test]
				[Arguments(true)]
				[Arguments(false)]
				public async Task WhenItemsDifferInNewlineStyle_ShouldSucceedWhenIgnoringNewlineStyle(
					bool ignoreNewlineStyle)
				{
					ImmutableArray<string?> subject = ["foo\r\nbar", "foo\nbar", "foo\rbar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo\nbar").IgnoringNewlineStyle(ignoreNewlineStyle);

					await That(Act).Throws<FailException>().OnlyIf(!ignoreNewlineStyle)
						.WithMessage("""
						             Expected that subject
						             is equal to "foo\nbar" for all items,
						             but only 1 of 3 were

						             Not matching items:
						             [
						               *
						             ]

						             Collection:
						             [
						               *
						             ]
						             """).AsWildcard();
				}

				[Test]
				[Arguments(true)]
				[Arguments(false)]
				public async Task WhenItemsDifferInTrailingWhiteSpace_ShouldSucceedWhenIgnoringTrailingWhiteSpace(
					bool ignoreTrailingWhiteSpace)
				{
					ImmutableArray<string?> subject = ["foo ", "foo", "foo\t",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo")
							.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

					await That(Act).Throws<FailException>().OnlyIf(!ignoreTrailingWhiteSpace)
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" for all items,
						             but only 1 of 3 were

						             Not matching items:
						             [
						               "foo ",
						               "foo\t"
						             ]

						             Collection:
						             [
						               "foo ",
						               "foo",
						               "foo\t"
						             ]
						             """);
				}

				[Test]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					string constantValue = "foo";
					ImmutableArray<string> subject = [..Factory.GetConstantValueEnumerable(constantValue, 20),];

					async Task Act()
						=> await That(subject)!.All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
