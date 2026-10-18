namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotImplement
	{
		public sealed class GenericTests
		{
			[Test]
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

			[Test]
			public async Task WhenTypeDoesNotImplementInterface_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotImplement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeImplementsDirectly_WithForceDirect_ShouldFail()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement<ITestInterface>(true);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not directly implement ThatType.ITestInterface,
					             but it did

					             Actual:
					             ThatType.ClassWithInterface
					             """);
			}

			[Test]
			public async Task WhenTypeImplementsIndirectly_WithForceDirect_ShouldSucceed()
			{
				Type subject = typeof(DerivedFromClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement<ITestInterface>(true);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeImplementsInterface_ShouldFail()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement<ITestInterface>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not implement ThatType.ITestInterface,
					             but it did

					             Actual:
					             ThatType.ClassWithInterface
					             """);
			}
		}

		public sealed class TypeTests
		{
			[Test]
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

			[Test]
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

			[Test]
			public async Task WhenTypeImplementsInterface_ShouldFail()
			{
				Type subject = typeof(ClassWithInterface);
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoesNotImplement(interfaceType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not implement ThatType.ITestInterface,
					             but it did

					             Actual:
					             ThatType.ClassWithInterface
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeDoesNotImplement_ShouldFail()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotImplement<ITestInterface>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             implements ThatType.ITestInterface,
					             but it did not

					             Actual:
					             ThatType.UnrelatedClass
					             """);
			}

			[Test]
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
