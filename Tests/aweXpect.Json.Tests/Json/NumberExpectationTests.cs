using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed class NumberExpectationTests
{
	[Test]
	public async Task DecimalType_WhenNumberIsOutOfRange_ShouldFail()
	{
		JsonElement subject = Parse("""{"value": 1e30}""");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				value = It.Is<decimal>().That.IsGreaterThan(0m),
			});

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new
			             {
			             	value = It.Is<decimal>().That.IsGreaterThan(0m),
			             } exactly,
			             but it differed as $.value was double
			             """);
	}

	[Test]
	public async Task FloatingPointType_ShouldAcceptExponentNotation()
	{
		JsonElement subject = Parse("""{"double": 1e3, "float": -2.5E-1, "decimal": 1.5e2}""");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				@double = It.Is<double>().That.IsEqualTo(1000.0),
				@float = It.Is<float>().That.IsEqualTo(-0.25f),
				@decimal = It.Is<decimal>().That.IsEqualTo(150m),
			});

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task IntegerType_WhenNumberHasExponent_ShouldFail()
	{
		JsonElement subject = Parse("""{"value": 1e3}""");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				value = It.Is<int>().That.IsEqualTo(1000),
			});

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new
			             {
			             	value = It.Is<int>().That.IsEqualTo(1000),
			             } exactly,
			             but it differed as $.value was double
			             """)
			.Because("System.Text.Json reads an integer type only from a number without fraction or exponent");
	}

	[Test]
	public async Task IntegerType_WhenNumberIsNotWhole_ShouldFail()
	{
		JsonElement subject = Parse("""{"value": 1.5}""");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				value = It.Is<int>().That.IsGreaterThan(0),
			});

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new
			             {
			             	value = It.Is<int>().That.IsGreaterThan(0),
			             } exactly,
			             but it differed as $.value was double
			             """);
	}

	[Test]
	[Arguments("256")]
	[Arguments("-1")]
	public async Task IntegerType_WhenNumberIsOutOfRange_ShouldFail(string number)
	{
		JsonElement subject = Parse($$"""{"value": {{number}}}""");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				value = It.Is<byte>().That.IsGreaterThan((byte)0),
			});

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new
			             {
			             	value = It.Is<byte>().That.IsGreaterThan((byte)0),
			             } exactly,
			             but it differed as $.value was int
			             """);
	}

	[Test]
	public async Task JsonNode_ShouldReceiveTheNumberAsTheExpectedType()
	{
		JsonNode? subject = JsonNode.Parse("""{"value": 5}""");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				value = It.Is<long>().That.IsEqualTo(5L),
			});

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Negated_WhenNumberMatches_ShouldFail()
	{
		JsonElement subject = Parse("""{"value": 5}""");

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(new
			{
				value = It.Is<long>().That.IsEqualTo(5L),
			}));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not match new
			             {
			             	value = It.Is<long>().That.IsEqualTo(5L),
			             } exactly,
			             but it did

			             Actual:
			             {
			               "value": 5
			             }
			             """);
	}

	[Test]
	public async Task NotANumber_ShouldFail()
	{
		JsonElement subject = Parse("""{"value": "5"}""");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				value = It.Is<long>().That.IsEqualTo(5L),
			});

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new
			             {
			             	value = It.Is<long>().That.IsEqualTo(5L),
			             } exactly,
			             but it differed as $.value was string
			             """);
	}

	[Test]
	public async Task NullableJsonElement_ShouldReceiveTheNumberAsTheExpectedType()
	{
		JsonElement? subject = Parse("""{"value": 5}""");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				value = It.Is<double>().That.IsEqualTo(5.0),
			});

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task NullableType_ShouldReceiveTheNumber()
	{
		JsonElement subject = Parse("""{"value": 5}""");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				value = It.Is<long?>().That.IsEqualTo(5L),
			});

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task NumericType_ShouldReceiveTheNumberAsThatType()
	{
		JsonElement subject = Parse("""
		                            {
		                              "sbyte": -128,
		                              "byte": 255,
		                              "short": -32768,
		                              "ushort": 65535,
		                              "int": -0,
		                              "uint": 4294967295,
		                              "long": 9223372036854775807,
		                              "ulong": 18446744073709551615,
		                              "float": 1,
		                              "double": 1,
		                              "large": 123456789012345678901234567890,
		                              "decimal": 79228162514264337593543950335,
		                              "precise": 0.1
		                            }
		                            """);

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				@sbyte = It.Is<sbyte>().That.IsEqualTo(sbyte.MinValue),
				@byte = It.Is<byte>().That.IsEqualTo(byte.MaxValue),
				@short = It.Is<short>().That.IsEqualTo(short.MinValue),
				@ushort = It.Is<ushort>().That.IsEqualTo(ushort.MaxValue),
				@int = It.Is<int>().That.IsEqualTo(0),
				@uint = It.Is<uint>().That.IsEqualTo(uint.MaxValue),
				@long = It.Is<long>().That.IsEqualTo(long.MaxValue),
				@ulong = It.Is<ulong>().That.IsEqualTo(ulong.MaxValue),
				@float = It.Is<float>().That.IsEqualTo(1f),
				@double = It.Is<double>().That.IsEqualTo(1.0),
				large = It.Is<double>().That.IsEqualTo(1.2345678901234568E+29),
				@decimal = It.Is<decimal>().That.IsEqualTo(decimal.MaxValue),
				precise = It.Is<decimal>().That.IsEqualTo(0.1m),
			});

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task String_ShouldReceiveTheNumberAsTheExpectedType()
	{
		string subject = """{"value": 5}""";

		async Task Act()
			=> await That(subject).IsValidJsonMatching(new
			{
				value = It.Is<decimal>().That.IsEqualTo(5m),
			});

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithMatching_ShouldReceiveTheNumberAsTheExpectedType()
	{
		JsonElement subject = Parse("""{"value": 5}""");

		async Task Act()
			=> await That(subject).IsObject(o => o.With("value").Matching(It.Is<long>().That.IsEqualTo(5L)));

		await That(Act).DoesNotThrow();
	}

	private static JsonElement Parse(string json)
	{
		using JsonDocument document = JsonDocument.Parse(json);
		return document.RootElement.Clone();
	}
}
