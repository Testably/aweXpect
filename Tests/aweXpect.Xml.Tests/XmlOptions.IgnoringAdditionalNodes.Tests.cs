using System.Xml.Linq;
using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed partial class XmlOptionsTests
{
	[UseEnglishMessages]
	public sealed class IgnoringAdditionalNodes
	{
		[Test]
		public async Task ForCollections_ShouldFindAnItemThatContainsTheExpectedXml()
		{
			string[] subject = ["<order id=\"1\"/>", "<order id=\"42\" status=\"open\"><item/></order>",];

			async Task Act()
				=> await That(subject).Contains("<order id=\"42\"/>").AsXml(o => o.IgnoringAdditionalNodes());

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task ForCollections_WhenNoItemContainsTheExpectedXml_ShouldListTheOption()
		{
			string[] subject = ["<order id=\"1\" status=\"open\"/>",];

			async Task Act()
				=> await That(subject).Contains("<order id=\"42\"/>").AsXml(o => o.IgnoringAdditionalNodes());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item XML equivalent to <order id="42"/> at least once,
				             but it did not contain it

				             Collection:
				             [
				               "<order id=\"1\" status=\"open\"/>"
				             ]

				             XML options:
				              - ignore additional nodes
				             """);
		}

		[Test]
		public async Task WhenCombinedWithIgnoringElementOrder_ShouldIgnoreTheOrderAndAdditionalNodes()
		{
			string subject = "<a><c y=\"1\"/><d/><b/></a>";

			async Task Act()
				=> await That(subject).IsEqualTo("<a><b/><c/></a>")
					.AsXml(o => o.IgnoringAdditionalNodes().IgnoringElementOrder());

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenComparisonFails_ShouldListTheOption()
		{
			string subject = "<order id=\"1\" status=\"open\"><item/></order>";

			async Task Act()
				=> await That(subject).IsEqualTo("<order id=\"42\"/>").AsXml(o => o.IgnoringAdditionalNodes());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is XML equivalent to <order id="42"/>,
				             but it differed as /order/@id was "1" instead of "42"

				             Actual:
				             <order id="1" status="open"><item/></order>

				             Expected:
				             <order id="42"/>

				             XML options:
				              - ignore additional nodes
				             """)
				.Because("the additional attribute and element are not reported");
		}

		[Test]
		public async Task WhenMemberToIgnoreOnlyOccursInAdditionalNodes_ShouldNameIt()
		{
			string subject = "<order id=\"1\" status=\"open\"/>";

			async Task Act()
				=> await That(subject).IsEqualTo("<order id=\"42\"/>")
					.AsXml(o => o.IgnoringAdditionalNodes().IgnoringMember("@status"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is XML equivalent to <order id="42"/>,
				             but it differed as /order/@id was "1" instead of "42"

				             Actual:
				             <order id="1" status="open"/>

				             Expected:
				             <order id="42"/>

				             XML options:
				              - ignore additional nodes
				              - ignore members: ["@status"]
				                "@status" matched no member
				             """)
				.Because("the additional nodes of the subject are not compared, so the member to ignore matched no compared path");
		}

		[Test]
		public async Task WhenNegated_ShouldFailForASubjectWithAdditionalNodes()
		{
			string subject = "<order id=\"42\" status=\"open\"/>";

			async Task Act()
				=> await That(subject).IsNotEqualTo("<order id=\"42\"/>").AsXml(o => o.IgnoringAdditionalNodes());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not XML equivalent to <order id="42"/>,
				             but it was "<order id=\"42\" status=\"open\"/>"

				             Expected:
				             <order id="42"/>

				             XML options:
				              - ignore additional nodes
				             """);
		}

		[Test]
		[Arguments("<a x=\"1\" y=\"2\"/>", "<a x=\"1\"/>", "attributes")]
		[Arguments("<a><b/><c/></a>", "<a><b/></a>", "child elements")]
		[Arguments("<a>text<b/></a>", "<a><b/></a>", "texts")]
		[Arguments("<a><b x=\"1\"><c/></b></a>", "<a><b/></a>", "nodes on every level")]
		public async Task WhenSubjectHasAdditionalNodes_ShouldSucceed(string subject, string expected, string because)
		{
			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsXml(o => o.IgnoringAdditionalNodes());

			await That(Act).DoesNotThrow().Because($"additional {because} are ignored");
		}

		[Test]
		public async Task WithIsValidXmlMatchingExactly_ShouldIgnoreAdditionalNodes()
		{
			string subject = "<a x=\"2\" y=\"2\"/>";

			async Task Act()
				=> await That(subject).IsValidXmlMatchingExactly("<a x=\"1\"/>", o => o.IgnoringAdditionalNodes());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is valid XML which matches <a x="1"/>,
				             but it differed as /a/@x was "2" instead of "1"

				             XML options (it):
				              - ignore additional nodes
				             """)
				.Because("the additional attribute is not reported, and the expectation is no longer exact");
		}

		[Test]
		public async Task WithMatches_WhenDisabled_ShouldCompareExactly()
		{
			XElement element = XElement.Parse("<a x=\"1\" y=\"2\"/>");

			async Task Act()
				=> await That(element).Matches("<a x=\"1\"/>", o => o.IgnoringAdditionalNodes(false));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that element
				             matches <a x="1"/> exactly,
				             but it differed as /a/@y had unexpected "2"
				             """)
				.Because("disabling the preset option makes Matches compare like MatchesExactly");
		}

		[Test]
		public async Task WithMatchesExactly_ShouldIgnoreAdditionalNodes()
		{
			XElement element = XElement.Parse("<a x=\"2\" y=\"2\"><b/></a>");

			async Task Act()
				=> await That(element).MatchesExactly("<a x=\"1\"/>", o => o.IgnoringAdditionalNodes());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that element
				             matches <a x="1"/>,
				             but it differed as /a/@x was "2" instead of "1"

				             XML options:
				              - ignore additional nodes
				             """)
				.Because("the option makes MatchesExactly compare like Matches");
		}

		[Test]
		public async Task WithMatchesExactly_WhenNegated_ShouldFailForASubjectWithAdditionalNodes()
		{
			XElement element = XElement.Parse("<a x=\"1\" y=\"2\"/>");

			async Task Act()
				=> await That(element)
					.DoesNotComplyWith(it => it.MatchesExactly("<a x=\"1\"/>", o => o.IgnoringAdditionalNodes()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that element
				             does not match <a x="1"/>,
				             but it did

				             Actual:
				             <a x="1" y="2" />

				             XML options:
				              - ignore additional nodes
				             """);
		}

		[Test]
		public async Task WithMatchesExactlyOnDocument_ShouldIgnoreAdditionalNodes()
		{
			XDocument document = XDocument.Parse("<a x=\"1\" y=\"2\"><b/></a>");

			async Task Act()
				=> await That(document).MatchesExactly("<a x=\"1\"/>", o => o.IgnoringAdditionalNodes());

			await That(Act).DoesNotThrow();
		}
	}
}
