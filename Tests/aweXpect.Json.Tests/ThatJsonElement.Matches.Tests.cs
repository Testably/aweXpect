using System.Text.Json;
using aweXpect.Json;

namespace aweXpect.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class Matches
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("true", true, true)]
			[InlineData("true", false, false)]
			[InlineData("false", true, false)]
			[InlineData("false", false, true)]
			public async Task BooleanValue_ShouldSucceedWhenMatching(string json, bool expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected,
					              but it differed as $ was {subject} instead of {expected}
					              """);
			}

			[Theory]
			[InlineData("42.1", 42.1, true)]
			[InlineData("1.2", 2.1, false)]
			public async Task DoubleValue_ShouldSucceedWhenMatching(string json, double expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected,
					              but it differed as $ was {json} instead of *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData("42", 42, true)]
			[InlineData("1", 2, false)]
			public async Task IntegerValue_ShouldSucceedWhenMatching(string json, int expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected,
					              but it differed as $ was {json} instead of {expected}
					              """);
			}

			[Theory]
			[InlineData("null", true)]
			[InlineData("{}", false)]
			public async Task NullValue_ShouldSucceedWhenMatching(string json, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(null);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches null,
					              but it differed as $ was object {json} instead of Null
					              """);
			}

			[Theory]
			[InlineData("\"foo\"", "foo", true)]
			[InlineData("\"foo\"", "bar", false)]
			public async Task StringValue_ShouldSucceedWhenMatching(string json, string expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected, o => o.IgnoringAdditionalProperties());

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected,
					              but it differed as $ was {json} instead of "{expected}"
					              """);
			}
		}

		public sealed class ArrayTests
		{
			[Theory]
			[MemberData(nameof(MatchingArrayValues))]
			public async Task MatchingValues_ShouldSucceed(object expected, string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NotMatchingArrayValues))]
			public async Task NotMatchingValues_ShouldFail(object expected, string json, string errorMessage)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches expected,
					              but it differed {errorMessage}
					              """);
			}

			[Fact]
			public async Task WhenElementsAreInDifferentOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).Matches([2, 1]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [2, 1],
					             but it differed as
					               $[0] was 1 instead of 2 and
					               $[1] was 2 instead of 1
					             """);
			}

			[Fact]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).Matches([1, 2, 3]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2, 3],
					             but it differed as $[2] had missing 3
					             """);
			}

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).Matches([1, 2], o => o.IgnoringAdditionalProperties());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_WhenNotIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2], o => o.IgnoringAdditionalProperties(false));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2] exactly,
					             but it differed as $[2] had unexpected 3
					             """);
			}

			public static TheoryData<object, string> MatchingArrayValues
				=> new()
				{
					{
						Array.Empty<string>(), "[]"
					},
					{
						Array.Empty<string>(), "[\"foo\"]"
					},
					{
						Array.Empty<int>(), "[]"
					},
					{
						Array.Empty<int>(), "[1, 2]"
					},
					{
						new[]
						{
							"foo", "bar",
						},
						"[\"foo\", \"bar\"]"
					},
				};

			public static TheoryData<object, string, string> NotMatchingArrayValues
				=> new()
				{
					{
						new[]
						{
							"foo",
						},
						"[]", "as $[0] had missing \"foo\""
					},
					{
						new[]
						{
							"bar", "foo",
						},
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
			public async Task ShouldFailIfPropertyIsMissing(string json,
				string? errorMessage = null)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = 2,
					});

				await That(Act).Throws<XunitException>().OnlyIf(errorMessage != null)
					.WithMessage($$"""
					               Expected that subject
					               matches new
					               					{
					               						foo = 2,
					               					},
					               but it differed as {{errorMessage}}
					               """);
			}

			[Theory]
			[InlineData("{}")]
			[InlineData("{\"foo\": 1}")]
			public async Task WhenExpectedIsEmpty_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(new object());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyHasDifferentValue_ShouldFail()
			{
				JsonElement subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						bar = 3,
					});

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             					{
					             						bar = 3,
					             					},
					             but it differed as $.bar was 2 instead of 3
					             """);
			}

			[Fact]
			public async Task WhenSubjectHasAdditionalProperties_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						bar = 2,
					}, o => o.IgnoringAdditionalProperties());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectHasAdditionalProperties_WhenNotIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						bar = 2,
					}, o => o.IgnoringAdditionalProperties(false));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             					{
					             						bar = 2,
					             					} exactly,
					             but it differed as $.foo had unexpected Null
					             """);
			}
		}
	}
}
