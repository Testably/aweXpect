using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class InheritFrom
	{
		public sealed class DirectlyTests
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task WhenAllTypesInheritDirectly_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

					async Task Act()
					{
						await That(subject).InheritFrom<BaseClass>().Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
				{
					Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

					async Task Act()
					{
						await That(subject).InheritFrom<ITestInterface>().Directly();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
				}

				[Test]
				public async Task WhenSomeTypesInheritIndirectly_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(GrandChildClass));

					async Task Act()
					{
						await That(subject).InheritFrom<BaseClass>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.GrandChildClass]
						             all inherit directly from ThatTypes.BaseClass,
						             but only 1 of 2 did

						             Not matching items:
						             [
						               ThatTypes.GrandChildClass
						             ]

						             Collection:
						             [
						               ThatTypes.DerivedClass1,
						               ThatTypes.GrandChildClass
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<Type?>? subject = null;

					async Task Act()
					{
						await That(subject).InheritFrom<BaseClass>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all inherit directly from ThatTypes.BaseClass,
						             but it was <null>
						             """);
				}
			}

			public sealed class TypeTests
			{
				[Test]
				public async Task WhenAllTypesInheritDirectly_ShouldSucceed()
				{
					IEnumerable<Type?> subject = new[]
					{
						typeof(DerivedClass1), typeof(DerivedClass2),
					};
					Type baseType = typeof(BaseClass);

					async Task Act()
					{
						await That(subject).InheritFrom(baseType).Directly();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
				{
					IEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithInterface1),
					};
					Type interfaceType = typeof(ITestInterface);

					async Task Act()
					{
						await That(subject).InheritFrom(interfaceType).Directly();
					}

					await That(Act).Throws<ArgumentException>()
						.WithMessage(
							"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
				}

				[Test]
				public async Task WhenTypesInheritIndirectly_ShouldFail()
				{
					IEnumerable<Type?> subject = new[]
					{
						typeof(GrandChildClass),
					};
					Type baseType = typeof(BaseClass);

					async Task Act()
					{
						await That(subject).InheritFrom(baseType).Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all inherit directly from ThatTypes.BaseClass,
						             but none of 1 did

						             Not matching items:
						             [
						               ThatTypes.GrandChildClass
						             ]

						             Collection:
						             [
						               ThatTypes.GrandChildClass
						             ]
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenAllTypesInheritDirectly_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(they => they.InheritFrom<BaseClass>().Directly());
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.DerivedClass2]
						             not all inherit directly from ThatTypes.BaseClass,
						             but all 2 did

						             Collection:
						             [
						               ThatTypes.DerivedClass1,
						               ThatTypes.DerivedClass2
						             ]
						             """);
				}

				[Test]
				public async Task WhenSomeTypesInheritIndirectly_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(GrandChildClass));

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(they => they.InheritFrom<BaseClass>().Directly());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
