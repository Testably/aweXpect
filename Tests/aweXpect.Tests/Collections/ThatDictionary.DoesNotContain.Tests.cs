using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class DoesNotContain
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("a", 2));

				await That(Act).DoesNotThrow()
					.Because("the entry for key a holds another value");
			}

			[Fact]
			public async Task WhenKeyIsMissing_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("b", 1));

				await That(Act).DoesNotThrow()
					.Because("the dictionary has no entry for key b");
			}

			[Fact]
			public async Task WhenPairExists_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 1,
					             but it did

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldApplyIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, };
				DateTime unexpected = value.AddMilliseconds(500);

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).DoesNotContain(1, unexpected);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not contain [1] = {Formatter.Format(unexpected)} ± 0:01,
					              but it did

					              Dictionary:
					              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
					              """)
					.Because("the value of the entry falls back to the default tolerance, as the items of a collection do");
			}
		}

		public sealed class KeyAndValueTests
		{
			[Fact]
			public async Task WhenEntryExists_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain("a", 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 1,
					             but it did

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenKeyIsMissing_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain("b", 1);

				await That(Act).DoesNotThrow()
					.Because("the key and value overload looks the entry up like the pair overload");
			}

			[Fact]
			public async Task WhenKeyIsNull_ShouldThrowArgumentNullException()
			{
				Dictionary<string, int> subject = new()
				{
					["a"] = 1,
				};

				async Task Act()
					=> await That(subject).DoesNotContain(null!, 1);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix()
					.Because("a null key could never be contained, so the negation would succeed vacuously");
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
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("A", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["A"] = 1,
					             but it did

					             Dictionary:
					             {["a"] = 1}
					             """);
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task WhenTheValueDiffersOnlyInCase_WithIgnoringCase_ShouldFail()
			{
				Dictionary<int, string> subject = new() { [1] = "Let It Be", };

				async Task Act()
					=> await That(subject).DoesNotContain(1, "let it be").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain [1] = "let it be" ignoring case,
					             but it did

					             Dictionary:
					             {
					               [1] = "Let It Be"
					             }
					             """);
			}

			[Fact]
			public async Task WhenTheValueDiffersOnlyInCase_WithoutIgnoringCase_ShouldSucceed()
			{
				Dictionary<int, string> subject = new() { [1] = "Let It Be", };

				async Task Act()
					=> await That(subject).DoesNotContain(1, "let it be");

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenTheValueLiesOutsideTheTolerance_ShouldSucceed()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.2, };

				async Task Act()
					=> await That(subject).DoesNotContain("a", 1.0).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTheValueLiesWithinTheTolerance_ShouldFail()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.05, };

				async Task Act()
					=> await That(subject).DoesNotContain("a", 1.0).Within(0.1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 1.0 ± 0.1,
					             but it did

					             Dictionary:
					             {["a"] = 1.05}
					             """);
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result =
					await That(subject).DoesNotContain(new KeyValuePair<string, int>("a", 2));

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForADictionary_WithKeyAndValue_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result = await That(subject).DoesNotContain("a", 2);

				await That(result).IsSameAs(subject);
			}
		}
	}
}
