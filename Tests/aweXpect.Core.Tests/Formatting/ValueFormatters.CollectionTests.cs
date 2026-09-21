using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class CollectionTests
	{
		[Fact]
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

		[Fact]
		public async Task InFailureMessage_WhenCountIsKnown_ShouldNameTheNumberOfRemainingItems()
		{
			int[] subject = Enumerable.Range(1, 25).ToArray();
			int[] expected = Enumerable.Range(1, 26).ToArray();

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<XunitException>()
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

		[Fact]
		public async Task InFailureMessage_WhenLazySequenceWasFullyEnumerated_ShouldNameTheNumberOfRemainingItems()
		{
			IEnumerable<int> subject = Lazy(Enumerable.Range(1, 25));
			int[] expected = Enumerable.Range(1, 26).ToArray();

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<XunitException>()
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

		[Fact]
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

		[Fact]
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

		[Theory]
		[InlineData(10, "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]")]
		[InlineData(11, "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 1 more)]")]
		[InlineData(25, "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 15 more)]")]
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

		[Fact]
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

		[Fact]
		public async Task WhenCountIsKnown_WithType_ShouldNameTheNumberOfRemainingItems()
		{
			int[] value = Enumerable.Range(1, 12).ToArray();

			string result = Formatter.Format(value, FormattingOptions.WithType);

			await That(result).IsEqualTo("int[] [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 2 more)]");
		}

		[Fact]
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

		[Fact]
		public async Task WhenGenericCollection_ShouldNameTheNumberOfRemainingItems()
		{
			HashSet<int> value = [..Enumerable.Range(1, 12),];

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 2 more)]");
		}

		[Fact]
		public async Task WhenNested_ShouldNameTheNumberOfRemainingItemsPerCollection()
		{
			int[][] value = Enumerable.Range(1, 12).Select(x => Enumerable.Range(x, 11).ToArray()).ToArray();

			string result = Formatter.Format(value);

			await That(result).StartsWith("[[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 1 more)], [2, ")
				.And.EndsWith("[10, 11, 12, 13, 14, 15, 16, 17, 18, 19, (… and 1 more)], (… and 2 more)]");
		}

		[Fact]
		public async Task WhenReadOnlyCollection_ShouldNameTheNumberOfRemainingItems()
		{
			IEnumerable<int> value = new ReadOnlyCollection(Enumerable.Range(1, 13).ToArray());

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (… and 3 more)]");
		}

		[Fact]
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

		[Fact]
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

		[Fact]
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

		private static IEnumerable<int> Lazy(IEnumerable<int> items)
		{
			foreach (int item in items)
			{
				yield return item;
			}
		}

		private sealed class ReadOnlyCollection(int[] items) : IReadOnlyCollection<int>
		{
			public int Count => items.Length;

			public IEnumerator<int> GetEnumerator()
				=> ((IEnumerable<int>)items).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()
				=> GetEnumerator();
		}
	}
}
