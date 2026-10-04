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
			public async Task WhenAllKeysOfAnEnumerableExist_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);
				IEnumerable<int> expected = new List<int> { 2, 1, };

				async Task Act()
					=> await That(subject).ContainsKeys(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedContainsNull_ShouldThrowArgumentNullException()
			{
				Dictionary<string, int> subject = new()
				{
					["foo"] = 1,
				};

				async Task Act()
					=> await That(subject).ContainsKeys("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
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
			public async Task WhenExpectedThrows_ShouldThrowTheExceptionOfTheExpectedItems()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				IEnumerable<int> GetExpected()
				{
					yield return 1;
					throw new InvalidOperationException("the expected keys are broken");
				}

				async Task Act()
					=> await That(subject).ContainsKeys(GetExpected());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("the expected keys are broken")
					.Because("an exception of the expected keys is not wrapped as if the subject threw it");
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
			public async Task WhenOneKeyOfAnEnumerableIsMissing_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);
				IEnumerable<int> expected = new List<int> { 0, 2, };

				async Task Act()
					=> await That(subject).ContainsKeys(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys expected,
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
			public async Task WhenAllKeysExist_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.ContainsKeys(1, 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain all keys [1, 2],
					             but it contained [
					               1,
					               2
					             ]

					             Dictionary:
					             {[1] = 0, [2] = 0, [3] = 0}
					             """);
			}

			[Fact]
			public async Task WhenExpectedContainsNull_ShouldThrowArgumentNullException()
			{
				Dictionary<string, int> subject = new()
				{
					["foo"] = 1,
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.ContainsKeys("foo", null!));

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

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
					             contains keys [0] whose values all are equal to "bar",
					             but it did not contain [
					               0
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
					             contains keys [1, 2] whose values all do not start with "f",
					             but only 1 of 2 did

					             Not matching items (values of keys [1, 2]):
					             [
					               [1] = "foo"
					             ]

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]
					             """)
					.Because("the plural connector must also govern the negated form of a string match type");
			}

			[Fact]
			public async Task WhenKeysExist_ButFewerValuesThanTheMinimumMatch_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2, 3).WhoseValues.AtLeast(2).AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2, 3] whose values at least 2 are equal to "foo",
					             but only 1 of 3 were

					             Not matching items (values of keys [1, 2, 3]):
					             [
					               [2] = "bar",
					               [3] = "baz"
					             ]

					             Collection (values of keys [1, 2, 3]):
					             [
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             ]
					             """);
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
					             contains keys [1, 2] whose values none start with "f",
					             but 1 of 2 did

					             Matching items (values of keys [1, 2]):
					             [
					               [1] = "foo"
					             ]

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]
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
					             contains keys [1, 2] whose values all are equal to "foo",
					             but only 1 of 2 were

					             Not matching items (values of keys [1, 2]):
					             [
					               [2] = "bar"
					             ]

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]
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
					             contains keys [1, 2] whose values all have Length that is equal to 4,
					             but none of 2 did

					             Not matching items (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]
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
					             but values of keys [1, 2] contained item "bar" at index 1 instead of "baz"

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]

					             Expected (values of keys [1, 2]):
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
					             contains keys [1, 2, 3] whose values all are unique,
					             but only 1 of 3 were

					             Not matching items (values of keys [1, 2, 3]):
					             [
					               [1] = "foo",
					               [3] = "foo"
					             ]

					             Collection (values of keys [1, 2, 3]):
					             [
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "foo"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenKeysExist_ButValuesAreNotUniqueIgnoringCase_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "FOO", "bar",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.All().AreUnique().IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values all are unique ignoring case,
					             but none of 2 were

					             Not matching items (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "FOO"
					             ]

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "FOO"
					             ]
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
					             contains keys [1, 2] whose values all start with "f",
					             but only 1 of 2 did

					             Not matching items (values of keys [1, 2]):
					             [
					               [2] = "bar"
					             ]

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]
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
					             but values of keys [1, 2] did not contain it

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]
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
					             contains keys [2] whose values all are equal to "foo",
					             but none of 1 were

					             Not matching items (values of keys [2]):
					             [
					               [2] = "bar"
					             ]

					             Collection (values of keys [2]):
					             [
					               [2] = "bar"
					             ]
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
					             contains keys [1, 2] whose values all satisfy v => v?.StartsWith("fo") == true,
					             but only 1 of 2 did

					             Not matching items (values of keys [1, 2]):
					             [
					               [2] = "bar"
					             ]

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]
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
					             but values of keys [1, 2] had only 2 items

					             Collection (values of keys [1, 2]):
					             [
					               [1] = "foo",
					               [2] = "bar"
					             ]
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
					             contains keys [1, 0, 3] whose values all are equal to "bar",
					             but it did not contain [
					               0
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
					             contains keys ["foo"] whose values all are equal to "",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenValueExpectationIsNegated_AndAllValuesAreUnique_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2, 3).WhoseValues
						.DoesNotComplyWith(values => values.All().AreUnique());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2, 3] whose values not all are unique,
					             but all 3 were

					             Collection (values of keys [1, 2, 3]):
					             [
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenValueExpectationIsNegated_AndNoValueStartsWithThePrefix_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(2, 3).WhoseValues
						.DoesNotComplyWith(values => values.None().ComplyWith(v => v.StartsWith("f")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [2, 3] whose values at least one starts with "f",
					             but none of 2 did

					             Collection (values of keys [2, 3]):
					             [
					               [2] = "bar",
					               [3] = "baz"
					             ]
					             """)
					.Because("the complement of the quantifier is singular, so the verb of the values must be singular too");
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
