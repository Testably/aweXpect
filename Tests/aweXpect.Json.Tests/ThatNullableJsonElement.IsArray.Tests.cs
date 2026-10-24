using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class IsArray
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonElement? subject = FromString("[1, 2]");

				JsonElement result = await That(subject).IsArray();

				await That(result.GetRawText()).IsEqualTo("[1, 2]");
			}

			[Test]
			[Arguments("[]")]
			[Arguments("[1, 2]")]
			public async Task WhenJsonIsAnArray_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoArray_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an array,
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement?[] items = [FromString("[]"), FromString("{}"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsArray()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are an array for all items,
					             but only 1 of 2 were

					             Not matching items (Items):
					             [
					               {}
					             ]

					             Collection (Items):
					             [
					               [],
					               {}
					             ]
					             """)
					.Because("the items are the plural subject of the expectation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("[]")]
			[Arguments("[1, 2]")]
			public async Task WhenJsonIsAnArray_ShouldFail(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array,
					             but it was an array
					             """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("2")]
			[Arguments("\"foo\"")]
			public async Task WhenJsonIsNoArray_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WhoseElementsTests
		{
			[Test]
			public async Task AtLeast_WhenEnoughElementsComply_ShouldSucceed()
			{
				JsonElement? subject = FromString("[{\"title\": \"Hey Jude\"}, \"Yesterday\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.AtLeast(1).ComplyWith(t => t.IsObject());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewElementsComply_ShouldFail()
			{
				JsonElement? subject = FromString("[{\"title\": \"Hey Jude\"}, \"Yesterday\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.AtLeast(2).ComplyWith(t => t.IsObject());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements are an object for at least 2 items,
					             but only 1 of 2 were

					             Not matching items (elements):
					             [
					               "Yesterday"
					             ]

					             Collection (elements):
					             [
					               {"title":"Hey Jude"},
					               "Yesterday"
					             ]
					             """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoArray_ShouldOnlyFailWithIsArray(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an array whose elements have exactly 2 items,
					              but it was {kindString}
					              """)
					.Because("the elements are only accessed when the subject is an array");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldOnlyFailWithIsArray()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements have exactly 2 items,
					             but it was <null>
					             """)
					.Because("the elements are only accessed when the subject is an array");
			}
		}

		public sealed class WhoseElementsNegatedTests
		{
			[Test]
			public async Task WhenElementsMeetTheExpectation_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray().WhoseElements.HasCount(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array whose elements have exactly 2 items,
					             but it was an array
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray().WhoseElements.HasCount(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array whose elements have exactly 2 items,
					             but it was <null>
					             """)
					.Because("a null subject fails IsArray also when it is negated");
			}
		}
	}
}
