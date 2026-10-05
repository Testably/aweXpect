using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class DoesNotContainKeys
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllKeysDoNotExist_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKeys(42, 43);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllKeysOfAnEnumerableDoNotExist_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);
				IEnumerable<int> unexpected = new List<int> { 42, 43, };

				async Task Act()
					=> await That(subject).DoesNotContainKeys(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAtLeastOneKeyExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKeys(42, 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys [42, 2],
					             but it contained [
					               2
					             ]

					             Dictionary:
					             {[1] = 0, [2] = 0, [3] = 0}
					             """);
			}

			[Fact]
			public async Task WhenAtLeastOneKeyOfAnEnumerableExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);
				IEnumerable<int> unexpected = new List<int> { 42, 2, };

				async Task Act()
					=> await That(subject).DoesNotContainKeys(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys unexpected,
					             but it contained [
					               2
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
					=> await That(subject).DoesNotContainKeys("foo", "bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys ["foo", "bar"],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedContainsNull_ShouldThrowArgumentNullException()
			{
				Dictionary<string, int> subject = new()
				{
					["foo"] = 1,
				};

				async Task Act()
					=> await That(subject).DoesNotContainKeys("bar", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKeys();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);
				int[]? unexpected = null;

				async Task Act()
					=> await That(subject).DoesNotContainKeys(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenNoKeyExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2,], [0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.DoesNotContainKeys(3, 4));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains any of keys [3, 4],
					             but it did not contain [
					               3,
					               4
					             ]

					             Dictionary:
					             {[1] = 0, [2] = 0}
					             """)
					.Because("the negation of DoesNotContainKeys only fails when none of the keys is contained");
			}

			[Fact]
			public async Task WhenOneKeyIsMissingAndOneExists_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.DoesNotContainKeys(42, 2));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedContainsNull_ShouldThrowArgumentNullException()
			{
				Dictionary<string, int> subject = new()
				{
					["foo"] = 1,
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.DoesNotContainKeys("bar", null!));

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (AndOrResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>>)
						That(subject).DoesNotContainKeys("b");

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}

		public sealed class ThrowingSubjectTests
		{
			[Fact]
			public async Task WhenTheKeyComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				ThrowingKeyComparer<string> comparer = new(exception);
				Dictionary<string, int> subject = new(comparer) { ["a"] = 1, ["b"] = 2, };
				comparer.IsArmed = true;

				async Task Act()
					=> await That(subject).DoesNotContainKeys("x", "y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys ["x", "y"],
					             but it did throw an InvalidOperationException:
					               comparer failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}

			[Fact]
			public async Task WhenTheLookupThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("lookup failed");
				IDictionary<string, int> subject =
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.ContainsKey) { ["a"] = 1, ["b"] = 2, };

				async Task Act()
					=> await That(subject).DoesNotContainKeys("x", "y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys ["x", "y"],
					             but it did throw an InvalidOperationException:
					               lookup failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}
		}
	}
}
