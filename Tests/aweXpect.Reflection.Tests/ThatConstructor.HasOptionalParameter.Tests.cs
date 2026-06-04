using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasOptionalParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorHasNoOptionalParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an optional parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenConstructorHasOptionalParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithOptionalParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an optional parameter,
					             but it was <null>
					             """);
			}
		}

		public sealed class TypedOverloadsTests
		{
			[Fact]
			public async Task GenericExactType_WhenParameterIsNotOptional_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameterExactly<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with optional modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericExactTypeAndName_WhenParameterIsNotOptional_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameterExactly<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with optional modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericType_WhenParameterIsNotOptional_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with optional modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericTypeAndName_WhenParameterIsNotOptional_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with optional modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Name_WhenParameterIsNotOptional_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter with name "value" with optional modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenOnlySomeParametersAreOptional_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithMixedParameters).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task Type_WhenParameterIsNotOptional_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with optional modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task TypeAndName_WhenParameterIsNotOptional_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with optional modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactType_WhenParameterIsNotOptional_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with optional modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactTypeAndName_WhenParameterIsNotOptional_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with optional modifier,
					             but it did not
					             """);
			}
#pragma warning restore CA2263
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorHasNoOptionalParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasOptionalParameter());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasOptionalParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithOptionalParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasOptionalParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have an optional parameter,
					             but it did
					             """);
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithOptionalParameter
		{
			public ClassWithOptionalParameter(int value = 0)
			{
			}
		}

		private class ClassWithoutModifiers
		{
			public ClassWithoutModifiers(int value)
			{
			}
		}

		private class ClassWithMixedParameters
		{
			public ClassWithMixedParameters(int value, int other = 0)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
