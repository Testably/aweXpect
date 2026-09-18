using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed class QuantifierResultShape
{
	public sealed class Tests
	{
		[Fact]
		public async Task NegatedAtLeast_WhenEnumerationStoppedEarly_ShouldNameTheSeenItems()
		{
			IEnumerable<int> subject = ThatEnumerable.ToEnumerable([1, 1, 1, 2,]);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.AtLeast(2).AreEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("*but at least 2 of at least 2 were*").AsWildcard();
		}

		[Fact]
		public async Task NegatedAtLeast_WhenTotalIsKnown_ShouldNameTheTotal()
		{
			int[] subject = [1, 1, 1, 2,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.AtLeast(2).AreEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("*but 3 of 4 were*").AsWildcard();
		}

		[Fact]
		public async Task NegatedAtLeastComplyWith_WhenTotalIsKnown_ShouldNameTheTotal()
		{
			int[] subject = [1, 1, 1, 2,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.AtLeast(2).ComplyWith(item => item.IsEqualTo(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("*but 3 of 4 were*").AsWildcard();
		}

		[Fact]
		public async Task NegatedMoreThan_WhenTotalIsKnown_ShouldNameTheTotal()
		{
			int[] subject = [1, 1, 1, 2,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(1).AreEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("*but 3 of 4 were*").AsWildcard();
		}
	}
}
