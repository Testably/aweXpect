using aweXpect.Core;
using aweXpect.Equivalency;

// ReSharper disable NotAccessedPositionalProperty.Local

namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyComparerTests
{
	public sealed class CustomTypeTests
	{
		[Fact]
		public async Task WhenCustomOptionsAreRegisteredForABaseType_ShouldApplyThemToADerivedValue()
		{
			SomeWrapper actual = new(new SomeDerivedRecord([1, 2,]));
			SomeWrapper expected = new(new SomeDerivedRecord([2, 1,]));
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				CustomOptions =
				{
					{
						typeof(SomeBaseRecord), new EquivalencyTypeOptions
						{
							IgnoreCollectionOrder = true,
						}
					},
				},
			});

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue()
				.Because("a member of an abstract type is always an instance of a derived type");
		}

		[Fact]
		public async Task WhenPropertiesDiffer_IgnoreCollectionOrderOnlySetForOneProperty_ShouldFailForOtherProperty()
		{
			SomeRecord actual = new(new SomeCustomRecord([1, 2,]), new SomeOtherRecord([1, 2,]));
			SomeRecord expected = new(new SomeCustomRecord([2, 1,]), new SomeOtherRecord([2, 1,]));
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				CustomOptions =
				{
					{
						typeof(SomeOtherRecord), new EquivalencyTypeOptions
						{
							IgnoreCollectionOrder = true,
						}
					},
				},
			});

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

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

		[Fact]
		public async Task WhenPropertiesDiffer_ShouldReturnFalse()
		{
			SomeRecord actual = new(new SomeCustomRecord([1, 2,]), new SomeOtherRecord([1, 2,]));
			SomeRecord expected = new(new SomeCustomRecord([2, 1,]), new SomeOtherRecord([2, 1,]));
			EquivalencyComparer sut = new(new EquivalencyOptions());

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

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

		[Fact]
		public async Task WhenPropertiesDifferButIgnoreCollectionOrderIsSet_ShouldReturnTrue()
		{
			SomeRecord actual = new(new SomeCustomRecord([1, 2,]), new SomeOtherRecord([1, 2,]));
			SomeRecord expected = new(new SomeCustomRecord([2, 1,]), new SomeOtherRecord([2, 1,]));
			EquivalencyComparer sut = new(new EquivalencyOptions
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
