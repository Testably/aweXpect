using System.Xml.Linq;
using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed partial class XmlOptionsTests
{
	[UseEnglishMessages]
	public sealed class IgnoringElementOrder
	{
		[Theory]
		[InlineData("<a><c/><b/></a>", "<a><b/><c/></a>", "elements of different names")]
		[InlineData("<a><b x=\"2\"/><b x=\"1\"/></a>", "<a><b x=\"1\"/><b x=\"2\"/></a>", "elements of the same name")]
		[InlineData("<a><b><d/><c/></b></a>", "<a><b><c/><d/></b></a>", "nested elements")]
		[InlineData("<a><b/>x</a>", "<a>x<b/></a>", "texts")]
		[InlineData("<p>c<b/>a</p>", "<p>a<b/>c</p>", "texts in mixed content")]
		[InlineData("<a><b x=\"2\"><c/></b><b x=\"1\"/><d/></a>", "<a><d/><b x=\"1\"/><b x=\"2\"><c/></b></a>",
			"elements with content")]
		public async Task WhenOnlyTheOrderDiffers_ShouldSucceed(string subject, string expected, string because)
		{
			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringElementOrder());

			await That(Act).DoesNotThrow().Because($"the order of {because} is ignored");
		}

		[Fact]
		public async Task WhenOptionIsDisabled_ShouldVerifyTheOrder()
		{
			async Task Act()
				=> await That("<a><c/><b/></a>").IsEqualTo("<a><b/><c/></a>").AsXml(o => o.IgnoringElementOrder(false));

			await That(Act).Throws<XunitException>()
				.WithMessage("*/a had the child nodes in the order c, b instead of b, c*").AsWildcard();
		}

		[Fact]
		public async Task WhenElementsOfTheSameNameDiffer_ShouldCompareTheRemainingElementsInTheirOrder()
		{
			string subject = "<a><b x=\"2\"/><b x=\"3\"/></a>";
			string expected = "<a><b x=\"1\"/><b x=\"2\"/></a>";

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringElementOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is XML equivalent to <a><b x="1"/><b x="2"/></a>,
				             but it differed as /a/b[1]/@x was "3" instead of "1"

				             Actual:
				             <a><b x="2"/><b x="3"/></a>

				             Expected:
				             <a><b x="1"/><b x="2"/></a>

				             XML options:
				              - ignore element order
				             """)
				.Because("the equivalent elements are paired first, and the remaining ones are compared");
		}

		[Fact]
		public async Task WhenElementIsMissing_ShouldReportIt()
		{
			string subject = "<a><b x=\"3\"/><b x=\"1\"/></a>";
			string expected = "<a><b x=\"1\"/><b x=\"2\"/><b x=\"3\"/></a>";

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringElementOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("*but it differed as /a/b[2] was missing*").AsWildcard();
		}

		[Fact]
		public async Task WhenElementIsUnexpected_ShouldReportIt()
		{
			string subject = "<a><b x=\"1\"/><b x=\"2\"/><b x=\"3\"/></a>";
			string expected = "<a><b x=\"2\"/><b x=\"1\"/></a>";

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringElementOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("*but it differed as /a/b[3] was unexpected*").AsWildcard();
		}

		[Fact]
		public async Task WhenCombinedWithIgnoringMember_ShouldPairByTheRemainingContent()
		{
			string subject = "<order><item sku=\"B2\" id=\"1\"/><item sku=\"A1\" id=\"2\"/></order>";
			string expected = "<order><item sku=\"A1\" id=\"8\"/><item sku=\"B2\" id=\"9\"/></order>";

			async Task Act()
				=> await That(subject).IsEqualTo(expected)
					.AsXml(o => o.IgnoringElementOrder().IgnoringMember("@id"));

			await That(Act).DoesNotThrow()
				.Because("the ignored members are also ignored when looking for an equivalent element");
		}

		[Fact]
		public async Task WhenCombinedWithIgnoringMemberAtPosition_ShouldIgnoreThePositionInEachDocument()
		{
			string subject = "<a><b x=\"1\"/><b x=\"9\"/></a>";
			string expected = "<a><b x=\"1\"/><b x=\"8\"/></a>";

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringElementOrder().IgnoringMember("b[2]"));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenComparisonFails_ShouldListAllOptions()
		{
			async Task Act()
				=> await That("<a x=\"1\"/>").IsEqualTo("<a x=\"2\"/>")
					.AsXml(o => o.IgnoringElementOrder().IgnoringMember("@id"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             *
				             XML options:
				              - ignore element order
				              - ignore members: ["@id"]
				                "@id" matched no member
				             """).AsWildcard();
		}

		[Fact]
		public async Task WithMatches_ShouldIgnoreTheOrderAndAdditionalElements()
		{
			XElement element = XElement.Parse("<order><note/><item sku=\"B2\"/><item sku=\"A1\"/></order>");

			async Task Act()
				=> await That(element).Matches("<order><item sku=\"A1\"/><item sku=\"B2\"/></order>",
					o => o.IgnoringElementOrder());

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WithMatches_WhenAnExpectedElementMatchesSeveralElements_ShouldPairGreedily()
		{
			XElement element = XElement.Parse("<a><b x=\"1\"/><b/></a>");

			async Task Act()
				=> await That(element).Matches("<a><b/><b x=\"1\"/></a>", o => o.IgnoringElementOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("*but it differed as /a/b[2]/@x was missing*").AsWildcard()
				.Because("the first expected element is paired with the first element it matches, although the second expected element needed it");
		}
	}
}
