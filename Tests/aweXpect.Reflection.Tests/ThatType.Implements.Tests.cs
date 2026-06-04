using System;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class Implements
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenTypeImplementsInterfaceDirectly_ShouldSucceed()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).Implements<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsInterfaceIndirectly_ShouldSucceed()
			{
				Type subject = typeof(DerivedFromClassWithInterface);

				async Task Act()
				{
					await That(subject).Implements<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsDirectly_WithForceDirect_ShouldSucceed()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).Implements<ITestInterface>(true);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsIndirectly_WithForceDirect_ShouldFail()
			{
				Type subject = typeof(DerivedFromClassWithInterface);

				async Task Act()
				{
					await That(subject).Implements<ITestInterface>(true);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             directly implements ThatType.ITestInterface,
					             but it implemented ThatType.ITestInterface only indirectly

					             Actual:
					             ThatType.DerivedFromClassWithInterface
					             """);
			}

			[Fact]
			public async Task WhenTypeImplementsDerivedInterfaceDirectly_WithForceDirect_ShouldSucceed()
			{
				Type subject = typeof(ClassWithDerivedInterface);

				async Task Act()
				{
					await That(subject).Implements<IDerivedTestInterface>(true);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInterfaceIsInheritedViaAnotherInterface_WithForceDirect_ShouldFail()
			{
				Type subject = typeof(ClassWithDerivedInterface);

				async Task Act()
				{
					await That(subject).Implements<ITestInterface>(true);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             directly implements ThatType.ITestInterface,
					             but it implemented ThatType.ITestInterface only indirectly

					             Actual:
					             ThatType.ClassWithDerivedInterface
					             """);
			}

			[Fact]
			public async Task WhenInterfaceIsInheritedViaAnotherInterface_WithoutForceDirect_ShouldSucceed()
			{
				Type subject = typeof(ClassWithDerivedInterface);

				async Task Act()
				{
					await That(subject).Implements<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeDoesNotImplementInterface_ShouldFail()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).Implements<ITestInterface>();
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
			public async Task WhenTypeDoesNotImplementInterface_WithForceDirect_ShouldReportNotImplemented()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).Implements<ITestInterface>(true);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             directly implements ThatType.ITestInterface,
					             but it did not implement ThatType.ITestInterface

					             Actual:
					             ThatType.UnrelatedClass
					             """);
			}

			[Fact]
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).Implements<BaseClass>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was ThatType.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenTypeImplementsInterface_ShouldSucceed()
			{
				Type subject = typeof(ClassWithInterface);
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).Implements(interfaceType);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeDoesNotImplementInterface_ShouldFail()
			{
				Type subject = typeof(UnrelatedClass);
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).Implements(interfaceType);
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
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);
				Type classType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).Implements(classType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was ThatType.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeDoesNotImplement_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Implements<ITestInterface>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplements_ShouldFail()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Implements<ITestInterface>());
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
	}
}
