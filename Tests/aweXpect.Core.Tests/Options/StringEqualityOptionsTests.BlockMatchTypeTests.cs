using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class BlockMatchTypeTests
	{
		[Fact]
		public async Task AreConsideredEqual_BothNull_ShouldReturnTrue()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			bool result = await sut.AreConsideredEqual<string?>(null, null);

			await That(result).IsTrue();
		}

		[Theory]
		[InlineData(null, "foo")]
		[InlineData("foo", null)]
		public async Task AreConsideredEqual_OneNull_ShouldReturnFalse(string? actual, string? expected)
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsFalse();
		}

		[Fact]
		public async Task AreConsideredEqual_WhenActualHasAdditionalLines_ShouldReturnFalse()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			bool result = await sut.AreConsideredEqual("class C\n{\n    Foo();\n}", "{\n    Foo();\n}");

			await That(result).IsFalse();
		}

		[Fact]
		public async Task AreConsideredEqual_WhenBlockIsIndentedAsAWhole_ShouldReturnTrue()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			bool result = await sut.AreConsideredEqual("    {\n        Foo();\n    }", "{\n    Foo();\n}");

			await That(result).IsTrue();
		}

		[Fact]
		public async Task AsBlock_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new();

			StringEqualityOptions result = sut.AsBlock();

			await That(result).IsSameAs(sut);
		}

		[Theory]
		[InlineData("public int Foo\n{\n    get;\n}", 1)]
		[InlineData("    public int Foo\n    {\n        get;\n    }", 1)]
		[InlineData("\tpublic int Foo\n\t{\n\t    get;\n\t}", 1)]
		[InlineData("class C\n{\n    public int Foo\n    {\n        get;\n    }\n}", 1)]
		[InlineData("    public int Foo\r\n    {\r\n        get;\r\n    }", 1)]
		[InlineData("public int Foo\n{\nget;\n}", 0)]
		[InlineData("public int Foo\n{\n        get;\n}", 0)]
		[InlineData("    public int Foo\n{\n    get;\n}", 0)]
		[InlineData("// public int Foo\n// {\n//     get;\n// }", 0)]
		[InlineData("public int Foo\n{\n    get; set;\n}", 0)]
		[InlineData("public int Foo\n{\n    get;", 0)]
		public async Task CountOccurrences_ShouldRequireTheSameWhiteSpacePrefixOnAllLines(string actual,
			int expectedCount)
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			int result = await sut.CountOccurrences(actual, "public int Foo\n{\n    get;\n}");

			await That(result).IsEqualTo(expectedCount);
		}

		[Fact]
		public async Task CountOccurrences_ShouldCountNonOverlappingOccurrences()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			int result = await sut.CountOccurrences("a\nb\na\nb\na", "a\nb\na");

			await That(result).IsEqualTo(1);
		}

		[Fact]
		public async Task CountOccurrences_ShouldNotMatchMidLine()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			int result = await sut.CountOccurrences("public int Foo;", "int Foo");

			await That(result).IsEqualTo(0);
		}

		[Fact]
		public async Task CountOccurrences_WhenActualLineEndsWithExpectedLine_ShouldNotMatch()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			int result = await sut.CountOccurrences("public int Foo;", "int Foo;");

			await That(result).IsEqualTo(0);
		}

		[Theory]
		[InlineData("a\n\nb")]
		[InlineData("  a\n\n  b")]
		[InlineData("  a\n   \n  b")]
		[InlineData("  a\n\t\n  b")]
		public async Task CountOccurrences_WhenBlockContainsBlankLine_ShouldMatchAnyWhiteSpaceOnlyLine(
			string actual)
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			int result = await sut.CountOccurrences(actual, "a\n\nb");

			await That(result).IsEqualTo(1);
		}

		[Fact]
		public async Task CountOccurrences_WhenBlockContainsBlankLine_ShouldNotMatchNonBlankLine()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			int result = await sut.CountOccurrences("a\nx\nb", "a\n\nb");

			await That(result).IsEqualTo(0);
		}

		[Theory]
		[InlineData("a\nb", 0)]
		[InlineData("a\nb\n", 1)]
		[InlineData("a\nb\n  ", 1)]
		public async Task CountOccurrences_WhenBlockEndsWithNewline_ShouldRequireTrailingBlankLine(string actual,
			int expectedCount)
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			int result = await sut.CountOccurrences(actual, "a\nb\n");

			await That(result).IsEqualTo(expectedCount);
		}

		[Fact]
		public async Task CountOccurrences_WhenCaseIsIgnored_ShouldIgnoreCase()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock().IgnoringCase();

			int result = await sut.CountOccurrences("  FOO\n  bar", "foo\nBAR");

			await That(result).IsEqualTo(1);
		}

		[Fact]
		public async Task CountOccurrences_WhenComparerIsUsed_ShouldUseComparer()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock().UsingComparer(StringComparer.OrdinalIgnoreCase);

			int result = await sut.CountOccurrences("  FOO\n  bar", "foo\nBAR");

			await That(result).IsEqualTo(1);
		}

		[Fact]
		public async Task CountOccurrences_WhenOccurrencesAreIndentedDifferently_ShouldCountAll()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			int result = await sut.CountOccurrences("\ta\n\tb\nx\n  a\n  b\na\nb", "a\nb");

			await That(result).IsEqualTo(3);
		}

		[Fact]
		public async Task GetExpectation_ShouldIncludeAsBlock()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			string result = sut.GetExpectation("foo", ExpectationGrammars.Active);

			await That(result).IsEqualTo("matches \"foo\" as block");
		}

		[Fact]
		public async Task GetExtendedFailure_Null_ShouldReturnItWasNull()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, null, "foo");

			await That(result).IsEqualTo("it was <null>");
		}

		[Fact]
		public async Task GetExtendedFailure_ShouldReturnActualAsSingleLine()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo\nbar", "foo");

			await That(result).IsEqualTo("it was \"foo\\nbar\"");
		}

		[Fact]
		public async Task ToString_ShouldReturnAsBlock()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock();

			string result = sut.ToString();

			await That(result).IsEqualTo(" as block");
		}

		[Fact]
		public async Task ToString_WhenCaseIsIgnored_ShouldReturnAsBlockIgnoringCase()
		{
			StringEqualityOptions sut = new();
			sut.AsBlock().IgnoringCase();

			string result = sut.ToString();

			await That(result).IsEqualTo(" as block ignoring case");
		}
	}
}
