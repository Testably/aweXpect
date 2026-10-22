namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotInheritDirectlyFrom
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotInheritDirectlyFrom<ITestInterface>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatType.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenTypeDoesNotInherit_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritDirectlyFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeInheritsDirectly_ShouldFail()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritDirectlyFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit directly from ThatType.BaseClass,
					             but it did

					             Actual:
					             ThatType.DerivedClass
					             """);
			}

			[Test]
			public async Task WhenTypeInheritsIndirectly_ShouldSucceed()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritDirectlyFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotInheritDirectlyFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit directly from ThatType.BaseClass,
					             but it was <null>
					             """);
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoesNotInheritDirectlyFrom(interfaceType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatType.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenBaseTypeIsNull_ShouldThrowArgumentNullException()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotInheritDirectlyFrom(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("baseType").And
					.WithMessage("The 'baseType' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenTypeInheritsDirectly_ShouldFail()
			{
				Type subject = typeof(DerivedClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritDirectlyFrom(baseType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not inherit directly from ThatType.BaseClass,
					             but it did

					             Actual:
					             ThatType.DerivedClass
					             """);
			}

			[Test]
			public async Task WhenTypeInheritsIndirectly_ShouldSucceed()
			{
				Type subject = typeof(GrandChildClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoesNotInheritDirectlyFrom(baseType);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeInheritsDirectly_ShouldSucceed()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotInheritDirectlyFrom<BaseClass>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeInheritsIndirectly_ShouldFail()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotInheritDirectlyFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits directly from ThatType.BaseClass,
					             but it did only indirectly

					             Actual:
					             ThatType.GrandChildClass
					             """);
			}
		}
	}
}
