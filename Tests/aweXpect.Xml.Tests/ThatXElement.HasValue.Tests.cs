using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Xml.Results;

namespace aweXpect.Xml.Tests;

public sealed partial class ThatXElement
{
	public sealed class HasValue
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCombinedWithHasElement_ShouldVerifyTheValueOfTheSubject()
			{
				XElement element = XElement.Parse("<p>a<b>x</b>c</p>");

				async Task Act()
					=> await That(element).HasElement("b").And.HasValue(value => value.IsEqualTo("axc"));

				await That(Act).DoesNotThrow()
					.Because("the value is taken from the subject, not from the child element of a previous expectation");
			}

			[Test]
			public async Task WhenElementContainsCData_ShouldIncludeItsText()
			{
				XElement element = XElement.Parse("<script>if (<![CDATA[a < b]]>) {}</script>");

				async Task Act()
					=> await That(element).HasValue(value => value.IsEqualTo("if (a < b) {}"));

				await That(Act).DoesNotThrow()
					.Because("the text of CDATA sections is part of the value, like for XElement.Value");
			}

			[Test]
			public async Task WhenElementHasMixedContent_ShouldConcatenateTheTextOfAllDescendants()
			{
				XElement element = XElement.Parse("<p>a<b>x</b>c</p>");

				async Task Act()
					=> await That(element).HasValue(value => value.IsEqualTo("ac"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has a value which is equal to "ac",
					             but value of the element was "axc", which differs at index 1:
					                 ↓ (actual)
					               "axc"
					               "ac"
					                 ↑ (expected)
					             """)
					.Because("the value contains the text of the child elements, like XElement.Value");
			}

			[Test]
			public async Task WhenElementIsEmpty_ShouldHaveAnEmptyValue()
			{
				XElement element = XElement.Parse("<title><!-- none --></title>");

				async Task Act()
					=> await That(element).HasValue(value => value.IsEmpty());

				await That(Act).DoesNotThrow()
					.Because("an element without text has an empty value instead of null");
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				XElement element = new("title");

				async Task Act()
					=> await That(element).HasValue(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNegatedAndValueDiffers_ShouldSucceed()
			{
				XElement element = XElement.Parse("<title>Yesterday</title>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasValue(value => value.IsEqualTo("Let It Be")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedAndValueMatches_ShouldFail()
			{
				XElement element = XElement.Parse("<title>Let It Be</title>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasValue(value => value.IsEqualTo("Let It Be")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             does not have a value which is equal to "Let It Be",
					             but it did

					             Actual:
					             <title>Let It Be</title>
					             """);
			}

			[Test]
			public async Task WhenNestedInHasElement_ShouldVerifyTheValueOfTheChildElement()
			{
				XElement element = XElement.Parse("<track><title>Yesterday</title></track>");

				async Task Act()
					=> await That(element).HasElement("title", title => title.HasValue(value => value.StartsWith("Let")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has element "title" which has a value which starts with "Let",
					             but value of the element was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).HasValue(value => value.IsEqualTo("Let It Be"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a value which is equal to "Let It Be",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotSatisfyTheExpectation_ShouldFail()
			{
				XElement element = XElement.Parse("<title>Yesterday</title>");

				async Task Act()
					=> await That(element).HasValue(value => value.IsEqualTo("Let It Be"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has a value which is equal to "Let It Be",
					             but value of the element was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let It Be"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenValueSatisfiesTheExpectation_ShouldSucceed()
			{
				XElement element = XElement.Parse("<title>Let It Be</title>");

				async Task Act()
					=> await That(element).HasValue(value => value.StartsWith("Let"));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithOperatorTests
		{
			[Test]
			public async Task WhenContaining_ShouldCountTheOccurrences()
			{
				XElement element = XElement.Parse("<title>Let It Be</title>");

				async Task Act()
					=> await That(element).HasValue().Containing("e").AtLeast(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has a value containing "e" at least 3 times,
					             but value of the element contained "e" twice in "Let It Be"

					             Actual:
					             <title>Let It Be</title>
					             """);
			}

			[Test]
			public async Task WhenElementHasMixedContent_ShouldCompareTheTextOfAllDescendants()
			{
				XElement element = XElement.Parse("<p>a<b>x</b>c</p>");

				async Task Act()
					=> await That(element).HasValue().EqualTo("axc");

				await That(Act).DoesNotThrow()
					.Because("the value contains the text of the child elements, like XElement.Value");
			}

			[Test]
			public async Task WhenEqualTo_IgnoringCase_ShouldSucceed()
			{
				XElement element = XElement.Parse("<title>Let It Be</title>");

				async Task Act()
					=> await That(element).HasValue().EqualTo("LET IT BE").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				XElement element = XElement.Parse("<title>Let It Be</title>");

				async Task Act()
					=> await That(element).HasValue().StartingWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expected' value cannot be null.").AsPrefix()
					.Because("the value of an element is never null");
			}

			[Test]
			public async Task WhenNestedInHasElement_ShouldVerifyTheValueOfTheChildElement()
			{
				XElement element = XElement.Parse("<track><title>Yesterday</title></track>");

				async Task Act()
					=> await That(element).HasElement("title", title => title.HasValue().StartingWith("Let"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has element "title" which has a value starting with "Let",
					             but value of the element was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)

					             Actual (element "title"):
					             <title>Yesterday</title>
					             """);
			}

			[Test]
			public async Task WhenOperatorIsCalledTwice_ShouldThrowInvalidOperationException()
			{
				XElement element = XElement.Parse("<title>Let It Be</title>");

				async Task Act()
					=> await That(element).CompliesWith(it =>
					{
						XmlValueResult<XElement, IThat<XElement?>> result = it.HasValue();
						result.StartingWith("Let");
						result.EndingWith("Be");
					});

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("HasValue() accepts only one comparison, so call only one operator on its result.");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				XElement? subject = null;

				async Task Act()
					=> await That(subject).HasValue().EqualTo("Let It Be");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a value equal to "Let It Be",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueDiffers_ShouldFail()
			{
				XElement element = XElement.Parse("<title>Yesterday</title>");

				async Task Act()
					=> await That(element).HasValue().EqualTo("Let It Be");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             has a value equal to "Let It Be",
					             but value of the element was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let It Be"
					                ↑ (expected)

					             Actual:
					             <title>Yesterday</title>
					             """);
			}

			[Test]
			public async Task WhenValueMatches_ShouldContinueOnTheElement()
			{
				XElement element = XElement.Parse("<title>Let It Be</title>");

				XElement result = await That(element).HasValue().StartingWith("Let").And.HasValue().EndingWith("Be");

				await That(result).IsSameAs(element);
			}

			[Test]
			public async Task WithoutOperator_ShouldSucceed()
			{
				XElement element = XElement.Parse("<title><!-- none --></title>");

				async Task Act()
					=> await That(element).HasValue();

				await That(Act).DoesNotThrow()
					.Because("every element has a value, which is empty without text");
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Test]
			public async Task WhenOperatorIsNegated_ShouldCombineBothNegations()
			{
				XElement element = XElement.Parse("<title>Yesterday</title>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasValue().NotEqualTo("Let It Be"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             does not have a value not equal to "Let It Be",
					             but value of the element was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let It Be"
					                ↑ (expected)

					             Actual:
					             <title>Yesterday</title>
					             """);
			}

			[Test]
			public async Task WhenValueDiffers_ShouldSucceed()
			{
				XElement element = XElement.Parse("<title>Yesterday</title>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasValue().EqualTo("Let It Be"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueMatches_ShouldFail()
			{
				XElement element = XElement.Parse("<title>Let It Be</title>");

				async Task Act()
					=> await That(element).DoesNotComplyWith(it => it.HasValue().EqualTo("Let It Be"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that element
					             does not have a value equal to "Let It Be",
					             but value of the element was "Let It Be"

					             Actual:
					             <title>Let It Be</title>
					             """);
			}
		}
	}
}
