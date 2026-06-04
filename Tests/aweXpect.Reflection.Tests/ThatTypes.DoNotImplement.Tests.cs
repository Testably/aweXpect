using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotImplement
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenAllTypesImplementInterface_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).DoNotImplement<ITestInterface>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.ClassWithInterface2]
					             all do not implement ThatTypes.ITestInterface,
					             but it contained types that implement ThatTypes.ITestInterface [
					               ThatTypes.ClassWithInterface1,
					               ThatTypes.ClassWithInterface2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableTypesDoNotImplementInterface_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass), typeof(BaseClass),
				};

				async Task Act()
				{
					await That(subject).DoNotImplement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoTypeImplementsInterface_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass), typeof(BaseClass));

				async Task Act()
				{
					await That(subject).DoNotImplement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsIndirectly_WithForceDirect_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(DerivedFromClassWithInterface1));

				async Task Act()
				{
					await That(subject).DoNotImplement<ITestInterface>(true);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass),
				};
				Type classType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoNotImplement(classType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was ThatTypes.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
			}

			[Fact]
			public async Task WhenNoTypeImplementsInterface_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass), typeof(BaseClass),
				};
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoNotImplement(interfaceType);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenNoTypeImplements_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotImplement<ITestInterface>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.UnrelatedClass]
					             at least one implements ThatTypes.ITestInterface,
					             but it only contained types that do not implement ThatTypes.ITestInterface [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}
	}
}
