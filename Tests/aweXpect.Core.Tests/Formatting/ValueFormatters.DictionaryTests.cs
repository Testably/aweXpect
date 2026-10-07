using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class DictionaryTests
	{
		[Test]
		public async Task ShouldFormatItems()
		{
			string expectedResult = "{[\"1\"] = 1, [\"2\"] = 2, [\"3\"] = 3, [\"4\"] = 4}";
			Dictionary<string, int> value = Enumerable.Range(1, 4).ToDictionary(i => i.ToString(), i => i);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task ShouldFormatNestedGenericValues()
		{
			string expectedResult = "{[\"a\"] = [1, 2], [\"b\"] = [3]}";
			Dictionary<string, List<int>> value = new()
			{
				["a"] = [1, 2,],
				["b"] = [3,],
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task ShouldFormatValueTypeKeysAndValues()
		{
			string expectedResult = "{[1] = True, [2] = False}";
			Dictionary<int, bool> value = new()
			{
				[1] = true,
				[2] = false,
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task ShouldNameTheNumberOfRemainingEntries()
		{
			string expectedResult =
				"{[1] = 1, [2] = 2, [3] = 3, [4] = 4, [5] = 5, [6] = 6, [7] = 7, [8] = 8, [9] = 9, [10] = 10, (… and 2 more)}";
			Dictionary<int, int> value = Enumerable.Range(1, 12).ToDictionary(i => i, i => i);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenAKeyValuePairFormatterIsRegistered_ShouldUseItAlsoWhenBoxed()
		{
			using IDisposable _ = ValueFormatter.Register(new PairKeyFormatter());
			Dictionary<PairKey, int> value = new()
			{
				[new PairKey()] = 1,
			};
			Hashtable nonGeneric = new()
			{
				["1"] = 1,
			};

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			string memberResult = Formatter.Format(new Holder(value), FormattingOptions.SingleLine);
			string nonGenericResult = Formatter.Format(nonGeneric);

			await That(result).IsEqualTo("{CUSTOM(1)}");
			await That(objectResult).IsEqualTo("{CUSTOM(1)}")
				.Because("a dictionary that lost its static type still offers its entries to the registered formatters");
			await That(memberResult).IsEqualTo("ValueFormatters.DictionaryTests.Holder { Value = {CUSTOM(1)} }")
				.Because("a dictionary that is a member is formatted boxed");
			await That(nonGenericResult).IsEqualTo("{[\"1\"] = 1}");
		}

		[Test]
		public async Task WhenCountThrows_AsTaskResult_ShouldFormatEntries()
		{
			Task<object> value = Task.FromResult<object>(new ThrowingCountHashtable
			{
				["1"] = 1,
			});

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("Task<object> (RanToCompletion, {[\"1\"] = 1})");
		}

		[Test]
		public async Task WhenCountThrows_AsTupleItem_ShouldFormatEntries()
		{
			(int, ThrowingCountHashtable) value = (0, new ThrowingCountHashtable
			{
				["1"] = 1,
			});

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo("(0, {[\"1\"] = 1})");
		}

		[Test]
		public async Task WhenCountThrows_ShouldFormatEntries()
		{
			string expectedResult = "{[\"1\"] = 1}";
			ThrowingCountHashtable value = new()
			{
				["1"] = 1,
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("the count is only needed to name the number of remaining entries");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenCountThrows_WhenAFormatterIsRegistered_ShouldFormatEntries()
		{
			using IDisposable _ = ValueFormatter.Register(new PairKeyFormatter());
			ThrowingCountHashtable value = new()
			{
				["1"] = 1,
			};

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("{[\"1\"] = 1}");
		}

		[Test]
		public async Task WhenCountThrows_WithLineBreaks_ShouldFormatEntries()
		{
			ThrowingCountHashtable value = new()
			{
				["1"] = 1,
			};

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo("""
			                             {
			                               ["1"] = 1
			                             }
			                             """);
		}

		[Test]
		public async Task WhenDictionaryContainsItself_ShouldDetectTheRecursion()
		{
			string expectedResult = "{[\"self\"] = ValueFormatters.DictionaryTests.Holder { Value = { *recursive* } }}";
			Dictionary<string, Holder> value = new();
			value["self"] = new Holder(value);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenNestedDeeperThanTheMaximumDepth_ShouldLeaveOutTheEntriesOfTheDeepestDictionary()
		{
			Dictionary<string, object> value = new();
			Dictionary<string, object> current = value;
			for (int i = 0; i < 1000; i++)
			{
				Dictionary<string, object> inner = new();
				current["a"] = inner;
				current = inner;
			}

			string expectedResult =
				string.Concat(Enumerable.Repeat("{[\"a\"] = ", 20)) + "{ … }" + new string('}', 20);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenNonGeneric_ShouldFormatEntries()
		{
			string expectedResult = "{[\"1\"] = 1}";
			Hashtable value = new()
			{
				["1"] = 1,
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenNotADictionary_ShouldStillFormatKeyValuePairs()
		{
			string expectedResult = "[[\"1\"] = 1, [\"2\"] = 2]";
			IEnumerable<KeyValuePair<string, int>> value = Enumerable.Range(1, 2)
				.Select(i => new KeyValuePair<string, int>(i.ToString(), i));
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("the square brackets indicate that the sequence is no dictionary");
			await That(sb.ToString()).IsEqualTo(expectedResult)
				.Because("the square brackets indicate that the sequence is no dictionary");
		}

		[Test]
		public async Task WhenNotADictionaryAndBoxed_ShouldStillFormatKeyValuePairs()
		{
			string expectedResult = "[[\"1\"] = 1, [\"2\"] = 2]";
			object value = Enumerable.Range(1, 2)
				.Select(i => new KeyValuePair<string, int>(i.ToString(), i));
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("each boxed entry is formatted as a pair, even though the sequence type is no longer known");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenNull_ShouldUseDefaultNullString()
		{
			Dictionary<int, object>? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value!);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value!);

			await That(result).IsEqualTo(ValueFormatter.NullString);
			await That(objectResult).IsEqualTo(ValueFormatter.NullString);
			await That(sb.ToString()).IsEqualTo(ValueFormatter.NullString);
		}

		[Test]
		public async Task WhenOnlyAGenericDictionary_ShouldUseBraces()
		{
			string expectedResult = "{[\"a\"] = 1}";
			IDictionary<string, object?> value = new ExpandoObject();
			value["a"] = 1;

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);

			await That(value is IDictionary).IsFalse();
			await That(result).IsEqualTo(expectedResult)
				.Because("a dictionary is rendered in braces, even when it does not implement the non-generic IDictionary");
			await That(objectResult).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenOnlyAGenericReadOnlyDictionary_ShouldUseBraces()
		{
			string expectedResult = "{[\"a\"] = 1}";
			ReadOnlyDictionaryOnly<int> value = new(new Dictionary<string, int>
			{
				["a"] = 1,
			});
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("a dictionary is rendered in braces, even when it does not implement the non-generic IDictionary");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenOnlyAGenericReadOnlyDictionary_WhenCountThrows_ShouldFormatEntries()
		{
			string expectedResult = "{[\"a\"] = 1}";
			ReadOnlyDictionaryOnly<int> value = new(new Dictionary<string, int>
			{
				["a"] = 1,
			}, new InvalidOperationException("count failed"));
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenOnlyAGenericReadOnlyDictionary_WhenCountThrows_ShouldSayThatMoreEntriesMayFollow()
		{
			string expectedResult =
				"{[\"1\"] = 1, [\"2\"] = 2, [\"3\"] = 3, [\"4\"] = 4, [\"5\"] = 5, [\"6\"] = 6, [\"7\"] = 7, [\"8\"] = 8, [\"9\"] = 9, [\"10\"] = 10, (… and maybe more)}";
			ReadOnlyDictionaryOnly<int> value = new(Enumerable.Range(1, 12).ToDictionary(i => i.ToString(), i => i),
				new InvalidOperationException("count failed"));
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenOnlyAGenericReadOnlyDictionaryContainsItself_ShouldDetectTheRecursionInBraces()
		{
			Dictionary<string, object> inner = new();
			ReadOnlyDictionaryOnly<object> value = new(inner);
			inner["self"] = new Holder(value);
			string expectedResult = "{[\"self\"] = ValueFormatters.DictionaryTests.Holder { Value = { *recursive* } }}";

			string result = Formatter.Format((object?)value);

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithLineBreaks_ShouldEscapeStringKeys()
		{
			string expectedResult = """
			                        {
			                          ["a\nb"] = 1
			                        }
			                        """;
			Dictionary<string, int> value = new()
			{
				["a\nb"] = 1,
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);
			Formatter.Format(sb, value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult)
				.Because("a string key is escaped like a collection item, so its line break cannot break the indentation");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithLineBreaks_ShouldEscapeStringValues()
		{
			string expectedResult = """
			                        {
			                          ["a"] = "x\ny",
			                          ["b"] = "say \"hi\""
			                        }
			                        """;
			Dictionary<string, string> value = new()
			{
				["a"] = "x\ny",
				["b"] = "say \"hi\"",
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);
			Formatter.Format(sb, value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult)
				.Because("a string value is escaped like a collection item, so line breaks and quotes stay unambiguous");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithLineBreaks_ShouldKeepNonStringValuesOnMultipleLines()
		{
			string expectedResult = """
			                        {
			                          ["a"] = [
			                            1,
			                            2
			                          ]
			                        }
			                        """;
			Dictionary<string, List<int>> value = new()
			{
				["a"] = [1, 2,],
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);
			Formatter.Format(sb, value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult)
				.Because("only string keys and values are forced onto a single line");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithLineBreaks_ShouldTruncateLongStringValues()
		{
			string expectedResult = """
			                        {
			                          ["a"] = "abcde…"
			                        }
			                        """;
			Dictionary<string, string> value = new()
			{
				["a"] = "abcdefgh",
			};
			StringBuilder sb = new();

			string result;
			string objectResult;
			using (Customize.aweXpect.Formatting().MaximumStringLength.Set(5))
			{
				result = Formatter.Format(value, FormattingOptions.MultipleLines);
				objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);
				Formatter.Format(sb, value, FormattingOptions.MultipleLines);
			}

			await That(result).IsEqualTo(expectedResult)
				.Because("a string value is truncated like a collection item");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithType_ShouldIncludeTypeInformation()
		{
			string expectedResult = "Dictionary<string, int> {[\"1\"] = 1, [\"2\"] = 2, [\"3\"] = 3}";
			Dictionary<string, int> value = Enumerable.Range(1, 3).ToDictionary(i => i.ToString(), i => i);
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		/// <remarks>
		///     Throws once its value was read too often, so that following a cycle fails the test instead of
		///     overflowing the stack.
		/// </remarks>
		private sealed class Holder(object value)
		{
			private int _reads;

			public object Value
				=> ++_reads > 100 ? throw new InvalidOperationException("read too often") : value;
		}

		private sealed class PairKey;

		private sealed class PairKeyFormatter : IValueFormatter
		{
			public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
			{
				if (value is KeyValuePair<PairKey, int> pair)
				{
					stringBuilder.Append("CUSTOM(").Append(pair.Value).Append(')');
					return true;
				}

				return false;
			}
		}

		private sealed class ReadOnlyDictionaryOnly<TValue>(
			Dictionary<string, TValue> inner,
			Exception? countException = null)
			: IReadOnlyDictionary<string, TValue>
		{
			public int Count => countException is null ? inner.Count : throw countException;
			public TValue this[string key] => inner[key];
			public IEnumerable<string> Keys => inner.Keys;
			public IEnumerable<TValue> Values => inner.Values;
			public bool ContainsKey(string key) => inner.ContainsKey(key);
			public bool TryGetValue(string key, out TValue value) => inner.TryGetValue(key, out value!);
			public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() => inner.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		private sealed class ThrowingCountHashtable : Hashtable
		{
			public override int Count => throw new InvalidOperationException("count failed");
		}
	}
}
