using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class ContainingMatchTypeTests
	{
		[Theory]
		[InlineData("forget", " get", false)]
		[InlineData("for get", " get", true)]
		[InlineData("get", "\t get", true)]
		[InlineData("forget ", "for ", false)]
		[InlineData("for", "for \t", true)]
		[InlineData("a", " a ", true)]
		[InlineData("a b", " ", true)]
		[InlineData(" ab ", " ", false)]
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

		[Fact]
		public async Task Contains_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new("expected");

			StringEqualityOptions result = sut.Containing();

			await That(result).IsSameAs(sut);
		}
	}
}
