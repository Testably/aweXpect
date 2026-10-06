using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class Keys
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectationOnKeysFails_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).Keys.Contains(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain an item equal to 0 at least once,
					             but it did not contain it

					             Collection (keys):
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenExpectationOnKeysIsSatisfied_ShouldSucceed()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).Keys.Contains(2).And.IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMemberOfWhose_ShouldReferToTheKeysAsIt()
			{
				MapClass subject = new(ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]));

				async Task Act()
					=> await That(subject).Whose(o => o.Map, m => m.Keys.Contains(0));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Map has keys that contain an item equal to 0 at least once,
					             but it did not contain it

					             Collection (Map.keys):
					             [1, 2, 3]
					             """)
					.Because("the keys, not the member Map, are the subject of the continued expectation");
			}

			[Test]
			public async Task WhenNegatedExpectationOnKeysFails_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Keys.Contains(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that do not contain an item equal to 2,
					             but it contained 2 at least once

					             Collection (keys):
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenNegatedExpectationOnKeysIsSatisfied_ShouldSucceed()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Keys.Contains(4));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Keys.Contains(0));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that do not contain an item equal to 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).Keys.Contains(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain an item equal to 0 at least once,
					             but it was <null>
					             """);
			}

			private sealed class MapClass(IDictionary<int, string> map)
			{
				public IDictionary<int, string> Map { get; } = map;
			}
		}

		public sealed class DictionaryTests
		{
			[Test]
			public async Task WhenExpectationOnKeysIsSatisfied_ShouldSucceed()
			{
				Dictionary<int, string> subject = new()
				{
					[1] = "foo",
					[2] = "bar",
				};

				async Task Act()
					=> await That(subject).Keys.Contains(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).Keys.Contains(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain an item equal to 2 at least once,
					             but it was <null>
					             """);
			}
		}

		public sealed class KeyComparerTests
		{
			[Test]
			public async Task ForAFrozenDictionary_ShouldUseTheKeyComparer()
			{
				IDictionary<string, int> subject = new Dictionary<string, int>
					{
						{
							"a", 1
						},
					}
					.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

				async Task Act()
					=> await That(subject).Keys.Contains("A");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForAnImmutableDictionary_ShouldUseTheKeyComparer()
			{
				IDictionary<string, int> subject = ImmutableDictionary.Create<string, int>(StringComparer.OrdinalIgnoreCase)
					.Add("a", 1);

				async Task Act()
					=> await That(subject).Keys.Contains("A");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForAnImmutableSortedDictionary_ShouldOrderByTheKeyComparer()
			{
				IDictionary<string, int> subject = ImmutableSortedDictionary.Create<string, int>(new ReverseComparer())
					.Add("a", 1)
					.Add("b", 2);

				async Task Act()
					=> await That(subject).Keys.IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForAReadOnlyDictionary_ShouldUseTheKeyComparerOfTheWrappedDictionary()
			{
				IDictionary<string, int> subject = new ReadOnlyDictionary<string, int>(
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
					{
						{
							"a", 1
						},
					});

				async Task Act()
					=> await That(subject).Keys.Contains("A");

				await That(Act).DoesNotThrow()
					.Because("ContainsKey(\"A\") on the same dictionary succeeds");
			}

			[Test]
			public async Task ForAnImmutableSortedDictionaryWithTheDefaultComparer_ShouldOrderByTheKeyComparer()
			{
				IDictionary<string, int> subject = ImmutableSortedDictionary.Create<string, int>()
					.Add("a", 1)
					.Add("B", 2);

				async Task Act()
					=> await That(subject).Keys.IsInAscendingOrder();

				await That(Act).DoesNotThrow()
					.Because("the keys of a sorted dictionary are always in its own order");
			}

			[Test]
			public async Task ForASortedDictionary_ShouldOrderByTheKeyComparer()
			{
				SortedDictionary<string, int> subject = new(new ReverseComparer())
				{
					{
						"a", 1
					},
					{
						"b", 2
					},
				};

				async Task Act()
					=> await That(subject).Keys.IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForASortedDictionaryWithTheDefaultComparer_ShouldOrderByTheKeyComparer()
			{
				SortedDictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
					{
						"B", 2
					},
				};

				async Task Act()
					=> await That(subject).Keys.IsInAscendingOrder();

				await That(Act).DoesNotThrow()
					.Because("the keys of a sorted dictionary are always in its own order");
			}

			[Test]
			public async Task ForASortedListWithTheDefaultComparer_ShouldOrderByTheKeyComparer()
			{
				SortedList<string, int> subject = new()
				{
					{
						"a", 1
					},
					{
						"B", 2
					},
				};

				async Task Act()
					=> await That(subject).Keys.IsInAscendingOrder();

				await That(Act).DoesNotThrow()
					.Because("the keys of a sorted list are always in its own order");
			}

			[Test]
			public async Task IsEqualTo_ShouldUseTheKeyComparer()
			{
				Dictionary<string, int> subject = new(StringComparer.OrdinalIgnoreCase)
				{
					{
						"a", 1
					},
					{
						"b", 2
					},
				};

				async Task Act()
					=> await That(subject).Keys.IsEqualTo(["A", "B",]);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IsEquivalentTo_ForASortedDictionaryWithTheDefaultComparer_ShouldNotIgnoreTheOrder()
			{
				SortedDictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
					{
						"b", 2
					},
				};

				async Task Act()
					=> await That(subject).Keys.IsEquivalentTo(new[]
					{
						"b", "a",
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that are equivalent to new[] { "b", "a", },
					             but it was not:
					               Element [0] differed:
					                   Actual: "a"
					                 Expected: "b"
					             and
					               Element [1] differed:
					                   Actual: "b"
					                 Expected: "a"

					             Equivalency options (keys):
					              - include public fields and properties
					             """)
					.Because("the keys of a sorted dictionary are an ordered sequence, not a set");
			}

			[Test]
			public async Task ShouldUseTheKeyComparer()
			{
				IDictionary<string, int> subject = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
				{
					{
						"a", 1
					},
				};

				async Task Act()
					=> await That(subject).Keys.Contains("A");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Using_ShouldOverrideTheKeyComparer()
			{
				Dictionary<string, int> subject = new(StringComparer.OrdinalIgnoreCase)
				{
					{
						"a", 1
					},
				};

				async Task Act()
					=> await That(subject).Keys.Contains("a").Using(new AllDifferentComparer());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain "a" using AllDifferentComparer at least once,
					             but it did not contain it

					             Collection (keys):
					             [
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WhenKeysContainTheKeyAccordingToTheKeyComparer_NegatedShouldFail()
			{
				Dictionary<string, int> subject = new(StringComparer.OrdinalIgnoreCase)
				{
					{
						"a", 1
					},
				};

				async Task Act()
					=> await That(subject).Keys.DoesNotContain("A");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that do not contain "A" using the subject's StringComparer.OrdinalIgnoreCase,
					             but it contained "a" once

					             Collection (keys):
					             [
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WhenKeysDoNotContainTheKeyAccordingToTheKeyComparer_ShouldNameTheComparer()
			{
				Dictionary<string, int> subject = new(StringComparer.OrdinalIgnoreCase)
				{
					{
						"a", 1
					},
				};

				async Task Act()
					=> await That(subject).Keys.Contains("b");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain "b" using the subject's StringComparer.OrdinalIgnoreCase at least once,
					             but it did not contain it

					             Collection (keys):
					             [
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WithTheDefaultKeyComparer_ShouldUseTheDefaultEquality()
			{
				Dictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
				};

				async Task Act()
					=> await That(subject).Keys.Contains("A");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain "A" at least once,
					             but it did not contain it

					             Collection (keys):
					             [
					               "a"
					             ]
					             """);
			}
		}

		public sealed class OverloadTests
		{
			[Test]
			public async Task ForASortedDictionary_ShouldNotBeAmbiguous()
			{
				SortedDictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
				};

				async Task Act()
					=> await That(subject).Keys.Contains("a");

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}
	}
}
