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

			[Fact]
			public async Task WhenPropertiesAreExpectedToHaveValueKinds_ShouldNegateThem()
			{
				JsonElement subject = FromString("{\"id\": 42, \"name\": \"foo\", \"active\": false, \"deletedAt\": null}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o
						.With("id").ANumber().And
						.With("name").AString().And
						.With("active").ABoolean().And
						.With("deletedAt").Null()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.id is not a number or $.name is not a string or $.active is not a boolean or $.deletedAt is not null,
					             but it was

					             Actual:
					             {
					               "id": 42,
					               "name": "foo",
					               "active": false,
					               "deletedAt": null
					             }
					             """);
			}

			[Fact]
			public async Task WhenPropertyHasAnotherValueKind_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"id\": 42, \"name\": \"foo\"}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o
						.With("id").ANumber().And
						.With("name").Null()));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsExpectedNotToExistAndDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.Without("bar")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.bar exists,
					             but it was

					             Actual:
					             {
					               "foo": 1
					             }
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsExpectedNotToExistButExists_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.Without("foo")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsExpectedNotToExistButIsNull_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.Without("foo"),
						o => o.TreatingNullAsAbsent()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.foo exists,
					             but it was

					             Actual:
					             {
					               "foo": null
					             }
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsExpectedToMatchNullButIsMissing_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.With("foo").Matching(null),
						o => o.TreatingNullAsAbsent()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.foo does not match null,
					             but it was

					             Actual:
					             {}
					             """);
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

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public async Task WhenMatchingArrayInDifferentOrder_ShouldFailUnlessIgnoringArrayOrder(bool ignoreArrayOrder)
			{
				JsonElement subject = FromString("{\"foo\": [1, 2]}");

				async Task Act()
					=> await That(subject).IsObject(
						o => o.With("foo").Matching(new[] { 2, 1, }),
						o => o.IgnoringArrayOrder(ignoreArrayOrder));

				await That(Act).Throws<XunitException>().OnlyIf(!ignoreArrayOrder)
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches new[] { 2, 1, },
					             but it differed as
					               $.foo[0] was 1 instead of 2 and
					               $.foo[1] was 2 instead of 1
					             """);
			}

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public async Task WhenMatchingArrayWithAdditionalElements_ShouldFailUnlessIgnoringAdditionalArrayElements(
				bool ignoreAdditionalArrayElements)
			{
				JsonElement subject = FromString("{\"foo\": [1, 2, 3]}");

				async Task Act()
					=> await That(subject).IsObject(
						o => o.With("foo").Matching(new[] { 1, 2, }),
						o => o.IgnoringAdditionalArrayElements(ignoreAdditionalArrayElements));

				await That(Act).Throws<XunitException>().OnlyIf(!ignoreAdditionalArrayElements)
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches new[] { 1, 2, },
					             but it differed as $.foo[2] had unexpected 3
					             """);
			}

			[Fact]
			public async Task WhenMatchingArrayWithAdditionalObject_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": [{\"bar\": 1, \"baz\": 2}, {\"bar\": 2}]}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new[] { new { bar = 1, }, }));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches new[] { new { bar = 1, }, },
					             but it differed as $.foo[1] had unexpected {"bar":2}
					             """)
					.Because("additional properties are ignored by default, but additional array elements are not");
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

			[Fact]
			public async Task WhenMatchingNullOnMissingProperty_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(null));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches null,
					             but it differed as property $.foo did not exist
					             """)
					.Because("by default a missing property differs from a property with a null value");
			}

			[Theory]
			[InlineData("{\"foo\": null}")]
			[InlineData("{}")]
			public async Task WhenMatchingNull_WhenTreatingNullAsAbsent_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(null),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMatchingValueOnNullProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 1,
					             but it differed as $.foo was <null> instead of 1
					             """);
			}

			[Theory]
			[InlineData("{\"foo\": null}")]
			[InlineData("{}")]
			public async Task WhenMatchingValue_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(1),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 1,
					             but it differed as property $.foo did not exist
					             """)
					.Because("a property with a null value is treated as absent");
			}

			[Fact]
			public async Task WhenMatchingItIsOnNullProperty_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(It.Is<string?>().That.IsNull()),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches It.Is<string?>().That.IsNull(),
					             but it differed as property $.foo did not exist
					             """)
					.Because("only an expected null value accepts an absent property");
			}

			[Theory]
			[InlineData("{\"foo\": null}")]
			[InlineData("{}")]
			public async Task WhenAnObject_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject(),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object,
					             but it differed as property $.foo did not exist
					             """);
			}

			[Fact]
			public async Task WhenAnObjectOnNullProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object,
					             but it differed as $.foo was null
					             """);
			}

			[Theory]
			[InlineData("{\"foo\": null}")]
			[InlineData("{}")]
			public async Task WhenAnObjectWithExpectation_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject(f => f.With("bar").Matching(1)),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object and $.foo.bar matches 1,
					             but it differed as property $.foo did not exist
					             """);
			}

			[Theory]
			[InlineData("{\"foo\": null}")]
			[InlineData("{}")]
			public async Task WhenAnArray_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnArray(a => a.With(0).Elements()),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array with 0 elements,
					             but it differed as property $.foo did not exist
					             """);
			}

			[Fact]
			public async Task WhenAbsentPropertyIsCombinedWithOtherExpectations_WhenTreatingNullAsAbsent_ShouldListAllFailuresInOrder()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": null, \"baz\": 3}");

				async Task Act()
					=> await That(subject).IsObject(o => o
							.With("foo").Matching(2).And
							.With("bar").Matching(true).And
							.With("baz").Matching(4),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2 and $.bar matches true and $.baz matches 4,
					             but it differed as
					               $.foo was 1 instead of 2 and
					               property $.bar did not exist and
					               $.baz was 3 instead of 4
					             """);
			}

			[Fact]
			public async Task WhenMatchingNullInNestedObject_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": 1}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject(f => f.With("baz").Matching(null)),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMatchingObjectWithNullProperty_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": 1}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new
						{
							bar = 1,
							baz = (int?)null,
						}),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow()
					.Because("the option also applies to the comparison of the matched value");
			}

			[Fact]
			public async Task WhenMatchingObjectWithNullProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": 1}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new
					{
						bar = 1,
						baz = (int?)null,
					}));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches new
					             {
					             	bar = 1,
					             	baz = (int?)null,
					             },
					             but it differed as $.foo.baz was missing
					             """);
			}
		}

		public sealed class WithValueKindTests
		{
			[Theory]
			[InlineData("true")]
			[InlineData("false")]
			public async Task WhenPropertiesHaveTheExpectedValueKinds_ShouldSucceed(string active)
			{
				JsonElement subject = FromString(
					$"{{\"id\": 42, \"name\": \"foo\", \"active\": {active}, \"deletedAt\": null}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("id").ANumber().And
						.With("name").AString().And
						.With("active").ABoolean().And
						.With("deletedAt").Null());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesHaveOtherValueKinds_ShouldListAllFailuresInOrder()
			{
				JsonElement subject = FromString(
					"{\"id\": \"42\", \"name\": 1, \"active\": \"yes\", \"deletedAt\": false}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("id").ANumber().And
						.With("name").AString().And
						.With("active").ABoolean().And
						.With("deletedAt").Null());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.id is a number and $.name is a string and $.active is a boolean and $.deletedAt is null,
					             but it differed as
					               $.id was a string and
					               $.name was a number and
					               $.active was a string and
					               $.deletedAt was false
					             """);
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("2", "a number")]
			[InlineData("true", "true")]
			[InlineData("false", "false")]
			[InlineData("null", "null")]
			public async Task WhenPropertyIsNoString_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"foo\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AString());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is a string,
					              but it differed as $.foo was {kindString}
					              """);
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("\"2\"", "a string")]
			[InlineData("true", "true")]
			[InlineData("false", "false")]
			[InlineData("null", "null")]
			public async Task WhenPropertyIsNoNumber_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"foo\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").ANumber());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is a number,
					              but it differed as $.foo was {kindString}
					              """);
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("1", "a number")]
			[InlineData("\"true\"", "a string")]
			[InlineData("null", "null")]
			public async Task WhenPropertyIsNoBoolean_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"foo\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").ABoolean());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is a boolean,
					              but it differed as $.foo was {kindString}
					              """);
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("0", "a number")]
			[InlineData("\"null\"", "a string")]
			[InlineData("true", "true")]
			[InlineData("false", "false")]
			public async Task WhenPropertyIsNotNull_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"foo\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Null());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is null,
					              but it differed as $.foo was {kindString}
					              """);
			}

			[Fact]
			public async Task WhenPropertyDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("bar").Null());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.bar is null,
					             but it differed as property $.bar did not exist
					             """)
					.Because("a missing property is not a property with a null value");
			}

			[Theory]
			[InlineData("{\"foo\": null}")]
			[InlineData("{}")]
			public async Task WhenNull_WhenTreatingNullAsAbsent_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Null(), o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow()
					.Because("an absent property is accepted when it is expected to be null");
			}

			[Theory]
			[InlineData("{\"foo\": null}")]
			[InlineData("{}")]
			public async Task WhenAString_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AString(), o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is a string,
					             but it differed as property $.foo did not exist
					             """);
			}

			[Fact]
			public async Task WhenNestedPropertyHasAnotherValueKind_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": {\"baz\": \"1\"}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnObject(f => f
							.With("bar").AnObject(b => b
								.With("baz").ANumber())));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object and $.foo.bar is an object and $.foo.bar.baz is a number,
					             but it differed as $.foo.bar.baz was a string
					             """);
			}

			[Theory]
			[InlineData("[]", "an array")]
			[InlineData("\"foo\"", "a string")]
			public async Task WhenJsonIsNoObject_ShouldOnlyReportTheKind(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AString());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is a string,
					              but it was {kindString}
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

			[Fact]
			public async Task WhenPropertyIsNull_ShouldCountIt()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With(1).Properties());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object with 1 property,
					             but it differed as $ had 2 properties
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldNotCountIt(int expected, bool isMatch)
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With(expected).Properties(), o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage("""
					             Expected that subject
					             is an object with 2 properties,
					             but it differed as $ had 1 property
					             """);
			}
		}

		public sealed class WithoutTests
		{
			[Fact]
			public async Task WhenCombinedWithOtherExpectations_ShouldListAllFailuresInOrder()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2, \"baz\": 3}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").Matching(2).And
						.Without("bar").And
						.With("baz").Matching(4));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2 and $.bar does not exist and $.baz matches 4,
					             but it differed as
					               $.foo was 1 instead of 2 and
					               $.bar was 2 and
					               $.baz was 3 instead of 4
					             """);
			}

			[Theory]
			[InlineData("[]", "an array")]
			[InlineData("2", "a number")]
			public async Task WhenJsonIsNoObject_ShouldOnlyReportTheKind(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo does not exist,
					              but it was {kindString}
					              """);
			}

			[Fact]
			public async Task WhenNestedObjectContainsProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"address\": {\"street\": \"Main\", \"internalId\": 42}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("address").AnObject(a => a.Without("internalId")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.address is an object and $.address.internalId does not exist,
					             but it differed as $.address.internalId was 42
					             """);
			}

			[Fact]
			public async Task WhenNestedObjectDoesNotContainProperty_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"address\": {\"street\": \"Main\"}, \"internalId\": 42}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("address").AnObject(a => a.Without("internalId")));

				await That(Act).DoesNotThrow()
					.Because("only the property of the nested object is considered");
			}

			[Fact]
			public async Task WhenNestedPropertyIsNoObject_ShouldOnlyReportTheKind()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnObject(a => a.Without("bar")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object and $.foo.bar does not exist,
					             but it differed as $.foo was a number
					             """);
			}

			[Fact]
			public async Task WhenPropertyDiffersInCase_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"Foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).DoesNotThrow()
					.Because("property names in JSON are case-sensitive");
			}

			[Fact]
			public async Task WhenPropertyDoesNotExist_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("bar"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyExists_ShouldFail()
			{
				JsonElement subject = FromString("{\"name\": \"John\", \"passwordHash\": \"8f14e4\"}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("name").Matching("John").And
						.Without("passwordHash"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.name matches "John" and $.passwordHash does not exist,
					             but it differed as $.passwordHash was "8f14e4"
					             """);
			}

			[Fact]
			public async Task WhenPropertyExistsMultipleTimes_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"foo\": 2}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo does not exist,
					             but it differed as $.foo was 2
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsAnObject_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": [1, 2]}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo does not exist,
					             but it differed as $.foo was {"bar":[1,2]}
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo does not exist,
					             but it differed as $.foo was <null>
					             """)
					.Because("a property with a null value exists");
			}

			[Theory]
			[InlineData("{\"foo\": null}")]
			[InlineData("{}")]
			public async Task WhenPropertyIsNullOrMissing_WhenTreatingNullAsAbsent_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"), o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyHasValue_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"), o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo does not exist,
					             but it differed as $.foo was 1
					             """);
			}

			[Fact]
			public async Task WhenNestedPropertyIsNull_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"address\": {\"street\": \"Main\", \"internalId\": null}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
							.With("address").AnObject(a => a.Without("internalId")),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyNameContainsSpecialCharacters_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo.bar\": 1, \"foo\": {\"bar\": 2}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo.bar"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo.bar does not exist,
					             but it differed as $.foo.bar was 1
					             """)
					.Because("the property name is no path");
			}

			[Fact]
			public async Task WhenPropertyNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without(null!));

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("propertyName").And
					.WithMessage("The 'propertyName' cannot be null.*").AsWildcard()
					.Because("an invalid argument is thrown as it is instead of failing the expectation");
			}
		}
	}
}
