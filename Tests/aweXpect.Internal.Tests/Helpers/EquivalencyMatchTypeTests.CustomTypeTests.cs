using aweXpect.Core;
using aweXpect.Equivalency;

// ReSharper disable NotAccessedPositionalProperty.Local

namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyMatchTypeTests
{
	public sealed class CustomTypeTests
	{
		[Test]
		public async Task WhenCustomOptionsAreRegisteredForABaseType_ShouldApplyThemToADerivedValue()
		{
			SomeWrapper actual = new(new SomeDerivedRecord([1, 2,]));
			SomeWrapper expected = new(new SomeDerivedRecord([2, 1,]));
			EquivalencyMatchType sut = new(new EquivalencyOptions().For<SomeBaseRecord>(o => o with
			{
				IgnoreCollectionOrder = true,
			}));

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue()
				.Because("a member of an abstract type is always an instance of a derived type");
		}

		[Test]
		public async Task WhenPropertiesDiffer_IgnoreCollectionOrderOnlySetForOneProperty_ShouldFailForOtherProperty()
		{
			SomeRecord actual = new(new SomeCustomRecord([1, 2,]), new SomeOtherRecord([1, 2,]));
			SomeRecord expected = new(new SomeCustomRecord([2, 1,]), new SomeOtherRecord([2, 1,]));
			EquivalencyMatchType sut = new(new EquivalencyOptions().For<SomeOtherRecord>(o => o with
			{
				IgnoreCollectionOrder = true,
			}));

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;
			string failure = explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Element CustomRecord.Values[0] differed:
			                                    Actual: 1
			                                  Expected: 2
			                              and
			                                Element CustomRecord.Values[1] differed:
			                                    Actual: 2
			                                  Expected: 1
			                              """);
		}

		[Test]
		public async Task WhenPropertiesDiffer_ShouldReturnFalse()
		{
			SomeRecord actual = new(new SomeCustomRecord([1, 2,]), new SomeOtherRecord([1, 2,]));
			SomeRecord expected = new(new SomeCustomRecord([2, 1,]), new SomeOtherRecord([2, 1,]));
			EquivalencyMatchType sut = new(new EquivalencyOptions());

			IObjectMatchResult explanation = await sut.AreConsideredEqualWithExplanation(actual, expected);
			bool result = explanation.IsMatch;
			string failure = explanation.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Element CustomRecord.Values[0] differed:
			                                    Actual: 1
			                                  Expected: 2
			                              and
			                                Element CustomRecord.Values[1] differed:
			                                    Actual: 2
			                                  Expected: 1
			                              and
			                                Element OtherRecord.Values[0] differed:
			                                    Actual: 1
			                                  Expected: 2
			                              and
			                                Element OtherRecord.Values[1] differed:
			                                    Actual: 2
			                                  Expected: 1
			                              """);
		}

		[Test]
		public async Task WhenPropertiesDifferButIgnoreCollectionOrderIsSet_ShouldReturnTrue()
		{
			SomeRecord actual = new(new SomeCustomRecord([1, 2,]), new SomeOtherRecord([1, 2,]));
			SomeRecord expected = new(new SomeCustomRecord([2, 1,]), new SomeOtherRecord([2, 1,]));
			EquivalencyMatchType sut = new(new EquivalencyOptions
			{
				IgnoreCollectionOrder = true,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		private record SomeRecord(SomeCustomRecord CustomRecord, SomeOtherRecord OtherRecord);

		private record SomeCustomRecord(int[] Values);

		private record SomeOtherRecord(int[] Values);

		private record SomeWrapper(SomeBaseRecord Value);

		private abstract record SomeBaseRecord(int[] Values);

		private sealed record SomeDerivedRecord(int[] Values) : SomeBaseRecord(Values);
	}
}
