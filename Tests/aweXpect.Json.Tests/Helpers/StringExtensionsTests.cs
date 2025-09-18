namespace aweXpect.Json.Tests.Helpers;

public class StringExtensionsTests
{
	public sealed class TrimCommonWhiteSpace
	{
		[Fact]
		public async Task WhenAnyLaterLineHasNoWhiteSpace_ShouldReturnUnchangedInput()
		{
			string input = """
			               foo
			                   bar
			               baz
			                  bay
			               """;

			async Task Act()
				=> await That("1").IsValidJsonMatching(2, null, input);

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that "1"
				               is valid JSON which matches {{input}},
				               but it differed as $ was 1 instead of 2
				               """);
		}

		[Fact]
		public async Task WhenEmpty_ShouldReturnEmptyString()
		{
			string input = string.Empty;

			async Task Act()
				=> await That("1").IsValidJsonMatching(2, null, input);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that "1"
				             is valid JSON which matches ,
				             but it differed as $ was 1 instead of 2
				             """);
		}

		[Fact]
		public async Task WhenLinesHaveDifferentWhiteSpace_ShouldKeepAllWhiteSpace()
		{
			string input = """
			               foo
			                   bar
			               	baz
			               """;

			async Task Act()
				=> await That("1").IsValidJsonMatching(2, null, input);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that "1"
				             is valid JSON which matches foo
				                 bar
				             	baz,
				             but it differed as $ was 1 instead of 2
				             """);
		}

		[Fact]
		public async Task WhenLinesHaveSomeCommonWhiteSpace_ShouldTrim()
		{
			string input = """
			               foo
			                   bar
			                 baz
			                  bay
			               """;

			async Task Act()
				=> await That("1").IsValidJsonMatching(2, null, input);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that "1"
				             is valid JSON which matches foo
				               bar
				             baz
				              bay,
				             but it differed as $ was 1 instead of 2
				             """);
		}

		[Fact]
		public async Task WhenOnlyHasOneLine_ShouldReturnLine()
		{
			string input = "foo";

			async Task Act()
				=> await That("1").IsValidJsonMatching(2, null, input);

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that "1"
				               is valid JSON which matches {{input}},
				               but it differed as $ was 1 instead of 2
				               """);
		}

		[Fact]
		public async Task WhenTwoLines_ShouldTrimSecondLine()
		{
			string input = """
			               foo
			                	 bar
			               """;

			async Task Act()
				=> await That("1").IsValidJsonMatching(2, null, input);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that "1"
				             is valid JSON which matches foo
				             bar,
				             but it differed as $ was 1 instead of 2
				             """);
		}
	}
}
