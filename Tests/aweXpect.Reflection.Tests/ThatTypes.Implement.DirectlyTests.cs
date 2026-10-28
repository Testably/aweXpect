using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class Implement
	{
		public sealed class DirectlyTests
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task WhenAllTypesImplementDirectly_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

					async Task Act()
					{
						await That(subject).Implement<ITestInterface>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
				{
					Filtered.Types subject = In.Types(typeof(ClassWithInterface1));

					async Task Act()
					{
						await That(subject).Implement<BaseClass>().Directly();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check implementation of must be an interface, but it was ThatTypes.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
				}

				[Test]
				public async Task WhenSomeTypeImplementsIndirectly_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(DerivedFromClassWithInterface1));

					async Task Act()
					{
						await That(subject).Implement<ITestInterface>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.DerivedFromClassWithInterface1]
						             all directly implement ThatTypes.ITestInterface,
						             but only 1 of 2 did

						             Not matching items:
						             [
						               ThatTypes.DerivedFromClassWithInterface1
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
						await That(subject).Implement<ITestInterface>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all directly implement ThatTypes.ITestInterface,
						             but it was <null>
						             """);
				}
			}

			public sealed class TypeTests
			{
				[Test]
				public async Task WhenAllTypesImplementDirectly_ShouldSucceed()
				{
					IEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithInterface1), typeof(ClassWithInterface2),
					};
					Type interfaceType = typeof(ITestInterface);

					async Task Act()
					{
						await That(subject).Implement(interfaceType).Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
				{
					IEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithInterface1),
					};
					Type classType = typeof(BaseClass);

					async Task Act()
					{
						await That(subject).Implement(classType).Directly();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check implementation of must be an interface, but it was ThatTypes.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
				}

				[Test]
				public async Task WhenTypesImplementIndirectly_ShouldFail()
				{
					IEnumerable<Type?> subject = new[]
					{
						typeof(DerivedFromClassWithInterface1),
					};
					Type interfaceType = typeof(ITestInterface);

					async Task Act()
					{
						await That(subject).Implement(interfaceType).Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all directly implement ThatTypes.ITestInterface,
						             but none of 1 did

						             Not matching items:
						             [
						               ThatTypes.DerivedFromClassWithInterface1
						             ]

						             Collection:
						             [
						               ThatTypes.DerivedFromClassWithInterface1
						             ]
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenAllTypesImplementDirectly_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(they => they.Implement<ITestInterface>().Directly());
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.ClassWithInterface2]
						             not all directly implement ThatTypes.ITestInterface,
						             but all 2 did

						             Collection:
						             [
						               ThatTypes.ClassWithInterface1,
						               ThatTypes.ClassWithInterface2
						             ]
						             """);
				}

				[Test]
				public async Task WhenSomeTypeImplementsIndirectly_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(DerivedFromClassWithInterface1));

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(they => they.Implement<ITestInterface>().Directly());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
