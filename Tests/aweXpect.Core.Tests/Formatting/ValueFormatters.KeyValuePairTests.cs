using System.Collections.Generic;
using System.Text;
using aweXpect.Core.Metadata;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class KeyValuePairTests
	{
		[Test]
		public async Task ShouldFormatKeyAndValue()
		{
			string expectedResult = "[\"foo\"] = 42";
			KeyValuePair<string, int> value = new("foo", 42);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenBoxed_ShouldFormatKeyAndValue()
		{
			string expectedResult = "[\"foo\"] = 42";
			object value = new KeyValuePair<string, int>("foo", 42);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("a boxed pair has lost its type arguments, so the members are read through the registry or reflection");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenBoxedAndOnlyPartiallyRegistered_ShouldFallBackToToString()
		{
			TypeMetadataRegistry.RegisterProperty<KeyValuePair<PartiallyRegisteredKey, int>, string>(
				nameof(KeyValuePair<object, object>.Key), _ => "from registry");
			object value = new KeyValuePair<PartiallyRegisteredKey, int>(new PartiallyRegisteredKey(), 1);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[probe, 1]")
				.Because("a registration without both members cannot render the pair, so the plain rendering stays");
		}

		[Test]
		public async Task WhenBoxedAndRegistered_ShouldReadThroughTheRegistration()
		{
			TypeMetadataRegistry.RegisterProperty<KeyValuePair<RegisteredKey, int>, string>(
				nameof(KeyValuePair<object, object>.Key), _ => "from registry");
			TypeMetadataRegistry.RegisterProperty<KeyValuePair<RegisteredKey, int>, int>(
				nameof(KeyValuePair<object, object>.Value), _ => 999);
			object value = new KeyValuePair<RegisteredKey, int>(new RegisteredKey(), 1);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[\"from registry\"] = 999")
				.Because("under the JIT reflection would render the same pair, so only bogus accessors prove the registry path");
		}

		[Test]
		public async Task WhenBoxedValueRefersBackToTheOwner_ShouldDetectTheRecursion()
		{
			Owner value = new();
			value.Pair = new KeyValuePair<string, Owner>("self", value);
			string expectedResult =
				"ValueFormatters.KeyValuePairTests.Owner { Pair = [\"self\"] = ValueFormatters.KeyValuePairTests.Owner { *recursive* } }";

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("the pair is rendered within the formatting context of its owner, so the cycle is detected");
		}

		[Test]
		public async Task WhenKeyAndValueAreNull_ShouldUseDefaultNullString()
		{
			string expectedResult = $"[{ValueFormatter.NullString}] = {ValueFormatter.NullString}";
			KeyValuePair<string?, object?> value = new(null, null);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithLineBreaks_ShouldEscapeStringKeyAndValue()
		{
			string expectedResult = "[\"a\\nb\"] = \"say \\\"hi\\\"\"";
			KeyValuePair<string, string> value = new("a\nb", "say \"hi\"");
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);
			Formatter.Format(sb, value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult)
				.Because("the key and value are escaped like collection items");
			await That(objectResult).IsEqualTo(expectedResult)
				.Because("a boxed pair is escaped like a typed one");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithType_ShouldIncludeTypeInformation()
		{
			string expectedResult = "KeyValuePair<string, int> [\"foo\"] = 42";
			KeyValuePair<string, int> value = new("foo", 42);
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult)
				.Because("the brackets convey the pair structure, but not the types of its key and value");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		private sealed class Owner
		{
			public KeyValuePair<string, Owner> Pair { get; set; }
		}

		private sealed class PartiallyRegisteredKey
		{
			public override string ToString() => "probe";
		}

		private sealed class RegisteredKey;
	}
}
