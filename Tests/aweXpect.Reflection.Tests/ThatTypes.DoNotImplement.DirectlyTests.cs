using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class DoNotImplement
	{
		public sealed class DirectlyTests
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
				{
					Filtered.Types subject = In.Types(typeof(UnrelatedClass));

					async Task Act()
					{
						await That(subject).DoNotImplement<BaseClass>().Directly();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check implementation of must be an interface, but it was ThatTypes.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
				}

				[Test]
				public async Task WhenNoTypeImplementsDirectly_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(DerivedFromClassWithInterface1), typeof(UnrelatedClass));

					async Task Act()
					{
						await That(subject).DoNotImplement<ITestInterface>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSomeTypeImplementsDirectly_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(DerivedFromClassWithInterface1));

					async Task Act()
					{
						await That(subject).DoNotImplement<ITestInterface>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.DerivedFromClassWithInterface1]
						             none directly implement ThatTypes.ITestInterface,
						             but 1 of 2 did

						             Matching items:
						             [
						               ThatTypes.ClassWithInterface1
						             ]

						             Collection:
						             [
						               ThatTypes.ClassWithInterface1,
						               ThatTypes.DerivedFromClassWithInterface1
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<Type?>? subject = null;

					async Task Act()
					{
						await That(subject).DoNotImplement<ITestInterface>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             none directly implement ThatTypes.ITestInterface,
						             but it was <null>
						             """);
				}
			}

			public sealed class TypeTests
			{
				[Test]
				public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
				{
					IEnumerable<Type?> subject = new[]
					{
						typeof(UnrelatedClass),
					};
					Type classType = typeof(BaseClass);

					async Task Act()
					{
						await That(subject).DoNotImplement(classType).Directly();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check implementation of must be an interface, but it was ThatTypes.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
				}

				[Test]
				public async Task WhenTypesImplementDirectly_ShouldFail()
				{
					IEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithInterface1),
					};
					Type interfaceType = typeof(ITestInterface);

					async Task Act()
					{
						await That(subject).DoNotImplement(interfaceType).Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             none directly implement ThatTypes.ITestInterface,
						             but 1 of 1 did

						             Matching items:
						             [
						               ThatTypes.ClassWithInterface1
						             ]

						             Collection:
						             [
						               ThatTypes.ClassWithInterface1
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypesImplementIndirectly_ShouldSucceed()
				{
					IEnumerable<Type?> subject = new[]
					{
						typeof(DerivedFromClassWithInterface1),
					};
					Type interfaceType = typeof(ITestInterface);

					async Task Act()
					{
						await That(subject).DoNotImplement(interfaceType).Directly();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenNoTypeImplementsDirectly_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(DerivedFromClassWithInterface1));

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(they => they.DoNotImplement<ITestInterface>().Directly());
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that in types [ThatTypes.DerivedFromClassWithInterface1]
						             at least one directly implements ThatTypes.ITestInterface,
						             but none of 1 did

						             Collection:
						             [
						               ThatTypes.DerivedFromClassWithInterface1
						             ]
						             """);
				}

				[Test]
				public async Task WhenSomeTypeImplementsDirectly_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(DerivedFromClassWithInterface1));

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(they => they.DoNotImplement<ITestInterface>().Directly());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
