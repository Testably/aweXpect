using aweXpect.Core;
using aweXpect.Equivalency;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace aweXpect.Internal.Tests.Helpers;

public sealed partial class EquivalencyComparerTests
{
	public sealed class ComparisonTypeTests
	{
		[Fact]
		public async Task CanSpecifyComparisonTypeForSpecificTypes()
		{
			MyClassWithDifferentProperties actual = new()
			{
				Property1 = new MyClass1
				{
					Value = 1,
				},
				Property2 = new MyClass2
				{
					Value = 1,
				},
			};
			MyClassWithDifferentProperties expected = new()
			{
				Property1 = new MyClass1
				{
					Value = 1,
				},
				Property2 = new MyClass2
				{
					Value = 1,
				},
			};
			EquivalencyComparer sut = new(new EquivalencyOptions()
				.For<MyClass2>(o => o with
				{
					ComparisonType = EquivalencyComparisonType.ByValue,
				}));

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                Property Property2 differed:
			                                    Actual: EquivalencyComparerTests.ComparisonTypeTests.MyClass2 { Value = 1 }
			                                  Expected: EquivalencyComparerTests.ComparisonTypeTests.MyClass2 { Value = 1 }
			                              """);
		}

		[Fact]
		public async Task WhenComparingByValue_ShouldUseObjectEqualsForClasses()
		{
			MyClass actual = new()
			{
				MyValue = "foo",
			};
			MyClass expected = new()
			{
				MyValue = "foo",
			};
			EquivalencyComparer sut = new(new EquivalencyOptions
			{
				DefaultComparisonTypeSelector = _ => EquivalencyComparisonType.ByValue,
			});

			bool result = await sut.AreConsideredEqual(actual, expected);
			string failure = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(result).IsFalse();
			await That(failure).IsEqualTo("""
			                              it was not:
			                                It differed:
			                                    Actual: EquivalencyComparerTests.MyClass { MyValue = "foo", Nested = <null> }
			                                  Expected: EquivalencyComparerTests.MyClass { MyValue = "foo", Nested = <null> }
			                              """);
		}

		private sealed class MyClassWithDifferentProperties
		{
			public MyClass1? Property1 { get; set; }
			public MyClass2? Property2 { get; set; }
		}

		private sealed class MyClass1
		{
			public int Value { get; set; }
		}

		private sealed class MyClass2
		{
			public int Value { get; set; }
		}
	}
}
