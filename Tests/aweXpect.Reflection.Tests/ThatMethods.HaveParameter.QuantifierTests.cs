using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class HaveParameter
	{
		private static MethodInfo? QuantifierMethod(string name)
			=> typeof(QuantifierMethods).GetMethod(name);

		public sealed class QuantifierTests
		{
			[Test]
			public async Task AllOverloads_WhenAllItemsMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = [QuantifierMethod(nameof(QuantifierMethods.AllKinds)),];
				Type intType = typeof(int);
				Type intArrayType = typeof(int[]);

				async Task Act()
				{
					await That(subject).All().HaveParameter<int>();
					await That(subject).All().HaveParameter(intType);
					await That(subject).All().HaveParameter<int>("inValue");
					await That(subject).All().HaveParameter(intType, "inValue");
					await That(subject).All().HaveParameter("inValue");
					await That(subject).All().HaveParameterExactly<int>();
					await That(subject).All().HaveParameterExactly(intType);
					await That(subject).All().HaveParameterExactly<int>("inValue");
					await That(subject).All().HaveParameterExactly(intType, "inValue");
					await That(subject).All().HaveInParameter();
					await That(subject).All().HaveInParameter<int>();
					await That(subject).All().HaveInParameter(intType);
					await That(subject).All().HaveInParameter<int>("inValue");
					await That(subject).All().HaveInParameter(intType, "inValue");
					await That(subject).All().HaveInParameter("inValue");
					await That(subject).All().HaveInParameterExactly<int>();
					await That(subject).All().HaveInParameterExactly(intType);
					await That(subject).All().HaveInParameterExactly<int>("inValue");
					await That(subject).All().HaveInParameterExactly(intType, "inValue");
					await That(subject).All().HaveOptionalParameter();
					await That(subject).All().HaveOptionalParameter<int>();
					await That(subject).All().HaveOptionalParameter(intType);
					await That(subject).All().HaveOptionalParameter<int>("optionalValue");
					await That(subject).All().HaveOptionalParameter(intType, "optionalValue");
					await That(subject).All().HaveOptionalParameter("optionalValue");
					await That(subject).All().HaveOptionalParameterExactly<int>();
					await That(subject).All().HaveOptionalParameterExactly(intType);
					await That(subject).All().HaveOptionalParameterExactly<int>("optionalValue");
					await That(subject).All().HaveOptionalParameterExactly(intType, "optionalValue");
					await That(subject).All().HaveOutParameter();
					await That(subject).All().HaveOutParameter<int>();
					await That(subject).All().HaveOutParameter(intType);
					await That(subject).All().HaveOutParameter<int>("outValue");
					await That(subject).All().HaveOutParameter(intType, "outValue");
					await That(subject).All().HaveOutParameter("outValue");
					await That(subject).All().HaveOutParameterExactly<int>();
					await That(subject).All().HaveOutParameterExactly(intType);
					await That(subject).All().HaveOutParameterExactly<int>("outValue");
					await That(subject).All().HaveOutParameterExactly(intType, "outValue");
					await That(subject).All().HaveParamsParameter();
					await That(subject).All().HaveParamsParameter<int[]>();
					await That(subject).All().HaveParamsParameter(intArrayType);
					await That(subject).All().HaveParamsParameter<int[]>("paramsValues");
					await That(subject).All().HaveParamsParameter(intArrayType, "paramsValues");
					await That(subject).All().HaveParamsParameter("paramsValues");
					await That(subject).All().HaveParamsParameterExactly<int[]>();
					await That(subject).All().HaveParamsParameterExactly(intArrayType);
					await That(subject).All().HaveParamsParameterExactly<int[]>("paramsValues");
					await That(subject).All().HaveParamsParameterExactly(intArrayType, "paramsValues");
					await That(subject).All().HaveRefParameter();
					await That(subject).All().HaveRefParameter<int>();
					await That(subject).All().HaveRefParameter(intType);
					await That(subject).All().HaveRefParameter<int>("refValue");
					await That(subject).All().HaveRefParameter(intType, "refValue");
					await That(subject).All().HaveRefParameter("refValue");
					await That(subject).All().HaveRefParameterExactly<int>();
					await That(subject).All().HaveRefParameterExactly(intType);
					await That(subject).All().HaveRefParameterExactly<int>("refValue");
					await That(subject).All().HaveRefParameterExactly(intType, "refValue");
					await That(subject).All().HaveParameterCount(5);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_AtIndex_ShouldOnlyCountItemsWithTheParameterAtTheIndex()
			{
				IEnumerable<MethodInfo?> subject =
				[
					QuantifierMethod(nameof(QuantifierMethods.AllKinds)),
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
				];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveParameter<int>().AtIndex(1);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has parameter of type int at index 1 for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.AllKinds(in int inValue, out int outValue, ref int refValue, int optionalValue = 0, params int[] paramsValues),
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AtLeast_HaveInParameter_WhenTooFewItemsHaveOne_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					QuantifierMethod(nameof(QuantifierMethods.AllKinds)),
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
				];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveInParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has parameter of type int with in modifier for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.AllKinds(in int inValue, out int outValue, ref int refValue, int optionalValue = 0, params int[] paramsValues),
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]
					             """);
			}

			[Test]
			public async Task AtMost_HaveParameterCount_WhenTooManyItemsMatch_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
					QuantifierMethod(nameof(QuantifierMethods.OtherPlainInt)),
				];

				async Task Act()
				{
					await That(subject).AtMost(1).HaveParameterCount(1);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has one parameter for at most one item,
					             but 2 of 2 did

					             Matching items:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value),
					               void ThatMethods.HaveParameter.QuantifierMethods.OtherPlainInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value),
					               void ThatMethods.HaveParameter.QuantifierMethods.OtherPlainInt(int value)
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_HaveNoParameters_WhenTheNumberOfItemsMatches_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					QuantifierMethod(nameof(QuantifierMethods.NoParameters)),
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
				];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveNoParameters();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_HaveParameterWithName_IgnoringCase_ShouldUseTheOption()
			{
				IEnumerable<MethodInfo?> subject =
				[
					QuantifierMethod(nameof(QuantifierMethods.NoParameters)),
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
				];

				async Task Act()
				{
					await That(subject).None().HaveParameter("VALUE").IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has parameter with name "VALUE" for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.NoParameters(),
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<MethodInfo?> subject = [null, QuantifierMethod(nameof(QuantifierMethods.PlainInt)),];

				async Task Act()
				{
					await That(subject).AtMost(1).HaveParameter<int>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtMost(1).HaveParameter<int>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has parameter of type int for at most one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has parameter of type int for more than one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AllOverloads_WhenAllItemsMatch_ShouldSucceed()
			{
				ImmutableArray<MethodInfo?> subject = [QuantifierMethod(nameof(QuantifierMethods.AllKinds)),];
				Type intType = typeof(int);
				Type intArrayType = typeof(int[]);

				async Task Act()
				{
					await That(subject).All().HaveParameter(intType);
					await That(subject).All().HaveParameter(intType, "inValue");
					await That(subject).All().HaveParameter("inValue");
					await That(subject).All().HaveParameterExactly(intType);
					await That(subject).All().HaveParameterExactly(intType, "inValue");
					await That(subject).All().HaveInParameter();
					await That(subject).All().HaveInParameter(intType);
					await That(subject).All().HaveInParameter(intType, "inValue");
					await That(subject).All().HaveInParameter("inValue");
					await That(subject).All().HaveInParameterExactly(intType);
					await That(subject).All().HaveInParameterExactly(intType, "inValue");
					await That(subject).All().HaveOptionalParameter();
					await That(subject).All().HaveOptionalParameter(intType);
					await That(subject).All().HaveOptionalParameter(intType, "optionalValue");
					await That(subject).All().HaveOptionalParameter("optionalValue");
					await That(subject).All().HaveOptionalParameterExactly(intType);
					await That(subject).All().HaveOptionalParameterExactly(intType, "optionalValue");
					await That(subject).All().HaveOutParameter();
					await That(subject).All().HaveOutParameter(intType);
					await That(subject).All().HaveOutParameter(intType, "outValue");
					await That(subject).All().HaveOutParameter("outValue");
					await That(subject).All().HaveOutParameterExactly(intType);
					await That(subject).All().HaveOutParameterExactly(intType, "outValue");
					await That(subject).All().HaveParamsParameter();
					await That(subject).All().HaveParamsParameter(intArrayType);
					await That(subject).All().HaveParamsParameter(intArrayType, "paramsValues");
					await That(subject).All().HaveParamsParameter("paramsValues");
					await That(subject).All().HaveParamsParameterExactly(intArrayType);
					await That(subject).All().HaveParamsParameterExactly(intArrayType, "paramsValues");
					await That(subject).All().HaveRefParameter();
					await That(subject).All().HaveRefParameter(intType);
					await That(subject).All().HaveRefParameter(intType, "refValue");
					await That(subject).All().HaveRefParameter("refValue");
					await That(subject).All().HaveRefParameterExactly(intType);
					await That(subject).All().HaveRefParameterExactly(intType, "refValue");
					await That(subject).All().HaveParameterCount(5);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_HaveOutParameter_WhenTooFewItemsHaveOne_ShouldFail()
			{
				ImmutableArray<MethodInfo?> subject =
				[
					QuantifierMethod(nameof(QuantifierMethods.AllKinds)),
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
				];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveOutParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an out parameter for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.AllKinds(in int inValue, out int outValue, ref int refValue, int optionalValue = 0, params int[] paramsValues),
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_HaveNoParameters_WhenTheNumberOfItemsMatches_ShouldSucceed()
			{
				ImmutableArray<MethodInfo?> subject =
				[
					QuantifierMethod(nameof(QuantifierMethods.NoParameters)),
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
				];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveNoParameters();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AllOverloads_WhenAllItemsMatch_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[] { QuantifierMethod(nameof(QuantifierMethods.AllKinds)), }.ToTestAsyncEnumerable();
				Type intType = typeof(int);
				Type intArrayType = typeof(int[]);

				async Task Act()
				{
					await That(subject).All().HaveParameter<int>();
					await That(subject).All().HaveParameter(intType);
					await That(subject).All().HaveParameter<int>("inValue");
					await That(subject).All().HaveParameter(intType, "inValue");
					await That(subject).All().HaveParameter("inValue");
					await That(subject).All().HaveParameterExactly<int>();
					await That(subject).All().HaveParameterExactly(intType);
					await That(subject).All().HaveParameterExactly<int>("inValue");
					await That(subject).All().HaveParameterExactly(intType, "inValue");
					await That(subject).All().HaveInParameter();
					await That(subject).All().HaveInParameter<int>();
					await That(subject).All().HaveInParameter(intType);
					await That(subject).All().HaveInParameter<int>("inValue");
					await That(subject).All().HaveInParameter(intType, "inValue");
					await That(subject).All().HaveInParameter("inValue");
					await That(subject).All().HaveInParameterExactly<int>();
					await That(subject).All().HaveInParameterExactly(intType);
					await That(subject).All().HaveInParameterExactly<int>("inValue");
					await That(subject).All().HaveInParameterExactly(intType, "inValue");
					await That(subject).All().HaveOptionalParameter();
					await That(subject).All().HaveOptionalParameter<int>();
					await That(subject).All().HaveOptionalParameter(intType);
					await That(subject).All().HaveOptionalParameter<int>("optionalValue");
					await That(subject).All().HaveOptionalParameter(intType, "optionalValue");
					await That(subject).All().HaveOptionalParameter("optionalValue");
					await That(subject).All().HaveOptionalParameterExactly<int>();
					await That(subject).All().HaveOptionalParameterExactly(intType);
					await That(subject).All().HaveOptionalParameterExactly<int>("optionalValue");
					await That(subject).All().HaveOptionalParameterExactly(intType, "optionalValue");
					await That(subject).All().HaveOutParameter();
					await That(subject).All().HaveOutParameter<int>();
					await That(subject).All().HaveOutParameter(intType);
					await That(subject).All().HaveOutParameter<int>("outValue");
					await That(subject).All().HaveOutParameter(intType, "outValue");
					await That(subject).All().HaveOutParameter("outValue");
					await That(subject).All().HaveOutParameterExactly<int>();
					await That(subject).All().HaveOutParameterExactly(intType);
					await That(subject).All().HaveOutParameterExactly<int>("outValue");
					await That(subject).All().HaveOutParameterExactly(intType, "outValue");
					await That(subject).All().HaveParamsParameter();
					await That(subject).All().HaveParamsParameter<int[]>();
					await That(subject).All().HaveParamsParameter(intArrayType);
					await That(subject).All().HaveParamsParameter<int[]>("paramsValues");
					await That(subject).All().HaveParamsParameter(intArrayType, "paramsValues");
					await That(subject).All().HaveParamsParameter("paramsValues");
					await That(subject).All().HaveParamsParameterExactly<int[]>();
					await That(subject).All().HaveParamsParameterExactly(intArrayType);
					await That(subject).All().HaveParamsParameterExactly<int[]>("paramsValues");
					await That(subject).All().HaveParamsParameterExactly(intArrayType, "paramsValues");
					await That(subject).All().HaveRefParameter();
					await That(subject).All().HaveRefParameter<int>();
					await That(subject).All().HaveRefParameter(intType);
					await That(subject).All().HaveRefParameter<int>("refValue");
					await That(subject).All().HaveRefParameter(intType, "refValue");
					await That(subject).All().HaveRefParameter("refValue");
					await That(subject).All().HaveRefParameterExactly<int>();
					await That(subject).All().HaveRefParameterExactly(intType);
					await That(subject).All().HaveRefParameterExactly<int>("refValue");
					await That(subject).All().HaveRefParameterExactly(intType, "refValue");
					await That(subject).All().HaveParameterCount(5);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_HaveRefParameterExactly_WhenTooFewItemsHaveOne_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					QuantifierMethod(nameof(QuantifierMethods.AllKinds)),
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveRefParameterExactly<int>("refValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has parameter of exact type int with name "refValue" with ref modifier for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.AllKinds(in int inValue, out int outValue, ref int refValue, int optionalValue = 0, params int[] paramsValues),
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]
					             """);
			}

			[Test]
			public async Task DoNotHaveParameter_WhenAnItemHasTheParameter_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					QuantifierMethod(nameof(QuantifierMethods.NoParameters)),
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).DoNotHaveParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have parameter of type int with name "value",
					             but 1 of 2 did

					             Matching items:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameter.QuantifierMethods.NoParameters(),
					               void ThatMethods.HaveParameter.QuantifierMethods.PlainInt(int value)
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_HaveNoParameters_WhenTheNumberOfItemsMatches_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					QuantifierMethod(nameof(QuantifierMethods.NoParameters)),
					QuantifierMethod(nameof(QuantifierMethods.PlainInt)),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).Exactly(1).HaveNoParameters();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private sealed class QuantifierMethods
		{
			public void AllKinds(in int inValue, out int outValue, ref int refValue, int optionalValue = 0,
				params int[] paramsValues)
				=> outValue = inValue + refValue + optionalValue + paramsValues.Length;

			public void NoParameters()
			{
			}

			public void OtherPlainInt(int value)
			{
			}

			public void PlainInt(int value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
