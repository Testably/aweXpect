using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class InheritsFrom
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenTypeDoesNotInherit_ShouldFail()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).InheritsFrom<BaseClass>();
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
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).InheritsFrom<ITestInterface>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatType.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Fact]
			public async Task WhenTypeInheritsDirectly_WithForceDirect_ShouldSucceed()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).InheritsFrom<BaseClass>(true);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInheritsFromBaseClass_ShouldSucceed()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).InheritsFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_ShouldSucceed()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
				{
					await That(subject).InheritsFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_WithForceDirect_ShouldFail()
			{
				Type subject = typeof(GrandChildClass);

				async Task Act()
				{
					await That(subject).InheritsFrom<BaseClass>(true);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             inherits directly from ThatType.BaseClass,
					             but it inherited from ThatType.BaseClass only indirectly

					             Actual:
					             ThatType.GrandChildClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsSameAsBaseType_ShouldFail()
			{
				Type subject = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).InheritsFrom<BaseClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatType.BaseClass,
					             but it did not inherit from ThatType.BaseClass

					             Actual:
					             ThatType.BaseClass
					             """);
			}

			[Fact]
			public async Task WhenTypeDoesNotInherit_WithForceDirect_ShouldReportNotInherited()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).InheritsFrom<BaseClass>(true);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             inherits directly from ThatType.BaseClass,
					             but it did not inherit from ThatType.BaseClass

					             Actual:
					             ThatType.UnrelatedClass
					             """);
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenTypeDoesNotInherit_ShouldFail()
			{
				Type subject = typeof(UnrelatedClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).InheritsFrom(baseType);
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
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).InheritsFrom(interfaceType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatType.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Fact]
			public async Task WhenTypeInheritsDirectly_WithForceDirect_ShouldSucceed()
			{
				Type subject = typeof(DerivedClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).InheritsFrom(baseType, true);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInheritsFromBaseClass_ShouldSucceed()
			{
				Type subject = typeof(DerivedClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).InheritsFrom(baseType);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeInheritsIndirectly_WithForceDirect_ShouldFail()
			{
				Type subject = typeof(GrandChildClass);
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).InheritsFrom(baseType, true);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             inherits directly from ThatType.BaseClass,
					             but it inherited from ThatType.BaseClass only indirectly

					             Actual:
					             ThatType.GrandChildClass
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeDoesNotInherit_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.InheritsFrom<BaseClass>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.InheritsFrom<ITestInterface>());
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatType.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Fact]
			public async Task WhenTypeInherits_ShouldFail()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.InheritsFrom<BaseClass>());
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

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.InheritsFrom<BaseClass>());
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
	}
}
