using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasParamsParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodHasParamsParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has a params parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? methodInfo = null;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has a params parameter,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSomeButNotAllParametersAreParamsParameters_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMixedParameters))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Generic_WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter<int[]>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int[] with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericWithName_WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter<int[]>("values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int[] with name "values" with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task TypeWithName_WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter(typeof(int[]), "values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int[] with name "values" with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Name_WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter("values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter with name "values" with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericExactly_WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameterExactly<int[]>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int[] with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task TypeExactly_WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameterExactly(typeof(int[]));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int[] with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericExactlyWithName_WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameterExactly<int[]>("values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int[] with name "values" with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task TypeExactlyWithName_WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of exact type int[] with name "values" with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Type_WhenMethodHasParamsParameterOfType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_WhenMethodHasNoParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter(typeof(int[]));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int[] with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Type_WhenMethodHasParamsParameterOfTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task TypeExactly_WhenMethodHasParamsParameterOfExactType_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameterExactly(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task TypeExactly_WhenMethodHasParamsParameterOfExactTypeWithName_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!;

				async Task Act()
				{
					await That(methodInfo).HasParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodHasParamsParameter_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParamsParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have a params parameter,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenMethodHasNoParamsParameter_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!;

				async Task Act()
				{
					await That(methodInfo).DoesNotComplyWith(it => it.HasParamsParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithParamsParameter(params int[] values)
			{
			}

			public void MethodWithMixedParameters(int first, params int[] values)
			{
			}

			public void MethodWithoutModifiers(int[] values)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
