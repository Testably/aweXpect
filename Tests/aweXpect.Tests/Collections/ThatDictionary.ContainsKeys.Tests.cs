using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class ContainsKeys
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllKeysExists_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).ContainsKeys(2, 1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).ContainsKeys();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);
				int[]? expected = null;

				async Task Act()
					=> await That(subject).ContainsKeys(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenOneKeyIsMissing_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).ContainsKeys(0, 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [0, 2],
					             but it did not contain [
					               0
					             ]

					             Dictionary:
					             {[1] = 0, [2] = 0, [3] = 0}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).ContainsKeys("foo", "bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys ["foo", "bar"],
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenOneKeyIsMissingAndOneExists_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.ContainsKeys(42, 2));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WhoseValuesTests
		{
			[Fact]
			public async Task WhenKeysAreMissing_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(0).WhoseValues.All().AreEqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [0] whose values are equal to "bar" for all items,
					             but it did not contain [
					               0
					             ]

					             Collection:
					             []

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_AndAValueIsNull_ShouldSucceed()
			{
				IDictionary<int, string?> subject = ToDictionary([1, 2,], ["foo", null,]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.IsEqualTo(["foo", null,]);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenKeysExist_ButAValueStartsWithTheUnexpectedPrefix_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.All()
						.ComplyWith(v => v.DoesNotStartWith("f"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values do not start with "f" for all items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               [1] = "foo"
					             ]

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """)
					.Because("the plural connector must also govern the negated form of a string match type");
			}

			[Fact]
			public async Task WhenKeysExist_ButOneValueCompliesWithNone_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.None().ComplyWith(v => v.StartsWith("f"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values start with "f" for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               [1] = "foo"
					             ]

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ButSomeValuesDoNotMatch_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.All().AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values are equal to "foo" for all items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               [2] = "bar"
					             ]

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ButValueMembersDoNotComply_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2,], ["foo", "bar",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.All()
						.ComplyWith(v => v.Whose(s => s.Length, l => l.IsEqualTo(4)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values have Length which is equal to 4 for all items,
					             but none of 2 did

					             Not matching items:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar"
					             }
					             """)
					.Because("the connector already introduced the values, so the member must not start a second \"whose\"");
			}

			[Fact]
			public async Task WhenKeysExist_ButValuesAreNotEqualToExpected_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.IsEqualTo(["foo", "baz",]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values are equal to collection ["foo", "baz",] in order,
					             but values [1, 2] contained item "bar" at index 1 instead of "baz"

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }

					             Expected:
					             [
					               "foo",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ButValuesAreNotUnique_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "foo",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2, 3).WhoseValues.All().AreUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2, 3] whose values are unique for all items,
					             but only 1 of 3 were

					             Not matching items:
					             [
					               [1] = "foo",
					               [3] = "foo"
					             ]

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "foo"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "foo"
					             }
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ButValuesDoNotComply_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.All().ComplyWith(v => v.StartsWith("f"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values start with "f" for all items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               [2] = "bar"
					             ]

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ButValuesDoNotContainExpectedValue_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.Contains("baz");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values contain "baz" at least once,
					             but values [1, 2] did not contain it

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ButValuesDoNotMatch_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(2).WhoseValues.All().AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [2] whose values are equal to "foo" for all items,
					             but none of 1 were

					             Not matching items:
					             [
					               [2] = "bar"
					             ]

					             Collection:
					             [
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ButValuesDoNotSatisfy_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.All().Satisfy(v => v?.StartsWith("fo") == true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values satisfy v => v?.StartsWith("fo") == true for all items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               [2] = "bar"
					             ]

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ButValuesHaveDifferentCount_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.HasCount(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values have exactly 3 items,
					             but values [1, 2] had only 2 items

					             Collection:
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ShouldSucceed()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(2).WhoseValues.All().AreEqualTo("bar");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenOnlySomeKeysAreMissing_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 0, 3).WhoseValues.All().AreEqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 0, 3] whose values are equal to "bar" for all items,
					             but it did not contain [
					               0
					             ]

					             Not matching items:
					             [
					               [1] = "foo",
					               [3] = "baz"
					             ]

					             Collection:
					             [
					               [1] = "foo",
					               [3] = "baz"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<string, string>? subject = null;

				async Task Act()
					=> await That(subject).ContainsKeys("foo").WhoseValues.All().AreEqualTo("");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys ["foo"] whose values are equal to "" for all items,
					             but it was <null>
					             """);
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ContainsKeysResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, string, int>)
						That(subject).ContainsKeys("a");

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}
	}
}
