using System.Collections.Generic;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class ContainingMatchTypeTests
	{
		[Test]
		[Arguments("foobar", "oba", true)]
		[Arguments("foobar", "bar", true)]
		[Arguments("foobar", "xyz", false)]
		[Arguments("foobar", "OBA", false)]
		[Arguments("fo", "foo", false)]
		public async Task AreConsideredEqual_WhenAComparerIsUsed_ShouldSearchTheSubstringWithIt(
			string actual, string expected, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.Containing().Using(StringComparer.Ordinal);

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
		public async Task AreConsideredEqual_WhenAComparerIsUsed_ShouldUseItForEachCandidate()
		{
			StringEqualityOptions sut = new("expected");
			sut.Containing().Using(StringComparer.OrdinalIgnoreCase);

			bool result = await sut.AreConsideredEqual("FOOBAR", "oba");

			await That(result).IsTrue()
				.Because("the comparer decides whether a substring matches, not an ordinal search");
		}

		[Test]
		public async Task AreConsideredEqual_WhenBothAreNull_ShouldReturnTrue()
		{
			StringEqualityOptions sut = new("expected");
			sut.Containing();

			bool result = await sut.AreConsideredEqual(null, (string?)null);

			await That(result).IsTrue();
		}

		[Test]
		[Arguments("forget", " get", false)]
		[Arguments("for get", " get", true)]
		[Arguments("get", "\t get", true)]
		[Arguments("forget ", "for ", false)]
		[Arguments("for", "for \t", true)]
		[Arguments("a", " a ", true)]
		[Arguments("a b", " ", true)]
		[Arguments(" ab ", " ", false)]
		public async Task
			AreConsideredEqual_WhenWhiteSpaceIsIgnored_ShouldOnlyIgnoreTheWhiteSpaceOfTheSubstringAtTheEdgesOfTheSubject(
				string actual, string expected, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.Containing().IgnoringLeadingWhiteSpace().IgnoringTrailingWhiteSpace();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectMatch)
				.Because("the whitespace of the substring is only optional where it reaches an edge of the subject");
		}

		[Test]
		public async Task Contains_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new("expected");

			StringEqualityOptions result = sut.Containing();

			await That(result).IsSameAs(sut);
		}

		[Test]
		[Arguments(ExpectationGrammars.Active, "contains \"foo\"")]
		[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not contain \"foo\"")]
		[Arguments(ExpectationGrammars.None, "containing \"foo\"")]
		[Arguments(ExpectationGrammars.Negated, "not containing \"foo\"")]
		public async Task GetExpectation_ShouldDescribeTheSubstring(ExpectationGrammars grammars, string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.Containing();

			string result = sut.GetExpectation("foo", grammars);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		[Arguments(null, "foo", "it was <null>")]
		[Arguments("", "foo", "it was \"\"")]
		[Arguments("bar", null, "it was \"bar\"")]
		public async Task GetExtendedFailure_WhenSubjectIsEmptyOrAnyValueIsNull_ShouldOnlyStateTheSubject(
			string? actual, string? expected, string expectedFailure)
		{
			StringEqualityOptions sut = new("expected");
			sut.Containing();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsEqualTo(expectedFailure);
		}

		[Test]
		[Arguments(false, " containing")]
		[Arguments(true, " containing ignoring case")]
		public async Task ToString_ShouldIncludeTheMatchTypeAndTheOptions(bool ignoreCase, string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.Containing().IgnoringCase(ignoreCase);

			string result = sut.ToString();

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task ToString_WhenAComparerIsUsed_ShouldNameTheComparer()
		{
			StringEqualityOptions sut = new("expected");
			sut.Containing().Using(new IgnoreCaseComparer());

			string result = sut.ToString();

			await That(result)
				.IsEqualTo(" containing using StringEqualityOptionsTests.ContainingMatchTypeTests.IgnoreCaseComparer");
		}

		private sealed class IgnoreCaseComparer : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y) => StringComparer.OrdinalIgnoreCase.Equals(x, y);

			public int GetHashCode(string obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj);
		}
	}
}
