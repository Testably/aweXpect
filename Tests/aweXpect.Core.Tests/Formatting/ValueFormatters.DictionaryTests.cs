using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class DictionaryTests
	{
		[Fact]
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

		[Fact]
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

		[Fact]
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

		[Fact]
		public async Task WhenNonGeneric_ShouldFormatEntries()
		{
			string expectedResult = "{[\"1\"] = 1}";
			Hashtable value = new()
			{
				["1"] = 1,
			};
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
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

		[Fact]
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

		[Fact]
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
	}
}
