using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class ContainsValues
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllValuesExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues(42, 41);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllValuesExists_WithNull_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [null, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues(42, null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				int[]? expected = null;

				async Task Act()
					=> await That(subject).ContainsValues(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenOneValueIsMissing_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues(42, 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains values [42, 2],
					             but it did not contain [
					               2
					             ] in [
					               41,
					               42,
					               43
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenOneValueIsMissing_WithNull_ShouldFail()
			{
				IReadOnlyDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues(42, null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains values [42, <null>],
					             but it did not contain [
					               <null>
					             ] in [
					               41,
					               42,
					               43
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).ContainsValues("foo", "bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains values ["foo", "bar"],
					             but it was <null>
					             """);
			}
		}
	}
}
