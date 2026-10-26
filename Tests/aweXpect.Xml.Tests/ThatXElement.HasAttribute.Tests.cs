using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Xml.Results;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXElement
{
	public sealed class HasAttribute
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAttributeExists_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("title");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAttributeDoesNotExist_ShouldFail()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("artist");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "artist",
					             but it did not have attribute "artist"

					             Actual:
					             <track title="Let It Be" />
					             """);
			}

			[Test]
			public async Task WhenAttributeIsInAnotherNamespace_ShouldFail()
			{
				XElement element = XElement.Parse("<track xmlns:m=\"urn:meta\" m:title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "title",
					             but it did not have attribute "title"

					             Actual:
					             <track xmlns:m="urn:meta" m:title="Let It Be" />
					             """)
					.Because("the attribute is looked up by its namespace and local name, like by XElement.Attribute");
			}

			[Test]
			public async Task WhenAttributeIsInTheSpecifiedNamespace_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track xmlns:m=\"urn:meta\" m:title=\"Let It Be\"/>");
				XNamespace meta = "urn:meta";

				async Task Act()
					=> await That(element).HasAttribute(meta + "title", value => value.IsEqualTo("Let It Be"));

				await That(Act).DoesNotThrow()
					.Because("the prefix of the attribute is not compared");
			}

			[Test]
			public async Task WhenAttributeInNamespaceDoesNotExist_ShouldNameTheNamespace()
			{
				XElement element = XElement.Parse("<track/>");
				XNamespace meta = "urn:meta";

				async Task Act()
					=> await That(element).HasAttribute(meta + "title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "{urn:meta}title",
					             but it did not have attribute "{urn:meta}title"

					             Actual:
					             <track />
					             """);
			}

			[Test]
			public async Task WhenValueSatisfiesTheExpectation_ShouldSucceed()
			{
				XElement element = XElement.Parse("<item price=\"1.00\"/>");

				async Task Act()
					=> await That(element).HasAttribute("price", value => value.StartsWith("1."));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueDoesNotSatisfyTheExpectation_ShouldFail()
			{
				XElement element = XElement.Parse("<item price=\"2.00\"/>");

				async Task Act()
					=> await That(element).HasAttribute("price", value => value.IsEqualTo("1.00"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "price" whose value is equal to "1.00",
					             but value of attribute "price" was "2.00", which differs*
					             """).AsWildcard()
					.Because("the value is verified with the string expectations of aweXpect");
			}

			[Test]
			public async Task WhenAttributeDoesNotExist_ShouldNotVerifyItsValue()
			{
				XElement element = XElement.Parse("<item/>");

				async Task Act()
					=> await That(element).HasAttribute("price", value => value.IsEqualTo("1.00"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "price" whose value is equal to "1.00",
					             but it did not have attribute "price"

					             Actual:
					             <item />
					             """)
					.Because("the value of a missing attribute is not compared");
			}

			[Test]
			public async Task WhenCombinedWithAnd_ShouldVerifyAllAttributes()
			{
				XElement element = XElement.Parse("<item sku=\"A1\" price=\"2.00\"/>");

				async Task Act()
					=> await That(element)
						.HasAttribute("sku", value => value.IsEqualTo("A1")).And
						.HasAttribute("price", value => value.IsEqualTo("2.00")).And
						.DoesNotHaveAttribute("discount");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSecondOfCombinedValuesDiffers_ShouldNameItsAttribute()
			{
				XElement element = XElement.Parse("<item sku=\"A1\" price=\"2.00\"/>");

				async Task Act()
					=> await That(element)
						.HasAttribute("sku", value => value.IsEqualTo("A1")).And
						.HasAttribute("price", value => value.IsEqualTo("1.00"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "sku" whose value is equal to "A1" and has attribute "price" whose value is equal to "1.00",
					             but value of attribute "price" was "2.00", which differs*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNegatedAndValueMatches_ShouldFail()
			{
				XElement element = XElement.Parse("<item price=\"1.00\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasAttribute("price", value => value.IsEqualTo("1.00")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             does not have attribute "price" whose value is equal to "1.00",
					             but it did

					             Actual:
					             <item price="1.00" />
					             """);
			}

			[Test]
			[Arguments("<item price=\"2.00\"/>")]
			[Arguments("<item/>")]
			public async Task WhenNegatedAndValueDiffersOrAttributeIsMissing_ShouldSucceed(string subject)
			{
				XElement element = XElement.Parse(subject);

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasAttribute("price", value => value.IsEqualTo("1.00")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("track");

				async Task Act()
					=> await That(element).HasAttribute("title", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations");
			}

			[Test]
			public async Task WhenNameIsNullWithExpectations_ShouldThrowArgumentNullException()
			{
				XElement element = new("track");

				async Task Act()
					=> await That(element).HasAttribute(null!, value => value.IsEmpty());

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).HasAttribute("title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has attribute "title",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("track");

				async Task Act()
					=> await That(element).HasAttribute(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}
		}

		public sealed class WithOperatorTests
		{
			[Test]
			public async Task WhenAttributeDoesNotExist_ShouldFail()
			{
				XElement element = XElement.Parse("<track/>");

				async Task Act()
					=> await That(element).HasAttribute("id").EqualTo("7");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "id" equal to "7",
					             but it did not have attribute "id"

					             Actual:
					             <track />
					             """);
			}

			[Test]
			public async Task WhenCombinedWithAnd_ShouldContinueOnTheElement()
			{
				XElement element = XElement.Parse("<track id=\"7\" title=\"Let It Be\"/>");

				XElement result = await That(element)
					.HasAttribute("id").EqualTo("7").And
					.HasAttribute("title").StartingWith("Let");

				await That(result).IsSameAs(element);
			}

			[Test]
			public async Task WhenContaining_ShouldCountTheOccurrences()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("title").Containing("e").AtLeast(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "title" containing "e" at least 3 times,
					             but value of attribute "title" contained "e" twice in "Let It Be"

					             Actual:
					             <track title="Let It Be" />
					             """);
			}

			[Test]
			public async Task WhenEndingWith_IgnoringCase_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("title").EndingWith("BE").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEqualTo_AsWildcard_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("title").EqualTo("Let*").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				XElement element = XElement.Parse("<track id=\"7\"/>");

				async Task Act()
					=> await That(element).HasAttribute("id").EqualTo(null);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expected' value cannot be null.").AsPrefix()
					.Because("the value of an attribute is never null");
			}

			[Test]
			public async Task WhenNestedInHasElement_ShouldNameTheAttribute()
			{
				XElement element = XElement.Parse("<album><track id=\"8\"/></album>");

				async Task Act()
					=> await That(element).HasElement("track", track => track.HasAttribute("id").EqualTo("7"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has element "track" which has attribute "id" equal to "7",
					             but value of attribute "id" was "8", which differs at index 0:
					                ↓ (actual)
					               "8"
					               "7"
					                ↑ (expected)

					             Actual (element "track"):
					             <track id="8" />
					             """);
			}

			[Test]
			public async Task WhenNotStartingWith_AndValueStartsWithIt_ShouldFail()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("title").NotStartingWith("Let");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "title" not starting with "Let",
					             but value of attribute "title" was "Let It Be"

					             Actual:
					             <track title="Let It Be" />
					             """);
			}

			[Test]
			public async Task WhenOperatorIsCalledTwice_ShouldThrowInvalidOperationException()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).CompliesWith(it =>
					{
						XmlValueResult<XElement, IThat<XElement?>> result = it.HasAttribute("title");
						result.StartingWith("Let");
						result.EndingWith("Be");
					});

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("HasAttribute(name) accepts only one comparison, so call only one operator on its result.");
			}

			[Test]
			[Arguments("<track title=\"Let It Be\"/>")]
			[Arguments("<track/>")]
			public async Task WhenPatternIsInvalid_ShouldThrowArgumentException(string xml)
			{
				XElement element = XElement.Parse(xml);

				async Task Act()
					=> await That(element).HasAttribute("title").EqualTo("[").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix()
					.Because("an invalid pattern is rejected whether the attribute exists or not");
			}

			[Test]
			public async Task WhenStartingWith_AndValueDiffers_ShouldFail()
			{
				XElement element = XElement.Parse("<track title=\"Yesterday\"/>");

				async Task Act()
					=> await That(element).HasAttribute("title").StartingWith("Let");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has attribute "title" starting with "Let",
					             but value of attribute "title" was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)

					             Actual:
					             <track title="Yesterday" />
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).HasAttribute("id").EqualTo("7");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has attribute "id" equal to "7",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueIsEqual_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track id=\"7\"/>");

				async Task Act()
					=> await That(element).HasAttribute("id").EqualTo("7");

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Test]
			public async Task WhenAttributeDoesNotExist_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasAttribute("id").EqualTo("7"));

				await That(Act).DoesNotThrow()
					.Because("a missing attribute has no value equal to the expected value, like for HasAttribute(name)");
			}

			[Test]
			public async Task WhenOperatorIsNegated_ShouldCombineBothNegations()
			{
				XElement element = XElement.Parse("<track title=\"Yesterday\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasAttribute("title").NotStartingWith("Let"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             does not have attribute "title" not starting with "Let",
					             but value of attribute "title" was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)

					             Actual:
					             <track title="Yesterday" />
					             """);
			}

			[Test]
			public async Task WhenValueDiffers_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track id=\"8\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasAttribute("id").EqualTo("7"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueMatches_ShouldFail()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasAttribute("title").Containing("e").AtLeast(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             does not have attribute "title" containing "e" at least twice,
					             but value of attribute "title" contained "e" at least twice in "Let It Be"

					             Actual:
					             <track title="Let It Be" />
					             """);
			}
		}
	}

	public sealed class DoesNotHaveAttribute
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAttributeDoesNotExist_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).DoesNotHaveAttribute("artist");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAttributeExists_ShouldFail()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).DoesNotHaveAttribute("title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             does not have attribute "title",
					             but it did

					             Actual:
					             <track title="Let It Be" />
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveAttribute("title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have attribute "title",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("track");

				async Task Act()
					=> await That(element).DoesNotHaveAttribute(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}
		}
	}
}
