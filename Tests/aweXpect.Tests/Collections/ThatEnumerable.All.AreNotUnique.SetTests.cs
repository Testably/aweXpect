using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreNotUnique
		{
			public sealed class SetTests
			{
				[Test]
				public async Task ShouldUseTheComparerOfTheSet()
				{
					HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase)
					{
						"a",
						"b",
					};

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique using the subject's StringComparer.OrdinalIgnoreCase for all items,
						             but none of 2 were

						             Not matching items:
						             [
						               "a",
						               "b"
						             ]

						             Collection:
						             [
						               "a",
						               "b"
						             ]
						             """)
						.Because("a set never holds two items that its comparer considers equal");
				}

				[Test]
				public async Task Using_ShouldOverrideTheComparerOfTheSet()
				{
					HashSet<object> subject = new(new AllDifferentComparer())
					{
						1,
						2,
					};

					async Task Act()
						=> await That(subject).All().AreNotUnique().Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
