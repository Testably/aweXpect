using System.Xml.Linq;
using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXElement
{
	public sealed class Matches
	{
		[UseEnglishMessages]
		public sealed class Tests
		{
			[Test]
			[Arguments("<a x=\"1\" y=\"2\"/>", "<a x=\"1\"/>", "additional attributes are ignored")]
			[Arguments("<a><b/><c/></a>", "<a><b/></a>", "additional child elements are ignored")]
			[Arguments("<a><b/><b/></a>", "<a><b/></a>", "additional occurrences of a child element are ignored")]
			[Arguments("<a>text<b/></a>", "<a><b/></a>", "additional texts are ignored")]
			[Arguments("<a><b x=\"1\"><c/></b></a>", "<a><b/></a>", "additional nodes are ignored at every level")]
			[Arguments("<p:a xmlns:p=\"urn:x\" y='2'>\n  <p:b/>\n</p:a>", "<a xmlns=\"urn:x\"><b/></a>",
				"the elements are compared by their content")]
			public async Task WhenSubjectContainsTheExpectedXml_ShouldSucceed(string subject, string expected,
				string because)
			{
				XElement element = XElement.Parse(subject);

				async Task Act()
					=> await That(element).Matches(expected);

				await That(Act).DoesNotThrow().Because(because);
			}

			[Test]
			[Arguments("<a x=\"2\"/>", "<a x=\"1\"/>", "/a/@x was \"2\" instead of \"1\"")]
			[Arguments("<a/>", "<a x=\"1\"/>", "/a/@x was missing")]
			[Arguments("<a><c/></a>", "<a><b/></a>", "/a/b was missing")]
			[Arguments("<a>y</a>", "<a>x</a>", "/a/text() was \"y\" instead of \"x\"")]
			[Arguments("<a><c/><b/></a>", "<a><b/><c/></a>", "/a had the child nodes in the order c, b instead of b, c")]
			[Arguments("<a xmlns=\"urn:v1\"/>", "<a xmlns=\"urn:v2\"/>",
				"/a was in namespace \"urn:v1\" instead of namespace \"urn:v2\"")]
			[Arguments("<b/>", "<a/>", "/a was <b> instead of <a>")]
			public async Task WhenSubjectDiffers_ShouldFail(string subject, string expected, string difference)
			{
				XElement element = XElement.Parse(subject);

				async Task Act()
					=> await That(element).Matches(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that element
					              matches {expected},
					              but it differed as {difference}
					              """);
			}

			[Test]
			public async Task WhenSubjectHasMultipleDifferences_ShouldListAllDifferences()
			{
				XElement element = XElement.Parse("<order id=\"42\"><item sku=\"B2\"/></order>");

				async Task Act()
					=> await That(element).Matches("<order id=\"43\"><item sku=\"A1\"/></order>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             matches <order id="43"><item sku="A1"/></order>,
					             but it differed as
					               /order/@id was "42" instead of "43" and
					               /order/item/@sku was "B2" instead of "A1"
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNestedElement_ShouldStartThePathsAtTheElement()
			{
				XElement element = XElement.Parse("<order><item sku=\"B2\"/></order>").Element("item")!;

				async Task Act()
					=> await That(element).Matches("<item sku=\"A1\"/>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             matches <item sku="A1"/>,
					             but it differed as /item/@sku was "B2" instead of "A1"
					             """)
					.Because("the subject is compared without its ancestors");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).Matches("<a/>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches <a/>,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedIsInvalidXml_ShouldThrowArgumentException()
			{
				XElement element = new("a");

				async Task Act()
					=> await That(element).Matches("<a>");

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected XML is invalid: *").AsWildcard();
			}

			[Test]
			public async Task WhenExpectedContainsDtd_ShouldThrowArgumentException()
			{
				XElement element = new("a");

				async Task Act()
					=> await That(element).Matches("<!DOCTYPE a><a/>");

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected XML is invalid, because it contains a document type definition (DTD), which is prohibited for security reasons.");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("a");

				async Task Act()
					=> await That(element).Matches(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected XML cannot be null.*").AsWildcard();
			}
		}

		[UseEnglishMessages]
		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectMatches_ShouldFail()
			{
				XElement element = XElement.Parse("<a x=\"1\" y=\"2\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.Matches("<a x=\"1\"/>"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             does not match <a x="1"/>,
					             but it did

					             Actual:
					             <a x="1" y="2" />
					             """);
			}

			[Test]
			public async Task WhenSubjectDiffers_ShouldSucceed()
			{
				XElement element = XElement.Parse("<a x=\"2\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.Matches("<a x=\"1\"/>"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches("<a/>"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match <a/>,
					             but it was <null>
					             """);
			}
		}
	}

	public sealed class MatchesExactly
	{
		[UseEnglishMessages]
		public sealed class Tests
		{
			[Test]
			[Arguments("<a x=\"1\" y=\"2\"/>", "<a x=\"1\"/>", "/a/@y was unexpected")]
			[Arguments("<a><b/><c/></a>", "<a><b/></a>", "/a/c was unexpected")]
			[Arguments("<a>text<b/></a>", "<a><b/></a>", "/a/text() was unexpected")]
			[Arguments("<a x=\"2\"/>", "<a x=\"1\"/>", "/a/@x was \"2\" instead of \"1\"")]
			public async Task WhenSubjectDiffers_ShouldFail(string subject, string expected, string difference)
			{
				XElement element = XElement.Parse(subject);

				async Task Act()
					=> await That(element).MatchesExactly(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that element
					              matches {expected} exactly,
					              but it differed as {difference}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsEquivalent_ShouldSucceed()
			{
				XElement element = XElement.Parse("<p:a xmlns:p=\"urn:x\" y='2' x='1'>\n  <p:b/>\n</p:a>");

				async Task Act()
					=> await That(element).MatchesExactly("<a xmlns=\"urn:x\" x=\"1\" y=\"2\"><b></b></a>");

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("<a>\n  <b/>\n</a>", "<a><b/></a>")]
			[Arguments("<a>x<!-- c -->\n<!-- c -->y</a>", "<a>xy</a>")]
			[Arguments("<a xml:space=\"preserve\"><b xml:space=\"default\"> <c/> </b></a>",
				"<a xml:space=\"preserve\"><b xml:space=\"default\"><c/></b></a>")]
			public async Task WhenSubjectKeepsInsignificantWhitespace_ShouldIgnoreIt(string subject, string expected)
			{
				XElement element = XElement.Parse(subject, LoadOptions.PreserveWhitespace);

				async Task Act()
					=> await That(element).MatchesExactly(expected);

				await That(Act).DoesNotThrow()
					.Because("whitespace-only text is dropped like by XElement.Parse, unless xml:space preserves it");
			}

			[Test]
			[Arguments("<a xml:space=\"preserve\"> <b/></a>", "<a xml:space=\"preserve\"><b/></a>", "/a/text() was unexpected")]
			[Arguments("<a xml:space=\"preserve\"><b> <c/></b></a>", "<a xml:space=\"preserve\"><b><c/></b></a>",
				"/a/b/text() was unexpected")]
			[Arguments("<a><![CDATA[ ]]></a>", "<a/>", "/a/text() was unexpected")]
			public async Task WhenSubjectHasSignificantWhitespace_ShouldCompareIt(string subject, string expected,
				string difference)
			{
				XElement element = XElement.Parse(subject, LoadOptions.PreserveWhitespace);

				async Task Act()
					=> await That(element).MatchesExactly(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"*but it differed as {difference}*").AsWildcard()
					.Because("whitespace is significant within xml:space=\"preserve\" and in CDATA sections");
			}

			[Test]
			public async Task WhenAncestorOfSubjectPreservesWhitespace_ShouldCompareIt()
			{
				XElement element = XElement.Parse("<r xml:space=\"preserve\"><a> <b/></a></r>", LoadOptions.PreserveWhitespace)
					.Element("a")!;

				async Task Act()
					=> await That(element).MatchesExactly("<a><b/></a>");

				await That(Act).Throws<FailException>()
					.WithMessage("*but it differed as /a/text() was unexpected*").AsWildcard()
					.Because("xml:space applies to the subject also when it is set on an ancestor");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).MatchesExactly("<a/>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches <a/> exactly,
					             but it was <null>
					             """);
			}
		}

		[UseEnglishMessages]
		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectMatchesExactly_ShouldFail()
			{
				XElement element = XElement.Parse("<a x=\"1\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.MatchesExactly("<a x='1'></a>"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             does not match <a x='1'></a> exactly,
					             but it did

					             Actual:
					             <a x="1" />
					             """);
			}

			[Test]
			public async Task WhenSubjectHasAdditionalNodes_ShouldSucceed()
			{
				XElement element = XElement.Parse("<a x=\"1\" y=\"2\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.MatchesExactly("<a x=\"1\"/>"));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
