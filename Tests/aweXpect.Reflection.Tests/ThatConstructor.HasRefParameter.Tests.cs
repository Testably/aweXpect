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
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
