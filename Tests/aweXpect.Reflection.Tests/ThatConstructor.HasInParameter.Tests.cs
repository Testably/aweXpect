using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasInParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorHasInParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithInParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasNoInParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an in parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasInParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an in parameter,
					             but it was <null>
					             """);
			}
		}

		public sealed class TypedOverloadsTests
		{
			[Fact]
			public async Task GenericType_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with in modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericTypeAndName_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with in modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Name_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter with name "value" with in modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericExactType_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with in modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericExactTypeAndName_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with in modifier,
					             but it did not
					             """);
			}

#pragma warning disable CA2263
			[Fact]
			public async Task Type_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with in modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task TypeAndName_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with in modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactType_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with in modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactTypeAndName_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with in modifier,
					             but it did not
					             """);
			}
#pragma warning restore CA2263

			[Fact]
			public async Task WhenOnlySomeParametersAreIn_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithMixedParameters).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorHasInParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithInParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasInParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have an in parameter,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenConstructorHasNoInParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasInParameter());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task GenericType_WhenParameterIsIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithInParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasInParameter<int>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter of type int with in modifier,
					             but it did
					             """);
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithInParameter
		{
			public ClassWithInParameter(in int value)
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
			public ClassWithMixedParameters(in int value, int other)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
