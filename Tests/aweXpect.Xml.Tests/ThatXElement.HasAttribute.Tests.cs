using System.Xml.Linq;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXElement
{
	public sealed class HasAttribute
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAttributeExists_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("title");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAttributeDoesNotExist_ShouldFail()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("artist");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has attribute "artist",
					             but it did not have attribute "artist"
					             """);
			}

			[Fact]
			public async Task WhenAttributeIsInAnotherNamespace_ShouldFail()
			{
				XElement element = XElement.Parse("<track xmlns:m=\"urn:meta\" m:title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).HasAttribute("title");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has attribute "title",
					             but it did not have attribute "title"
					             """)
					.Because("the attribute is looked up by its namespace and local name, like by XElement.Attribute");
			}

			[Fact]
			public async Task WhenAttributeIsInTheSpecifiedNamespace_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track xmlns:m=\"urn:meta\" m:title=\"Let It Be\"/>");
				XNamespace meta = "urn:meta";

				async Task Act()
					=> await That(element).HasAttribute(meta + "title", value => value.IsEqualTo("Let It Be"));

				await That(Act).DoesNotThrow()
					.Because("the prefix of the attribute is not compared");
			}

			[Fact]
			public async Task WhenAttributeInNamespaceDoesNotExist_ShouldNameTheNamespace()
			{
				XElement element = XElement.Parse("<track/>");
				XNamespace meta = "urn:meta";

				async Task Act()
					=> await That(element).HasAttribute(meta + "title");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has attribute "{urn:meta}title",
					             but it did not have attribute "{urn:meta}title"
					             """);
			}

			[Fact]
			public async Task WhenValueSatisfiesTheExpectation_ShouldSucceed()
			{
				XElement element = XElement.Parse("<item price=\"1.00\"/>");

				async Task Act()
					=> await That(element).HasAttribute("price", value => value.StartsWith("1."));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueDoesNotSatisfyTheExpectation_ShouldFail()
			{
				XElement element = XElement.Parse("<item price=\"2.00\"/>");

				async Task Act()
					=> await That(element).HasAttribute("price", value => value.IsEqualTo("1.00"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has attribute "price" whose value is equal to "1.00",
					             but value of attribute "price" was "2.00", which differs*
					             """).AsWildcard()
					.Because("the value is verified with the string expectations of aweXpect");
			}

			[Fact]
			public async Task WhenAttributeDoesNotExist_ShouldNotVerifyItsValue()
			{
				XElement element = XElement.Parse("<item/>");

				async Task Act()
					=> await That(element).HasAttribute("price", value => value.IsEqualTo("1.00"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has attribute "price" whose value is equal to "1.00",
					             but it did not have attribute "price"
					             """)
					.Because("the value of a missing attribute is not compared");
			}

			[Fact]
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

			[Fact]
			public async Task WhenSecondOfCombinedValuesDiffers_ShouldNameItsAttribute()
			{
				XElement element = XElement.Parse("<item sku=\"A1\" price=\"2.00\"/>");

				async Task Act()
					=> await That(element)
						.HasAttribute("sku", value => value.IsEqualTo("A1")).And
						.HasAttribute("price", value => value.IsEqualTo("1.00"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             has attribute "sku" whose value is equal to "A1" and has attribute "price" whose value is equal to "1.00",
					             but value of attribute "price" was "2.00", which differs*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNegatedAndValueMatches_ShouldFail()
			{
				XElement element = XElement.Parse("<item price=\"1.00\"/>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasAttribute("price", value => value.IsEqualTo("1.00")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             does not have attribute "price" whose value is equal to "1.00",
					             but it did
					             """);
			}

			[Theory]
			[InlineData("<item price=\"2.00\"/>")]
			[InlineData("<item/>")]
			public async Task WhenNegatedAndValueDiffersOrAttributeIsMissing_ShouldSucceed(string subject)
			{
				XElement element = XElement.Parse(subject);

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasAttribute("price", value => value.IsEqualTo("1.00")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("track");

				async Task Act()
					=> await That(element).HasAttribute("title", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations");
			}

			[Fact]
			public async Task WhenNameIsNullWithExpectations_ShouldThrowArgumentNullException()
			{
				XElement element = new("track");

				async Task Act()
					=> await That(element).HasAttribute(null!, value => value.IsEmpty());

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).HasAttribute("title");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has attribute "title",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("track");

				async Task Act()
					=> await That(element).HasAttribute(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}
		}
	}

	public sealed class DoesNotHaveAttribute
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAttributeDoesNotExist_ShouldSucceed()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).DoesNotHaveAttribute("artist");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAttributeExists_ShouldFail()
			{
				XElement element = XElement.Parse("<track title=\"Let It Be\"/>");

				async Task Act()
					=> await That(element).DoesNotHaveAttribute("title");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that element
					             does not have attribute "title",
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveAttribute("title");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have attribute "title",
					             but it was <null>
					             """);
			}

			[Fact]
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
