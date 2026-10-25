using System.Linq;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public sealed class StringExtensionsTests
{
	public sealed class GetLineCountTests
	{
		[Test]
		[Arguments(null, 0)]
		[Arguments("", 0)]
		[Arguments("a", 1)]
		[Arguments("\n", 1)]
		[Arguments("\r", 1)]
		[Arguments("\r\n", 1)]
		[Arguments("a\n", 1)]
		[Arguments("a\r", 1)]
		[Arguments("a\r\n", 1)]
		[Arguments("a\nb", 2)]
		[Arguments("a\rb", 2)]
		[Arguments("a\r\nb", 2)]
		[Arguments("a\n\n", 2)]
		[Arguments("a\n\r", 2)]
		[Arguments("\r\n\r\n", 2)]
		[Arguments("a\nb\n", 2)]
		[Arguments("a\r\nb\r\n", 2)]
		[Arguments("a\nb\rc\r\nd", 4)]
		public async Task ShouldReturnExpectedLineCount(string? value, int expected)
		{
			int result = value.GetLineCount();

			await That(result).IsEqualTo(expected);
			// GetLineCount duplicates the line semantics of GetLines, so both must agree.
			await That(result).IsEqualTo(value.GetLines().Count());
		}
	}
}
