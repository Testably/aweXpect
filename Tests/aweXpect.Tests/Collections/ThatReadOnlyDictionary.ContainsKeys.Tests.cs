using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class ContainsKeys
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllKeysExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).ContainsKeys(2, 1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).ContainsKeys();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);
				int[]? expected = null;

				async Task Act()
					=> await That(subject).ContainsKeys(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenOneKeyIsMissing_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).ContainsKeys(0, 2);

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).ContainsKeys("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains keys ["foo", "bar"],
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenOneKeyIsMissingAndOneExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.ContainsKeys(42, 2));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WhoseValuesTests
		{
			[Test]
			public async Task WhenKeysAreMissing_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(0).WhoseValues.All().AreEqualTo("bar");

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_AndAValueIsNull_ShouldSucceed()
			{
				IReadOnlyDictionary<int, string?> subject = ToDictionary([1, 2,], ["foo", null,]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.IsEqualTo(["foo", null,]);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenKeysExist_ButOneValueCompliesWithNone_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.None().ComplyWith(v => v.StartsWith("f"));

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_ButSomeValuesDoNotMatch_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.All().AreEqualTo("foo");

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_ButValuesAreNotEqualToExpected_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.IsEqualTo(["foo", "baz",]);

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_ButValuesAreNotUnique_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "foo",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2, 3).WhoseValues.All().AreUnique();

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_ButValuesDoNotComply_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.All().ComplyWith(v => v.StartsWith("f"));

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_ButValuesDoNotContainExpectedValue_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.Contains("baz");

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_ButValuesDoNotMatch_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(2).WhoseValues.All().AreEqualTo("foo");

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_ButValuesDoNotSatisfy_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.All().Satisfy(v => v?.StartsWith("fo") == true);

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_ButValuesHaveDifferentCount_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.HasCount(3);

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenKeysExist_ShouldSucceed()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(2).WhoseValues.All().AreEqualTo("bar");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnlySomeKeysAreMissing_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 0, 3).WhoseValues.All().AreEqualTo("bar");

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<string, string>? subject = null;

				async Task Act()
					=> await That(subject).ContainsKeys("foo").WhoseValues.All().AreEqualTo("");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains keys ["foo"] whose values all are equal to "",
					             but it was <null>
					             """);
			}
		}

		public sealed class ThrowingSubjectTests
		{
			[Test]
			public async Task WhenTheKeyComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				ThrowingKeyComparer<string> comparer = new(exception);
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new Dictionary<string, int>(comparer)
					{
						["a"] = 1,
						["b"] = 2,
					});
				comparer.IsArmed = true;

				async Task Act()
					=> await That(subject).ContainsKeys("a", "b");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains keys ["a", "b"],
					             but it did throw an InvalidOperationException:
					               comparer failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Test]
			public async Task WhenTheLookupThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("lookup failed");
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.ContainsKey)
					{
						["a"] = 1,
						["b"] = 2,
					});

				async Task Act()
					=> await That(subject).ContainsKeys("a", "b");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains keys ["a", "b"],
					             but it did throw an InvalidOperationException:
					               lookup failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}
		}
	}
}
