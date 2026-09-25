using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEmpty
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).IsNotEmpty()
						.And.IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenArrayContainsValues_ShouldSucceed()
			{
				string[] subject = ["foo",];

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenArrayIsEmpty_ShouldFail()
			{
				string[] subject = [];

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsValues_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2,]);

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable((int[]) []);

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was <null>
					             """);
			}
		}
	}
}
