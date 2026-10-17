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
			[Theory]
			[InlineData("<a x=\"1\" y=\"2\"/>", "<a x=\"1\"/>", "additional attributes are ignored")]
			[InlineData("<a><b/><c/></a>", "<a><b/></a>", "additional child elements are ignored")]
			[InlineData("<a><b/><b/></a>", "<a><b/></a>", "additional occurrences of a child element are ignored")]
			[InlineData("<a>text<b/></a>", "<a><b/></a>", "additional texts are ignored")]
			[InlineData("<a><b x=\"1\"><c/></b></a>", "<a><b/></a>", "additional nodes are ignored at every level")]
			[InlineData("<p:a xmlns:p=\"urn:x\" y='2'>\n  <p:b/>\n</p:a>", "<a xmlns=\"urn:x\"><b/></a>",
				"the elements are compared by their content")]
			public async Task WhenSubjectContainsTheExpectedXml_ShouldSucceed(string subject, string expected,
				string because)
			{
				XElement element = XElement.Parse(subject);

				async Task Act()
					=> await That(element).Matches(expected);

				await That(Act).DoesNotThrow().Because(because);
			}

			[Theory]
			[InlineData("<a x=\"2\"/>", "<a x=\"1\"/>", "/a/@x was \"2\" instead of \"1\"")]
			[InlineData("<a/>", "<a x=\"1\"/>", "/a/@x was missing")]
			[InlineData("<a><c/></a>", "<a><b/></a>", "/a/b was missing")]
			[InlineData("<a>y</a>", "<a>x</a>", "/a/text() was \"y\" instead of \"x\"")]
			[InlineData("<a><c/><b/></a>", "<a><b/><c/></a>", "/a had the child nodes in the order c, b instead of b, c")]
			[InlineData("<a xmlns=\"urn:v1\"/>", "<a xmlns=\"urn:v2\"/>",
				"/a was in namespace \"urn:v1\" instead of namespace \"urn:v2\"")]
			[InlineData("<b/>", "<a/>", "/a was <b> instead of <a>")]
			public async Task WhenSubjectDiffers_ShouldFail(string subject, string expected, string difference)
			{
				XElement element = XElement.Parse(subject);

				async Task Act()
					=> await That(element).Matches(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that element
					              matches {expected},
					              but it differed as {difference}
					              """);
			}

			[Fact]
			public async Task WhenSubjectHasMultipleDifferences_ShouldListAllDifferences()
			{
				XElement element = XElement.Parse("<order id=\"42\"><item sku=\"B2\"/></order>");

				async Task Act()
					=> await That(element).Matches("<order id=\"43\"><item sku=\"A1\"/></order>");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             matches <order id="43"><item sku="A1"/></order>,
					             but it differed as
					               /order/@id was "42" instead of "43" and
					               /order/item/@sku was "B2" instead of "A1"
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNestedElement_ShouldStartThePathsAtTheElement()
			{
				XElement element = XElement.Parse("<order><item sku=\"B2\"/></order>").Element("item")!;

				async Task Act()
					=> await That(element).Matches("<item sku=\"A1\"/>");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             matches <item sku="A1"/>,
					             but it differed as /item/@sku was "B2" instead of "A1"
					             """)
					.Because("the subject is compared without its ancestors");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).Matches("<a/>");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches <a/>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsInvalidXml_ShouldThrowArgumentException()
			{
				XElement element = new("a");

				async Task Act()
					=> await That(element).Matches("<a>");

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected XML is invalid: *").AsWildcard();
			}

			[Fact]
			public async Task WhenExpectedContainsDtd_ShouldThrowArgumentException()
			{
				XElement element = new("a");

				async Task Act()
					=> await That(element).Matches("<!DOCTYPE a><a/>");

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected XML is invalid, because it contains a document type definition (DTD), which is prohibited for security reasons.");
			}

			[Fact]
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
			[Fact]
			public async Task WhenSubjectMatches_ShouldFail()
			{
				XElement element = XElement.Parse("<a x=\"1\" y=\"2\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.Matches("<a x=\"1\"/>"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             does not match <a x="1"/>,
					             but it did

					             Actual:
					             <a x="1" y="2" />
					             """);
			}

			[Fact]
			public async Task WhenSubjectDiffers_ShouldSucceed()
			{
				XElement element = XElement.Parse("<a x=\"2\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.Matches("<a x=\"1\"/>"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches("<a/>"));

				await That(Act).Throws<XunitException>()
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
			[Theory]
			[InlineData("<a x=\"1\" y=\"2\"/>", "<a x=\"1\"/>", "/a/@y was unexpected")]
			[InlineData("<a><b/><c/></a>", "<a><b/></a>", "/a/c was unexpected")]
			[InlineData("<a>text<b/></a>", "<a><b/></a>", "/a/text() was unexpected")]
			[InlineData("<a x=\"2\"/>", "<a x=\"1\"/>", "/a/@x was \"2\" instead of \"1\"")]
			public async Task WhenSubjectDiffers_ShouldFail(string subject, string expected, string difference)
			{
				XElement element = XElement.Parse(subject);

				async Task Act()
					=> await That(element).MatchesExactly(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that element
					              matches {expected} exactly,
					              but it differed as {difference}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsEquivalent_ShouldSucceed()
			{
				XElement element = XElement.Parse("<p:a xmlns:p=\"urn:x\" y='2' x='1'>\n  <p:b/>\n</p:a>");

				async Task Act()
					=> await That(element).MatchesExactly("<a xmlns=\"urn:x\" x=\"1\" y=\"2\"><b></b></a>");

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("<a>\n  <b/>\n</a>", "<a><b/></a>")]
			[InlineData("<a>x<!-- c -->\n<!-- c -->y</a>", "<a>xy</a>")]
			[InlineData("<a xml:space=\"preserve\"><b xml:space=\"default\"> <c/> </b></a>",
				"<a xml:space=\"preserve\"><b xml:space=\"default\"><c/></b></a>")]
			public async Task WhenSubjectKeepsInsignificantWhitespace_ShouldIgnoreIt(string subject, string expected)
			{
				XElement element = XElement.Parse(subject, LoadOptions.PreserveWhitespace);

				async Task Act()
					=> await That(element).MatchesExactly(expected);

				await That(Act).DoesNotThrow()
					.Because("whitespace-only text is dropped like by XElement.Parse, unless xml:space preserves it");
			}

			[Theory]
			[InlineData("<a xml:space=\"preserve\"> <b/></a>", "<a xml:space=\"preserve\"><b/></a>", "/a/text() was unexpected")]
			[InlineData("<a xml:space=\"preserve\"><b> <c/></b></a>", "<a xml:space=\"preserve\"><b><c/></b></a>",
				"/a/b/text() was unexpected")]
			[InlineData("<a><![CDATA[ ]]></a>", "<a/>", "/a/text() was unexpected")]
			public async Task WhenSubjectHasSignificantWhitespace_ShouldCompareIt(string subject, string expected,
				string difference)
			{
				XElement element = XElement.Parse(subject, LoadOptions.PreserveWhitespace);

				async Task Act()
					=> await That(element).MatchesExactly(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"*but it differed as {difference}*").AsWildcard()
					.Because("whitespace is significant within xml:space=\"preserve\" and in CDATA sections");
			}

			[Fact]
			public async Task WhenAncestorOfSubjectPreservesWhitespace_ShouldCompareIt()
			{
				XElement element = XElement.Parse("<r xml:space=\"preserve\"><a> <b/></a></r>", LoadOptions.PreserveWhitespace)
					.Element("a")!;

				async Task Act()
					=> await That(element).MatchesExactly("<a><b/></a>");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed as /a/text() was unexpected*").AsWildcard()
					.Because("xml:space applies to the subject also when it is set on an ancestor");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).MatchesExactly("<a/>");

				await That(Act).Throws<XunitException>()
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
			[Fact]
			public async Task WhenSubjectMatchesExactly_ShouldFail()
			{
				XElement element = XElement.Parse("<a x=\"1\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.MatchesExactly("<a x='1'></a>"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             does not match <a x='1'></a> exactly,
					             but it did

					             Actual:
					             <a x="1" />
					             """);
			}

			[Fact]
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
