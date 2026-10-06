using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class ContainsValue
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenNullValueDoesNotExist_ShouldFail()
			{
				IReadOnlyDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains value <null>,
					             but it did not contain value <null>

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenNullValueExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, null, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(null);

				await That(Act).DoesNotThrow();
			}
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject)!.ContainsValue("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains value "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(42);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueIsMissing_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains value 2,
					             but it did not contain value 2

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}
		}

		public sealed class WithinTests
		{
			[Test]
			public async Task WhenTheValueLiesWithinTheTolerance_ShouldSucceed()
			{
				ReadOnlyDictionary<string, double> subject = new(new Dictionary<string, double> { { "a", 1.05 }, });

				ReadOnlyDictionary<string, double> result = await That(subject).ContainsValue(1.0).Within(0.1);

				await That(result).IsSameAs(subject)
					.Because("the tolerance overload keeps the subject type, too");
			}
		}

		public sealed class OverloadTests
		{
			[Test]
			public async Task ForAReadOnlyDictionaryOfStrings_ShouldKeepTheSubjectType()
			{
				ReadOnlyDictionary<string, string?> subject =
					new(new Dictionary<string, string?> { { "a", "foo" }, });

				ReadOnlyDictionary<string, string?> result = await That(subject).ContainsValue("FOO").IgnoringCase();

				await That(result).IsSameAs(subject);
			}
		}

		public sealed class ThrowingSubjectTests
		{
			[Test]
			public async Task WhenTheEnumerationThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("enumeration failed");
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.Enumeration) { ["a"] = 1, ["b"] = 2, });

				async Task Act()
					=> await That(subject).ContainsValue(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains value 1,
					             but it did throw an InvalidOperationException:
					               enumeration failed

					             Dictionary:
					             [the enumeration did throw an InvalidOperationException: enumeration failed]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}
		}
	}
}
