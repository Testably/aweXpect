using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class IntegerWithinTests
	{
		[Fact]
		public async Task AllAreEqualTo_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
		{
			int[] subject = [9, 20, 31,];

			async Task Act()
				=> await That(subject).All().AreEqualTo(20).Within(11);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Contains_ForBytes_WhenTheItemLiesWithinTheTolerance_ShouldSucceed()
		{
			byte[] subject = [1, 200,];

			async Task Act()
				=> await That(subject).Contains(198).Within(2);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Contains_ForLongs_WhenTheDistanceExceedsTheRangeOfTheType_ShouldFail()
		{
			long[] subject = [long.MinValue,];

			async Task Act()
				=> await That(subject).Contains(long.MaxValue).Within(long.MaxValue);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              contains an item equal to {Formatter.Format(long.MaxValue)} ± {Formatter.Format(long.MaxValue)} at least once,
				              but it did not contain it

				              Collection:
				              {Formatter.Format(subject)}
				              """)
				.Because("the distance between the extremes is computed without overflow");
		}

		[Fact]
		public async Task Contains_ForNullableInts_WhenTheItemLiesWithinTheTolerance_ShouldSucceed()
		{
			int?[] subject = [null, 9,];

			async Task Act()
				=> await That(subject).Contains(10).Within(1);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Contains_ForShorts_WhenTheDistanceExceedsTheRangeOfTheType_ShouldFail()
		{
			short[] subject = [short.MinValue,];

			async Task Act()
				=> await That(subject).Contains(short.MaxValue).Within(short.MaxValue);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 32767 ± 32767 at least once,
				             but it did not contain it

				             Collection:
				             [-32768]
				             """)
				.Because("the distance of a short is computed in a wider type");
		}

		[Fact]
		public async Task Contains_ForULongs_WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
		{
			ulong[] subject = [ulong.MaxValue,];

			async Task Act()
				=> await That(subject).Contains(0UL).Within(ulong.MaxValue);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Contains_WhenTheItemLiesOutsideTheTolerance_ShouldFail()
		{
			int[] subject = [8, 20, 32,];

			async Task Act()
				=> await That(subject).Contains(10).Within(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 10 ± 1 at least once,
				             but it did not contain it

				             Collection:
				             [8, 20, 32]
				             """);
		}

		[Fact]
		public async Task Contains_WhenTheItemLiesWithinTheTolerance_ShouldSucceed()
		{
			int[] subject = [9, 20, 31,];

			async Task Act()
				=> await That(subject).Contains(10).Within(1);

			await That(Act).DoesNotThrow()
				.Because("the items of a collection have the same tolerance as a single number");
		}

		[Fact]
		public async Task Contains_WhenTheToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			int[] subject = [9, 20, 31,];

			object Act()
				=> That(subject).Contains(10).Within(-1);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("tolerance").And
				.WithMessage("The tolerance must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task DoesNotContain_WhenTheItemLiesWithinTheTolerance_ShouldFail()
		{
			IEnumerable<long> subject = [9, 20, 31,];

			async Task Act()
				=> await That(subject).DoesNotContain(10).Within(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to 10 ± 1,
				             but it contained 9 at least once

				             Collection:
				             [9, 20, 31]
				             """);
		}

		[Fact]
		public async Task EndsWith_WhenTheItemsLieWithinTheTolerance_ShouldSucceed()
		{
			uint[] subject = [9, 20, 31,];

			async Task Act()
				=> await That(subject).EndsWith(21, 30).Within(1);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task HasItem_WhenTheItemLiesWithinTheTolerance_ShouldSucceed()
		{
			sbyte[] subject = [9, 20, 31,];

			async Task Act()
				=> await That(subject).HasItem(21).Within(1).AtIndex(1);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task IsContainedIn_WhenTheItemsLieWithinTheTolerance_ShouldSucceed()
		{
			ushort[] subject = [9, 20,];

			async Task Act()
				=> await That(subject).IsContainedIn([10, 21, 30,]).Within(1);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task IsEqualTo_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
		{
			int[] subject = [9, 20, 32,];

			async Task Act()
				=> await That(subject).IsEqualTo([10, 20, 30,]).Within(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [10, 20, 30,] ± 1 in order,
				             but it contained item 32 at index 2 instead of 30

				             Collection:
				             [9, 20, 32]

				             Expected:
				             [10, 20, 30]
				             """);
		}

		[Fact]
		public async Task IsEqualTo_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
		{
			int[] subject = [9, 20, 31,];

			async Task Act()
				=> await That(subject).IsEqualTo([10, 20, 30,]).Within(1);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task StartsWith_WhenTheItemsLieWithinTheTolerance_ShouldSucceed()
		{
			IEnumerable<int?> subject = [9, null, 31,];

			async Task Act()
				=> await That(subject).StartsWith(10, null).Within(1);

			await That(Act).DoesNotThrow();
		}
	}
}
