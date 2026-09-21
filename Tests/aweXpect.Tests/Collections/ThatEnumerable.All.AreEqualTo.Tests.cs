using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEqualTo
		{
			public sealed class Tests
			{
				[Theory]
				[InlineData(double.NaN, false)]
				[InlineData(1.0, true)]
				public async Task DoubleNaNValues_ShouldBeConsideredEqual(double additionalValue, bool expectFailure)
				{
					IEnumerable<double> subject = [double.NaN, double.NaN, additionalValue,];

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
					IEnumerable<float> subject = [float.NaN, float.NaN, additionalValue,];

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

			public sealed class ItemTests
			{
				[Fact]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1)
							.And.All().AreEqualTo(1);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<int> subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [2, (… and maybe more)]

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
						               (… and maybe more)
						             ]
						             """);
				}

				[Fact]
				public async Task ShouldSupportNullableValues()
				{
					IEnumerable<int?> subject = Factory.GetConstantValueEnumerable<int?>(null, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					int[] subject = Factory.GetFibonacciNumbers(20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					int[] subject = Factory.GetFibonacciNumbers(20).ToArray();

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

				[Fact]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					int[] subject = Factory.GetConstantValueEnumerable(constantValue, 20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					int constantValue = 42;
					IEnumerable<int>? subject = null!;

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 42 for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringItemTests
			{
				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<string> subject = Factory.GetFibonacciNumbers(i => $"item-{i}");

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-1");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "item-1" for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [
						               "item-2",
						               (… and maybe more)
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
						               "item-55",
						               (… and maybe more)
						             ]
						             """);
				}

				[Fact]
				public async Task ShouldSupportNullableValues()
				{
					IEnumerable<string?> subject = Factory.GetConstantValueEnumerable<string?>(null, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					string[] subject = Factory.GetFibonacciNumbers(i => $"item-{i}", 20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-5").Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					string[] subject = Factory.GetFibonacciNumbers(i => $"item-{i}", 10).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-5");

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
					string[] subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo")
							.IgnoringCase()
							.IgnoringNewlineStyle()
							.IgnoringLeadingWhiteSpace()
							.IgnoringTrailingWhiteSpace();

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenItemsDiffer_ShouldShowIgnoringCaseInMessage()
				{
					string[] subject = ["bar",];

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
					string[] subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringLeadingWhiteSpace();

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenItemsDiffer_ShouldShowIgnoringNewlineStyleInMessage()
				{
					string[] subject = ["bar",];

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
					string[] subject = ["bar",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringTrailingWhiteSpace();

					await That(Act).Throws<XunitException>()
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

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WhenItemsDifferInCase_ShouldSucceedWhenIgnoringCase(bool ignoreCase)
				{
					string[] subject = ["foo", "FOO",];

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
					string[] subject = [" foo", "foo", "\tfoo",];

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
					string[] subject = ["foo\r\nbar", "foo\nbar", "foo\rbar",];

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
					string[] subject = ["foo ", "foo", "foo\t",];

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
					string[] subject = Factory.GetConstantValueEnumerable(constantValue, 20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					string constantValue = "foo";
					IEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedItemTests
			{
				[Fact]
				public async Task WhenAllItemsMatch_ShouldFail()
				{
					int[] subject = [1, 1, 1,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for not all items,
						             but all 3 were

						             Collection:
						             [1, 1, 1]
						             """);
				}

				[Fact]
				public async Task WhenOneItemDoesNotMatch_ShouldSucceed()
				{
					int[] subject = [1, 2, 1,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreEqualTo(1));

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
