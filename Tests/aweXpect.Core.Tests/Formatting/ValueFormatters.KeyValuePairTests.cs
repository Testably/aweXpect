using System.Collections.Generic;
using System.Text;
using aweXpect.Core.Metadata;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class KeyValuePairTests
	{
		[Fact]
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

		[Fact]
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

		[Fact]
		public async Task WhenBoxedAndOnlyPartiallyRegistered_ShouldFallBackToToString()
		{
			TypeMetadataRegistry.RegisterProperty<KeyValuePair<PartiallyRegisteredKey, int>, string>(
				nameof(KeyValuePair<object, object>.Key), _ => "from registry");
			object value = new KeyValuePair<PartiallyRegisteredKey, int>(new PartiallyRegisteredKey(), 1);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("[probe, 1]")
				.Because("a registration without both members cannot render the pair, so the plain rendering stays");
		}

		[Fact]
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

		[Fact]
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

		[Fact]
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

		[Fact]
		public async Task WithType_ShouldNotIncludeTypeInformation()
		{
			string expectedResult = "[\"foo\"] = 42";
			KeyValuePair<string, int> value = new("foo", 42);
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult).Because("the brackets already convey the pair structure");
			await That(sb.ToString()).IsEqualTo(expectedResult)
				.Because("the brackets already convey the pair structure");
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
