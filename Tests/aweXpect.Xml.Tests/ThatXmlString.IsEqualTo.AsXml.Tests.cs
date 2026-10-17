using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXmlString
{
	public sealed class IsEqualTo
	{
		public sealed class AsXml
		{
			[UseEnglishMessages]
			public sealed class Tests
			{
				[Theory]
				[InlineData("<a><b>1</b></a>", "<a>\n  <b>1</b>\n</a>", "whitespace-only text between elements is ignored")]
				[InlineData("<a x=\"1\" y=\"2\"/>", "<a y='2' x='1'/>", "the order of attributes is ignored")]
				[InlineData("<p:a xmlns:p=\"urn:x\"><p:b/></p:a>", "<a xmlns=\"urn:x\"><b/></a>", "prefixes are not compared")]
				[InlineData("<a xmlns:unused=\"urn:y\"/>", "<a/>", "namespace declarations are no attributes")]
				[InlineData("<a/>", "<a></a>", "an empty element can be written in both forms")]
				[InlineData("<a><![CDATA[<b>]]></a>", "<a>&lt;b&gt;</a>", "CDATA sections are compared as text")]
				[InlineData("<a>x<![CDATA[y]]>z</a>", "<a>xyz</a>", "adjacent text and CDATA sections are joined")]
				[InlineData("<a><![CDATA[]]></a>", "<a/>", "an empty CDATA section is no text")]
				[InlineData("<a>x<!-- comment -->y</a>", "<a>xy</a>", "comments are ignored")]
				[InlineData("<?xml version=\"1.0\"?><!-- c --><?pi data?><a><?pi data?></a>", "<a/>",
					"the XML declaration, comments and processing instructions are ignored")]
				[InlineData("<a>&#65;&#x42;&amp;</a>", "<a>AB&amp;</a>", "character and entity references are resolved")]
				[InlineData("<a>x\r\ny</a>", "<a>x\ny</a>", "line endings are normalized")]
				public async Task WhenDocumentsAreEquivalent_ShouldSucceed(string subject, string expected, string because)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsXml();

					await That(Act).DoesNotThrow().Because(because);
				}

				[Theory]
				[InlineData("<a x=\"1\"/>", "<a x=\"2\"/>", "/a/@x was \"1\" instead of \"2\"")]
				[InlineData("<a/>", "<a x=\"2\"/>", "/a/@x was missing")]
				[InlineData("<a x=\"1\"/>", "<a/>", "/a/@x was unexpected")]
				[InlineData("<a xmlns:p=\"urn:p\" p:x=\"1\"/>", "<a x=\"1\"/>",
					"/a/@x was in namespace \"urn:p\" instead of no namespace")]
				[InlineData("<b/>", "<a/>", "/a was <b> instead of <a>")]
				[InlineData("<a xmlns=\"urn:v1\"/>", "<a xmlns=\"urn:v2\"/>",
					"/a was in namespace \"urn:v1\" instead of namespace \"urn:v2\"")]
				[InlineData("<a>1.0</a>", "<a>1.00</a>", "/a/text() was \"1.0\" instead of \"1.00\"")]
				[InlineData("<a> x </a>", "<a>x</a>", "/a/text() was \" x \" instead of \"x\"")]
				[InlineData("<a xml:space=\"preserve\"> </a>", "<a xml:space=\"preserve\"/>", "/a/text() was unexpected")]
				[InlineData("<a/>", "<a>x</a>", "/a/text() was missing")]
				[InlineData("<a><b>1</b><b>3</b></a>", "<a><b>1</b><b>2</b></a>", "/a/b[2]/text() was \"3\" instead of \"2\"")]
				[InlineData("<a><b/></a>", "<a><b/><b/></a>", "/a/b[2] was missing")]
				[InlineData("<a><b/><b/></a>", "<a><b/></a>", "/a/b[2] was unexpected")]
				[InlineData("<a><b/><c/></a>", "<a><b/></a>", "/a/c was unexpected")]
				[InlineData("<a><x/><b/><c/></a>", "<a><b/><c/></a>", "/a/x was unexpected")]
				[InlineData("<a><c/><b/></a>", "<a><b/><c/></a>", "/a had the child nodes in the order c, b instead of b, c")]
				[InlineData("<a><b>1</b><c/><b>2</b></a>", "<a><b>1</b><b>2</b><c/></a>",
					"/a had the child nodes in the order b[1], c, b[2] instead of b[1], b[2], c")]
				[InlineData("<a><b/>x</a>", "<a>x<b/></a>", "/a had the child nodes in the order b, text() instead of text(), b")]
				[InlineData("<a xmlns:p=\"urn:p\"><p:b/></a>", "<a><b/></a>", "/a/b was in namespace \"urn:p\" instead of no namespace")]
				[InlineData("<a>x<b/>y</a>", "<a>x<b/>z</a>", "/a/text()[2] was \"y\" instead of \"z\"")]
				[InlineData("<a><b><c d=\"1\"/></b></a>", "<a><b><c d=\"2\"/></b></a>", "/a/b/c/@d was \"1\" instead of \"2\"")]
				public async Task WhenDocumentsDiffer_ShouldFail(string subject, string expected, string difference)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is XML equivalent to {expected},
						              but it differed as {difference}

						              Actual:
						              {subject}

						              Expected:
						              {expected}
						              """);
				}

				[Fact]
				public async Task WhenRootElementDiffersByName_ShouldNotCompareItsContent()
				{
					string subject = "<b x=\"1\"><c/></b>";
					string expected = "<a x=\"2\"><d/></a>";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("*but it differed as /a was <b> instead of <a>*").AsWildcard()
						.Because("the content of different elements cannot be compared meaningfully");
				}

				[Fact]
				public async Task WhenElementDiffersByNamespace_ShouldNotCompareItsContent()
				{
					string subject = "<a xmlns=\"urn:v1\"><b x=\"1\"/></a>";
					string expected = "<a xmlns=\"urn:v2\"><b x=\"2\"/></a>";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("*but it differed as /a was in namespace \"urn:v1\" instead of namespace \"urn:v2\"*").AsWildcard()
						.Because("the children are in the namespace of the element as well and would only repeat the difference");
				}

				[Theory]
				[InlineData("<a><b/></a>", "<a><c/></a>", "/a/c was missing", "/a/b was unexpected")]
				[InlineData("<a><b/></a>", "<a>x</a>", "/a/text() was missing", "/a/b was unexpected")]
				[InlineData("<a>x</a>", "<a><b/></a>", "/a/b was missing", "/a/text() was unexpected")]
				public async Task WhenChildDiffersByName_ShouldReportMissingAndUnexpectedChild(string subject,
					string expected, string missing, string unexpected)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is XML equivalent to {expected},
						              but it differed as
						                {missing} and
						                {unexpected}

						              Actual:
						              {subject}

						              Expected:
						              {expected}
						              """)
						.Because("the children are paired by their name, so that their paths name the same node in both documents");
				}

				[Fact]
				public async Task WhenDocumentsHaveMultipleDifferences_ShouldListAllDifferences()
				{
					string subject = """
					                 <order id="42" status="open">
					                   <item sku="A1" price="1.0"/>
					                   <item sku="B2" price="2.0"/>
					                   <note>urgent</note>
					                 </order>
					                 """;
					string expected = """
					                  <order id="43">
					                    <item sku="A1" price="1.0"/>
					                    <item sku="B2" price="2.00" quantity="1"/>
					                  </order>
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is XML equivalent to {expected},
						              but it differed as
						                /order/@id was "42" instead of "43" and
						                /order/@status was unexpected and
						                /order/item[2]/@price was "2.0" instead of "2.00" and
						                /order/item[2]/@quantity was missing and
						                /order/note was unexpected

						              Actual:
						              {subject}

						              Expected:
						              {expected}
						              """);
				}

				[Fact]
				public async Task WhenContainsMoreThanMaximumNumberOfCollectionItemsDifferences_ShouldLimitListOfDifferences()
				{
					using IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(2);
					string subject = "<a b=\"1\" c=\"1\" d=\"1\" e=\"1\"/>";
					string expected = "<a/>";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is XML equivalent to <a/>,
						             but it differed as
						               /a/@b was unexpected and
						               /a/@c was unexpected and
						                … (2 more)

						             Actual:
						             <a b="1" c="1" d="1" e="1"/>

						             Expected:
						             <a/>
						             """);
				}

				[Theory]
				[InlineData("foo", "Data at the root level is invalid. Line 1, position 1.")]
				[InlineData("<a><b>", "Unexpected end of file has occurred. The following elements are not closed: b, a. Line 1, position 7.")]
				[InlineData("<a/><b/>", "There are multiple root elements. Line 1, position 6.")]
				public async Task WhenSubjectIsInvalidXml_ShouldFail(string subject, string errorMessage)
				{
					async Task Act()
						=> await That(subject).IsEqualTo("<a/>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is XML equivalent to <a/>,
						              but it was "{subject}", which could not be parsed as XML: {errorMessage}

						              Actual:
						              {subject}

						              Expected:
						              <a/>
						              """);
				}

				[Fact]
				public async Task WhenSubjectContainsDtd_ShouldFail()
				{
					string subject = "<!DOCTYPE a [<!ENTITY e \"x\">]><a>&e;</a>";

					async Task Act()
						=> await That(subject).IsEqualTo("<a>x</a>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is XML equivalent to <a>x</a>,
						             but it was "<!DOCTYPE a [<!ENTITY e \"x\">]><a>&e;</a>", which could not be parsed as XML, because it contained a document type definition (DTD), which is prohibited for security reasons

						             Actual:
						             <!DOCTYPE a [<!ENTITY e "x">]><a>&e;</a>

						             Expected:
						             <a>x</a>
						             """)
						.Because("DTD processing is prohibited to prevent XXE and entity expansion attacks");
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					string? subject = null;

					async Task Act()
						=> await That(subject).IsEqualTo("<a/>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is XML equivalent to <a/>,
						             but it was <null>
						             """);
				}

				[Theory]
				[InlineData("foo", "Data at the root level is invalid. Line 1, position 1.")]
				[InlineData("<a><b>", "Unexpected end of file has occurred. The following elements are not closed: b, a. Line 1, position 7.")]
				public async Task WhenExpectedIsInvalidXml_ShouldThrowArgumentException(string expected, string errorMessage)
				{
					async Task Act()
						=> await That("<a/>").IsEqualTo(expected).AsXml();

					await That(Act).Throws<ArgumentException>()
						.WithMessage($"The expected XML is invalid: {errorMessage}");
				}

				[Fact]
				public async Task WhenExpectedContainsDtd_ShouldThrowArgumentException()
				{
					async Task Act()
						=> await That("<a>x</a>").IsEqualTo("<!DOCTYPE a [<!ENTITY e \"x\">]><a>&e;</a>").AsXml();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The expected XML is invalid, because it contains a document type definition (DTD), which is prohibited for security reasons.");
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					async Task Act()
						=> await That("<a/>").IsEqualTo(null).AsXml();

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The expected XML cannot be null.*").AsWildcard();
				}

				[Fact]
				public async Task WhenIgnoringCase_ShouldThrowInvalidOperationException()
				{
					async Task Act()
						=> await That("<a/>").IsEqualTo("<a/>").AsXml().IgnoringCase();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("IgnoringCase cannot be combined with AsXml.")
						.Because("XML is compared by its content, so the casing option cannot be honoured");
				}

				[Fact]
				public async Task WhenUsingComparer_ShouldThrowInvalidOperationException()
				{
					async Task Act()
						=> await That("<a/>").IsEqualTo("<a/>").Using(StringComparer.Ordinal).AsXml();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with AsXml.")
						.Because("XML is compared by its content, so a custom comparer cannot be honoured");
				}

				[Fact]
				public async Task WhenCombinedWithAnotherMatchType_ShouldThrowInvalidOperationException()
				{
					async Task Act()
						=> await That("<a/>").IsEqualTo("<a/>").AsXml().AsWildcard();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("AsWildcard cannot be combined with AsXml.");
				}

				[Fact]
				public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
				{
					async Task Act()
						=> await That("<a/>").IsEqualTo("<a/>").AsXml().AsXml();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("AsXml cannot be specified more than once.");
				}

				[Theory]
				[InlineData(nameof(ThatString.Contains))]
				[InlineData(nameof(ThatString.DoesNotContain))]
				public async Task ForStringContains_ShouldNotBeAvailable(string methodName)
				{
					Type result = typeof(ThatString)
						.GetMethod(methodName, [typeof(IThat<string?>), typeof(string),])!.ReturnType;

					void Act()
						=> typeof(XmlExtensions).GetMethod(nameof(XmlExtensions.AsXml))!.MakeGenericMethod(result);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("GenericArguments[0], ?aweXpect.Results.StringOccurrenceCountResult`2*?TResult?.").AsWildcard()
						.Because("parts of an XML string are usually no XML document, so IsEqualTo(…).AsXml() should be used instead");
				}

				[Fact]
				public async Task ForCollections_ShouldCompareEachItem()
				{
					string[] subject = ["<a/>", "<a></a>", "<b/>",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("<a/>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is XML equivalent to <a/> for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [
						               "<b/>"
						             ]

						             Collection:
						             [
						               "<a/>",
						               "<a></a>",
						               "<b/>"
						             ]
						             """);
				}

				[Fact]
				public async Task ForCollections_WhenItemIsInvalidXml_ShouldFail()
				{
					string[] subject = ["<a/>", "foo",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("<a/>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is XML equivalent to <a/> for all items,
						             but an item was "foo", which could not be parsed as XML: Data at the root level is invalid. Line 1, position 1.
						             """);
				}

				[Fact]
				public async Task ForCollections_WhenEmptyAndExpectedIsInvalidXml_ShouldThrowArgumentException()
				{
					string[] subject = [];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").AsXml();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The expected XML is invalid: *").AsWildcard()
						.Because("an invalid expected XML is rejected also when no item is compared with it");
				}

				[Fact]
				public async Task ForCollectionContains_ShouldCompareEachItem()
				{
					string[] subject = ["<a x=\"2\"/>",];

					async Task Act()
						=> await That(subject).Contains("<a x=\"1\"/>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             contains an item XML equivalent to <a x="1"/> at least once,
						             but it did not contain it

						             Collection:
						             [
						               "<a x=\"2\"/>"
						             ]
						             """);
				}

				[Fact]
				public async Task WithMessage_ShouldCompareTheMessageAsXml()
				{
					Action subject = () => throw new InvalidOperationException("<a x=\"2\"/>");

					async Task Act()
						=> await That(subject).Throws<InvalidOperationException>().WithMessage("<a x=\"1\"/>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             throws an InvalidOperationException with message XML equivalent to <a x="1"/>,
						             but message differed as /a/@x was "2" instead of "1"

						             Message:
						             <a x="2"/>
						             """);
				}
			}

			[UseEnglishMessages]
			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenSubjectIsEquivalent_ShouldFail()
				{
					string subject = "<a y=\"2\" x=\"1\"/>";

					async Task Act()
						=> await That(subject).IsNotEqualTo("<a x=\"1\" y=\"2\"></a>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not XML equivalent to <a x="1" y="2"></a>,
						             but it was "<a y=\"2\" x=\"1\"/>"

						             Expected:
						             <a x="1" y="2"></a>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNotEquivalent_ShouldSucceed()
				{
					async Task Act()
						=> await That("<a x=\"1\"/>").IsNotEqualTo("<a x=\"2\"/>").AsXml();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsInvalidXml_ShouldFail()
				{
					string subject = "foo";

					async Task Act()
						=> await That(subject).IsNotEqualTo("<a/>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not XML equivalent to <a/>,
						             but it was "foo", which could not be parsed as XML: Data at the root level is invalid. Line 1, position 1.

						             Actual:
						             foo

						             Expected:
						             <a/>
						             """)
						.Because("a subject that is no XML is not different from the unexpected XML either");
				}

				[Fact]
				public async Task WhenExpectedIsInvalidXml_ShouldThrowArgumentException()
				{
					async Task Act()
						=> await That("<a/>").IsNotEqualTo("foo").AsXml();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The expected XML is invalid: *").AsWildcard()
						.Because("an invalid expected XML would let the negation pass for every subject");
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					string? subject = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo("<a/>").AsXml();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not XML equivalent to <a/>,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
