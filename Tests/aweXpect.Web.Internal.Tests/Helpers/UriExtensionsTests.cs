using aweXpect.Helpers;

namespace aweXpect.Web.Internal.Tests;

public sealed class UriExtensionsTests
{
	[Theory]
	[InlineData("https://awexpect.com/")]
	[InlineData("https://awexpect.com/?")]
	[InlineData("https://awexpect.com/?page=2")]
	[InlineData("https://awexpect.com/?Name=1")]
	[InlineData("https://awexpect.com/?=1&&")]
	public async Task GetQueryParameterValues_WhenNameIsMissing_ShouldReturnNull(string uri)
	{
		string[]? result = new Uri(uri).GetQueryParameterValues("name");

		await That(result).IsNull();
	}

	[Theory]
	[InlineData("https://awexpect.com/?name=1", "1")]
	[InlineData("https://awexpect.com/?name", "")]
	[InlineData("https://awexpect.com/?name=", "")]
	[InlineData("https://awexpect.com/?name=a=b", "a=b")]
	[InlineData("https://awexpect.com/?name=a%26b", "a&b")]
	[InlineData("https://awexpect.com/?name=a+b", "a b")]
	[InlineData("https://awexpect.com/?name=a%2Bb", "a+b")]
	[InlineData("https://awexpect.com/?na%6De=1", "1")]
	[InlineData("https://awexpect.com/?other=2&name=1#name=3", "1")]
	public async Task GetQueryParameterValues_ShouldReturnTheDecodedValue(string uri, string expected)
	{
		string[]? result = new Uri(uri).GetQueryParameterValues("name");

		await That(result).IsEqualTo([expected,]);
	}

	[Fact]
	public async Task GetQueryParameterValues_WhenNameIsRepeated_ShouldReturnAllValuesInOrder()
	{
		Uri uri = new("https://awexpect.com/?tag=b&page=2&tag=a&tag=b");

		string[]? result = uri.GetQueryParameterValues("tag");

		await That(result).IsEqualTo(["b", "a", "b",]);
	}

	[Fact]
	public async Task GetQueryParameterValues_WhenNameIsWhitespace_ShouldIgnoreTheParameter()
	{
		Uri uri = new("https://awexpect.com/?%20=1");

		string[]? result = uri.GetQueryParameterValues(" ");

		await That(result).IsNull();
	}
}
