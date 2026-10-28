using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using static aweXpect.Expect;

namespace aweXpect.Xml.Aot;

internal static class Checks
{
	public static readonly Check[] All =
	[
		new("a valid XML string passes",
			() => ShouldPass(async () => await That("<a><b c=\"1\"/></a>").IsValidXml())),
		new("an invalid XML string fails and names the parse error",
			() => ShouldFail(async () => await That("<a><b>").IsValidXml(), "is valid XML", "could not be parsed")),
		new("a valid XML string fails the negation and names the subject",
			() => ShouldFail(async () => await That("<a/>").IsNotValidXml(), "is not valid XML", "but it was \"<a/>\"")),
		new("an XML string with a DTD fails",
			() => ShouldFail(async () => await That("<!DOCTYPE a [<!ENTITY e \"x\">]><a>&e;</a>").IsValidXml(),
				"is valid XML", "contained a document type definition (DTD)")),
		new("the root element of a valid XML string can be verified",
			() => ShouldPass(async () => await That("<a><b c=\"1\"/></a>")
				.IsValidXml(r => r.Satisfies(e => e.Name.LocalName == "a")))),
		new("a failing expectation on the root element fails",
			() => ShouldFail(async () => await That("<a/>")
				.IsValidXml(r => r.Satisfies(e => e.Name.LocalName == "b")), "is valid XML which satisfies")),
		new("equivalent XML strings pass",
			() => ShouldPass(async () => await That("<a y=\"2\" x=\"1\"><b/></a>")
				.IsEqualTo("<a x=\"1\" y=\"2\">\n  <b></b>\n</a>").AsXml())),
		new("different XML strings fail and name the path of the difference",
			() => ShouldFail(async () => await That("<a><b x=\"1\"/></a>").IsEqualTo("<a><b x=\"2\"/></a>").AsXml(),
				"is XML equivalent to", "/a/b/@x was \"1\" instead of \"2\"")),
		new("an element that contains the expected XML matches",
			() => ShouldPass(async () => await That(XElement.Parse("<a x=\"1\" y=\"2\"><b/></a>")).Matches("<a x=\"1\"/>"))),
		new("a document whose root has additional nodes does not match exactly",
			() => ShouldFail(async () => await That(XDocument.Parse("<a x=\"1\"/>")).MatchesExactly("<a/>"),
				"matches <a/> exactly", "/a/@x had unexpected \"1\"")),
		new("a valid XML string that differs from the expected XML fails",
			() => ShouldFail(async () => await That("<a x=\"2\"/>").IsValidXmlMatching("<a x=\"1\"/>"),
				"is valid XML which matches", "/a/@x was \"2\" instead of \"1\"")),
		new("an element with the expected attribute value and child element passes",
			() => ShouldPass(async () => await That(XElement.Parse("<a x=\"1\"><b/></a>"))
				.HasAttribute("x", x => x.IsEqualTo("1")).And.HasElement("b").And.DoesNotHaveElement("c"))),
		new("an element whose attribute value differs fails and names the attribute",
			() => ShouldFail(async () => await That(XElement.Parse("<a x=\"2\"/>")).HasAttribute("x", x => x.IsEqualTo("1")),
				"has attribute \"x\" whose value is equal to \"1\"", "value of attribute \"x\" was \"2\"")),
		new("a failed lookup shows the element it ran on",
			() => ShouldFail(async () => await That(XElement.Parse("<a y=\"2\"><b/></a>")).HasAttribute("x"),
				"did not have attribute \"x\"", "Actual:", "<a y=\"2\">", "  <b />")),
		new("a document whose root element has the expected attribute and child element passes",
			() => ShouldPass(async () => await That(XDocument.Parse("<a x=\"1\"><b/></a>"))
				.HasAttribute("x", x => x.IsEqualTo("1")).And.HasElement("b", b => b.Matches("<b/>")).And
				.DoesNotHaveElement("a"))),
		new("a document whose root element has the expected value passes",
			() => ShouldPass(async () => await That(XDocument.Parse("<a>x<b>y</b></a>")).HasValue(v => v.IsEqualTo("xy")))),
		new("a child element whose value differs fails and names the value",
			() => ShouldFail(async () => await That(XElement.Parse("<a><b>2</b></a>"))
					.HasElement("b", b => b.HasValue(v => v.IsEqualTo("1"))),
				"has element \"b\" which has a value which is equal to \"1\"", "value of the element was \"2\"")),
		new("a document without a root element fails also when negated",
			() => ShouldFail(async () => await That(new XDocument()).DoesNotHaveAttribute("x"),
				"does not have attribute \"x\"", "had no root element")),
		new("XML strings that only differ in ignored members pass",
			() => ShouldPass(async () => await That("<a id=\"1\" etag=\"x\"><b/></a>").IsEqualTo("<a id=\"2\"><b/></a>")
				.AsXml(o => o.IgnoringMember("/a/@id").IgnoringMembers(path => path.EndsWith("/@etag"))))),
		new("a failure lists the ignored members",
			() => ShouldFail(async () => await That(XElement.Parse("<a id=\"1\" x=\"1\"/>"))
					.Matches("<a id=\"2\" x=\"2\"/>", o => o.IgnoringMember("@id")),
				"/a/@x was \"1\" instead of \"2\"", "ignore members: [\"@id\"]")),
		new("reordered elements pass when ignoring the element order",
			() => ShouldPass(async () => await That(XElement.Parse("<a><b x=\"2\"/><c/><b x=\"1\"/></a>"))
				.MatchesExactly("<a><b x=\"1\"/><b x=\"2\"/><c/></a>", o => o.IgnoringElementOrder()))),
		new("a reordered element that differs fails when ignoring the element order",
			() => ShouldFail(async () => await That("<a><b x=\"3\"/><b x=\"2\"/></a>")
					.IsEqualTo("<a><b x=\"1\"/><b x=\"2\"/></a>").AsXml(o => o.IgnoringElementOrder()),
				"/a/b[1]/@x was \"3\" instead of \"1\"", "ignore element order")),
		new("a misspelled member to ignore is named with the meant path",
			() => ShouldFail(async () => await That("<a createdAt=\"1\"/>").IsEqualTo("<a createdAt=\"2\"/>")
					.AsXml(o => o.IgnoringMember("@createdat")),
				"\"@createdat\" matched no member (did you mean \"@createdAt\"?)")),
		new("the option methods need only the namespace aweXpect",
			() => ShouldPass(Consumer.Xml.ConsumerChecks.OptionsWithoutPackageNamespace)),
	];

	private static async Task<string?> ShouldPass(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName}: {exception.Message}";
		}
	}

	/// <summary>
	///     Expects the failure exception whose message contains every part.
	/// </summary>
	private static async Task<string?> ShouldFail(Func<Task> act, params string[] parts)
	{
		try
		{
			await act();
		}
		catch (FailException exception)
		{
			string? missing = Array.Find(parts, part => !exception.Message.Contains(part, StringComparison.Ordinal));
			return missing is null ? null : $"message lacks \"{missing}\": {exception.Message}";
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} instead of {typeof(FailException).FullName}: {exception.Message}";
		}

		return "did not throw";
	}
}
