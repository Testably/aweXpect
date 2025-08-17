using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotInheritFrom
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenTypeDoesNotInherit_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom<BaseClass>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsInterface_ShouldFail()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom<ITestInterface>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.ITestInterface,
					             but it did inherit from ThatType.ITestInterface
					             """);
			}

			[Fact]
			public async Task WhenTypeInherits_ShouldFail()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom<BaseClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.BaseClass,
					             but it did inherit from ThatType.BaseClass
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsDirectly_WithForceDirect_ShouldFail()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom<BaseClass>(true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit directly from ThatType.BaseClass,
					             but it did inherit directly from ThatType.BaseClass
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_ShouldFail()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom<BaseClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.BaseClass,
					             but it did inherit from ThatType.BaseClass
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_WithForceDirect_ShouldSucceed()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom<BaseClass>(true);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenTypeDoesNotInherit_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom(baseType);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsInterface_ShouldFail()
			{
				Type subject = typeof(ClassWithInterface);
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom(interfaceType);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.ITestInterface,
					             but it did inherit from ThatType.ITestInterface
					             """);
			}

			[Fact]
			public async Task WhenTypeInherits_ShouldFail()
			{
				Type subject = typeof(DerivedClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom(baseType);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.BaseClass,
					             but it did inherit from ThatType.BaseClass
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_ShouldFail()
			{
				Type subject = typeof(GrandChildClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).DoesNotInheritFrom(baseType);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit from ThatType.BaseClass,
					             but it did inherit from ThatType.BaseClass
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeDoesNotInherit_ShouldFail()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotInheritFrom<BaseClass>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatType.BaseClass,
					             but it did not inherit from ThatType.BaseClass, but was ThatType.UnrelatedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeImplementsInterface_ShouldSucceed()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotInheritFrom<ITestInterface>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInherits_ShouldSucceed()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotInheritFrom<BaseClass>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_ShouldSucceed()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotInheritFrom<BaseClass>());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
