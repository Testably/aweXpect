using System.Xml.Linq;
using aweXpect.Xml.Tests.Helpers;

namespace aweXpect.Xml.Tests;

public sealed class ThatXDocument
{
	public sealed class Matches
	{
		[UseEnglishMessages]
		public sealed class Tests
		{
			[Fact]
			public async Task WhenRootContainsTheExpectedXml_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<?xml version=\"1.0\"?><!-- c --><a x=\"1\" y=\"2\"><b/></a>");

				async Task Act()
					=> await That(document).Matches("<a x=\"1\"/>");

				await That(Act).DoesNotThrow()
					.Because("the root element is compared, ignoring its additional nodes");
			}

			[Fact]
			public async Task WhenRootDiffers_ShouldFail()
			{
				XDocument document = XDocument.Parse("<a x=\"2\"/>");

				async Task Act()
					=> await That(document).Matches("<a x=\"1\"/>");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that document
					             matches <a x="1"/>,
					             but it differed as /a/@x was "2" instead of "1"
					             """);
			}

			[Fact]
			public async Task WhenDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).Matches("<a/>");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that document
					             matches <a/>,
					             but it had no root element
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XDocument? subject = null;

				async Task Act()
					=> await That(subject).Matches("<a/>");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches <a/>,
					             but it was <null>
					             """);
			}
		}

		[UseEnglishMessages]
		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.Matches("<a/>"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that document
					             does not match <a/>,
					             but it had no root element
					             """)
					.Because("a document without a root element cannot be compared with the expected XML");
			}

			[Fact]
			public async Task WhenRootMatches_ShouldFail()
			{
				XDocument document = XDocument.Parse("<a x=\"1\"/>");

				async Task Act()
					=> await That(document).DoesNotComplyWith(it => it.Matches("<a/>"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that document
					             does not match <a/>,
					             but it did

					             Actual:
					             <a x="1" />
					             """);
			}
		}
	}

	public sealed class MatchesExactly
	{
		[UseEnglishMessages]
		public sealed class Tests
		{
			[Fact]
			public async Task WhenRootHasAdditionalNodes_ShouldFail()
			{
				XDocument document = XDocument.Parse("<a x=\"1\"><b/></a>");

				async Task Act()
					=> await That(document).MatchesExactly("<a/>");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that document
					             matches <a/> exactly,
					             but it differed as
					               /a/@x was unexpected and
					               /a/b was unexpected
					             """);
			}

			[Fact]
			public async Task WhenRootIsEquivalent_ShouldSucceed()
			{
				XDocument document = XDocument.Parse("<?xml version=\"1.0\"?><a y='2' x='1'/>");

				async Task Act()
					=> await That(document).MatchesExactly("<a x=\"1\" y=\"2\"></a>");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDocumentHasNoRoot_ShouldFail()
			{
				XDocument document = new();

				async Task Act()
					=> await That(document).MatchesExactly("<a/>");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that document
					             matches <a/> exactly,
					             but it had no root element
					             """);
			}
		}
	}
}
