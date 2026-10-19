using BenchmarkDotNet.Attributes;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify that a JSON string is valid.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _validJson = """
	                                     {
	                                       "prop": "value",
	                                       "prop2": 1.2,
	                                       "prop3": false,
	                                       "prop4": null,
	                                       "prop5": { "nested": "value" },
	                                       "prop6": ["an", "array", "with", "5", "items"]
	                                     }
	                                     """;

	[Benchmark]
	public async Task IsValidJson_aweXpect()
		=> await Expect.That(_validJson).IsValidJson();
}
