using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed partial class WhichAreGeneric
	{
		public sealed class WithArgument
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task WhenIndexIsAlreadySpecified_ShouldUseTheLastIndex()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>().AtIndexFromEnd(0).AtIndexFromEnd(1);

					await That(subject.GetDescription())
						.StartsWith("generic methods with argument of type ThatMethod.BaseClass at index 1 from end in");
				}

				[Test]
				public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					void Act()
					{
						In.Type<MyTestClass>()
							.Methods()
							.WhichAreGeneric()
							.WithArgument<ThatMethod.BaseClass>().AtIndex(-1);
					}

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("index").And
						.WithMessage("The index must not be negative.").AsPrefix();
				}

				[Test]
				public async Task WhenMethodMatchesADerivedType_ShouldNotBeIncluded()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.DerivedClass>();

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument of type ThatMethod.DerivedClass in");
				}

				[Test]
				public async Task WhenMethodMatchesTheTypeExactly_ShouldBeIncluded()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument of type ThatMethod.BaseClass in");
				}

				[Test]
				[Arguments(0, 0)]
				[Arguments(1, 1)]
				[Arguments(2, 0)]
				public async Task WhenMethodMatchesTheTypeExactlyAtTheGivenIndex_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>().AtIndex(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith($"generic methods with argument of type ThatMethod.BaseClass at index {index} in");
				}

				[Test]
				public async Task WhenMethodMatchesTheTypeExactlyAtTheGivenIndex_ShouldNotChangeTheFilterItWasCalledOn()
				{
					Reflection.MethodFilters.GenericMethodsWithArgument withArgument = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>();

					Filtered.Methods atIndex0 = withArgument.AtIndex(0);

					await That(withArgument).HasCount(1)
						.Because("AtIndex must return a new filter instead of changing the one it was called on");
					await That(withArgument.GetDescription())
						.StartsWith("generic methods with argument of type ThatMethod.BaseClass in");
					await That(atIndex0).IsEmpty();
				}

				[Test]
				[Arguments(0, 1)]
				[Arguments(1, 0)]
				[Arguments(2, 0)]
				public async Task WhenMethodMatchesTheTypeExactlyAtTheGivenIndexFromEnd_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>().AtIndexFromEnd(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith(
							$"generic methods with argument of type ThatMethod.BaseClass at index {index} from end in");
				}

				[Test]
				public async Task WhenMethodMatchesTheTypeExactlyAtTheGivenIndexFromEnd_ShouldNotChangeTheFilterItWasCalledOn()
				{
					Reflection.MethodFilters.GenericMethodsWithArgument withArgument = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>();

					Filtered.Methods fromEnd = withArgument.AtIndexFromEnd(1);

					await That(withArgument).HasCount(1)
						.Because("AtIndexFromEnd must return a new filter instead of changing the one it was called on");
					await That(withArgument.GetDescription())
						.StartsWith("generic methods with argument of type ThatMethod.BaseClass in");
					await That(fromEnd).IsEmpty();
				}

				[Test]
				public async Task WhenNoMethodMatchesTheType_ShouldBeEmpty()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<string>();

					await That(subject).IsEmpty();
					await That(subject.GetDescription()).StartsWith("generic methods with argument of type string in");
				}
			}

			public sealed class GenericNamedTests
			{
				[Test]
				public async Task ShouldSupportAsPrefix()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<object>("TFo").AsPrefix();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument of type object and name starting with \"TFo\" in");
				}

				[Test]
				public async Task ShouldSupportAsRegex()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<object>("T[a-zA-Z]*o").AsRegex();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith(
							"generic methods with argument of type object and name matching regex \"T[a-zA-Z]*o\" in");
				}

				[Test]
				public async Task ShouldSupportAsSuffix()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<object>("Foo").AsSuffix();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument of type object and name ending with \"Foo\" in");
				}

				[Test]
				public async Task ShouldSupportAsWildcard()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<object>("T??o").AsWildcard();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument of type object and name matching \"T??o\" in");
				}

				[Test]
				[Arguments(true, 1, " ignoring case")]
				[Arguments(false, 0, "")]
				public async Task ShouldSupportIgnoringCase(bool ignoringCase, int expectedCount, string expectedSuffix)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<object>("TfOo").IgnoringCase(ignoringCase);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith(
							$"generic methods with argument of type object and name equal to \"TfOo\"{expectedSuffix} in");
				}

				[Test]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<object>("TFoO").Using(new IgnoreCaseForVocalsComparer());

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith(
							"generic methods with argument of type object and name equal to \"TFoO\" using IgnoreCaseForVocalsComparer in");
				}

				[Test]
				public async Task WhenMethodMatchesADerivedTypeAndName_ShouldNotBeIncluded()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.DerivedClass>("T2");

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.StartsWith(
							"generic methods with argument of type ThatMethod.DerivedClass and name equal to \"T2\" in");
				}

				[Test]
				public async Task WhenMethodMatchesTheTypeAndNameExactly_ShouldBeIncluded()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>("T2");

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith(
							"generic methods with argument of type ThatMethod.BaseClass and name equal to \"T2\" in");
				}

				[Test]
				public async Task WhenMethodMatchesTheTypeExactlyButNotName_ShouldNotBeIncluded()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>("T1");

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.StartsWith(
							"generic methods with argument of type ThatMethod.BaseClass and name equal to \"T1\" in");
				}

				[Test]
				public async Task WhenNoMethodMatchesTheType_ShouldBeEmpty()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<string>("Foo");

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument of type string and name equal to \"Foo\" in");
				}

				[Test]
				[Arguments(0, 0)]
				[Arguments(1, 1)]
				[Arguments(2, 0)]
				public async Task WhenTypeMatchesTheTypeAndNameExactlyAtTheGivenIndex_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>("T2").AtIndex(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith(
							$"generic methods with argument of type ThatMethod.BaseClass and name equal to \"T2\" at index {index} in");
				}

				[Test]
				[Arguments(0, 1)]
				[Arguments(1, 0)]
				[Arguments(2, 0)]
				public async Task
					WhenTypeMatchesTheTypeAndNameExactlyAtTheGivenIndexFromEnd_ShouldIncludeTheExpectedCount(
						int index, int expectedCount)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>("T2").AtIndexFromEnd(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith(
							$"generic methods with argument of type ThatMethod.BaseClass and name equal to \"T2\" at index {index} from end in");
				}
			}

			public sealed class NamedTests
			{
				[Test]
				public async Task ShouldSupportAsPrefix()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("TFo").AsPrefix();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument name starting with \"TFo\" in");
				}

				[Test]
				public async Task ShouldSupportAsRegex()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("T[a-zA-Z]*o").AsRegex();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument name matching regex \"T[a-zA-Z]*o\" in");
				}

				[Test]
				public async Task ShouldSupportAsSuffix()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("Foo").AsSuffix();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument name ending with \"Foo\" in");
				}

				[Test]
				public async Task ShouldSupportAsWildcard()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("T??o").AsWildcard();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument name matching \"T??o\" in");
				}

				[Test]
				[Arguments(true, 1, " ignoring case")]
				[Arguments(false, 0, "")]
				public async Task ShouldSupportIgnoringCase(bool ignoringCase, int expectedCount, string expectedSuffix)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("TfOo").IgnoringCase(ignoringCase);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith($"generic methods with argument name equal to \"TfOo\"{expectedSuffix} in");
				}

				[Test]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("TFoO").Using(new IgnoreCaseForVocalsComparer());

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith(
							"generic methods with argument name equal to \"TFoO\" using IgnoreCaseForVocalsComparer in");
				}

				[Test]
				public async Task WhenAStoredFilterIsContinuedTwice_ShouldKeepTheOptionsApart()
				{
					Reflection.MethodFilters.GenericMethodsWithNamedArgument stored = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("T").AsPrefix();

					Reflection.MethodFilters.GenericMethodsWithNamedArgument atIndex1 = stored.AtIndex(1);
					Reflection.MethodFilters.GenericMethodsWithNamedArgument fromEnd1 = stored.AtIndexFromEnd(1);

					await That(stored).HasCount(2)
						.Because("AtIndex and AtIndexFromEnd must return a new filter instead of changing the one they were called on");
					await That(atIndex1).HasCount(1);
					await That(atIndex1.GetDescription())
						.StartsWith("generic methods with argument name starting with \"T\" at index 1 in");
					await That(fromEnd1).HasCount(1);
					await That(fromEnd1.GetDescription())
						.StartsWith("generic methods with argument name starting with \"T\" at index 1 from end in");
				}

				[Test]
				public async Task WhenIndexIsSetBeforeAStringOption_ShouldKeepBoth()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("t2").AtIndex(1).IgnoringCase();

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument name equal to \"t2\" ignoring case at index 1 in");
				}

				[Test]
				public async Task WhenMethodDoesNotMatchTheName_ShouldNotBeIncluded()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("T3");

					await That(subject).IsEmpty();
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument name equal to \"T3\" in");
				}

				[Test]
				public async Task WhenMethodMatchesTheNameExactly_ShouldBeIncluded()
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("TFoo");

					await That(subject).HasCount(1);
					await That(subject.GetDescription())
						.StartsWith("generic methods with argument name equal to \"TFoo\" in");
				}

				[Test]
				[Arguments(0, 0)]
				[Arguments(1, 1)]
				[Arguments(2, 0)]
				public async Task WhenTypeMatchesTheNameExactlyAtTheGivenIndex_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("T2").AtIndex(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith($"generic methods with argument name equal to \"T2\" at index {index} in");
				}

				[Test]
				[Arguments(0, 1)]
				[Arguments(1, 0)]
				[Arguments(2, 0)]
				public async Task WhenTypeMatchesTheNameExactlyAtTheGivenIndexFromEnd_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("T2").AtIndexFromEnd(index);

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith($"generic methods with argument name equal to \"T2\" at index {index} from end in");
				}
			}

			// ReSharper disable UnusedMember.Local
			// ReSharper disable UnusedTypeParameter
			private class MyTestClass
			{
				public static void GenericWithUnrestrictedParameterMethod<TFoo>()
				{
				}

				public static void GenericWithRestrictedSecondParameterMethod<T1, T2>()
					where T2 : ThatMethod.BaseClass
				{
				}
			}
			// ReSharper restore UnusedTypeParameter
			// ReSharper restore UnusedMember.Local
		}
	}
}
