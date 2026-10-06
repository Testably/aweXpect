using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreUnique
		{
			public sealed class SetTests
			{
				[Test]
				public async Task ForAnUntypedSetOfObjects_ShouldUseTheComparerOfTheSet()
				{
					IEnumerable subject = new HashSet<object>(new AllDifferentComparer())
					{
						1,
						1,
					};

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow()
						.Because("a set never holds two items that its comparer considers equal");
				}

				[Test]
				public async Task ForAStringSet_ShouldUseTheComparerOfTheSet()
				{
					HashSet<string> subject = new(new AllDifferentComparer())
					{
						"a",
						"a",
					};

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow()
						.Because("a set never holds two items that its comparer considers equal");
				}

				[Test]
				public async Task ShouldUseTheComparerOfTheSet()
				{
					HashSet<object> subject = new(new AllDifferentComparer())
					{
						1,
						1,
					};

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow()
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
						=> await That(subject).All().AreUnique().Using(new AllEqualComparer());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique using AllEqualComparer for all items,
						             but none of 2 were

						             Not matching items:
						             [
						               1,
						               2
						             ]

						             Collection:
						             [
						               1,
						               2
						             ]
						             """);
				}

				[Test]
				public async Task WhenIgnoringCase_ShouldIgnoreTheComparerOfTheSet()
				{
					HashSet<string> subject = new(StringComparer.Ordinal)
					{
						"a",
						"A",
					};

					async Task Act()
						=> await That(subject).All().AreUnique().IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique ignoring case for all items,
						             but none of 2 were

						             Not matching items:
						             [
						               "a",
						               "A"
						             ]

						             Collection:
						             [
						               "a",
						               "A"
						             ]
						             """);
				}

				[Test]
				public async Task WithDefaultComparer_ShouldUseTheDefaultEquality()
				{
					HashSet<object> subject = [1, 1L,];

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but none of 2 were

						             Not matching items:
						             [
						               1,
						               1
						             ]

						             Collection:
						             [
						               1,
						               1
						             ]
						             """)
						.Because("a set with the default comparer keeps the default equality, which compares numbers by value");
				}
			}
		}
	}
}
