using System.Text.Json;

namespace aweXpect.Json.Tests;

public class JsonOptionsTests
{
	[Fact]
	public async Task DocumentOptions_ShouldDefaultToAllowTrailingCommas()
	{
		JsonOptions sut = new();

		await That(sut.DocumentOptions.AllowTrailingCommas).IsTrue();
	}

	[Fact]
	public async Task WithJsonOptions_ShouldSetDocumentOptions()
	{
		int maxDepth = new Random().Next(1, 10);
		JsonOptions sut = new();

		JsonOptions result = sut.WithJsonOptions(o => o with
		{
			MaxDepth = maxDepth,
		});

		await That(result.DocumentOptions.MaxDepth).IsEqualTo(maxDepth);
	}

	[Fact]
	public async Task WithJsonOptions_ShouldSupportProvidingFixedOptions()
	{
		int maxDepth = new Random().Next(1, 10);
		JsonOptions sut = new();
		JsonDocumentOptions documentOptions = new()
		{
			MaxDepth = maxDepth,
		};

		JsonOptions result = sut.WithJsonOptions(_ => documentOptions);

		await That(result.DocumentOptions.MaxDepth).IsEqualTo(maxDepth);
	}
}
