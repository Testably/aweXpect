using System.Collections.Generic;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class Matching
	{
		public sealed class Tests
		{
			[Test]
			public async Task DoesNotHaveParameter_WhenParameterMatches_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.Count))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotHaveParameter<int>().Matching(p => p.Name == "count");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have parameter of type int matching p => p.Name == "count",
					             but it did
					             """);
			}

			[Test]
			public async Task HasParameter_WhenNoParameterMatches_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.Count))!;

				async Task Act()
				{
					await That(methodInfo).HasParameter<int>().Matching(p => p.Name == "index");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int matching p => p.Name == "index",
					             but it did not
					             """);
			}

			[Test]
			public async Task HasParameter_WhenParameterMatches_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.Count))!;

				async Task Act()
				{
					await That(methodInfo).HasParameter<int>().Matching(p => p.Name == "count");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HasParamsParameter_WhenParamsParameterIsACollection_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.ParamsCollection))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter().Matching(p => !p.ParameterType.IsArray);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HasParamsParameter_WhenParamsParameterIsAnArray_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.ParamsArray))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter().Matching(p => !p.ParameterType.IsArray);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has a params parameter matching p => !p.ParameterType.IsArray,
					             but it did not
					             """);
			}

			[Test]
			public async Task HasRefParameter_WithAtIndex_WhenRefParameterIsElsewhere_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.Index))!;

				async Task Act()
				{
					await That(methodInfo).HasRefParameter().AtIndex(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has a ref parameter at index 0,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.Count))!;

				async Task Act()
				{
					await That(methodInfo).HasParameter<int>().Matching(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.Count))!;
				InvalidOperationException exception = new("boom");

				async Task Act()
				{
					await That(methodInfo).HasParameter<int>().Matching(_ => throw exception);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int matching _ => throw exception,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """).And
					.Whose(e => e.InnerException, inner => inner.IsSameAs(exception))
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class TestClass
		{
			public void ParamsArray(params int[] values)
			{
			}

			public void ParamsCollection(params List<int> values)
			{
			}

			public void Count(int count)
			{
			}

			public void Index(int index, ref int value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
