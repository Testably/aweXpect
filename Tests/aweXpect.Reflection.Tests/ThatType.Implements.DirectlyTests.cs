namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed partial class Implements
	{
		public sealed class DirectlyTests
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task WhenInterfaceIsInheritedViaAnotherInterface_ShouldFail()
				{
					Type subject = typeof(ClassWithDerivedInterface);

					async Task Act()
					{
						await That(subject).Implements<ITestInterface>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             directly implements ThatType.ITestInterface,
						             but it did only indirectly

						             Actual:
						             ThatType.ClassWithDerivedInterface
						             """);
				}

				[Test]
				public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
				{
					Type subject = typeof(ClassWithInterface);

					async Task Act()
					{
						await That(subject).Implements<BaseClass>().Directly();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check implementation of must be an interface, but it was ThatType.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
				}

				[Test]
				public async Task WhenTypeDoesNotImplementInterface_ShouldFail()
				{
					Type subject = typeof(UnrelatedClass);

					async Task Act()
					{
						await That(subject).Implements<ITestInterface>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             directly implements ThatType.ITestInterface,
						             but it did not

						             Actual:
						             ThatType.UnrelatedClass
						             """);
				}

				[Test]
				public async Task WhenTypeImplementsDerivedInterfaceDirectly_ShouldSucceed()
				{
					Type subject = typeof(ClassWithDerivedInterface);

					async Task Act()
					{
						await That(subject).Implements<IDerivedTestInterface>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeImplementsDirectly_ShouldSucceed()
				{
					Type subject = typeof(ClassWithInterface);

					async Task Act()
					{
						await That(subject).Implements<ITestInterface>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeImplementsIndirectly_ShouldFail()
				{
					Type subject = typeof(DerivedFromClassWithInterface);

					async Task Act()
					{
						await That(subject).Implements<ITestInterface>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             directly implements ThatType.ITestInterface,
						             but it did only indirectly

						             Actual:
						             ThatType.DerivedFromClassWithInterface
						             """);
				}

				[Test]
				public async Task WhenTypeIsNull_ShouldFail()
				{
					Type? subject = null;

					async Task Act()
					{
						await That(subject).Implements<ITestInterface>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             directly implements ThatType.ITestInterface,
						             but it was <null>
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
						await That(subject).Implements(classType).Directly();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check implementation of must be an interface, but it was ThatType.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
				}

				[Test]
				public async Task WhenInterfaceTypeIsNull_ShouldThrowArgumentNullException()
				{
					Type subject = typeof(ClassWithInterface);

					async Task Act()
					{
						await That(subject).Implements(null!).Directly();
					}

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("interfaceType").And
						.WithMessage("The 'interfaceType' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenTypeImplementsDirectly_ShouldSucceed()
				{
					Type subject = typeof(ClassWithInterface);
					Type interfaceType = typeof(ITestInterface);

					async Task Act()
					{
						await That(subject).Implements(interfaceType).Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeImplementsIndirectly_ShouldFail()
				{
					Type subject = typeof(DerivedFromClassWithInterface);
					Type interfaceType = typeof(ITestInterface);

					async Task Act()
					{
						await That(subject).Implements(interfaceType).Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             directly implements ThatType.ITestInterface,
						             but it did only indirectly

						             Actual:
						             ThatType.DerivedFromClassWithInterface
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenTypeImplementsDirectly_ShouldFail()
				{
					Type subject = typeof(ClassWithInterface);

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(it => it.Implements<ITestInterface>().Directly());
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
				public async Task WhenTypeImplementsIndirectly_ShouldSucceed()
				{
					Type subject = typeof(DerivedFromClassWithInterface);

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(it => it.Implements<ITestInterface>().Directly());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
