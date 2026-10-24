namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed partial class InheritsFrom
	{
		public sealed class DirectlyTests
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
				{
					Type subject = typeof(ClassWithInterface);

					async Task Act()
					{
						await That(subject).InheritsFrom<ITestInterface>().Directly();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check inheritance from must be a class, but it was the interface ThatType.ITestInterface. Use 'Implements' to check for interface implementations.");
				}

				[Test]
				public async Task WhenTypeDoesNotInherit_ShouldFail()
				{
					Type subject = typeof(UnrelatedClass);

					async Task Act()
					{
						await That(subject).InheritsFrom<BaseClass>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             inherits directly from ThatType.BaseClass,
						             but it did not

						             Actual:
						             ThatType.UnrelatedClass
						             """);
				}

				[Test]
				public async Task WhenTypeInheritsDirectly_ShouldSucceed()
				{
					Type subject = typeof(DerivedClass);

					async Task Act()
					{
						await That(subject).InheritsFrom<BaseClass>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeInheritsIndirectly_ShouldFail()
				{
					Type subject = typeof(GrandChildClass);

					async Task Act()
					{
						await That(subject).InheritsFrom<BaseClass>().Directly();
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

				[Test]
				public async Task WhenTypeIsNull_ShouldFail()
				{
					Type? subject = null;

					async Task Act()
					{
						await That(subject).InheritsFrom<BaseClass>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             inherits directly from ThatType.BaseClass,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenTypeIsSameAsBaseType_ShouldFail()
				{
					Type subject = typeof(BaseClass);

					async Task Act()
					{
						await That(subject).InheritsFrom<BaseClass>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             inherits directly from ThatType.BaseClass,
						             but it did not

						             Actual:
						             ThatType.BaseClass
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
						await That(subject).InheritsFrom(interfaceType).Directly();
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
						await That(subject).InheritsFrom(null!).Directly();
					}

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("baseType").And
						.WithMessage("The 'baseType' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenTypeInheritsDirectly_ShouldSucceed()
				{
					Type subject = typeof(DerivedClass);
					Type baseType = typeof(BaseClass);

					async Task Act()
					{
						await That(subject).InheritsFrom(baseType).Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeInheritsIndirectly_ShouldFail()
				{
					Type subject = typeof(GrandChildClass);
					Type baseType = typeof(BaseClass);

					async Task Act()
					{
						await That(subject).InheritsFrom(baseType).Directly();
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

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenTypeInheritsDirectly_ShouldFail()
				{
					Type subject = typeof(DerivedClass);

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(it => it.InheritsFrom<BaseClass>().Directly());
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
						await That(subject).DoesNotComplyWith(it => it.InheritsFrom<BaseClass>().Directly());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
