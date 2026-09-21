using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEmpty
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldCallEnumeratorOnlyOnce()
			{
				IEnumerable<int> subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenArrayContainsValues_ShouldFail()
			{
				string[] subject = ["foo",];

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               "foo"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenArrayIsEmpty_ShouldSucceed()
			{
				string[] subject = [];

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsValues_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2,]);

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1,
					               1,
					               2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable((int[]) []);

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was <null>
					             """);
			}
		}
	}
}
