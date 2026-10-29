using System.Collections.Generic;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class Matching
	{
		public sealed class Tests
		{
			[Test]
			public async Task DoNotHaveParameter_WhenAParameterMatches_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.Count))!,
					typeof(TestClass).GetMethod(nameof(TestClass.Index))!,
				};

				async Task Act()
				{
					await That(methods).DoNotHaveParameter<int>().Matching(p => p.Name == "count");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             none have parameter of type int matching p => p.Name == "count",
					             but 1 of 2 did

					             Matching items:
					             [
					               void ThatMethods.Matching.TestClass.Count(int count)
					             ]

					             Collection:
					             [
					               void ThatMethods.Matching.TestClass.Count(int count),
					               void ThatMethods.Matching.TestClass.Index(int index, string value)
					             ]
					             """);
			}

			[Test]
			public async Task HaveParameter_WhenAllParametersMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.Count))!,
					typeof(TestClass).GetMethod(nameof(TestClass.Index))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameter<int>().Matching(p => p.Position == 0);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveParameter_WhenNotAllParametersMatch_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.Count))!,
					typeof(TestClass).GetMethod(nameof(TestClass.Index))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameter<int>().Matching(p => p.Name == "count");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int matching p => p.Name == "count",
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.Matching.TestClass.Index(int index, string value)
					             ]

					             Collection:
					             [
					               void ThatMethods.Matching.TestClass.Count(int count),
					               void ThatMethods.Matching.TestClass.Index(int index, string value)
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task HaveParamsParameter_ForAsyncEnumerable_ShouldSupportMatching()
			{
				IAsyncEnumerable<MethodInfo?> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.ParamsArray))!);

				async Task Act()
				{
					await That(methods).HaveParamsParameter().Matching(p => p.ParameterType.IsArray);
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task HaveParamsParameter_WhenNotAllAreParamsCollections_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.ParamsArray))!,
					typeof(TestClass).GetMethod(nameof(TestClass.ParamsCollection))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameter().Matching(p => !p.ParameterType.IsArray);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have a params parameter matching p => !p.ParameterType.IsArray,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.Matching.TestClass.ParamsArray(params int[] values)
					             ]

					             Collection:
					             [
					               void ThatMethods.Matching.TestClass.ParamsArray(params int[] values),
					               void ThatMethods.Matching.TestClass.ParamsCollection(params List<int> values)
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			private static async IAsyncEnumerable<MethodInfo?> ToAsyncEnumerable(params MethodInfo?[] items)
			{
				foreach (MethodInfo? item in items)
				{
					yield return item;
				}

				await Task.CompletedTask;
			}
#endif
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

			public void Index(int index, string value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
