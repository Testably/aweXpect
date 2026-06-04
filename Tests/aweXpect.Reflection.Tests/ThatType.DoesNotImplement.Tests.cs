using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotImplement
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement<BaseClass>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was ThatType.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
			}

			[Fact]
			public async Task WhenTypeDoesNotImplementInterface_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotImplement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsDirectly_WithForceDirect_ShouldFail()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement<ITestInterface>(true);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not directly implement ThatType.ITestInterface,
					             but it did directly implement ThatType.ITestInterface

					             Actual:
					             ThatType.ClassWithInterface
					             """);
			}

			[Fact]
			public async Task WhenTypeImplementsIndirectly_WithForceDirect_ShouldSucceed()
			{
				Type subject = typeof(DerivedFromClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement<ITestInterface>(true);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsInterface_ShouldFail()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement<ITestInterface>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not implement ThatType.ITestInterface,
					             but it did implement ThatType.ITestInterface

					             Actual:
					             ThatType.ClassWithInterface
					             """);
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);
				Type classType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoesNotImplement(classType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was ThatType.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
			}

			[Fact]
			public async Task WhenTypeDoesNotImplementInterface_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement(interfaceType);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsInterface_ShouldFail()
			{
				Type subject = typeof(ClassWithInterface);
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement(interfaceType);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not implement ThatType.ITestInterface,
					             but it did implement ThatType.ITestInterface

					             Actual:
					             ThatType.ClassWithInterface
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeDoesNotImplement_ShouldFail()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotImplement<ITestInterface>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             implements ThatType.ITestInterface,
					             but it did not implement ThatType.ITestInterface

					             Actual:
					             ThatType.UnrelatedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeImplements_ShouldSucceed()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotImplement<ITestInterface>());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
