namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonString
{
	public sealed class IsValidJsonMatchingExactly
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("true", true, true)]
			[InlineData("true", false, false)]
			[InlineData("false", true, false)]
			[InlineData("false", false, true)]
			public async Task BooleanValue_ShouldSucceedWhenMatching(string subject, bool expected, bool isMatch)
			{
				string subjectString = subject == "true" ? "True" : "False";

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              is valid JSON which matches expected exactly,
					              but it differed as $ was {subjectString} instead of {expected}
					              """);
			}

			[Theory]
			[InlineData("42.1", 42.1, true)]
			[InlineData("1.2", 2.1, false)]
			public async Task DoubleValue_ShouldSucceedWhenMatching(string subject, double expected, bool isMatch)
			{
				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              is valid JSON which matches expected exactly,
					              but it differed as $ was {subject} instead of *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData("42", 42, true)]
			[InlineData("1", 2, false)]
			public async Task IntegerValue_ShouldSucceedWhenMatching(string subject, int expected, bool isMatch)
			{
				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              is valid JSON which matches expected exactly,
					              but it differed as $ was {subject} instead of {expected}
					              """);
			}

			[Theory]
			[InlineData("null", true)]
			[InlineData("{}", false)]
			public async Task NullValue_ShouldSucceedWhenMatching(string subject, bool isMatch)
			{
				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(null);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              is valid JSON which matches null exactly,
					              but it differed as $ was object {subject} instead of Null
					              """);
			}

			[Theory]
			[InlineData("\"foo\"", "foo", true)]
			[InlineData("\"foo\"", "bar", false)]
			public async Task StringValue_ShouldSucceedWhenMatching(string subject, string expected, bool isMatch)
			{
				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              is valid JSON which matches expected exactly,
					              but it differed as $ was {subject} instead of "{expected}"
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches null exactly,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNullJson_WhenMatchingAgainstNull_ShouldSucceed()
			{
				string? subject = "null";

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(null);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ArrayTests
		{
			[Theory]
			[MemberData(nameof(MatchingArrayValues))]
			public async Task MatchingValues_ShouldSucceed(string[] expected, string subject)
			{
				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NotMatchingArrayValues))]
			public async Task NotMatchingValues_ShouldFail(string[] expected, string subject, string errorMessage)
			{
				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is valid JSON which matches expected exactly,
					              but it differed {errorMessage}
					              """);
			}

			[Fact]
			public async Task WhenElementsAreInDifferentOrder_ShouldFail()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly([2, 1,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches [2, 1,] exactly,
					             but it differed as
					               $[0] was 1 instead of 2 and
					               $[1] was 2 instead of 1
					             """);
			}

			[Fact]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly([1, 2, 3,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches [1, 2, 3,] exactly,
					             but it differed as $[2] had missing 3
					             """);
			}

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_ShouldFail()
			{
				string subject = "[1, 2, 3]";

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly([1, 2,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches [1, 2,] exactly,
					             but it differed as $[2] had unexpected 3
					             """);
			}

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringAdditionalProperties_ShouldSucceed()
			{
				string subject = "[1, 2, 3]";

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly([1, 2,], o => o.IgnoringAdditionalProperties());

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<string[], string> MatchingArrayValues
				=> new()
				{
					{
						[], "[]"
					},
					{
						[
							"foo", "bar",
						],
						"[\"foo\", \"bar\"]"
					},
				};

			public static TheoryData<string[], string, string> NotMatchingArrayValues
				=> new()
				{
					{
						[], "[\"foo\"]", "as $[0] had unexpected \"foo\""
					},
					{
						[], "[\"1\", \"2\"]", """
						                      as
						                        $[0] had unexpected "1" and
						                        $[1] had unexpected "2"
						                      """
					},
					{
						[
							"foo",
						],
						"[]", "as $[0] had missing \"foo\""
					},
					{
						[
							"bar", "foo",
						],
						"[\"foo\", \"bar\"]", """
						                      as
						                        $[0] was "foo" instead of "bar" and
						                        $[1] was "bar" instead of "foo"
						                      """
					},
				};
		}

		public sealed class ObjectTests
		{
			[Theory]
			[InlineData("{}", "$.foo was missing")]
			[InlineData("{\"foo\": 2}")]
			public async Task ShouldFailIfPropertyIsMissing(string subject,
				string? errorMessage = null)
			{
				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(new
					{
						foo = 2,
					});

				await That(Act).Throws<XunitException>().OnlyIf(errorMessage != null)
					.WithMessage($$"""
					               Expected that subject
					               is valid JSON which matches new
					               					{
					               						foo = 2,
					               					} exactly,
					               but it differed as {{errorMessage}}
					               """);
			}

			[Theory]
			[InlineData("{}", true)]
			[InlineData("{\"foo\": 1}", false)]
			public async Task WhenExpectedIsEmpty_ShouldSucceedWhenJsonIsEmptyObject(string subject, bool isMatch)
			{
				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(new object());

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches new object() exactly,
					             but it differed as $.foo had unexpected 1
					             """);
			}

			[Fact]
			public async Task WhenPropertyHasDifferentValue_ShouldFail()
			{
				string subject = "{\"bar\": 2}";

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(new
					{
						bar = 3,
					});

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches new
					             					{
					             						bar = 3,
					             					} exactly,
					             but it differed as $.bar was 2 instead of 3
					             """);
			}

			[Fact]
			public async Task WhenSubjectHasAdditionalProperties_ShouldFail()
			{
				string subject = "{\"foo\": null, \"bar\": 2}";

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(new
					{
						bar = 2,
					});

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches new
					             					{
					             						bar = 2,
					             					} exactly,
					             but it differed as $.foo had unexpected Null
					             """);
			}

			[Fact]
			public async Task WhenSubjectHasAdditionalProperties_WhenIgnoringAdditionalProperties_ShouldSucceed()
			{
				string subject = "{\"foo\": null, \"bar\": 2}";

				async Task Act()
					=> await That(subject).IsValidJsonMatchingExactly(new
					{
						bar = 2,
					}, o => o.IgnoringAdditionalProperties());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
