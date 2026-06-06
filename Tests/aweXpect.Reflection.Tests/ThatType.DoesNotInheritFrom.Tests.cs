using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotInheritFrom
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom<ITestInterface>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatType.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Fact]
			public async Task WhenTypeDoesNotInherit_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInherits_ShouldFail()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom<BaseClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.BaseClass,
					             but it did inherit from ThatType.BaseClass

					             Actual:
					             ThatType.DerivedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsDirectly_WithForceDirect_ShouldFail()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom<BaseClass>(true);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit directly from ThatType.BaseClass,
					             but it did inherit directly from ThatType.BaseClass

					             Actual:
					             ThatType.DerivedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_ShouldFail()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom<BaseClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.BaseClass,
					             but it did inherit from ThatType.BaseClass

					             Actual:
					             ThatType.GrandChildClass
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_WithForceDirect_ShouldSucceed()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom<BaseClass>(true);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom(interfaceType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatType.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Fact]
			public async Task WhenTypeDoesNotInherit_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom(baseType);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInherits_ShouldFail()
			{
				Type subject = typeof(DerivedClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom(baseType);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.BaseClass,
					             but it did inherit from ThatType.BaseClass

					             Actual:
					             ThatType.DerivedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_ShouldFail()
			{
				Type subject = typeof(GrandChildClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritFrom(baseType);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.BaseClass,
					             but it did inherit from ThatType.BaseClass

					             Actual:
					             ThatType.GrandChildClass
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotInheritFrom<ITestInterface>());
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatType.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Fact]
			public async Task WhenTypeDoesNotInherit_ShouldFail()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotInheritFrom<BaseClass>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatType.BaseClass,
					             but it did not inherit from ThatType.BaseClass

					             Actual:
					             ThatType.UnrelatedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeInherits_ShouldSucceed()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotInheritFrom<BaseClass>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_ShouldSucceed()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotInheritFrom<BaseClass>());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
