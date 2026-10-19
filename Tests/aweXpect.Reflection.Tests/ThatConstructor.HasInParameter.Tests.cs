using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasInParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenConstructorHasInParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithInParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorHasNoInParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an in parameter,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasInParameter();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an in parameter,
					             but it was <null>
					             """);
			}
		}

		public sealed class TypedOverloadsTests
		{
			[Test]
			public async Task GenericExactType_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with in modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task GenericExactTypeAndName_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with in modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task GenericType_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with in modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task GenericTypeAndName_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with in modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task Name_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter with name "value" with in modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenOnlySomeParametersAreIn_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithMixedParameters).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter();
				}

				await That(Act).DoesNotThrow();
			}

#pragma warning disable CA2263
			[Test]
			public async Task Type_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with in modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task TypeAndName_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameter(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with in modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task ExactType_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly(typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with in modifier,
					             but it did not
					             """);
			}

			[Test]
			public async Task ExactTypeAndName_WhenParameterIsNotIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasInParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with in modifier,
					             but it did not
					             """);
			}
#pragma warning restore CA2263
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task GenericType_WhenParameterIsIn_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithInParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasInParameter<int>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter of type int with in modifier,
					             but it did
					             """);
			}

			[Test]
			public async Task WhenConstructorHasInParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithInParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasInParameter());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have an in parameter,
					             but it did
					             """);
			}

			[Test]
			public async Task WhenConstructorHasNoInParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasInParameter());
				}

				await That(Act).DoesNotThrow();
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
