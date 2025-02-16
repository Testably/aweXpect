using System.Text.Json;

namespace aweXpect.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsArray
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("{\"foo\":{}}", "an object")]
			[InlineData("{\"foo\":2}", "a number")]
			[InlineData("{\"foo\":\"foo\"}", "a string")]
			public async Task WhenInnerJsonIsNoArray_AndExpectElementAt_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnArray(a => a.At(0).Matching(null)));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is an array and $.foo[0] matches null,
					              but it differed as $.foo was {kindString} instead of an array
					              """);
			}

			[Theory]
			[InlineData("{\"foo\":{}}", "an object")]
			[InlineData("{\"foo\":2}", "a number")]
			[InlineData("{\"foo\":\"foo\"}", "a string")]
			public async Task WhenInnerJsonIsNoArray_AndExpectElementCount_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnArray(a => a.With(0).Elements()));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is an array with 0 elements,
					              but it differed as $.foo was {kindString} instead of an array
					              """);
			}

			[Theory]
			[InlineData("{\"foo\":{}}", "an object")]
			[InlineData("{\"foo\":2}", "a number")]
			[InlineData("{\"foo\":\"foo\"}", "a string")]
			public async Task WhenInnerJsonIsNoArray_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnArray());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is an array,
					              but it differed as $.foo was {kindString} instead of an array
					              """);
			}

			[Theory]
			[InlineData("[]")]
			[InlineData("[1, 2]")]
			public async Task WhenJsonIsAnArray_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("2", "a number")]
			[InlineData("\"foo\"", "a string")]
			public async Task WhenJsonIsNoArray_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an array,
					              but it was {kindString} instead of an array
					              """);
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("2", "a number")]
			[InlineData("\"foo\"", "a string")]
			public async Task WhenJsonIsNoArray_WithExpectations_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o.At(0).Matching(true));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an array and $[0] matches true,
					              but it was {kindString} instead of an array
					              """);
			}
		}

		public sealed class WithNumberOfElementsTests
		{
			[Fact]
			public async Task WhenNull_ShouldNotAddFailureMessage()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnArray(a => a
							.With(1).Elements()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array with 1 element,
					             but it differed as property $.foo did not exist
					             """);
			}

			[Fact]
			public async Task WhenNumberDiffers_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).IsArray(o => o
						.With(3).Elements());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an array with 3 elements,
					             but it differed as $ had 2 elements
					             """);
			}

			[Theory]
			[InlineData("[]", 0)]
			[InlineData("[\"foo\"]", 1)]
			[InlineData("[1, 2, 3]", 3)]
			public async Task WhenNumberMatches_ShouldSucceed(string json, int expected)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o
						.With(expected).Elements());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithArraysTests
		{
			[Fact]
			public async Task WhenArrayIsNull_ShouldNotAddFailureMessage()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnArray(a => a
							.WithArrays(p => p.With(0).Elements())));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array and $.foo[0] is an array with 0 elements,
					             but it differed as property $.foo did not exist
					             """);
			}

			[Fact]
			public async Task WhenExpectationIsNull_ShouldSkipIndex()
			{
				JsonElement subject = FromString("[[1],[2],[3]]");

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithArrays(o => o.At(0).Matching(1), null, o => o.At(0).Matching(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an array and $[0][0] matches 1 and $[2] is an array and $[2][0] matches 2,
					             but it differed as $[2][0] was 3 instead of 2
					             """);
			}

			[Fact]
			public async Task WhenExpectationsMatchInDifferentOrder_ShouldFail()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   [1],
				                                   [3],
				                                   [2],
				                                   [4]
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithArrays(
							o => o.At(0).Matching(1),
							o => o.At(0).Matching(2),
							o => o.At(0).Matching(3)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an array and $[0][0] matches 1 and $[1] is an array and $[1][0] matches 2 and $[2] is an array and $[2][0] matches 3,
					             but it differed as
					               $[1][0] was 3 instead of 2 and
					               $[2][0] was 2 instead of 3
					             """);
			}

			[Fact]
			public async Task WhenExpectationsMatchInOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   [1],
				                                   [2],
				                                   [3],
				                                   [4]
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithArrays(
							o => o.At(0).Matching(1),
							o => o.At(0).Matching(2),
							o => o.At(0).Matching(3)));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithElementsTests
		{
			[Fact]
			public async Task WhenArrayIsNull_ShouldNotAddFailureMessage()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnArray(a => a
							.WithElements(2, 3)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array and $.foo[0] matches 2 and $.foo[1] matches 3,
					             but it differed as property $.foo did not exist
					             """);
			}

			[Fact]
			public async Task WhenExpectationsMatchInDifferentOrder_ShouldFail()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   3, null, true, 1.2, false, "bar"
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithElements(3, true, null, 1.2, false, "bar"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches 3 and $[1] matches True and $[2] matches Null and $[3] matches 1.2 and $[4] matches False and $[5] matches "bar",
					             but it differed as
					               $[1] was Null instead of True and
					               $[2] was boolean True instead of Null
					             """);
			}

			[Fact]
			public async Task WhenExpectationsMatchInOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   3, true, null, 1.2, false, "bar"
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithElements(3, true, null, 1.2, false, "bar"));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithObjectsTests
		{
			[Fact]
			public async Task WhenArrayIsNull_ShouldNotAddFailureMessage()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnArray(a => a
							.WithObjects(p => p.With(0).Properties())));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array and $.foo[0] is an object with 0 properties,
					             but it differed as property $.foo did not exist
					             """);
			}

			[Fact]
			public async Task WhenExpectationIsNull_ShouldSkipIndex()
			{
				JsonElement subject = FromString("[{},{},{\"foo\":true}]");

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithObjects(o => o.With(0).Properties(), null, o => o.With(0).Properties()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an object with 0 properties and $[2] is an object with 0 properties,
					             but it differed as $[2] had 1 property
					             """);
			}

			[Fact]
			public async Task WhenExpectationsMatchInDifferentOrder_ShouldFail()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"foo":1},
				                                   {"foo":3},
				                                   {"bar":2},
				                                   {"baz":4}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithObjects(
							o => o.With("foo").Matching(1),
							o => o.With("bar").Matching(2),
							o => o.With("foo").Matching(3)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an object and $[0].foo matches 1 and $[1] is an object and $[1].bar matches 2 and $[2] is an object and $[2].foo matches 3,
					             but it differed as
					               property $[1].bar did not exist and
					               property $[2].foo did not exist
					             """);
			}

			[Fact]
			public async Task WhenExpectationsMatchInOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"foo":1},
				                                   {"bar":2},
				                                   {"foo":3},
				                                   {"baz":4}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithObjects(
							o => o.With("foo").Matching(1),
							o => o.With("bar").Matching(2),
							o => o.With("foo").Matching(3)));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
