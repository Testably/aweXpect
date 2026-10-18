using System.Xml.Linq;
using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed partial class XmlOptionsTests
{
	[UseEnglishMessages]
	public sealed class IgnoringMember
	{
		[Test]
		[Arguments("@createdAt", "<a createdAt=\"1\"><b createdAt=\"2\"/></a>", "<a createdAt=\"3\"><b createdAt=\"4\"/></a>")]
		[Arguments("item/@id", "<order><item id=\"1\"/><item id=\"2\"/></order>", "<order><item id=\"3\"/><item id=\"4\"/></order>")]
		[Arguments("/order/@id", "<order id=\"1\"/>", "<order id=\"2\"/>")]
		[Arguments("item[2]", "<order><item/><item x=\"1\"/></order>", "<order><item/><item x=\"2\"><y/></item></order>")]
		[Arguments("item[1]/@x", "<order><item x=\"1\"/></order>", "<order><item x=\"2\"/></order>")]
		[Arguments("@createdAt", "<a/>", "<a createdAt=\"1\"/>")]
		[Arguments("@createdAt", "<a createdAt=\"1\"/>", "<a/>")]
		[Arguments("note", "<a/>", "<a><note/></a>")]
		[Arguments("note", "<a><note/></a>", "<a/>")]
		[Arguments("meta", "<a><meta><x>1</x></meta></a>", "<a><meta x=\"1\"/></a>")]
		[Arguments("price/text()", "<a><price>1.0</price></a>", "<a><price>1.00</price></a>")]
		[Arguments("note", "<a><b/><note/><c/></a>", "<a><note/><b/><c/></a>")]
		[Arguments("/a", "<b x=\"1\"/>", "<a x=\"2\"/>")]
		public async Task WhenOnlyIgnoredMembersDiffer_ShouldSucceed(string memberToIgnore, string subject,
			string expected)
		{
			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringMember(memberToIgnore));

			await That(Act).DoesNotThrow();
		}

		[Test]
		[Arguments("/order/@id", "<order><item id=\"1\"/></order>", "<order><item id=\"2\"/></order>",
			"/order/item/@id was \"1\" instead of \"2\"")]
		[Arguments("item[2]/@id", "<order><item id=\"1\"/><item id=\"2\"/></order>",
			"<order><item id=\"3\"/><item id=\"2\"/></order>", "/order/item[1]/@id was \"1\" instead of \"3\"")]
		[Arguments("d", "<a id=\"1\"/>", "<a id=\"2\"/>", "/a/@id was \"1\" instead of \"2\"")]
		[Arguments("@ID", "<a id=\"1\"/>", "<a id=\"2\"/>", "/a/@id was \"1\" instead of \"2\"")]
		[Arguments("b/@id", "<a id=\"1\"><b/></a>", "<a id=\"2\"><b/></a>", "/a/@id was \"1\" instead of \"2\"")]
		[Arguments("x/a/@id", "<a id=\"1\"/>", "<a id=\"2\"/>", "/a/@id was \"1\" instead of \"2\"")]
		public async Task WhenOtherMembersDiffer_ShouldFail(string memberToIgnore, string subject, string expected,
			string difference)
		{
			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringMember(memberToIgnore));

			await That(Act).Throws<FailException>()
				.WithMessage($"*but it differed as {difference}*").AsWildcard()
				.Because("only whole trailing segments of the path with the same case are ignored");
		}

		[Test]
		public async Task WhenComparisonFails_ShouldListTheMembersToIgnore()
		{
			string subject = "<a etag=\"1\" createdAt=\"x\" title=\"Let It Be\"/>";
			string expected = "<a etag=\"2\" createdAt=\"y\" title=\"Yesterday\"/>";

			async Task Act()
				=> await That(subject).IsEqualTo(expected)
					.AsXml(o => o.IgnoringMember("@createdAt").Ignoring(path => path.EndsWith("/@etag")));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is XML equivalent to <a etag="2" createdAt="y" title="Yesterday"/>,
				             but it differed as /a/@title was "Let It Be" instead of "Yesterday"

				             Actual:
				             <a etag="1" createdAt="x" title="Let It Be"/>

				             Expected:
				             <a etag="2" createdAt="y" title="Yesterday"/>

				             XML options:
				              - ignore members: ["@createdAt", path => path.EndsWith("/@etag")]
				             """);
		}

		[Test]
		public async Task WhenNegatedAndOnlyIgnoredMembersDiffer_ShouldFail()
		{
			string subject = "<a id=\"1\"/>";

			async Task Act()
				=> await That(subject).IsNotEqualTo("<a id=\"2\"/>").AsXml(o => o.IgnoringMember("@id"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not XML equivalent to <a id="2"/>,
				             but it was "<a id=\"1\"/>"

				             Expected:
				             <a id="2"/>

				             XML options:
				              - ignore members: ["@id"]
				             """);
		}

		[Test]
		public async Task WithMatches_ShouldIgnoreTheMembers()
		{
			XElement element = XElement.Parse("<order id=\"1\"><item sku=\"A1\"/></order>");

			async Task Act()
				=> await That(element).Matches("<order id=\"2\"><item sku=\"B2\"/></order>",
					o => o.IgnoringMember("@id"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that element
				             matches <order id="2"><item sku="B2"/></order>,
				             but it differed as /order/item/@sku was "A1" instead of "B2"

				             XML options:
				              - ignore members: ["@id"]
				             """);
		}

		[Test]
		public async Task WithMatchesExactlyOnDocument_ShouldIgnoreTheMembers()
		{
			XDocument document = XDocument.Parse("<order id=\"1\" createdAt=\"today\"/>");

			async Task Act()
				=> await That(document).MatchesExactly("<order id=\"1\"/>", o => o.IgnoringMember("@createdAt"));

			await That(Act).DoesNotThrow()
				.Because("an ignored member is not reported as unexpected");
		}

		[Test]
		public async Task WithIsValidXmlMatching_ShouldIgnoreTheMembers()
		{
			string subject = "<order id=\"1\"><item sku=\"A1\"/></order>";

			async Task Act()
				=> await That(subject).IsValidXmlMatchingExactly("<order id=\"2\"><item sku=\"A1\"/></order>",
					o => o.IgnoringMember("/order/@id"));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenMemberToIgnoreIsNull_ShouldThrowArgumentNullException()
		{
			async Task Act()
				=> await That("<a/>").IsEqualTo("<a/>").AsXml(o => o.IgnoringMember(null!));

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("memberToIgnore");
		}

		[Test]
		[Arguments("")]
		[Arguments("/")]
		[Arguments("//")]
		public async Task WhenMemberToIgnoreIsEmpty_ShouldThrowArgumentException(string memberToIgnore)
		{
			async Task Act()
				=> await That("<a/>").IsEqualTo("<a/>").AsXml(o => o.IgnoringMember(memberToIgnore));

			await That(Act).Throws<ArgumentException>()
				.WithParamName("memberToIgnore").And
				.WithMessage("The 'memberToIgnore' cannot be empty.*").AsWildcard();
		}

		[Test]
		public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
		{
			async Task Act()
				=> await That("<a/>").IsEqualTo("<a/>").AsXml(o => o.Ignoring(null!));

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("predicate");
		}
	}
}
