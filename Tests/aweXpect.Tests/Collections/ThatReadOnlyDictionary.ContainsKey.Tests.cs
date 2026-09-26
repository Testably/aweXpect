using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class ContainsKey
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IReadOnlyDictionary<string, int> subject = new Dictionary<string, int>
				{
					["foo"] = 1,
				};

				async Task Act()
					=> await That(subject).ContainsKey(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenKeyExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).ContainsKey(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenKeyIsMissing_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).ContainsKey(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains key 0,
					             but it did not contain 0

					             Dictionary:
					             {[1] = 0, [2] = 0, [3] = 0}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).ContainsKey("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains key "foo",
					             but it was <null>
					             """);
			}
		}

		public sealed class WhoseValueTests
		{
			[Fact]
			public async Task WhenKeyExists_ButLookingUpTheValueThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("lookup failed");
				IReadOnlyDictionary<int, string> subject = new ThrowingLookupDictionary(exception, 1, 2, 3);

				async Task Act()
					=> await That(subject).ContainsKey(2).WhoseValue.IsEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains key 2 whose value is equal to "foo",
					             but value [2] did throw an InvalidOperationException:
					               lookup failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a value that cannot be looked up fails the expectation instead of aborting its evaluation");
			}

			[Fact]
			public async Task WhenKeyExists_ButLookingUpTheValueThrows_WhenNegated_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("lookup failed");
				IReadOnlyDictionary<int, string> subject = new ThrowingLookupDictionary(exception, 1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.ContainsKey(2).WhoseValue.IsEqualTo("foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain key 2 whose value is equal to "foo",
					             but value [2] did throw an InvalidOperationException:
					               lookup failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a value that was never looked up cannot prove the negation either");
			}

			[Fact]
			public async Task WhenKeyExists_ButValueDoesNotMatch_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKey(2).WhoseValue.IsEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains key 2 whose value is equal to "foo",
					             but value [2] was "bar", which differs at index 0:
					                ↓ (actual)
					               "bar"
					               "foo"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenKeyExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKey(2).WhoseValue.IsEqualTo("bar");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenKeyIsMissing_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKey(0).WhoseValue.IsEqualTo("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains key 0 whose value is equal to "bar",
					             but it did not contain 0

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
				ReadOnlyDictionary<string, string>? subject = null;

				async Task Act()
					=> await That(subject).ContainsKey("foo").WhoseValue.IsEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains key "foo" whose value is empty,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).ContainsKey(4).And.ContainsKey(5);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains key 4 and contains key 5,
					             but it did not contain 4 and did not contain 5

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar",
					               [3] = "baz"
					             }
					             """);
			}
		}
	}
}
