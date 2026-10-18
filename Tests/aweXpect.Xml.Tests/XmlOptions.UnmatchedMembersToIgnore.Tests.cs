using System.Xml.Linq;
using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed partial class XmlOptionsTests
{
	[UseEnglishMessages]
	public sealed class UnmatchedMembersToIgnore
	{
		[Test]
		public async Task WhenMemberToIgnoreMatchesNoMember_ShouldSuggestTheClosestPath()
		{
			string subject = "<order createdAt=\"today\" status=\"open\"/>";
			string expected = "<order createdAt=\"yesterday\" status=\"open\"/>";

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringMember("@createdat"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is XML equivalent to <order createdAt="yesterday" status="open"/>,
				             but it differed as /order/@createdAt was "today" instead of "yesterday"

				             Actual:
				             <order createdAt="today" status="open"/>

				             Expected:
				             <order createdAt="yesterday" status="open"/>

				             XML options:
				              - ignore members: ["@createdat"]
				                "@createdat" matched no member (did you mean "@createdAt"?)
				             """)
				.Because("the names are compared case-sensitively, so a misspelled casing ignores nothing");
		}

		[Test]
		[Arguments("@createdAd", "<a createdAt=\"1\"/>", "<a createdAt=\"2\"/>", "@createdAt")]
		[Arguments("itme/@id", "<o><item id=\"1\"/><item id=\"2\"/></o>", "<o><item id=\"3\"/><item id=\"2\"/></o>",
			"item/@id")]
		[Arguments("/order/@idd", "<order id=\"1\"/>", "<order id=\"2\"/>", "/order/@id")]
		[Arguments("prise", "<a><price>1</price></a>", "<a><price>2</price></a>", "price")]
		public async Task WhenMemberToIgnoreIsMisspelled_ShouldSuggestTheMeantPath(string memberToIgnore,
			string subject, string expected, string suggestion)
		{
			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringMember(memberToIgnore));

			await That(Act).Throws<FailException>()
				.WithMessage($"*\"{memberToIgnore}\" matched no member (did you mean \"{suggestion}\"?)*").AsWildcard();
		}

		[Test]
		[Arguments("@unrelated", "<a x=\"1\"/>", "<a x=\"2\"/>")]
		[Arguments("item[3]/@id", "<o><item id=\"1\"/><item id=\"2\"/></o>", "<o><item id=\"3\"/><item id=\"2\"/></o>")]
		[Arguments("/a/b/@x", "<a x=\"1\"/>", "<a x=\"2\"/>")]
		public async Task WhenNoPathIsCloseOrSeveralAreEquallyClose_ShouldNotSuggestAPath(string memberToIgnore,
			string subject, string expected)
		{
			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringMember(memberToIgnore));

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              *
				              XML options:
				               - ignore members: ["{memberToIgnore}"]
				                 "{memberToIgnore}" matched no member
				              """).AsWildcard()
				.Because("a wrong suggestion misleads more than none");
		}

		[Test]
		public async Task WhenMemberToIgnoreMatched_ShouldNotNameIt()
		{
			string subject = "<a id=\"1\" x=\"1\"/>";

			async Task Act()
				=> await That(subject).IsEqualTo("<a id=\"2\" x=\"2\"/>")
					.AsXml(o => o.IgnoringMember("@id").Ignoring(path => path.EndsWith("/@etag")));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             *
				             XML options:
				              - ignore members: ["@id", path => path.EndsWith("/@etag")]
				             """).AsWildcard()
				.Because("a matched member to ignore and a predicate, which cannot be named, are not listed as unmatched");
		}

		[Test]
		public async Task WhenComparisonSucceeds_ShouldNotFailBecauseOfAnUnmatchedMemberToIgnore()
		{
			async Task Act()
				=> await That("<a x=\"1\"/>").IsEqualTo("<a x=\"1\"/>").AsXml(o => o.IgnoringMember("@unrelated"));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WithMatches_ShouldSuggestTheClosestPath()
		{
			XElement element = XElement.Parse("<order etag=\"x1\"/>");

			async Task Act()
				=> await That(element).Matches("<order etag=\"x2\"/>", o => o.IgnoringMember("@eTag"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that element
				             matches <order etag="x2"/>,
				             but it differed as /order/@etag was "x1" instead of "x2"

				             XML options:
				              - ignore members: ["@eTag"]
				                "@eTag" matched no member (did you mean "@etag"?)
				             """);
		}

		[Test]
		public async Task WithIgnoringElementOrder_ShouldCountMembersMatchedWhilePairing()
		{
			string subject = "<a><b id=\"1\" x=\"1\"/><b id=\"2\" x=\"2\"/></a>";
			string expected = "<a><b id=\"3\" x=\"2\"/><b id=\"4\" x=\"3\"/></a>";

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringElementOrder().IgnoringMember("@id"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             *
				             XML options:
				              - ignore element order
				              - ignore members: ["@id"]
				             """).AsWildcard()
				.Because("the member to ignore matched while looking for equivalent elements");
		}

		[Test]
		public async Task ForCollections_ShouldNameTheUnmatchedMemberToIgnoreOnce()
		{
			string[] subject = ["<a createdAt=\"1\"/>", "<a createdAt=\"2\"/>",];

			async Task Act()
				=> await That(subject).All().AreEqualTo("<a createdAt=\"3\"/>")
					.AsXml(o => o.IgnoringMember("@createdat"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             *
				             XML options:
				              - ignore members: ["@createdat"]
				                "@createdat" matched no member (did you mean "@createdAt"?)
				             """).AsWildcard();
		}
	}
}
