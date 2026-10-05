using System.Collections;
using System.Collections.Generic;
using aweXpect.Equivalency;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class CollectionMemberTests
		{
			[Test]
			public async Task WhenCollectionsDifferInAnItemAndAMember_ShouldListTheItemFirst()
			{
				PagedResult<string> subject = new(5, "a", "b");
				PagedResult<string> expected = new(7, "a", "c");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Element [1] differed:
					                   Actual: "b"
					                 Expected: "c"
					             and
					               Property TotalCount differed:
					                   Actual: 5
					                 Expected: 7

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenCollectionsDifferInAMember_ShouldFail()
			{
				PagedResult<string> subject = new(5, "a", "b");
				PagedResult<string> expected = new(7, "a", "b");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property TotalCount differed:
					                   Actual: 5
					                 Expected: 7

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenCollectionsDifferInAMember_WhenItIsIgnored_ShouldSucceed()
			{
				PagedResult<string> subject = new(5, "a", "b");
				PagedResult<string> expected = new(7, "a", "b");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IgnoringMember("TotalCount"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCollectionsDifferInAMember_WhenItIsIgnoredForTheirType_ShouldSucceed()
			{
				Search subject = new()
				{
					Page = new PagedResult<string>(5, "a", "b"),
				};
				Search expected = new()
				{
					Page = new PagedResult<string>(7, "a", "b"),
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.For<PagedResult<string>>(x => x.IgnoringMember("Page.TotalCount")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCollectionsDifferInAMember_WithoutProperties_ShouldSucceed()
			{
				PagedResult<string> subject = new(5, "a", "b");
				PagedResult<string> expected = new(7, "a", "b");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IncludingProperties(IncludeMembers.None));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCollectionsHaveEqualItemsAndMembers_ShouldSucceed()
			{
				PagedResult<string> subject = new(5, "a", "b");
				PagedResult<string> expected = new(5, "a", "b");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsAnArray_ShouldOnlyCompareTheItems()
			{
				PagedResult<string> subject = new(5, "a", "b");
				string[] expected = ["a", "b",];

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedMemberIsMissingOnTheActualCollection_ShouldFail()
			{
				string[] subject = ["a", "b",];
				PagedResult<string> expected = new(5, "a", "b");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property TotalCount was missing on the actual object

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenNestedCollectionsDifferInAMember_ShouldFailWithThePath()
			{
				Search subject = new()
				{
					Page = new PagedResult<string>(5, "a", "b"),
				};
				Search expected = new()
				{
					Page = new PagedResult<string>(7, "a", "b"),
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Page.TotalCount differed:
					                   Actual: 5
					                 Expected: 7

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			public sealed class Search
			{
				public PagedResult<string>? Page { get; set; }
			}

			/// <summary>
			///     Its <see cref="Count" /> implements the one of the framework interface, so it describes the items.
			/// </summary>
			public sealed class PagedResult<T>(int totalCount, params T[] items) : IReadOnlyCollection<T>
			{
				public int TotalCount { get; } = totalCount;

				public int Count => items.Length;

				public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)items).GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			}
		}
	}
}
