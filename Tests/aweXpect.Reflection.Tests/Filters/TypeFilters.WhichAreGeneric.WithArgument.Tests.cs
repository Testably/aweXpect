using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed partial class WhichAreGeneric
	{
		public sealed class WithArgument
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task WhenIndexIsAlreadySpecified_ShouldThrowInvalidOperationException()
				{
					void Act()
					{
						In.AssemblyContaining<WhichAreGeneric>().Types()
							.WhichAreGeneric()
							.WithArgument<ThatMethod.BaseClass>().AtIndex(0).AtIndexFromEnd(0);
					}

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("AtIndexFromEnd cannot be combined with AtIndex.");
				}

				[Test]
				public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					void Act()
					{
						In.AssemblyContaining<WhichAreGeneric>().Types()
							.WhichAreGeneric()
							.WithArgument<ThatMethod.BaseClass>().AtIndexFromEnd(-1);
					}

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("index").And
						.WithMessage("The index must not be negative.").AsPrefix();
				}

				[Test]
				public async Task WhenNoTypeMatchesTheType_ShouldBeEmpty()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<string>();

					await That(subject).IsEmpty();
					await That(subject.GetDescription()).Contains("with argument of type string in");
				}

				[Test]
				public async Task WhenTypeMatchesADerivedType_ShouldNotBeIncluded()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.DerivedClass>();

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.Contains("with argument of type ThatMethod.DerivedClass in");
				}

				[Test]
				public async Task WhenTypeMatchesTheTypeExactly_ShouldBeIncluded()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument of type ThatMethod.BaseClass in");
				}

				[Test]
				[Arguments(0, 0)]
				[Arguments(1, 1)]
				[Arguments(2, 0)]
				public async Task WhenTypeMatchesTheTypeExactlyAtTheGivenIndex_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>().AtIndex(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.Contains($"with argument of type ThatMethod.BaseClass at index {index} in");
				}

				[Test]
				public async Task WhenTypeMatchesTheTypeExactlyAtTheGivenIndex_ShouldNotChangeTheFilterItWasCalledOn()
				{
					Reflection.TypeFilters.GenericTypesWithArgument withArgument = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>();

					Filtered.Types atIndex0 = withArgument.AtIndex(0);

					await That(withArgument).HasCount(1)
						.Because("AtIndex must return a new filter instead of changing the one it was called on");
					await That(withArgument.GetDescription())
						.Contains("with argument of type ThatMethod.BaseClass in");
					await That(atIndex0).IsEmpty();
				}

				[Test]
				[Arguments(0, 1)]
				[Arguments(1, 0)]
				[Arguments(2, 0)]
				public async Task WhenTypeMatchesTheTypeExactlyAtTheGivenIndexFromEnd_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>().AtIndexFromEnd(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.Contains($"with argument of type ThatMethod.BaseClass at index {index} from end in");
				}

				[Test]
				public async Task WhenTypeMatchesTheTypeExactlyAtTheGivenIndexFromEnd_ShouldNotChangeTheFilterItWasCalledOn()
				{
					Reflection.TypeFilters.GenericTypesWithArgument withArgument = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>();

					Filtered.Types fromEnd = withArgument.AtIndexFromEnd(1);

					await That(withArgument).HasCount(1)
						.Because("AtIndexFromEnd must return a new filter instead of changing the one it was called on");
					await That(withArgument.GetDescription())
						.Contains("with argument of type ThatMethod.BaseClass in");
					await That(fromEnd).IsEmpty();
				}
			}

			public sealed class GenericNamedTests
			{
				[Test]
				public async Task ShouldSupportAsPrefix()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<object>("TFo").AsPrefix();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument of type object and name starting with \"TFo\" in");
				}

				[Test]
				public async Task ShouldSupportAsRegex()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<object>("T[a-zA-Z]*o").AsRegex();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument of type object and name matching regex \"T[a-zA-Z]*o\" in");
				}

				[Test]
				public async Task ShouldSupportAsSuffix()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<object>("Foo").AsSuffix();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument of type object and name ending with \"Foo\" in");
				}

				[Test]
				public async Task ShouldSupportAsWildcard()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<object>("T??o").AsWildcard();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument of type object and name matching \"T??o\" in");
				}

				[Test]
				[Arguments(true, 1, " ignoring case")]
				[Arguments(false, 0, "")]
				public async Task ShouldSupportIgnoringCase(bool ignoringCase, int expectedCount, string expectedSuffix)
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<object>("TfOo").IgnoringCase(ignoringCase);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.Contains($"with argument of type object and name equal to \"TfOo\"{expectedSuffix} in");
				}

				[Test]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<object>("TFoO").Using(new IgnoreCaseForVocalsComparer());

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains(
							"with argument of type object and name equal to \"TFoO\" using IgnoreCaseForVocalsComparer in");
				}

				[Test]
				public async Task WhenNoTypeMatchesTheType_ShouldBeEmpty()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<string>("Foo");

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.Contains("with argument of type string and name equal to \"Foo\" in");
				}

				[Test]
				public async Task WhenTypeMatchesADerivedTypeAndName_ShouldNotBeIncluded()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.DerivedClass>("T2");

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.Contains("with argument of type ThatMethod.DerivedClass and name equal to \"T2\" in");
				}

				[Test]
				public async Task WhenTypeMatchesTheTypeAndNameExactly_ShouldBeIncluded()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>("T2");

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument of type ThatMethod.BaseClass and name equal to \"T2\" in");
				}

				[Test]
				[Arguments(0, 0)]
				[Arguments(1, 1)]
				[Arguments(2, 0)]
				public async Task WhenTypeMatchesTheTypeAndNameExactlyAtTheGivenIndex_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>("T2").AtIndex(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.Contains(
							$"with argument of type ThatMethod.BaseClass and name equal to \"T2\" at index {index} in");
				}

				[Test]
				[Arguments(0, 1)]
				[Arguments(1, 0)]
				[Arguments(2, 0)]
				public async Task
					WhenTypeMatchesTheTypeAndNameExactlyAtTheGivenIndexFromEnd_ShouldIncludeTheExpectedCount(
						int index, int expectedCount)
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>("T2").AtIndexFromEnd(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.Contains(
							$"with argument of type ThatMethod.BaseClass and name equal to \"T2\" at index {index} from end in");
				}

				[Test]
				public async Task WhenTypeMatchesTheTypeExactlyButNotName_ShouldNotBeIncluded()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>("T1");

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.Contains("with argument of type ThatMethod.BaseClass and name equal to \"T1\" in");
				}
			}

			public sealed class NamedTests
			{
				[Test]
				public async Task ShouldSupportAsPrefix()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("TFo").AsPrefix();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument name starting with \"TFo\" in");
				}

				[Test]
				public async Task ShouldSupportAsRegex()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("T[a-zA-Z]*o").AsRegex();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument name matching regex \"T[a-zA-Z]*o\" in");
				}

				[Test]
				public async Task ShouldSupportAsSuffix()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("Foo").AsSuffix();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument name ending with \"Foo\" in");
				}

				[Test]
				public async Task ShouldSupportAsWildcard()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("T??o").AsWildcard();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument name matching \"T??o\" in");
				}

				[Test]
				[Arguments(true, 1, " ignoring case")]
				[Arguments(false, 0, "")]
				public async Task ShouldSupportIgnoringCase(bool ignoringCase, int expectedCount, string expectedSuffix)
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("TfOo").IgnoringCase(ignoringCase);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.Contains($"with argument name equal to \"TfOo\"{expectedSuffix} in");
				}

				[Test]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("TFoO").Using(new IgnoreCaseForVocalsComparer());

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument name equal to \"TFoO\" using IgnoreCaseForVocalsComparer in");
				}

				[Test]
				public async Task WhenAStoredFilterIsContinuedTwice_ShouldKeepTheOptionsApart()
				{
					Reflection.TypeFilters.GenericTypesWithNamedArgument stored = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("T").AsPrefix();

					Reflection.TypeFilters.GenericTypesWithNamedArgument atIndex1 = stored.AtIndex(1);
					Reflection.TypeFilters.GenericTypesWithNamedArgument fromEnd1 = stored.AtIndexFromEnd(1);

					await That(stored).HasCount(2)
						.Because("AtIndex and AtIndexFromEnd must return a new filter instead of changing the one they were called on");
					await That(atIndex1).HasCount(1);
					await That(atIndex1.GetDescription()).Contains("with argument name starting with \"T\" at index 1 in");
					await That(fromEnd1).HasCount(1);
					await That(fromEnd1.GetDescription()).Contains("with argument name starting with \"T\" at index 1 from end in");
				}

				[Test]
				public async Task WhenIndexIsSetBeforeAStringOption_ShouldKeepBoth()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("t2").AtIndex(1).IgnoringCase();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument name equal to \"t2\" ignoring case at index 1 in");
				}

				[Test]
				public async Task WhenTypeDoesNotMatchTheName_ShouldNotBeIncluded()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("T3");

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.Contains("with argument name equal to \"T3\" in");
				}

				[Test]
				public async Task WhenTypeMatchesTheNameExactly_ShouldBeIncluded()
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("TFoo");

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.Contains("with argument name equal to \"TFoo\" in");
				}

				[Test]
				[Arguments(0, 0)]
				[Arguments(1, 1)]
				[Arguments(2, 0)]
				public async Task WhenTypeMatchesTheNameExactlyAtTheGivenIndex_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("T2").AtIndex(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.Contains(
							$"with argument name equal to \"T2\" at index {index} in");
				}

				[Test]
				[Arguments(0, 1)]
				[Arguments(1, 0)]
				[Arguments(2, 0)]
				public async Task WhenTypeMatchesTheNameExactlyAtTheGivenIndexFromEnd_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Types subject = In
						.AssemblyContaining<WhichAreGeneric>()
						.Types().WithName("MyGenericTestClass").AsPrefix()
						.WhichAreGeneric()
						.WithArgument("T2").AtIndexFromEnd(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.Contains(
							$"with argument name equal to \"T2\" at index {index} from end in");
				}
			}

			// ReSharper disable UnusedTypeParameter
			private class MyGenericTestClass<TFoo>
			{
			}

			private class MyGenericTestClass<T1, T2>
				where T2 : ThatMethod.BaseClass
			{
			}
			// ReSharper restore UnusedTypeParameter
		}
	}
}
