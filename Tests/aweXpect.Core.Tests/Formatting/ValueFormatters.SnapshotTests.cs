using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class SnapshotTests
	{
		[Test]
		public async Task WhenCollectionHasMoreItemsThanTheMaximum_ShouldWriteAllItems()
		{
			int[] value = Enumerable.Range(1, 25).ToArray();
			string expectedResult = $"[{string.Join(", ", value)}]";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.Snapshot);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Snapshot);
			Formatter.Format(sb, value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(expectedResult)
				.Because("a snapshot must contain every item, not only the first ones");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCollectionHasMoreItemsThanTheMaximum_WithoutSnapshotOption_ShouldStillShortenIt()
		{
			int[] value = Enumerable.Range(1, 12).ToArray();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 2 more)]")
				.Because("failure messages keep shortening long collections");
		}

		[Test]
		public async Task WhenCollectionIsAConcurrentBag_ShouldSortTheItems()
		{
			ConcurrentBag<string> value = new(["b", "c", "a",]);

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("[\"a\", \"b\", \"c\"]")
				.Because("the order of a concurrent bag depends on the threads that added the items");
		}

		[Test]
		public async Task WhenCollectionIsAFrozenSet_ShouldSortTheItems()
		{
			FrozenSet<string> value = new[] { "b", "c", "a", }.ToFrozenSet();

			string result = Formatter.Format(value, FormattingOptions.Snapshot);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("[\"a\", \"b\", \"c\"]")
				.Because("a frozen set is an internal subclass of the abstract FrozenSet<T>");
			await That(objectResult).IsEqualTo("[\"a\", \"b\", \"c\"]");
		}

		[Test]
		public async Task WhenCollectionIsAHashSet_ShouldSortTheItems()
		{
			string[] items = Enumerable.Range(0, 50).Select(x => $"item {(x * 37) % 50}").ToArray();
			HashSet<string> value = [..items,];
			string expectedResult =
				$"[{string.Join(", ", items.OrderBy(x => x, StringComparer.Ordinal).Select(x => $"\"{x}\""))}]";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.Snapshot);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Snapshot);
			Formatter.Format(sb, value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(expectedResult)
				.Because("the hash code of a string differs in every run, so a snapshot must not depend on it");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCollectionIsAHashSet_WithIndentation_ShouldSortTheItemsOnTheirOwnLines()
		{
			HashSet<string> value = ["b", "c", "a",];
			string expectedResult = """
			                        [
			                            "a",
			                            "b",
			                            "c"
			                          ]
			                        """;

			string result = Formatter.Format(value, FormattingOptions.Indented() with
			{
				IsSnapshot = true,
			});

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCollectionIsAHashSet_WithoutSnapshotOption_ShouldKeepTheEnumerationOrder()
		{
			HashSet<string> value = ["b", "c", "a",];

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo("[\"b\", \"c\", \"a\"]")
				.Because("failure messages write the items in the order the collection enumerates them");
		}

		[Test]
		public async Task WhenCollectionIsAHashSetOfObjectsWithoutHashCode_ShouldSortTheItems()
		{
			HashSet<Item> value = [new Item("b"), new Item("c"), new Item("a"),];

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(
					"[ValueFormatters.SnapshotTests.Item { Name = \"a\" }, ValueFormatters.SnapshotTests.Item { Name = \"b\" }, ValueFormatters.SnapshotTests.Item { Name = \"c\" }]")
				.Because("the default hash code of an object differs in every run");
		}

		[Test]
		public async Task WhenCollectionIsAHashSetThatContainsItself_ShouldWriteTheRecursion()
		{
			HashSet<object> value = ["z",];
			value.Add(value);

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("[\"z\", [ *recursive* ]]");
		}

		[Test]
		public async Task WhenCollectionIsAHashSetWhoseEnumerationThrows_ShouldSortTheItemsBeforeTheException()
		{
			ThrowingHashSet value = [];

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(
					"[\"a\", \"b\", (the enumeration did throw an InvalidOperationException: enumeration failed)]")
				.Because("the exception stays after the items, which are sorted like those of a complete enumeration");
		}

		[Test]
		public async Task WhenCollectionIsAHashSetWithItemsThatFormatIdentically_ShouldWriteAllOfThem()
		{
			HashSet<Item> value = [new Item("b"), new Item("a"), new Item("b"),];

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(
				"[ValueFormatters.SnapshotTests.Item { Name = \"a\" }, ValueFormatters.SnapshotTests.Item { Name = \"b\" }, ValueFormatters.SnapshotTests.Item { Name = \"b\" }]");
		}

		[Test]
		public async Task WhenCollectionIsAHashSetWithNull_ShouldSortTheItems()
		{
			HashSet<string?> value = ["b", null, "a",];

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("[\"a\", \"b\", <null>]");
		}

		[Test]
		public async Task WhenCollectionIsAnImmutableHashSet_ShouldSortTheItems()
		{
			ImmutableHashSet<string> value = ImmutableHashSet.Create("b", "c", "a");

			string result = Formatter.Format(value, FormattingOptions.Snapshot);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("[\"a\", \"b\", \"c\"]");
			await That(objectResult).IsEqualTo("[\"a\", \"b\", \"c\"]");
		}

		[Test]
		public async Task WhenCollectionIsLazy_ShouldWriteAllItems()
		{
			IEnumerable<int> value = Enumerable.Range(1, 12).Select(x => x);

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]");
		}

		[Test]
		public async Task WhenCollectionIsNested_ShouldWriteAllItemsOnEveryLevel()
		{
			int[][] value = Enumerable.Range(0, 11).Select(x => Enumerable.Range(x * 11, 11).ToArray()).ToArray();
			string expectedResult =
				$"[{string.Join(", ", value.Select(inner => $"[{string.Join(", ", inner)}]"))}]";

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCollectionIsOrdered_ShouldKeepTheEnumerationOrder()
		{
			Comparer<string> descending = Comparer<string>.Create((x, y) => string.CompareOrdinal(y, x));
			List<string> list = ["b", "c", "a",];
			string[] array = ["b", "c", "a",];
			SortedSet<string> sortedSet = new(descending) { "b", "c", "a", };

			string listResult = Formatter.Format(list, FormattingOptions.Snapshot);
			string arrayResult = Formatter.Format(array, FormattingOptions.Snapshot);
			string sortedSetResult = Formatter.Format(sortedSet, FormattingOptions.Snapshot);

			await That(listResult).IsEqualTo("[\"b\", \"c\", \"a\"]");
			await That(arrayResult).IsEqualTo("[\"b\", \"c\", \"a\"]");
			await That(sortedSetResult).IsEqualTo("[\"c\", \"b\", \"a\"]")
				.Because("the order of a sorted set is given by its comparer");
		}

		[Test]
		public async Task WhenCollectionIsRecursive_ShouldWriteTheRecursion()
		{
			List<object> value = [1,];
			value.Add(value);

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("[1, [ *recursive* ]]");
		}

		[Test]
		public async Task WhenCollectionIsTooDeepForTheStack_ShouldThrowInsteadOfShortening()
		{
			List<object> value = [];
			List<object> current = value;
			for (int i = 1; i < 100_000; i++)
			{
				List<object> next = [];
				current.Add(next);
				current = next;
			}

			void Act() => Formatter.Format(value, FormattingOptions.Snapshot);

			await That(Act).Throws<InsufficientExecutionStackException>()
				.WithMessage(
					"The value is nested too deeply to be formatted completely on the remaining stack at a depth of *.")
				.AsWildcard()
				.Because("the enumeration must not write a too deep collection as a placeholder");
		}

		[Test]
		public async Task WhenCustomFormatterIsRegistered_ShouldReceiveTheOptions()
		{
			using IDisposable _ = ValueFormatter.Register(new TaggedFormatter());
			Tagged value = new(Enumerable.Range(1, 12).ToArray());

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("Tagged(IsSnapshot=True, [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12])")
				.Because("a custom formatter receives the options and passes them on to the nested values");
		}

		[Test]
		public async Task WhenCustomFormatterIsTooDeepForTheStack_ShouldThrowInsteadOfShortening()
		{
			using IDisposable _ = ValueFormatter.Register(new BoxedFormatter());
			Boxed value = new(null);
			for (int i = 1; i < 100_000; i++)
			{
				value = new Boxed(value);
			}

			void Act() => Formatter.Format(value, FormattingOptions.Snapshot);

			await That(Act).Throws<InsufficientExecutionStackException>()
				.WithMessage(
					"The value is nested too deeply to be formatted completely on the remaining stack at a depth of *.")
				.AsWildcard()
				.Because("a custom formatter must not write a too deep value as a placeholder");
		}

		[Test]
		public async Task WhenExceptionMessageContainsABackslash_ShouldEscapeIt()
		{
			InvalidOperationException lineBreak = new("a\nb");
			InvalidOperationException backslash = new("a\\nb");

			string lineBreakResult = Formatter.Format(lineBreak, FormattingOptions.Snapshot);
			string backslashResult = Formatter.Format(backslash, FormattingOptions.Snapshot);

			await That(lineBreakResult).IsEqualTo("InvalidOperationException: a\\nb");
			await That(backslashResult).IsEqualTo("InvalidOperationException: a\\\\nb")
				.Because("a snapshot must tell a line break from a backslash followed by an n");
		}

		[Test]
		public async Task WhenExceptionMessageContainsABackslash_WithoutSnapshotOption_ShouldKeepIt()
		{
			InvalidOperationException value = new("C:\\temp");

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo("InvalidOperationException: C:\\temp")
				.Because("failure messages keep the backslashes of unquoted text, which often holds a path");
		}

		[Test]
		public async Task WhenDictionaryHasMoreEntriesThanTheMaximum_ShouldWriteAllEntries()
		{
			Dictionary<int, string> value = Enumerable.Range(1, 12).ToDictionary(x => x, x => $"v{x}");
			string expectedResult = $"{{{string.Join(", ", value.Select(x => $"[{x.Key}] = \"{x.Value}\""))}}}";

			string result = Formatter.Format(value, FormattingOptions.Snapshot);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenDictionaryIsAConcurrentDictionary_ShouldSortTheEntriesByKey()
		{
			ConcurrentDictionary<int, string> value = new();
			value[2] = "b";
			value[10] = "a";
			value[1] = "c";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.Snapshot);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Snapshot);
			Formatter.Format(sb, value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("{[1] = \"c\", [10] = \"a\", [2] = \"b\"}")
				.Because("the entries are sorted by their formatted key, not by their whole text");
			await That(objectResult).IsEqualTo("{[1] = \"c\", [10] = \"a\", [2] = \"b\"}");
			await That(sb.ToString()).IsEqualTo("{[1] = \"c\", [10] = \"a\", [2] = \"b\"}");
		}

		[Test]
		public async Task WhenDictionaryIsAConcurrentDictionary_WithRegisteredFormatter_ShouldSortTheEntriesByKey()
		{
			using IDisposable _ = ValueFormatter.Register(new TaggedFormatter());
			ConcurrentDictionary<string, int> value = new();
			value["b"] = 1;
			value["c"] = 2;
			value["a"] = 3;

			string result = Formatter.Format((object?)value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("{[\"a\"] = 3, [\"b\"] = 1, [\"c\"] = 2}")
				.Because("with registered formatters, the entries are enumerated as key-value pairs");
		}

		[Test]
		public async Task WhenDictionaryIsAConcurrentDictionaryWithKeysThatFormatIdentically_ShouldSortTheEntriesByTheirText()
		{
			ConcurrentDictionary<Item, int> value = new();
			value[new Item("a")] = 2;
			value[new Item("a")] = 1;

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(
					"{[ValueFormatters.SnapshotTests.Item { Name = \"a\" }] = 1, [ValueFormatters.SnapshotTests.Item { Name = \"a\" }] = 2}")
				.Because("entries with the same formatted key are sorted by their whole text");
		}

		[Test]
		public async Task WhenDictionaryIsAFrozenDictionary_ShouldSortTheEntriesByKey()
		{
			FrozenDictionary<string, int> value = new Dictionary<string, int>
			{
				["b"] = 1,
				["c"] = 2,
				["a"] = 3,
			}.ToFrozenDictionary();

			string result = Formatter.Format(value, FormattingOptions.Snapshot);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("{[\"a\"] = 3, [\"b\"] = 1, [\"c\"] = 2}")
				.Because("a frozen dictionary is an internal subclass of the abstract FrozenDictionary<TKey, TValue>");
			await That(objectResult).IsEqualTo("{[\"a\"] = 3, [\"b\"] = 1, [\"c\"] = 2}");
		}

		[Test]
		public async Task WhenDictionaryIsAHashtable_ShouldSortTheEntriesByKey()
		{
			Hashtable value = new()
			{
				["b"] = 1,
				["c"] = 2,
				["a"] = 3,
			};

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("{[\"a\"] = 3, [\"b\"] = 1, [\"c\"] = 2}");
		}

		[Test]
		public async Task WhenDictionaryIsAnImmutableDictionary_ShouldSortTheEntriesByKey()
		{
			ImmutableDictionary<string, int> value = ImmutableDictionary<string, int>.Empty
				.Add("b", 1).Add("c", 2).Add("a", 3);

			string result = Formatter.Format(value, FormattingOptions.Snapshot);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("{[\"a\"] = 3, [\"b\"] = 1, [\"c\"] = 2}");
			await That(objectResult).IsEqualTo("{[\"a\"] = 3, [\"b\"] = 1, [\"c\"] = 2}");
		}

		[Test]
		public async Task WhenDictionaryIsOrdered_ShouldKeepTheEnumerationOrder()
		{
			Dictionary<string, int> dictionary = new()
			{
				["b"] = 1,
				["c"] = 2,
				["a"] = 3,
			};
			SortedDictionary<string, int> sortedDictionary =
				new(Comparer<string>.Create((x, y) => string.CompareOrdinal(y, x)))
				{
					["b"] = 1,
					["c"] = 2,
					["a"] = 3,
				};

			string dictionaryResult = Formatter.Format(dictionary, FormattingOptions.Snapshot);
			string sortedDictionaryResult = Formatter.Format(sortedDictionary, FormattingOptions.Snapshot);

			await That(dictionaryResult).IsEqualTo("{[\"b\"] = 1, [\"c\"] = 2, [\"a\"] = 3}")
				.Because("a dictionary keeps the order in which its entries were added, which may be intended");
			await That(sortedDictionaryResult).IsEqualTo("{[\"c\"] = 2, [\"b\"] = 1, [\"a\"] = 3}");
		}

		[Test]
		public async Task WhenExceptionMessageIsLongerThanTheMaximumLength_ShouldWriteTheWholeMessage()
		{
			string message = new('x', 150);
			InvalidOperationException value = new(message);

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo($"InvalidOperationException: {message}");
		}

		[Test]
		public async Task WhenGraphIsDeeperThanTheMaximumDepth_ShouldWriteEveryLevel()
		{
			LinkedNode value = LinkedNode.Chain(100);
			string expectedResult =
				string.Concat(Enumerable.Repeat("ValueFormatters.SnapshotTests.LinkedNode { Next = ", 100)) +
				"<null>" +
				string.Concat(Enumerable.Repeat(" }", 100));

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(expectedResult)
				.Because("only the remaining stack limits the depth of a snapshot");
		}

		[Test]
		public async Task WhenGraphIsRecursive_ShouldWriteTheRecursion()
		{
			SelfNode value = new()
			{
				Value = 1,
			};
			value.Next = value;

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(
				"ValueFormatters.SnapshotTests.SelfNode { Next = ValueFormatters.SnapshotTests.SelfNode { *recursive* }, Value = 1 }");
		}

		[Test]
		public async Task WhenGraphIsTooDeepForTheStack_ShouldThrowInsteadOfShortening()
		{
			LinkedNode value = LinkedNode.Chain(1_000_000);

			void Act() => Formatter.Format(value, FormattingOptions.Snapshot);

			await That(Act).Throws<InsufficientExecutionStackException>()
				.WithMessage(
					"The value is nested too deeply to be formatted completely on the remaining stack at a depth of *.")
				.AsWildcard()
				.Because("a snapshot must neither overflow the stack nor leave out the deepest levels");
		}

		[Test]
		public async Task WhenGraphSharesNodesOnEveryLevel_ShouldWriteEveryNode()
		{
			SharingNode value = new();
			for (int i = 1; i < 12; i++)
			{
				value = new SharingNode
				{
					Left = value,
					Right = value,
				};
			}

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(Count(result, "SharingNode { Left = ")).IsEqualTo(4095)
				.Because("a snapshot writes more than the 1000 nested objects of a failure message");
			await That(result).DoesNotContain("…");

			static int Count(string text, string part)
				=> (text.Length - text.Replace(part, "").Length) / part.Length;
		}

		[Test]
		public async Task WhenMultiDimensionalArrayHasMoreItemsThanTheMaximum_ShouldWriteAllItems()
		{
			int[,] value = new int[4, 4];
			value[3, 3] = 1;

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("[[0, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 1]]");
		}

		[Test]
		public async Task WhenObjectHasAHashSetMember_ShouldSortTheItems()
		{
			Wrapper value = new(new Wrapper[]
			{
				new(new HashSet<string> { "b", "c", "a", }),
				new(new ConcurrentDictionary<string, int>([new("b", 1), new("a", 2),])),
			});

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(
					"ValueFormatters.SnapshotTests.Wrapper { Inner = [ValueFormatters.SnapshotTests.Wrapper { Inner = [\"a\", \"b\", \"c\"] }, ValueFormatters.SnapshotTests.Wrapper { Inner = {[\"a\"] = 2, [\"b\"] = 1} }] }")
				.Because("nested collections in an object graph cannot be sorted by the caller");
		}

		[Test]
		public async Task WhenObjectHasALongCollectionMember_ShouldWriteAllItems()
		{
			Wrapper value = new(Enumerable.Range(1, 12).ToArray());

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo(
				"ValueFormatters.SnapshotTests.Wrapper { Inner = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12] }");
		}

		[Test]
		public async Task WhenObjectRendersItselfWithABackslash_ShouldEscapeIt()
		{
			Rendered value = new("a\\nb");

			string result = Formatter.Format(value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo("a\\\\nb")
				.Because("a snapshot must tell a line break from a backslash followed by an n");
		}

		[Test]
		public async Task WhenStringIsLongerThanTheMaximumLength_ShouldWriteTheWholeString()
		{
			string value = new('a', 150);
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.Snapshot);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.Snapshot);
			Formatter.Format(sb, value, FormattingOptions.Snapshot);

			await That(result).IsEqualTo($"\"{value}\"");
			await That(objectResult).IsEqualTo($"\"{value}\"");
			await That(sb.ToString()).IsEqualTo($"\"{value}\"");
		}

		[Test]
		public async Task WithIndentation_ShouldWriteAllItemsIndented()
		{
			int[] value = Enumerable.Range(1, 12).ToArray();
			string expectedResult = """
			                        [
			                            1,
			                            2,
			                            3,
			                            4,
			                            5,
			                            6,
			                            7,
			                            8,
			                            9,
			                            10,
			                            11,
			                            12
			                          ]
			                        """;

			string result = Formatter.Format(value, FormattingOptions.Indented() with
			{
				IsSnapshot = true,
			});

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithMultipleLines_WhenItemIsAnObject_ShouldWriteAllItemsOnTheirOwnLines()
		{
			Wrapper[] value = Enumerable.Range(1, 11).Select(x => new Wrapper(x)).ToArray();
			string expectedResult =
				"[" + Environment.NewLine +
				string.Join("," + Environment.NewLine, value.Select(x =>
					$"  ValueFormatters.SnapshotTests.Wrapper {{{Environment.NewLine}    Inner = {x.Inner}{Environment.NewLine}  }}")) +
				Environment.NewLine + "]";

			string result = Formatter.Format(value, FormattingOptions.MultipleLines with
			{
				IsSnapshot = true,
			});

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithType_ShouldPrefixTheTypeAndWriteAllItems()
		{
			int[] value = Enumerable.Range(1, 12).ToArray();

			string result = Formatter.Format(value, FormattingOptions.WithType with
			{
				IsSnapshot = true,
			});

			await That(result).IsEqualTo("int[] [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]");
		}

		private sealed class Boxed(Boxed? inner)
		{
			public Boxed? Inner { get; } = inner;
		}

		private sealed class BoxedFormatter : IValueFormatter
		{
			public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
			{
				if (value is not Boxed boxed)
				{
					return false;
				}

				stringBuilder.Append("Boxed");
				Formatter.Format(stringBuilder, new[] { boxed.Inner, }, options);
				return true;
			}
		}

		private sealed class Item(string name)
		{
			public string Name { get; } = name;
		}

		private sealed class LinkedNode
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public LinkedNode? Next { get; set; }

			public static LinkedNode Chain(int length)
			{
				LinkedNode root = new();
				LinkedNode current = root;
				for (int i = 1; i < length; i++)
				{
					current.Next = new LinkedNode();
					current = current.Next;
				}

				return root;
			}
		}

		private sealed class Rendered(string rendering)
		{
			public override string ToString() => rendering;
		}

		private sealed class SelfNode
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public SelfNode? Next { get; set; }

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
		}

		private sealed class SharingNode
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public SharingNode? Left { get; set; }

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public SharingNode? Right { get; set; }
		}

		private sealed class Tagged(int[] items)
		{
			public int[] Items { get; } = items;
		}

		private sealed class TaggedFormatter : IValueFormatter
		{
			public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
			{
				if (value is not Tagged tagged)
				{
					return false;
				}

				stringBuilder.Append("Tagged(IsSnapshot=").Append(options?.IsSnapshot).Append(", ");
				Formatter.Format(stringBuilder, tagged.Items, options);
				stringBuilder.Append(')');
				return true;
			}
		}

		/// <remarks>
		///     Reimplements the enumeration of a <see cref="HashSet{T}" />, which is not virtual.
		/// </remarks>
		private sealed class ThrowingHashSet : HashSet<string>, IEnumerable<string>
		{
			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				yield return "b";
				yield return "a";
				throw new InvalidOperationException("enumeration failed");
			}

			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<string>)this).GetEnumerator();
		}

		private sealed class Wrapper(object inner)
		{
			public object Inner { get; } = inner;
		}
	}
}
