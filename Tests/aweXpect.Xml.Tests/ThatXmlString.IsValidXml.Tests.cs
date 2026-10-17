using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXmlString
{
	public sealed class IsValidXml
	{
		[UseEnglishMessages]
		public sealed class Tests
		{
			[Theory]
			[InlineData("<!DOCTYPE a>")]
			[InlineData("<!DOCTYPE a><a/>")]
			[InlineData("<!DOCTYPE a [<!ENTITY e \"expanded\">]><a>&e;</a>")]
			[InlineData("<!DOCTYPE a [<!ENTITY a \"aaaa\"><!ENTITY b \"&a;&a;&a;&a;\">]><a>&b;</a>")]
			[InlineData("<?xml version=\"1.0\"?>\n<!-- comment -->\n<!DOCTYPE a SYSTEM \"http://example.com/a.dtd\"><a/>")]
			[InlineData("<!DOCTYPE a><a><b>")]
			[InlineData("<a/><!DOCTYPE a>")]
			public async Task WhenActualContainsDtd_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it contained a document type definition (DTD), which is prohibited for security reasons
					             """)
					.Because("DTD processing is prohibited to prevent XXE and entity expansion attacks");
			}

			[Fact]
			public async Task WhenActualContainsLoneSurrogate_ShouldFail()
			{
				string subject = "<a>\uD800</a>";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: '<', hexadecimal value 0x3C, is an invalid character. Line 1, position 5.
					             """)
					.Because("the reader reports a lone surrogate as an invalid pair with the character after it");
			}

			[Fact]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: Root element is missing.
					             """);
			}

			[Theory]
			[InlineData("foo", "Data at the root level is invalid. Line 1, position 1.")]
			[InlineData("<a><b>", "Unexpected end of file has occurred. The following elements are not closed: b, a. Line 1, position 7.")]
			[InlineData("<a></b>", "The 'a' start tag on line 1 position 2 does not match the end tag of 'b'. Line 1, position 6.")]
			[InlineData("<a/><b/>", "There are multiple root elements. Line 1, position 6.")]
			[InlineData("<a><!DOCTYPE a></a>", "Unexpected DTD declaration. Line 1, position 6.")]
			[InlineData("<a>\n  <b>\n</a>", "The 'b' start tag on line 2 position 4 does not match the end tag of 'a'. Line 3, position 3.")]
			[InlineData("<a>&foo;</a>", "Reference to undeclared entity 'foo'. Line 1, position 5.")]
			[InlineData("<a b=1/>", "'1' is an unexpected token. The expected token is '\"' or '''. Line 1, position 6.")]
			[InlineData("<x:a/>", "'x' is an undeclared prefix. Line 1, position 2.")]
			[InlineData("<a>\u0001</a>", "'\u0001', hexadecimal value 0x01, is an invalid character. Line 1, position 4.")]
			public async Task WhenActualIsInvalidXml_ShouldFail(string subject, string errorMessage)
			{
				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is valid XML,
					              but it could not be parsed: {errorMessage}
					              """);
			}

			[Theory]
			[InlineData("<a/>")]
			[InlineData("<a></a>")]
			[InlineData("\n  <a/>  \n")]
			[InlineData("<?xml version=\"1.0\" encoding=\"utf-8\"?><a/>")]
			[InlineData("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>\n<a/>")]
			[InlineData("<?xml version=\"1.0\" encoding=\"unknown\"?><a/>")]
			[InlineData("<?pi data?><!-- comment --><a/><!-- comment -->")]
			[InlineData("<a b=\"1\" c='2'><d>text</d><![CDATA[<not-an-element>]]></a>")]
			[InlineData("<a>&amp;&lt;&gt;&quot;&apos;&#65;&#x42;</a>")]
			[InlineData("<x:a xmlns:x=\"urn:x\" xmlns=\"urn:default\"><b/></x:a>")]
			public async Task WhenActualIsValidXml_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsWhitespace_ShouldFail()
			{
				string subject = " \t\r\n";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: Root element is missing.
					             """);
			}

			[Fact]
			public async Task WhenActualStartsWithByteOrderMark_ShouldFail()
			{
				string subject = "\uFEFF<a/>";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: Data at the root level is invalid. Line 1, position 1.
					             """)
					.Because("a byte order mark is an encoding signature of the bytes and no character of the document");
			}

			[Fact]
			public async Task WhenItemsArePlural_ShouldUsePluralVerb()
			{
				string subject = "<a/>\n<b>";

				async Task Act()
					=> await That(subject).HasLines(l => l.All().ComplyWith(it => it.IsValidXml()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has lines of which all are valid XML,
					             *
					             """).AsWildcard()
					.Because("the lines are the plural subject of the expectation");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenXmlDeclarationIsNotAtTheStart_ShouldFail()
			{
				string subject = " <?xml version=\"1.0\"?><a/>";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: Unexpected XML declaration. The XML declaration must be the first node in the document, and no white*space characters are allowed to appear before it. Line 1, position 4.
					             """).AsWildcard()
					.Because(".NET Framework spells \"white space\" in this message");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenActualIsInvalidXml_ShouldSucceed()
			{
				string subject = "<a><b>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsValidXml_ShouldFail()
			{
				string subject = "<a/>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML,
					             but it was "<a/>"
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML,
					             but it was <null>
					             """);
			}
		}
	}
}
