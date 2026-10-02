using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class Between
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(4, cts);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [0, 1, 2, 3, 4, (… and maybe more)]
						             """);
				}

				[Fact]
				public async Task WhenArrayContainsMatchingItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenArrayContainsTooFewItems_ShouldFail()
				{
					int[] subject = [1, 2,];

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it had only 2 items

						             Collection:
						             [1, 2]
						             """);
				}

				[Fact]
				public async Task WhenArrayContainsTooManyItems_ShouldFail()
				{
					int[] subject = [1, 2, 3, 4, 5, 6, 7,];

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it had 7 items

						             Collection:
						             [1, 2, 3, 4, 5, 6, 7]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2,]);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it had only 2 items

						             Collection:
						             [1, 2]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 4, 5, 6, 7,]);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it had at least 7 items

						             Collection:
						             [1, 2, 3, 4, 5, 6, 7, (… and maybe more)]
						             """);
				}

				[Theory]
				[InlineData(null, 3)]
				[InlineData(1, null)]
				public async Task WhenMinimumOrMaximumIsNull_ShouldFail(int? minimum, int? maximum)
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().Between(minimum).And(maximum);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} items,
						              but it had 3 items

						              Collection:
						              [1, 2, 3]
						              """)
						.Because("nothing can be ordered against a null bound");
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().Between(2).And(4);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has between 2 and 4 items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenArrayContainsMatchingItems_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().Between(3).And(6));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have between 3 and 6 items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenArrayContainsTooFewItems_ShouldSucceed()
				{
					int[] subject = [1, 2,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().Between(3).And(6));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().Between(3).And(6));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have between 3 and 6 items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 4, 5, 6, 7,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().Between(3).And(6));

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(null, 3)]
				[InlineData(1, null)]
				public async Task WhenMinimumOrMaximumIsNull_ShouldFail(int? minimum, int? maximum)
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().Between(minimum).And(maximum));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not have between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} items,
						              but it had 3 items

						              Collection:
						              [1, 2, 3]
						              """)
						.Because("nothing can be ordered against a null bound, so the negation fails as well");
				}
			}
		}
	}
}
