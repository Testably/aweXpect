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
				[Theory]
				[InlineData(double.NaN, false)]
				[InlineData(1.0, true)]
				public async Task DoubleNaNValues_ShouldBeConsideredEqual(double additionalValue, bool expectFailure)
				{
					ImmutableArray<double> subject = [double.NaN, double.NaN, additionalValue,];

					async Task Act()
						=> await That(subject).All().AreEqualTo(double.NaN);

					await That(Act).Throws<XunitException>().OnlyIf(expectFailure)
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

				[Theory]
				[InlineData(float.NaN, false)]
				[InlineData(1.0F, true)]
				public async Task FloatNaNValues_ShouldBeConsideredEqual(float additionalValue, bool expectFailure)
				{
					ImmutableArray<float> subject = [float.NaN, float.NaN, additionalValue,];

					async Task Act()
						=> await That(subject).All().AreEqualTo(float.NaN);

					await That(Act).Throws<XunitException>().OnlyIf(expectFailure)
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
				[Fact]
				public async Task ShouldSupportNullableValues()
				{
					ImmutableArray<int?> subject = [..Factory.GetConstantValueEnumerable<int?>(null, 20),];

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20).ToArray(),];

					async Task Act()
						=> await That(subject).All().AreEqualTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20).ToArray(),];

					async Task Act()
						=> await That(subject).All().AreEqualTo(5);

					await That(Act).Throws<XunitException>()
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
						               …
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

				[Fact]
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
				[Fact]
				public async Task ShouldSupportNullableValues()
				{
					ImmutableArray<string?> subject = [..Factory.GetConstantValueEnumerable<string?>(null, 20),];

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<string?> subject = [..Factory.GetFibonacciNumbers(i => $"item-{i}", 20),];

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-5").Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					ImmutableArray<string> subject = [..Factory.GetFibonacciNumbers(i => $"item-{i}", 10),];

					async Task Act()
						=> await That(subject)!.All().AreEqualTo("item-5");

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenItemsDiffer_ShouldShowAllConfigurationsInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo")
							.IgnoringCase()
							.IgnoringNewlineStyle()
							.IgnoringLeadingWhiteSpace()
							.IgnoringTrailingWhiteSpace();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" ignoring case, white-space and newline style for all items,
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

				[Fact]
				public async Task WhenItemsDiffer_ShouldShowIgnoringCaseInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringCase();

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenItemsDiffer_ShouldShowIgnoringLeadingWhiteSpaceInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringLeadingWhiteSpace();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" ignoring leading white-space for all items,
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

				[Fact]
				public async Task WhenItemsDiffer_ShouldShowIgnoringNewlineStyleInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringNewlineStyle();

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenItemsDiffer_ShouldShowIgnoringTrailingWhiteSpaceInMessage()
				{
					ImmutableArray<string?> subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringTrailingWhiteSpace();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" ignoring trailing white-space for all items,
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

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WhenItemsDifferInCase_ShouldSucceedWhenIgnoringCase(bool ignoreCase)
				{
					ImmutableArray<string?> subject = ["foo", "FOO",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringCase(ignoreCase);

					await That(Act).Throws<XunitException>().OnlyIf(!ignoreCase)
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

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WhenItemsDifferInLeadingWhiteSpace_ShouldSucceedWhenIgnoringLeadingWhiteSpace(
					bool ignoreLeadingWhiteSpace)
				{
					ImmutableArray<string?> subject = [" foo", "foo", "\tfoo",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo")
							.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);

					await That(Act).Throws<XunitException>().OnlyIf(!ignoreLeadingWhiteSpace)
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

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WhenItemsDifferInNewlineStyle_ShouldSucceedWhenIgnoringNewlineStyle(
					bool ignoreNewlineStyle)
				{
					ImmutableArray<string?> subject = ["foo\r\nbar", "foo\nbar", "foo\rbar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo\nbar").IgnoringNewlineStyle(ignoreNewlineStyle);

					await That(Act).Throws<XunitException>().OnlyIf(!ignoreNewlineStyle)
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

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WhenItemsDifferInTrailingWhiteSpace_ShouldSucceedWhenIgnoringTrailingWhiteSpace(
					bool ignoreTrailingWhiteSpace)
				{
					ImmutableArray<string?> subject = ["foo ", "foo", "foo\t",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo")
							.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

					await That(Act).Throws<XunitException>().OnlyIf(!ignoreTrailingWhiteSpace)
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

				[Fact]
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
