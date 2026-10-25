using System.Linq;
using System.Xml.Linq;
using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXmlString
{
	public sealed class IsValidXml
	{
		[UseEnglishMessages]
		public sealed class Tests
		{
			[Test]
			[Arguments("<!DOCTYPE a>")]
			[Arguments("<!DOCTYPE a><a/>")]
			[Arguments("<!DOCTYPE a [<!ENTITY e \"expanded\">]><a>&e;</a>")]
			[Arguments("<!DOCTYPE a [<!ENTITY a \"aaaa\"><!ENTITY b \"&a;&a;&a;&a;\">]><a>&b;</a>")]
			[Arguments("<?xml version=\"1.0\"?>\n<!-- comment -->\n<!DOCTYPE a SYSTEM \"http://example.com/a.dtd\"><a/>")]
			[Arguments("<!DOCTYPE a><a><b>")]
			[Arguments("<a/><!DOCTYPE a>")]
			public async Task WhenActualContainsDtd_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it contained a document type definition (DTD), which is prohibited for security reasons
					             """)
					.Because("DTD processing is prohibited to prevent XXE and entity expansion attacks");
			}

			[Test]
			public async Task WhenActualContainsLoneSurrogate_ShouldFail()
			{
				string subject = "<a>\uD800</a>";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: '<', hexadecimal value 0x3C, is an invalid character. Line 1, position 5.
					             """)
					.Because("the reader reports a lone surrogate as an invalid pair with the character after it");
			}

			[Test]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: Root element is missing.
					             """);
			}

			[Test]
			[Arguments("foo", "Data at the root level is invalid. Line 1, position 1.")]
			[Arguments("<a><b>", "Unexpected end of file has occurred. The following elements are not closed: b, a. Line 1, position 7.")]
			[Arguments("<a></b>", "The 'a' start tag on line 1 position 2 does not match the end tag of 'b'. Line 1, position 6.")]
			[Arguments("<a/><b/>", "There are multiple root elements. Line 1, position 6.")]
			[Arguments("<a><!DOCTYPE a></a>", "Unexpected DTD declaration. Line 1, position 6.")]
			[Arguments("<a>\n  <b>\n</a>", "The 'b' start tag on line 2 position 4 does not match the end tag of 'a'. Line 3, position 3.")]
			[Arguments("<a>&foo;</a>", "Reference to undeclared entity 'foo'. Line 1, position 5.")]
			[Arguments("<a b=1/>", "'1' is an unexpected token. The expected token is '\"' or '''. Line 1, position 6.")]
			[Arguments("<x:a/>", "'x' is an undeclared prefix. Line 1, position 2.")]
			[Arguments("<a>\u0001</a>", "'\u0001', hexadecimal value 0x01, is an invalid character. Line 1, position 4.")]
			public async Task WhenActualIsInvalidXml_ShouldFail(string subject, string errorMessage)
			{
				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is valid XML,
					              but it could not be parsed: {errorMessage}
					              """);
			}

			[Test]
			[Arguments("<a/>")]
			[Arguments("<a></a>")]
			[Arguments("\n  <a/>  \n")]
			[Arguments("<?xml version=\"1.0\" encoding=\"utf-8\"?><a/>")]
			[Arguments("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>\n<a/>")]
			[Arguments("<?xml version=\"1.0\" encoding=\"unknown\"?><a/>")]
			[Arguments("<?pi data?><!-- comment --><a/><!-- comment -->")]
			[Arguments("<a b=\"1\" c='2'><d>text</d><![CDATA[<not-an-element>]]></a>")]
			[Arguments("<a>&amp;&lt;&gt;&quot;&apos;&#65;&#x42;</a>")]
			[Arguments("<x:a xmlns:x=\"urn:x\" xmlns=\"urn:default\"><b/></x:a>")]
			public async Task WhenActualIsValidXml_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsWhitespace_ShouldFail()
			{
				string subject = " \t\r\n";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: Root element is missing.
					             """);
			}

			[Test]
			public async Task WhenActualStartsWithByteOrderMark_ShouldFail()
			{
				string subject = "\uFEFF<a/>";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: Data at the root level is invalid. Line 1, position 1.
					             """)
					.Because("a byte order mark is an encoding signature of the bytes and no character of the document");
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralVerb()
			{
				string subject = "<a/>\n<b>";

				async Task Act()
					=> await That(subject).HasLines(l => l.All().ComplyWith(it => it.IsValidXml()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines of which all are valid XML,
					             *
					             """).AsWildcard()
					.Because("the lines are the plural subject of the expectation");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenXmlDeclarationIsNotAtTheStart_ShouldFail()
			{
				string subject = " <?xml version=\"1.0\"?><a/>";

				async Task Act()
					=> await That(subject).IsValidXml();

				await That(Act).Throws<FailException>()
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
			[Test]
			public async Task WhenActualIsInvalidXml_ShouldSucceed()
			{
				string subject = "<a><b>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsValidXml_ShouldFail()
			{
				string subject = "<a/>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML,
					             but it was "<a/>"
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML,
					             but it was <null>
					             """);
			}
		}

		[UseEnglishMessages]
		public sealed class ExpectationsTests
		{
			[Test]
			public async Task WhenActualContainsDtd_ShouldFailWithoutExpandingEntities()
			{
				string subject = "<!DOCTYPE a [<!ENTITY e \"expanded\">]><a>&e;</a>";

				async Task Act()
					=> await That(subject).IsValidXml(r => r.Satisfies(e => e.Value == "expanded"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which satisfies e => e.Value == "expanded",
					             but it contained a document type definition (DTD), which is prohibited for security reasons
					             """);
			}

			[Test]
			public async Task WhenActualIsInvalidXml_ShouldFail()
			{
				string subject = "<a><b>";

				async Task Act()
					=> await That(subject).IsValidXml(r => r.Satisfies(e => e.Name == "a"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which satisfies e => e.Name == "a",
					             but it could not be parsed: Unexpected end of file has occurred. The following elements are not closed: b, a. Line 1, position 7.
					             """);
			}

			[Test]
			public async Task WhenCombinedWithAnd_ShouldApplyFollowingExpectationToString()
			{
				string subject = "<a/>";

				async Task Act()
					=> await That(subject).IsValidXml(r => r.Satisfies(e => e.Name == "a"))
						.And.HasLength().GreaterThan(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which satisfies e => e.Name == "a" and has length greater than 4,
					             but it had length 4
					             """);
			}

			[Test]
			public async Task WhenExpectationFails_ShouldFail()
			{
				string subject = "<a/>";

				async Task Act()
					=> await That(subject).IsValidXml(r => r.Satisfies(e => e.Name == "b"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which satisfies e => e.Name == "b",
					             but it was <a />
					             """);
			}

			[Test]
			public async Task WhenExpectationIsSatisfied_ShouldSucceed()
			{
				string subject = "<?xml version=\"1.0\"?><!-- comment --><a><b/></a>";

				async Task Act()
					=> await That(subject).IsValidXml(r => r.Satisfies(e => e.Name == "a"));

				await That(Act).DoesNotThrow()
					.Because("the expectations apply to the root element, not to the document");
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				string subject = "<a/>";

				async Task Act()
					=> await That(subject).IsValidXml(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNegatedAndActualContainsDtd_ShouldFail()
			{
				string subject = "<!DOCTYPE a [<!ENTITY e \"x\">]><a>&e;</a>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml(r => r.Satisfies(e => e.Value == "y")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML which satisfies e => e.Value == "y",
					             but it contained a document type definition (DTD), which is prohibited for security reasons
					             """)
					.Because("the expectations on the root element cannot be verified on a subject that is no valid XML, so the negation must not be met either");
			}

			[Test]
			public async Task WhenNegatedAndActualIsInvalidXml_ShouldFail()
			{
				string subject = "<a><b>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml(r => r.Satisfies(e => e.Name == "b")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML which satisfies e => e.Name == "b",
					             but it could not be parsed: Unexpected end of file has occurred. The following elements are not closed: b, a. Line 1, position 7.
					             """)
					.Because("the expectations on the root element cannot be verified on a subject that is no valid XML, so the negation must not be met either");
			}

			[Test]
			public async Task WhenNegatedAndExpectationFails_ShouldSucceed()
			{
				string subject = "<a/>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml(r => r.Satisfies(e => e.Name == "b")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedAndExpectationIsSatisfied_ShouldFail()
			{
				string subject = "<a/>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXml(r => r.Satisfies(e => e.Name == "a")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML which satisfies e => e.Name == "a",
					             but it was "<a/>"
					             """);
			}

			[Test]
			public async Task WhenRootHasNamespace_ShouldKeepIt()
			{
				string subject = "<x:a xmlns:x=\"urn:x\"/>";

				async Task Act()
					=> await That(subject).IsValidXml(r => r.Satisfies(e => e.Name == XName.Get("a", "urn:x")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("<a>\n  <b/>\n  <c/>\n</a>", 2)]
			[Arguments("<a> <b/> text <c/> </a>", 3)]
			[Arguments("<a xml:space=\"preserve\"> <b/> </a>", 3)]
			public async Task WhenSubjectContainsWhitespace_ShouldDropInsignificantWhitespace(string subject,
				int expectedNodes)
			{
				async Task Act()
					=> await That(subject).IsValidXml(r => r.Satisfies(e => e.Nodes().Count() == expectedNodes));

				await That(Act).DoesNotThrow()
					.Because("whitespace-only text is dropped like by XDocument.Parse, unless xml:space preserves it");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsValidXml(r => r.Satisfies(e => e.Name == "a"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which satisfies e => e.Name == "a",
					             but it was <null>
					             """);
			}
		}
	}

	public sealed class IsNotValidXml
	{
		[UseEnglishMessages]
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualContainsDtd_ShouldSucceed()
			{
				string subject = "<!DOCTYPE a [<!ENTITY e \"expanded\">]><a>&e;</a>";

				async Task Act()
					=> await That(subject).IsNotValidXml();

				await That(Act).DoesNotThrow()
					.Because("a document with a DTD is not valid XML, as DTD processing is prohibited");
			}

			[Test]
			[Arguments("")]
			[Arguments(" \t\r\n")]
			[Arguments("foo")]
			[Arguments("<a><b>")]
			[Arguments("<a></b>")]
			[Arguments("<a/><b/>")]
			[Arguments("<a>&foo;</a>")]
			[Arguments("<x:a/>")]
			public async Task WhenActualIsInvalidXml_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotValidXml();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("<a/>")]
			[Arguments("<a></a>")]
			[Arguments("<?pi data?><!-- comment --><a/>")]
			[Arguments("<x:a xmlns:x='urn:x'><b/></x:a>")]
			public async Task WhenActualIsValidXml_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotValidXml();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not valid XML,
					              but it was "{subject}"
					              """);
			}

			[Test]
			public async Task WhenCombinedWithAnd_ShouldApplyFollowingExpectationToString()
			{
				string subject = "<a>";

				async Task Act()
					=> await That(subject).IsNotValidXml().And.HasLength().GreaterThan(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML and has length greater than 3,
					             but it had length 3
					             """);
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralVerb()
			{
				string subject = "<b>\n<a/>";

				async Task Act()
					=> await That(subject).HasLines(l => l.All().ComplyWith(it => it.IsNotValidXml()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines of which all are not valid XML,
					             *
					             """).AsWildcard()
					.Because("the lines are the plural subject of the expectation");
			}

			[Test]
			public async Task WhenNegatedAndActualIsInvalidXml_ShouldFail()
			{
				string subject = "<a><b>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotValidXml());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it could not be parsed: Unexpected end of file has occurred. The following elements are not closed: b, a. Line 1, position 7.
					             """);
			}

			[Test]
			public async Task WhenNegatedAndSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotValidXml());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotValidXml();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML,
					             but it was <null>
					             """);
			}
		}
	}
}
