using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class Contains
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 2,
					             but it contained key "a" with value 1

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenKeyIsMissing_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("b", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["b"] = 1,
					             but it did not contain key "b"

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenPairExists_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("b", 2));

				await That(Act).DoesNotThrow()
					.Because("the dictionary holds the key with that value");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldApplyIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, };

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).Contains(1, value.AddMilliseconds(500));
				}

				await That(Act).DoesNotThrow()
					.Because("the value of the entry falls back to the default tolerance, as the items of a collection do");
			}

			[Fact]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldMentionIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, };
				DateTime expected = value.AddSeconds(2);

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).Contains(1, expected);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains [1] = {Formatter.Format(expected)} ± 0:01,
					              but it contained key 1 with value {Formatter.Format(value)}

					              Dictionary:
					              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
					              """)
					.Because("the applied default tolerance is part of the expectation");
			}
		}

		public sealed class ComparerTests
		{
			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_ShouldLookTheKeyUpThroughIt()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("A", 1));

				await That(Act).DoesNotThrow()
					.Because("ContainsKey finds the key through the comparer, and Contains must not contradict it");
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("A", 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["A"] = 2,
					             but it contained key "A" with value 1

					             Dictionary:
					             {["a"] = 1}
					             """);
			}
		}

		public sealed class KeyAndValueTests
		{
			[Fact]
			public async Task WhenEntryExists_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);

				async Task Act()
					=> await That(subject).Contains("b", 2);

				await That(Act).DoesNotThrow()
					.Because("the key and value overload looks the entry up like the pair overload");
			}

			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains("a", 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 2,
					             but it contained key "a" with value 1

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenKeyIsNull_ShouldThrowArgumentNullException()
			{
				Dictionary<string, int> subject = new()
				{
					["a"] = 1,
				};

				async Task Act()
					=> await That(subject).Contains(null!, 1);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix()
					.Because("a null key can never be contained, like for ContainsKey");
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result = await That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForADictionary_WithKeyAndValue_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result = await That(subject).Contains("a", 1);

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForAListOfPairs_ShouldStillBindToTheCollectionOverload()
			{
				List<KeyValuePair<string, int>> subject = [new("a", 1),];

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 1)).Once();

				await That(Act).DoesNotThrow()
					.Because("a collection of pairs that is no dictionary keeps the quantifier of the collection result");
			}

			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int>)
						That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}

			[Fact]
			public async Task ForASortedDictionary_WithKeyAndValue_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int>)
						That(subject).Contains("a", 1);

				await That(Act).DoesNotThrow()
					.Because("the key and value overloads need the same priority to stay unambiguous");
			}
		}
	}
}
