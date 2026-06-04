using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasOutParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorHasNoOutParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an out parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenConstructorHasOutParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithOutParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an out parameter,
					             but it was <null>
					             """);
			}
		}

		public sealed class TypedOverloadsTests
		{
			[Fact]
			public async Task GenericExactType_WhenParameterIsNotOut_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameterExactly<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with out modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericExactTypeAndName_WhenParameterIsNotOut_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameterExactly<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with out modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericType_WhenParameterIsNotOut_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with out modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericTypeAndName_WhenParameterIsNotOut_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with out modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Name_WhenParameterIsNotOut_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter with name "value" with out modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenOnlySomeParametersAreOut_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithMixedParameters).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter();
				}

				await That(Act).DoesNotThrow();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task Type_WhenParameterIsNotOut_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with out modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task TypeAndName_WhenParameterIsNotOut_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with out modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactType_WhenParameterIsNotOut_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with out modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactTypeAndName_WhenParameterIsNotOut_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOutParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with out modifier,
					             but it did not
					             """);
			}
#pragma warning restore CA2263
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorHasNoOutParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasOutParameter());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasOutParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithOutParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasOutParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have an out parameter,
					             but it did
					             """);
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithOutParameter
		{
			public ClassWithOutParameter(out int value)
			{
				value = 0;
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
			public ClassWithMixedParameters(out int value, int other)
			{
				value = 0;
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
