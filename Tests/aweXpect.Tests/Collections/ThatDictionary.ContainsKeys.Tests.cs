using System.Collections.Generic;

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
					.WithMessage("The expected cannot be null.").AsPrefix();
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
					             ] in [
					               1,
					               2,
					               3
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
					=> await That(subject).ContainsKeys(0).WhoseValues.AreEqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [0] whose values are equal to "bar" for all items,
					             but it did not contain [
					               0
					             ] in [
					               1,
					               2,
					               3
					             ]

					             Not matching items:
					             [
					               <null>,
					               (… and maybe others)
					             ]

					             Collection:
					             [
					               <null>
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
					=> await That(subject).ContainsKeys(1, 2).WhoseValues.AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 2] whose values are equal to "foo" for all items,
					             but not all were

					             Not matching items:
					             [
					               "bar",
					               (… and maybe others)
					             ]

					             Collection:
					             [
					               "foo",
					               "bar"
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
					=> await That(subject).ContainsKeys(2).WhoseValues.AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [2] whose values are equal to "foo" for all items,
					             but not all were

					             Not matching items:
					             [
					               "bar",
					               (… and maybe others)
					             ]

					             Collection:
					             [
					               "bar"
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
					=> await That(subject).ContainsKeys(2).WhoseValues.AreEqualTo("bar");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenOnlySomeKeysAreMissing_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKeys(1, 0, 3).WhoseValues.AreEqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys [1, 0, 3] whose values are equal to "bar" for all items,
					             but it did not contain [
					               0
					             ] in [
					               1,
					               2,
					               3
					             ]

					             Not matching items:
					             [
					               "foo",
					               (… and maybe others)
					             ]

					             Collection:
					             [
					               "foo",
					               <null>,
					               "baz"
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
					=> await That(subject).ContainsKeys("foo").WhoseValues.AreEqualTo("");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains keys ["foo"] whose values are equal to "" for all items,
					             but it was <null>
					             """);
			}
		}
	}
}
