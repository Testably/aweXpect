using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class Implement
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenAllTypesImplementInterface_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).Implement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableTypesImplementInterface_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface1), typeof(ClassWithInterface2),
				};

				async Task Act()
				{
					await That(subject).Implement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeDoesNotImplement_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).Implement<ITestInterface>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.UnrelatedClass]
					             all implement ThatTypes.ITestInterface,
					             but it contained types that do not implement ThatTypes.ITestInterface [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSomeTypeImplementsIndirectly_WithForceDirect_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedFromClassWithInterface1));

				async Task Act()
				{
					await That(subject).Implement<ITestInterface>(true);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedFromClassWithInterface1]
					             all directly implement ThatTypes.ITestInterface,
					             but it contained types that do not directly implement ThatTypes.ITestInterface [
					               ThatTypes.DerivedFromClassWithInterface1
					             ]
					             """);
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenAllTypesImplementInterface_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface1), typeof(ClassWithInterface2),
				};
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).Implement(interfaceType);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface1),
				};
				Type classType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).Implement(classType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was ThatTypes.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllTypesImplement_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Implement<ITestInterface>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.ClassWithInterface2]
					             not all implement ThatTypes.ITestInterface,
					             but it only contained types that implement ThatTypes.ITestInterface [
					               ThatTypes.ClassWithInterface1,
					               ThatTypes.ClassWithInterface2
					             ]
					             """);
			}
		}
	}
}
