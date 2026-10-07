using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif
using System.Linq;
using System.Text;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class CollectionTests
	{
		[Test]
		public async Task InFailureMessage_WhenCountIsKnown_ShouldNameTheNumberOfRemainingItems()
		{
			int[] subject = Enumerable.Range(1, 25).ToArray();
			int[] expected = Enumerable.Range(1, 26).ToArray();

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but it lacked 1 of 26 expected items: 26

				             Collection:
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
				               (… and 15 more)
				             ]

				             Expected:
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
				               (… and 16 more)
				             ]
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenCountThrows_ShouldListTheItems()
		{
			object subject = new ThrowingCountCollection([1, 2,]);

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [
				                 1,
				                 2
				               ]
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenEnumerationThrows_ShouldEscapeLineBreaksInTheMessage()
		{
			object subject = Throwing(new InvalidOperationException("enumeration\nfailed"));

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [the enumeration did throw an InvalidOperationException: enumeration\nfailed]
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenEnumerationThrowsAnExceptionWhoseMessageThrows_ShouldRenderAPlaceholder()
		{
			object subject = Throwing(new ThrowingMessageException());

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [the enumeration did throw a ThrowingMessageException: [Message of ThrowingMessageException did throw an InvalidOperationException]]
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenEnumerationThrows_ShouldRenderAPlaceholder()
		{
			object subject = Throwing(new InvalidOperationException("enumeration failed"));

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [the enumeration did throw an InvalidOperationException: enumeration failed]
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenEnumerationThrowsAfterTheFirstItem_ShouldEscapeLineBreaksInTheMessage()
		{
			object subject = Throwing(new InvalidOperationException("enumeration\nfailed"), 1);

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [
				                 1,
				                 (the enumeration did throw an InvalidOperationException: enumeration\nfailed)
				               ]
				             """)
				.Because("the placeholder must stay on the line of an item");
		}

		[Test]
		public async Task InFailureMessage_WhenEnumerationThrowsAfterTheFirstItem_ShouldRenderAPlaceholderAfterTheItems()
		{
			object subject = Throwing(new InvalidOperationException("enumeration failed"), 1, 2);

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [
				                 1,
				                 2,
				                 (the enumeration did throw an InvalidOperationException: enumeration failed)
				               ]
				             """)
				.Because("the items that were read before the exception are listed as well");
		}

		[Test]
		public async Task InFailureMessage_WhenLazySequenceWasFullyEnumerated_ShouldNameTheNumberOfRemainingItems()
		{
			IEnumerable<int> subject = Lazy(Enumerable.Range(1, 25));
			int[] expected = Enumerable.Range(1, 26).ToArray();

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but it lacked 1 of 26 expected items: 26

				             Collection:
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
				               (… and 15 more)
				             ]

				             Expected:
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
				               (… and 16 more)
				             ]
				             """);
		}

		[Test]
		public async Task ShouldFormatItems()
		{
			string expectedResult = "[\"1\", \"2\", \"3\", \"4\"]";
			IEnumerable<string> value = Enumerable.Range(1, 4).Select(x => x.ToString());
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task ShouldLimitTo10Items()
		{
			string expectedResult = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and maybe more)]";
			IEnumerable<int> value = Lazy(Enumerable.Range(1, 20));
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCollectionContainsItself_ShouldDetectTheRecursion()
		{
			string expectedResult = "[[ *recursive* ]]";
			SelfContainingCollection value = new();
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		[Arguments(10, "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]")]
		[Arguments(11, "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 1 more)]")]
		[Arguments(25, "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 15 more)]")]
		public async Task WhenCountIsKnown_ShouldNameTheNumberOfRemainingItems(int count, string expectedResult)
		{
			int[] value = Enumerable.Range(1, count).ToArray();
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCountIsKnown_WithLineBreaks_ShouldNameTheNumberOfRemainingItemsOnTheLastLine()
		{
			List<string> value = Enumerable.Range(1, 12).Select(x => x.ToString()).ToList();

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo("""
			                             [
			                               "1",
			                               "2",
			                               "3",
			                               "4",
			                               "5",
			                               "6",
			                               "7",
			                               "8",
			                               "9",
			                               "10",
			                               (… and 2 more)
			                             ]
			                             """);
		}

		[Test]
		public async Task WhenCountIsKnown_WithType_ShouldNameTheNumberOfRemainingItems()
		{
			int[] value = Enumerable.Range(1, 12).ToArray();

			string result = Formatter.Format(value, FormattingOptions.WithType);

			await That(result).IsEqualTo("int[] [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 2 more)]");
		}

		[Test]
		public async Task WhenCountIsNotKnown_ShouldNotEnumerateFurtherThanNeeded()
		{
			int enumeratedItems = 0;
			IEnumerable<int> value = Enumerable.Range(1, 25).Select(x =>
			{
				enumeratedItems++;
				return x;
			}).Where(_ => true);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and maybe more)]");
			await That(enumeratedItems).IsEqualTo(11);
		}

		[Test]
		public async Task WhenCountIsNotKnown_WithLineBreaks_ShouldSayOnTheLastLineThatMoreItemsMayFollow()
		{
			IEnumerable<int> value = Lazy(Enumerable.Range(1, 12));

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo("""
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
			                               (… and maybe more)
			                             ]
			                             """);
		}

		[Test]
		public async Task WhenCountAndEnumerationThrow_ShouldRenderAPlaceholderForTheEnumeration()
		{
			string expectedResult = "[the enumeration did throw an InvalidOperationException: enumeration failed]";
			ThrowingCountCollection value = new(Throwing(new InvalidOperationException("enumeration failed")));
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCountThrows_AsTaskResult_ShouldListTheItems()
		{
			Task<object> value = Task.FromResult<object>(new ThrowingCountCollection([1, 2, 3,]));

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("Task<object> (RanToCompletion, [1, 2, 3])");
		}

		[Test]
		public async Task WhenCountThrows_AsTupleItem_ShouldListTheItems()
		{
			(int, ThrowingCountCollection) value = (0, new ThrowingCountCollection([1, 2, 3,]));

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo("(0, [1, 2, 3])");
		}

		[Test]
		public async Task WhenCountThrows_ShouldListTheItems()
		{
			string expectedResult = "[1, 2, 3]";
			ThrowingCountCollection value = new([1, 2, 3,]);
			StringBuilder sb = new();
			StringBuilder nonGenericSb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);
			Formatter.Format(nonGenericSb, (IEnumerable)value);

			await That(result).IsEqualTo(expectedResult)
				.Because("the count is only needed to name the number of remaining items");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
			await That(nonGenericSb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCountThrows_ShouldSayThatMoreItemsMayFollow()
		{
			string expectedResult = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and maybe more)]";
			ThrowingCountCollection value = new(Enumerable.Range(1, 12));
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCountThrows_WithLineBreaks_ShouldSayOnTheLastLineThatMoreItemsMayFollow()
		{
			string expectedResult = """
			                        [
			                          1,
			                          2,
			                          3,
			                          (… and maybe more)
			                        ]
			                        """;
			ThrowingCountCollection value = new(Enumerable.Range(1, 12));
			using IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(3);

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCountThrows_WithTotalItemCount_ShouldNameTheNumberOfRemainingItems()
		{
			ThrowingCountCollection value = new(Enumerable.Range(1, 12));

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				TotalItemCount = 12,
			});

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 2 more)]");
		}

		[Test]
		public async Task WhenCountThrows_WithType_ShouldIncludeTypeInformation()
		{
			string expectedResult = "ValueFormatters.CollectionTests.ThrowingCountCollection [1, 2, 3]";
			ThrowingCountCollection value = new([1, 2, 3,]);

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
		}

#if NET8_0_OR_GREATER
		[Test]
		public async Task WhenDefaultImmutableArray_ShouldRenderAPlaceholderForTheEnumeration()
		{
			ImmutableArray<int> value = default;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object)value);
			Formatter.Format(sb, value);

			await That(result).StartsWith("[the enumeration did throw an InvalidOperationException: ").And.EndsWith("]")
				.Because("neither the count nor the items of an uninitialized array can be read");
			await That(objectResult).IsEqualTo(result);
			await That(sb.ToString()).IsEqualTo(result);
		}
#endif

		[Test]
		public async Task WhenEnumerationThrowsAfterTheFirstItem_ShouldRenderAPlaceholderAfterTheItems()
		{
			string expectedResult =
				"[1, 2, (the enumeration did throw an InvalidOperationException: enumeration failed)]";
			IEnumerable<int> value = Throwing(new InvalidOperationException("enumeration failed"), 1, 2);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenGenericCollection_ShouldNameTheNumberOfRemainingItems()
		{
			HashSet<int> value = [..Enumerable.Range(1, 12),];

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 2 more)]");
		}

		[Test]
		public async Task WhenGenericCollection_WhenCountThrows_ShouldListTheItems()
		{
			ThrowingCountGenericCollection value = new([1, 2, 3,]);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[1, 2, 3]");
		}

		[Test]
		public async Task WhenNested_ShouldNameTheNumberOfRemainingItemsPerCollection()
		{
			int[][] value = Enumerable.Range(1, 12).Select(x => Enumerable.Range(x, 11).ToArray()).ToArray();

			string result = Formatter.Format(value);

			await That(result).StartsWith("[[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 1 more)], [2, ")
				.And.EndsWith("[10, 11, 12, 13, 14, 15, 16, 17, 18, 19, (… and 1 more)], (… and 2 more)]");
		}

		[Test]
		public async Task WhenNestedDeeperThanTheMaximumDepth_ShouldLeaveOutTheItemsOfTheDeepestCollection()
		{
			List<object> value = [];
			List<object> current = value;
			for (int i = 0; i < 1000; i++)
			{
				List<object> inner = [];
				current.Add(inner);
				current = inner;
			}

			string expectedResult = new string('[', 20) + "[ … ]" + new string(']', 20);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenNull_ShouldUseDefaultNullString()
		{
			IEnumerable<int>? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value!);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value!);

			await That(result).IsEqualTo(ValueFormatter.NullString);
			await That(objectResult).IsEqualTo(ValueFormatter.NullString);
			await That(sb.ToString()).IsEqualTo(ValueFormatter.NullString);
		}

		[Test]
		public async Task WhenReadOnlyCollection_ShouldNameTheNumberOfRemainingItems()
		{
			IEnumerable<int> value = new ReadOnlyCollection(Enumerable.Range(1, 13).ToArray());

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 3 more)]");
		}

		[Test]
		public async Task WhenReadOnlyCollection_WhenCountThrows_ShouldListTheItems()
		{
			ThrowingCountReadOnlyCollection value = new([1, 2, 3,]);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[1, 2, 3]");
		}

		[Test]
		public async Task WhenSameInstanceIsContainedTwice_ShouldFormatBoth()
		{
			string expectedResult = "[[1, 2], [1, 2]]";
			int[] inner = [1, 2,];
			List<int[]> value = [inner, inner,];
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("an instance is only a recursion within its own items, not next to itself");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithType_Array_ShouldIncludeTypeInformation()
		{
			string expectedResult = "int[] [1, 2, 3, 4]";
			int[] value = [..Enumerable.Range(1, 4),];
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithType_EnumerableShouldIncludeTypeInformation()
		{
			string expectedResult = "List<int> [1, 2, 3, 4, 5]";
			IEnumerable<int> value = Enumerable.Range(1, 5).ToList();
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithIndentation_ShouldIndentTheItemsAndTheClosingBracket()
		{
			int[] value = [1, 2,];
			string expectedResult = """
			                        [
			                            1,
			                            2
			                          ]
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.Indented());
			Formatter.Format(sb, value, FormattingOptions.Indented());

			await That(result).IsEqualTo(expectedResult)
				.Because("every line after the first one starts with the indentation, like the members of an object");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithIndentation_WhenItemIsAnObject_ShouldIndentItsMembersOnce()
		{
			Item[] value =
			[
				new()
				{
					Value = 1,
				},
			];
			string expectedResult = """
			                        [
			                            ValueFormatters.CollectionTests.Item {
			                              Value = 1
			                            }
			                          ]
			                        """;

			string result = Formatter.Format(value, FormattingOptions.Indented());

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithLineBreaks_WhenItemIsNull_ShouldUseDefaultNullString()
		{
			object?[] value = [null, 1,];
			string expectedResult = """
			                        [
			                          <null>,
			                          1
			                        ]
			                        """;

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult);
		}

		private static IEnumerable<int> Lazy(IEnumerable<int> items)
		{
			foreach (int item in items)
			{
				yield return item;
			}
		}

		private static IEnumerable<int> Throwing(Exception exception, params int[] items)
		{
			foreach (int item in items)
			{
				yield return item;
			}

			throw exception;
		}

		private sealed class Item
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
		}

		private sealed class ReadOnlyCollection(int[] items) : IReadOnlyCollection<int>
		{
			public int Count => items.Length;

			public IEnumerator<int> GetEnumerator()
				=> ((IEnumerable<int>)items).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()
				=> GetEnumerator();
		}

		/// <remarks>
		///     Throws once it was enumerated too often, so that following the cycle fails the test instead of
		///     overflowing the stack.
		/// </remarks>
		private sealed class SelfContainingCollection : IEnumerable<object>
		{
			private int _enumerations;

			public IEnumerator<object> GetEnumerator()
			{
				if (++_enumerations > 100)
				{
					throw new InvalidOperationException("enumerated too often");
				}

				return ((IEnumerable<object>)new object[]
				{
					this,
				}).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
				=> GetEnumerator();
		}

		private sealed class ThrowingCountCollection(IEnumerable<int> items) : ICollection, IEnumerable<int>
		{
			public int Count => throw new InvalidOperationException("count failed");
			public bool IsSynchronized => false;
			public object SyncRoot => this;

			public void CopyTo(Array array, int index)
				=> throw new NotSupportedException();

			public IEnumerator<int> GetEnumerator()
				=> items.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()
				=> GetEnumerator();
		}

		private sealed class ThrowingCountGenericCollection(int[] items) : ICollection<int>
		{
			public int Count => throw new InvalidOperationException("count failed");
			public bool IsReadOnly => true;

			public void Add(int item)
				=> throw new NotSupportedException();

			public void Clear()
				=> throw new NotSupportedException();

			public bool Contains(int item)
				=> throw new NotSupportedException();

			public void CopyTo(int[] array, int arrayIndex)
				=> throw new NotSupportedException();

			public bool Remove(int item)
				=> throw new NotSupportedException();

			public IEnumerator<int> GetEnumerator()
				=> ((IEnumerable<int>)items).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()
				=> GetEnumerator();
		}

		private sealed class ThrowingCountReadOnlyCollection(int[] items) : IReadOnlyCollection<int>
		{
			public int Count => throw new InvalidOperationException("count failed");

			public IEnumerator<int> GetEnumerator()
				=> ((IEnumerable<int>)items).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()
				=> GetEnumerator();
		}
	}
}
