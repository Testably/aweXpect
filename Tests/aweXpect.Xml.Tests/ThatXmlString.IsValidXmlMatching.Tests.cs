using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXmlString
{
	public sealed class IsValidXmlMatching
	{
		[UseEnglishMessages]
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectContainsTheExpectedXml_ShouldSucceed()
			{
				string subject = "<a x=\"1\" y=\"2\"><b/></a>";

				async Task Act()
					=> await That(subject).IsValidXmlMatching("<a x=\"1\"/>");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectDiffers_ShouldFail()
			{
				string subject = "<a x=\"2\"/>";

				async Task Act()
					=> await That(subject).IsValidXmlMatching("<a x=\"1\"/>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which matches <a x="1"/>,
					             but it differed as /a/@x was "2" instead of "1"
					             """);
			}

			[Test]
			public async Task WhenSubjectIsInvalidXml_ShouldFail()
			{
				string subject = "<a><b>";

				async Task Act()
					=> await That(subject).IsValidXmlMatching("<a/>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which matches <a/>,
					             but it could not be parsed: Unexpected end of file has occurred. The following elements are not closed: b, a. Line 1, position 7.
					             """);
			}

			[Test]
			public async Task WhenSubjectContainsDtd_ShouldFail()
			{
				string subject = "<!DOCTYPE a [<!ENTITY e \"x\">]><a>&e;</a>";

				async Task Act()
					=> await That(subject).IsValidXmlMatching("<a>x</a>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which matches <a>x</a>,
					             but it contained a document type definition (DTD), which is prohibited for security reasons
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsValidXmlMatching("<a/>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which matches <a/>,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedIsInvalidXml_ShouldThrowArgumentException()
			{
				string subject = "<a/>";

				async Task Act()
					=> await That(subject).IsValidXmlMatching("<a>");

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected XML is invalid: *").AsWildcard();
			}
		}

		[UseEnglishMessages]
		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsInvalidXml_ShouldSucceed()
			{
				string subject = "<a>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXmlMatching("<a/>"));

				await That(Act).DoesNotThrow()
					.Because("a subject that is no valid XML is not valid XML which matches the expected XML either");
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreInvalidXml_ShouldThrowArgumentException()
			{
				string subject = "<a><b>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXmlMatching("<a>"));

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected XML is invalid: *").AsWildcard()
					.Because("an invalid expected XML would let the negation pass for every subject that is no valid XML");
			}

			[Test]
			public async Task WhenSubjectMatches_ShouldFail()
			{
				string subject = "<a x=\"1\"/>";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidXmlMatching("<a/>"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid XML which matches <a/>,
					             but it was "<a x=\"1\"/>"
					             """);
			}
		}
	}

	public sealed class IsValidXmlMatchingExactly
	{
		[UseEnglishMessages]
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectHasAdditionalNodes_ShouldFail()
			{
				string subject = "<a x=\"1\" y=\"2\"/>";

				async Task Act()
					=> await That(subject).IsValidXmlMatchingExactly("<a x=\"1\"/>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid XML which matches <a x="1"/> exactly,
					             but it differed as /a/@y was unexpected
					             """);
			}

			[Test]
			public async Task WhenSubjectIsEquivalent_ShouldSucceed()
			{
				string subject = "<a y=\"2\" x=\"1\"/>";

				async Task Act()
					=> await That(subject).IsValidXmlMatchingExactly("<a x=\"1\" y=\"2\"></a>");

				await That(Act).DoesNotThrow();
			}
		}
	}
}
