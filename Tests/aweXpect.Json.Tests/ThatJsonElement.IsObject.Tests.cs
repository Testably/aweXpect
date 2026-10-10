using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsObject
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectationIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectation").And
					.WithMessage("The 'expectation' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectationThrows_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(_ => throw new InvalidOperationException("Yesterday"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object,
					             but the expectation did throw an InvalidOperationException:
					               Yesterday
					             """);
			}

			[Fact]
			public async Task WhenNestedExpectationThrows_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": {}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnObject(_ => throw new InvalidOperationException("Yesterday")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object,
					             but the expectation did throw an InvalidOperationException:
					               Yesterday
					             """);
			}

			[Theory]
			[InlineData("{}")]
			[InlineData("{\"foo\": 1}")]
			public async Task WhenJsonIsAnObject_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("[]", "an array")]
			[InlineData("2", "a number")]
			[InlineData("\"foo\"", "a string")]
			public async Task WhenJsonIsNoObject_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object,
					              but it was {kindString}
					              """);
			}

			[Theory]
			[InlineData("[]", "an array")]
			[InlineData("2", "a number")]
			[InlineData("\"foo\"", "a string")]
			public async Task WhenJsonIsNoObject_WithExpectations_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(true));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo matches true,
					              but it was {kindString}
					              """);
			}

			[Fact]
			public async Task WhenMemberIsPlural_WithExpectation_ShouldUsePluralFormInResult()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await WhosePluralItems(That(subject), items => items.IsObject(o => o.With("foo").Matching(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Items are objects and $.foo matches 1,
					             but Items were an array
					             """)
					.Because("the plural member is the subject of the result");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsExpectedToBeAnArrayWithoutExpectation_ShouldNegateIt()
			{
				JsonElement subject = FromString("{\"bar\": []}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.With("bar").AnArray()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.bar is not an array,
					             but it was

					             Actual:
					             {
					               "bar": []
					             }
					             """);
			}

			[Fact]
			public async Task IsObject_ShouldBeChainable()
			{
				string json = """
				              {
				                "foo": 21,
				                "bar": []
				              }
				              """;
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsObject(o
							=> o.With(2).Properties().And.With("foo").Matching(21).And.With("bar")
								.AnArray(a => a.With(0).Elements())));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or not with 2 properties or $.foo does not match 21 or $.bar is not an array or not with 0 elements,
					             but it was

					             Actual:
					             {
					               "foo": 21,
					               "bar": []
					             }
					             """);
			}

			[Theory]
			[InlineData("{}")]
			[InlineData("{\"foo\": 1}")]
			public async Task WhenJsonIsAnObject_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an object,
					             but it was
					             """);
			}

			[Fact]
			public async Task WhenExpectationThrows_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsObject(_ => throw new InvalidOperationException("Yesterday")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an object,
					             but the expectation did throw an InvalidOperationException:
					               Yesterday
					             """)
					.Because("an expectation that throws answered nothing, so its negation must not be met either");
			}

			[Theory]
			[InlineData("[]")]
			[InlineData("2")]
			[InlineData("\"foo\"")]
			public async Task WhenJsonIsNoObject_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithTests
		{
			[Fact]
			public async Task WhenNestedInThatAll_ShouldIndentTheFailures()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2}");

				async Task Act()
					=> await ThatAll(That(subject).IsObject(o => o.With("foo").Matching(2).With("bar").Matching(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected all of the following to succeed:
					              [01] Expected that subject is an object and $.foo matches 2 and $.bar matches 1
					             but
					              [01] it differed as
					                     $.foo was 1 instead of 2 and
					                     $.bar was 2 instead of 1
					             """)
					.Because("every line of the failures belongs to the nested result");
			}

			[Fact]
			public async Task WhenPropertyNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With(null!).Matching(1));

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("propertyName").And
					.WithMessage("The 'propertyName' cannot be null.*").AsWildcard()
					.Because("an invalid argument is thrown as it is instead of failing the expectation");
			}

			[Fact]
			public async Task WhenMatchFails_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(
						o => o.With("foo").Matching(2),
						o => o.IgnoringAdditionalProperties());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2,
					             but it differed as $.foo was 1 instead of 2
					             """);
			}

			[Fact]
			public async Task WhenMatchingItIsOnNullProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(It.Is<string>().That.StartsWith("a")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches It.Is<string>().That.StartsWith("a"),
					             but it differed as $.foo was <null>
					             """)
					.Because("a null value has no content that could start with the prefix");
			}

			[Fact]
			public async Task WhenMatchingItIsWaitsLongerThanTheTimeout_ShouldStopAtTheTimeout()
			{
				JsonElement subject = FromString("{\"foo\": \"bar\"}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo")
							.Matching(It.Is<string>().That.Satisfies(_ => false).Within(TimeSpan.FromSeconds(30))))
						.WithTimeout(TimeSpan.FromMilliseconds(100));

				await That(Act).Throws<XunitException>().Within(TimeSpan.FromSeconds(10))
					.WithMessage("*but it did not finish within 0:00.100").AsWildcard()
					.Because("the nested expectation must be canceled by the timeout of the evaluation");
			}

			[Fact]
			public async Task WhenNestedMatchingItIsWaits_ShouldNotBlockTheCaller()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": \"baz\"}}");
				bool isSatisfied = false;

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject(f => f.With("bar")
						.Matching(It.Is<string>().That.Satisfies(_ => isSatisfied).Within(TimeSpan.FromSeconds(5)))));

				Task evaluation = Act();
				isSatisfied = true;

				await That(() => evaluation).DoesNotThrow()
					.Because("the evaluation must return to the caller while it waits instead of blocking it");
			}

			[Fact]
			public async Task WhenMatchSucceeds_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": 2}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(2));


				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMultipleMatchesFail_ShouldListAllFailures()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(2).With("bar").Matching(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2 and $.bar matches 1,
					             but it differed as
					               $.foo was 1 instead of 2 and
					               $.bar was 2 instead of 1
					             """);
			}

			[Fact]
			public async Task WhenNestedMatchesFail_ShouldListAllFailures()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": {\"baz\": 1, \"bat\": 2}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").Matching(2).And
						.With("bar").AnObject(p => p
							.With("baz").Matching(3).And
							.With("bat").Matching(3)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2 and $.bar is an object and $.bar.baz matches 3 and $.bar.bat matches 3,
					             but it differed as
					               $.foo was 1 instead of 2 and
					               $.bar.baz was 1 instead of 3 and
					               $.bar.bat was 2 instead of 3
					             """);
			}

			[Fact]
			public async Task WhenPropertyDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("bar").Matching(true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.bar matches true,
					             but it differed as property $.bar did not exist
					             """);
			}
		}

		public sealed class WithNumberOfPropertiesTests
		{
			[Fact]
			public async Task WhenNull_ShouldNotAddFailureMessage()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnObject(a => a
							.With(1).Properties()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object with 1 property,
					             but it differed as property $.foo did not exist
					             """);
			}

			[Fact]
			public async Task WhenNumberDiffers_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With(3).Properties());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object with 3 properties,
					             but it differed as $ had 2 properties
					             """);
			}

			[Theory]
			[InlineData("{}", 0)]
			[InlineData("{\"foo\": 1}", 1)]
			[InlineData("{\"foo\": 1, \"bar\": 2, \"baz\": 3}", 3)]
			public async Task WhenNumberMatches_ShouldSucceed(string json, int expected)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With(expected).Properties());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
