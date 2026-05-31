using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasRefParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorHasRefParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithRefParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasNoRefParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has a ref parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has a ref parameter,
					             but it was <null>
					             """);
			}
		}

		public sealed class TypedOverloadsTests
		{
			[Fact]
			public async Task GenericType_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericTypeAndName_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Name_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter with name "value" with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericExactType_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameterExactly<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task GenericExactTypeAndName_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameterExactly<int>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with ref modifier,
					             but it did not
					             """);
			}

#pragma warning disable CA2263
			[Fact]
			public async Task Type_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task TypeAndName_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value" with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactType_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task ExactTypeAndName_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameterExactly(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of exact type int with name "value" with ref modifier,
					             but it did not
					             """);
			}
#pragma warning restore CA2263

			[Fact]
			public async Task WhenOnlySomeParametersAreRef_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithMixedParameters).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorHasRefParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithRefParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasRefParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have a ref parameter,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenConstructorHasNoRefParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasRefParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithRefParameter
		{
			public ClassWithRefParameter(ref int value)
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
			public ClassWithMixedParameters(ref int value, int other)
			{
				value = 0;
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
