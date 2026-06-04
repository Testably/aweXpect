using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasParamsParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorHasNoParamsParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has a params parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenConstructorHasParamsParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithParamsParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has a params parameter,
					             but it was <null>
					             """);
			}
		}

		public sealed class TypedOverloadsTests
		{
			[Fact]
			public async Task GenericExactType_WhenParameterIsNotParams_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameterExactly<int[]>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int[] with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericExactTypeAndName_WhenParameterIsNotParams_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameterExactly<int[]>("values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int[] with name "values" with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericType_WhenParameterIsNotParams_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter<int[]>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int[] with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericTypeAndName_WhenParameterIsNotParams_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter<int[]>("values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int[] with name "values" with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Name_WhenParameterIsNotParams_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter("values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter with name "values" with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenOnlySomeParametersAreParams_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithMixedParameters).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task Type_WhenParameterIsNotParams_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter(typeof(int[]));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int[] with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task TypeAndName_WhenParameterIsNotParams_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter(typeof(int[]), "values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int[] with name "values" with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactType_WhenParameterIsNotParams_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameterExactly(typeof(int[]));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int[] with params modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactTypeAndName_WhenParameterIsNotParams_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int[] with name "values" with params modifier,
					             but it did not
					             """);
			}
#pragma warning restore CA2263
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorHasNoParamsParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParamsParameter());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasParamsParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithParamsParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParamsParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have a params parameter,
					             but it did
					             """);
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithParamsParameter
		{
			public ClassWithParamsParameter(params int[] values)
			{
			}
		}

		private class ClassWithoutModifiers
		{
			public ClassWithoutModifiers(int[] values)
			{
			}
		}

		private class ClassWithMixedParameters
		{
			public ClassWithMixedParameters(int value, params int[] values)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
