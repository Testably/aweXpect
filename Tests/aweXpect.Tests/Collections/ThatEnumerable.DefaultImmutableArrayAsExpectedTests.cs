using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using aweXpect.Core;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class DefaultImmutableArrayAsExpectedTests
	{
#if NET8_0_OR_GREATER
		[Test]
		public async Task AsyncEnumerable_Contains_ShouldThrowArgumentNullException()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1,]);
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task AsyncEnumerable_EndsWith_ShouldThrowArgumentNullException()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1,]);
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).EndsWith(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task AsyncEnumerable_IsContainedIn_ShouldThrowArgumentNullException()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1,]);
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsContainedIn(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task AsyncEnumerable_IsEqualTo_ShouldFail()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1,]);
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but the expected collection was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task AsyncEnumerable_IsNotContainedIn_ShouldThrowArgumentNullException()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1,]);
			ImmutableArray<int> unexpected = default;

			async Task Act()
				=> await That(subject).IsNotContainedIn(unexpected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task AsyncEnumerable_StartsWith_ShouldThrowArgumentNullException()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1,]);
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).StartsWith(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}
#endif

		[Test]
		public async Task Contains_ShouldThrowArgumentNullException()
		{
			int[] subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task ContainsKey_WhenTheKeyIsADefaultImmutableArray_ShouldSucceed()
		{
			Dictionary<ImmutableArray<int>, int> subject = new() { [default] = 1, };
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).ContainsKey(expected);

			await That(Act).DoesNotThrow()
				.Because("a default ImmutableArray is only a null collection, not a null key");
		}

		[Test]
		public async Task ContainsKeys_ShouldThrowArgumentNullException()
		{
			Dictionary<int, int> subject = new() { [1] = 1, };
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).ContainsKeys(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task ContainsPredicates_ShouldThrowArgumentNullException()
		{
			int[] subject = [1,];
			ImmutableArray<Expression<Func<int, bool>>> expected = default;

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task DictionaryIsEqualTo_ShouldFail()
		{
			Dictionary<int, int> subject = new() { [1] = 1, };
			ImmutableArray<KeyValuePair<int, int>> expected = default;

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to dictionary expected,
				             but the expected dictionary was <null>

				             Dictionary:
				             {[1] = 1}
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task DoesNotContain_ShouldThrowArgumentNullException()
		{
			int[] subject = [1,];
			ImmutableArray<int> unexpected = default;

			async Task Act()
				=> await That(subject).DoesNotContain(unexpected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task DoesNotEndWith_ShouldThrowArgumentNullException()
		{
			int[] subject = [1,];
			ImmutableArray<int> unexpected = default;

			async Task Act()
				=> await That(subject).DoesNotEndWith(unexpected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task DoesNotStartWith_ShouldThrowArgumentNullException()
		{
			int[] subject = [1,];
			ImmutableArray<int> unexpected = default;

			async Task Act()
				=> await That(subject).DoesNotStartWith(unexpected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task EndsWith_ShouldThrowArgumentNullException()
		{
			int[] subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).EndsWith(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

#if NET8_0_OR_GREATER
		[Test]
		public async Task ImmutableArray_Contains_ShouldThrowArgumentNullException()
		{
			ImmutableArray<int> subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task ImmutableArray_EndsWith_ShouldThrowArgumentNullException()
		{
			ImmutableArray<int> subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).EndsWith(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task ImmutableArray_IsContainedIn_ShouldThrowArgumentNullException()
		{
			ImmutableArray<int> subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsContainedIn(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task ImmutableArray_IsEqualTo_ShouldFail()
		{
			ImmutableArray<int> subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but the expected collection was <null>

				             Collection:
				             [1]
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task ImmutableArray_StartsWith_ShouldThrowArgumentNullException()
		{
			ImmutableArray<int> subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).StartsWith(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}
#endif

		[Test]
		public async Task IsContainedIn_ShouldThrowArgumentNullException()
		{
			int[] subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsContainedIn(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task IsEqualTo_InAnyOrder_ShouldFail()
		{
			int[] subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in any order,
				             but the expected collection was <null>

				             Collection:
				             [1]
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsEqualTo_ShouldFail()
		{
			int[] subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but the expected collection was <null>

				             Collection:
				             [1]
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsEqualTo_WhenSubjectIsNull_ShouldSucceed()
		{
			int[]? subject = null;
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).DoesNotThrow()
				.Because("a default ImmutableArray is equal to null, like a null collection");
		}

		[Test]
		public async Task IsEqualToExpectations_ShouldFail()
		{
			int[] subject = [1,];
			ImmutableArray<Action<IThat<int>>> expected = default;

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but the expected collection was <null>

				             Collection:
				             [1]
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsEqualToPredicates_ShouldFail()
		{
			int[] subject = [1,];
			ImmutableArray<Expression<Func<int, bool>>> expected = default;

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but the expected collection was <null>

				             Collection:
				             [1]
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsNotContainedIn_ShouldThrowArgumentNullException()
		{
			int[] subject = [1,];
			ImmutableArray<int> unexpected = default;

			async Task Act()
				=> await That(subject).IsNotContainedIn(unexpected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task IsNotEqualTo_ShouldSucceed()
		{
			int[] subject = [1,];
			ImmutableArray<int> unexpected = default;

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected);

			await That(Act).DoesNotThrow()
				.Because("a collection is not equal to a default ImmutableArray, like to a null collection");
		}

		[Test]
		public async Task IsNotEqualTo_WhenSubjectIsNull_ShouldFail()
		{
			int[]? subject = null;
			ImmutableArray<int> unexpected = default;

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not equal to collection unexpected in order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is equal to null, like a null collection");
		}

		[Test]
		public async Task IsNotOneOf_ShouldThrowArgumentNullException()
		{
			int subject = 1;
			ImmutableArray<int> unexpected = default;

			async Task Act()
				=> await That(subject).IsNotOneOf(unexpected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task IsOneOf_ShouldThrowArgumentNullException()
		{
			int subject = 1;
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsOneOf(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task StartsWith_ShouldThrowArgumentNullException()
		{
			int[] subject = [1,];
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).StartsWith(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Untyped_Contains_ShouldThrowArgumentNullException()
		{
			IEnumerable subject = new[] { 1, };
			IEnumerable expected = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Untyped_EndsWith_ShouldThrowArgumentNullException()
		{
			IEnumerable subject = new[] { 1, };
			IEnumerable expected = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).EndsWith(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Untyped_IsContainedIn_ShouldThrowArgumentNullException()
		{
			IEnumerable subject = new[] { 1, };
			IEnumerable expected = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsContainedIn(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Untyped_IsEqualTo_ShouldFail()
		{
			IEnumerable subject = new[] { 1, };
			IEnumerable expected = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but the expected collection was <null>

				             Collection:
				             [1]
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task Untyped_StartsWith_ShouldThrowArgumentNullException()
		{
			IEnumerable subject = new[] { 1, };
			IEnumerable expected = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).StartsWith(expected);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' value cannot be null.").AsPrefix();
		}
	}
}
