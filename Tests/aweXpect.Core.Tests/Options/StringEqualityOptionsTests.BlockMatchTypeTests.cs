using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class BlockMatchTypeTests
	{
		[Test]
		public async Task AreConsideredEqual_BothNull_ShouldReturnTrue()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			bool result = await sut.AreConsideredEqual<string?>(null, null);

			await That(result).IsTrue();
		}

		[Test]
		[Arguments(null, "foo")]
		[Arguments("foo", null)]
		public async Task AreConsideredEqual_OneNull_ShouldReturnFalse(string? actual, string? expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsFalse();
		}

		[Test]
		[Arguments("a\nb\n", "a\nb", true)]
		[Arguments("a\nb", "a\nb\r\n", true)]
		[Arguments("a\nb\n\n", "a\nb", false)]
		[Arguments("", "", true)]
		[Arguments("", "\n", false)]
		public async Task AreConsideredEqual_ShouldIgnoreASingleTrailingLineTerminator(string actual,
			string expected, bool expectedResult)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task AreConsideredEqual_WhenActualHasAdditionalLines_ShouldReturnFalse()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			bool result = await sut.AreConsideredEqual("class C\n{\n    Foo();\n}", "{\n    Foo();\n}");

			await That(result).IsFalse();
		}

		[Test]
		public async Task AreConsideredEqual_WhenBlockIsIndentedAsAWhole_ShouldReturnTrue()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			bool result = await sut.AreConsideredEqual("    {\n        Foo();\n    }", "{\n    Foo();\n}");

			await That(result).IsTrue();
		}

		[Test]
		public async Task AsBlock_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new("expected");

			StringEqualityOptions result = sut.AsBlock();

			await That(result).IsSameAs(sut);
		}

		[Test]
		public async Task CountOccurrences_ShouldCountNonOverlappingOccurrences()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			int result = await sut.CountOccurrences("a\nb\na\nb\na", "a\nb\na");

			await That(result).IsEqualTo(1);
		}

		[Test]
		[Arguments("a\nb", "a\nb\n", 1)]
		[Arguments("a\nb\n", "a\nb", 1)]
		[Arguments("x\na\nb", "a\nb\n", 1)]
		[Arguments("a\nb\n\n", "a\nb", 1)]
		[Arguments("a\nb\n  ", "a\nb\n", 1)]
		[Arguments("a\nb", "a\nb\n\n", 0)]
		[Arguments("a\nb\n\n", "a\nb\n\n", 1)]
		[Arguments("", "\n", 0)]
		public async Task CountOccurrences_ShouldIgnoreASingleTrailingLineTerminator(string actual,
			string expected, int expectedCount)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			int result = await sut.CountOccurrences(actual, expected);

			await That(result).IsEqualTo(expectedCount);
		}

		[Test]
		public async Task CountOccurrences_ShouldNotMatchMidLine()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			int result = await sut.CountOccurrences("public int Foo;", "int Foo");

			await That(result).IsEqualTo(0);
		}

		[Test]
		[Arguments("public int Foo\n{\n    get;\n}", 1)]
		[Arguments("    public int Foo\n    {\n        get;\n    }", 1)]
		[Arguments("\tpublic int Foo\n\t{\n\t    get;\n\t}", 1)]
		[Arguments("class C\n{\n    public int Foo\n    {\n        get;\n    }\n}", 1)]
		[Arguments("    public int Foo\r\n    {\r\n        get;\r\n    }", 1)]
		[Arguments("public int Foo\n{\nget;\n}", 0)]
		[Arguments("public int Foo\n{\n        get;\n}", 0)]
		[Arguments("    public int Foo\n{\n    get;\n}", 0)]
		[Arguments("// public int Foo\n// {\n//     get;\n// }", 0)]
		[Arguments("public int Foo\n{\n    get; set;\n}", 0)]
		[Arguments("public int Foo\n{\n    get;", 0)]
		public async Task CountOccurrences_ShouldRequireTheSameWhiteSpacePrefixOnAllLines(string actual,
			int expectedCount)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			int result = await sut.CountOccurrences(actual, "public int Foo\n{\n    get;\n}");

			await That(result).IsEqualTo(expectedCount);
		}

		[Test]
		public async Task CountOccurrences_WhenActualLineEndsWithExpectedLine_ShouldNotMatch()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			int result = await sut.CountOccurrences("public int Foo;", "int Foo;");

			await That(result).IsEqualTo(0);
		}

		[Test]
		[Arguments("a\n\nb")]
		[Arguments("  a\n\n  b")]
		[Arguments("  a\n   \n  b")]
		[Arguments("  a\n\t\n  b")]
		public async Task CountOccurrences_WhenBlockContainsBlankLine_ShouldMatchAnyWhiteSpaceOnlyLine(
			string actual)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			int result = await sut.CountOccurrences(actual, "a\n\nb");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenBlockContainsBlankLine_ShouldNotMatchNonBlankLine()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			int result = await sut.CountOccurrences("a\nx\nb", "a\n\nb");

			await That(result).IsEqualTo(0);
		}

		[Test]
		public async Task CountOccurrences_WhenCaseIsIgnored_ShouldIgnoreCase()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock().IgnoringCase();

			int result = await sut.CountOccurrences("  FOO\n  bar", "foo\nBAR");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenComparerIsUsed_ShouldUseComparer()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock().Using(StringComparer.OrdinalIgnoreCase);

			int result = await sut.CountOccurrences("  FOO\n  bar", "foo\nBAR");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenOccurrencesAreIndentedDifferently_ShouldCountAll()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			int result = await sut.CountOccurrences("\ta\n\tb\nx\n  a\n  b\na\nb", "a\nb");

			await That(result).IsEqualTo(3);
		}

		[Test]
		public async Task GetExpectation_ShouldIncludeAsBlock()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			string result = sut.GetExpectation("foo", ExpectationGrammars.Active);

			await That(result).IsEqualTo("matches \"foo\" as block");
		}

		[Test]
		public async Task GetExtendedFailure_Null_ShouldReturnItWasNull()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, null, "foo");

			await That(result).IsEqualTo("it was <null>");
		}

		[Test]
		public async Task GetExtendedFailure_ShouldReturnActualAsSingleLine()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo\nbar", "foo");

			await That(result).IsEqualTo("it was \"foo\\nbar\"");
		}

		[Test]
		public async Task ToString_ShouldReturnAsBlock()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			string result = sut.ToString();

			await That(result).IsEqualTo(" as block");
		}

		[Test]
		public async Task ToString_WhenCaseIsIgnored_ShouldReturnAsBlockIgnoringCase()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock().IgnoringCase();

			string result = sut.ToString();

			await That(result).IsEqualTo(" as block ignoring case");
		}
	}
}
