using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using aweXpect.Core;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class OverloadTests
		{
			[Test]
			public async Task WhenArraySubject_ShouldBindToTheArrayOverload()
			{
				int[] subject = [1, 2, 3,];

				AndOrResult<int[], IThat<int[]?>> result = That(subject).HasCount(3);

				await That(async () => await result).DoesNotThrow()
					.Because("the overload for arrays is more specific than the one for any collection type");
			}

			[Test]
			public async Task WhenDefaultImmutableArraySubject_ShouldFail()
			{
				ImmutableArray<int> subject = default;

				async Task Act()
					=> await That(subject).HasCount(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 3 items,
					             but it was <null>
					             """)
					.Because("a default immutable array is treated like a null collection on every target framework");
			}

			[Test]
			public async Task WhenDictionarySubject_ShouldAllowDictionaryExpectations()
			{
				Dictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
					{
						"b", 2
					},
				};

				async Task Act()
					=> await That(subject).HasCount(2).And.ContainsKey("a");

				await That(Act).DoesNotThrow()
					.Because("the result keeps the dictionary type, so that dictionary expectations can follow");
			}

			[Test]
			public async Task WhenDictionarySubject_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
				};

				Dictionary<string, int> result = await That(subject).HasCount(1);

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenDictionarySubject_WithComparison_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
				};

				Dictionary<string, int> result = await That(subject).HasCount().GreaterThan(0);

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenDictionarySubject_WithTooManyItems_ShouldFail()
			{
				Dictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
					{
						"b", 2
					},
				};

				async Task Act()
					=> await That(subject).HasCount(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly one item,
					             but it had 2 items

					             Collection:
					             {
					               ["a"] = 1,
					               ["b"] = 2
					             }
					             """);
			}

			[Test]
			public async Task WhenEnumerableSubject_ShouldBindToTheEnumerableOverload()
			{
				IEnumerable<int> subject = new CollectionWithDifferentCounts(2, 3);

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).DoesNotThrow()
					.Because("an IEnumerable<T> subject binds to its own overload, which reads the count of ICollection<T>");
			}

			[Test]
			public async Task WhenHashSetSubject_WithTooManyItems_ShouldFail()
			{
				HashSet<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().LessThan(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has fewer than 2 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenImmutableArraySubject_ShouldKeepTheSubjectType()
			{
				ImmutableArray<int> subject = ImmutableArray.Create(1, 2, 3);

				ImmutableArray<int> result = await That(subject).HasCount(3);

				await That(result).IsEqualTo(subject);
			}

			[Test]
			public async Task WhenListSubject_ShouldCompareTheItemsInFollowingExpectations()
			{
				List<int> subject = [1, 2, 3,];
				List<int> expected = [1, 2, 4,];

				async Task Act()
					=> await That(subject).HasCount(3).And.IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 3 items and is equal to collection expected in order,
					             but it contained item 3 at index 2 instead of 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [1, 2, 4]
					             """)
					.Because("a concrete collection type still binds the collection expectations that follow");
			}

			[Test]
			public async Task WhenListSubject_ShouldKeepTheSubjectType()
			{
				List<int> subject = [1, 2, 3,];

				List<int> result = await That(subject).HasCount(3).And.HasCount().LessThan(4);

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenNonGenericEnumerableSubject_ShouldKeepTheResultType()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};

				CollectionCountResult<AndOrResult<IEnumerable, IThat<IEnumerable?>>> withComparison =
					That(subject).HasCount();
				AndOrResult<IEnumerable, IThat<IEnumerable?>> withCount = That(subject).HasCount(2);

				await That(async () => await withComparison.EqualTo(2).And.HasCount(2)).DoesNotThrow();
				await That(async () => await withCount).DoesNotThrow()
					.Because("a non-generic enumerable keeps its result types, whichever overload it binds to");
			}

			[Test]
			public async Task WhenNullableListSubjectIsNull_ShouldFail()
			{
				List<int>? subject = null;

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenStringSubject_ShouldKeepTheSubjectType()
			{
				string subject = "abc";

				string result = await That(subject).HasCount(3);

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenStringSubject_WithTooManyItems_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it had at least 3 items

					             Collection:
					             ['a', 'b', 'c', (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenStructSubject_ShouldKeepTheSubjectType()
			{
				StructCollection subject = new(1, 2, 3);

				StructCollection result = await That(subject).HasCount(3);

				await That(result.Count).IsEqualTo(3);
			}

			[Test]
			public async Task WhenStructSubject_WithTooManyItems_ShouldFail()
			{
				StructCollection subject = new(1, 2, 3);

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it had at least 3 items

					             Collection:
					             [1, 2, 3, (… and maybe more)]
					             """);
			}

			private sealed class CollectionWithDifferentCounts(int count, int untypedCount)
				: ICollection<int>, ICollection
			{
				int ICollection.Count => untypedCount;
				public bool IsSynchronized => false;
				public object SyncRoot => this;

				public void CopyTo(Array array, int index) => throw new NotSupportedException();
				public int Count => count;
				public bool IsReadOnly => true;

				public IEnumerator<int> GetEnumerator() => Enumerable.Range(1, count).GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

				public void Add(int item) => throw new NotSupportedException();

				public void Clear() => throw new NotSupportedException();

				public bool Contains(int item) => throw new NotSupportedException();

				public void CopyTo(int[] array, int arrayIndex) => throw new NotSupportedException();

				public bool Remove(int item) => throw new NotSupportedException();
			}

			private readonly struct StructCollection(params int[] items) : IEnumerable<int>
			{
				public int Count => items.Length;

				public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)items).GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			}
		}
	}
}
