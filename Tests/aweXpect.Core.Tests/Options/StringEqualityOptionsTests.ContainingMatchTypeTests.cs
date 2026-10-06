using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class ContainingMatchTypeTests
	{
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
	}
}
