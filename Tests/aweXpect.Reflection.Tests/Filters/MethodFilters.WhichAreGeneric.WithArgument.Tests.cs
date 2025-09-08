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
				[Fact]
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

				[Fact]
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

				[Theory]
				[InlineData(0, 0)]
				[InlineData(1, 1)]
				[InlineData(2, 0)]
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

				[Theory]
				[InlineData(0, 1)]
				[InlineData(1, 0)]
				[InlineData(2, 0)]
				public async Task WhenMethodMatchesTheTypeExactlyAtTheGivenIndexFromEnd_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>().AtIndex(index).FromEnd();

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith(
							$"generic methods with argument of type ThatMethod.BaseClass at index {index} from end in");
				}

				[Fact]
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
				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Theory]
				[InlineData(true, 1, " ignoring case")]
				[InlineData(false, 0, "")]
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

				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Theory]
				[InlineData(0, 0)]
				[InlineData(1, 1)]
				[InlineData(2, 0)]
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

				[Theory]
				[InlineData(0, 1)]
				[InlineData(1, 0)]
				[InlineData(2, 0)]
				public async Task
					WhenTypeMatchesTheTypeAndNameExactlyAtTheGivenIndexFromEnd_ShouldIncludeTheExpectedCount(
						int index, int expectedCount)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument<ThatMethod.BaseClass>("T2").AtIndex(index).FromEnd();

					await That(subject).HasCount(expectedCount);
					await That(subject.GetDescription())
						.StartsWith(
							$"generic methods with argument of type ThatMethod.BaseClass and name equal to \"T2\" at index {index} from end in");
				}
			}

			public sealed class NamedTests
			{
				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Theory]
				[InlineData(true, 1, " ignoring case")]
				[InlineData(false, 0, "")]
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

				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Theory]
				[InlineData(0, 0)]
				[InlineData(1, 1)]
				[InlineData(2, 0)]
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

				[Theory]
				[InlineData(0, 1)]
				[InlineData(1, 0)]
				[InlineData(2, 0)]
				public async Task WhenTypeMatchesTheNameExactlyAtTheGivenIndexFromEnd_ShouldIncludeTheExpectedCount(
					int index, int expectedCount)
				{
					Filtered.Methods subject = In.Type<MyTestClass>()
						.Methods()
						.WhichAreGeneric()
						.WithArgument("T2").AtIndex(index).FromEnd();

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
