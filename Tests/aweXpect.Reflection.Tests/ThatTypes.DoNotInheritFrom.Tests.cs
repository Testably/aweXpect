using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotInheritFrom
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenAllTypesImplementInterface_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
					=> await That(subject).DoNotInheritFrom<ITestInterface>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.ClassWithInterface2]
					             all do not inherit from ThatTypes.ITestInterface,
					             but it contained types that inherit from ThatTypes.ITestInterface [
					               ThatTypes.ClassWithInterface1,
					               ThatTypes.ClassWithInterface2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenAllTypesInherit_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

				async Task Act()
					=> await That(subject).DoNotInheritFrom<BaseClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.DerivedClass2]
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass [
					               ThatTypes.DerivedClass1,
					               ThatTypes.DerivedClass2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenAllTypesInheritIndirectly_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(GrandChildClass));

				async Task Act()
					=> await That(subject).DoNotInheritFrom<BaseClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.GrandChildClass]
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass [
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}

			[Fact]
			public async Task WhenAtLeastOneTypeInherits_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(UnrelatedClass));

				async Task Act()
					=> await That(subject).DoNotInheritFrom<BaseClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.UnrelatedClass]
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass [
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNoTypesInherit_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass), typeof(GenericTests));

				async Task Act()
					=> await That(subject).DoNotInheritFrom<BaseClass>();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenAllTypesImplementInterface_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface1), typeof(ClassWithInterface2),
				};
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
					=> await That(subject).DoNotInheritFrom(interfaceType);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit from ThatTypes.ITestInterface,
					             but it contained types that inherit from ThatTypes.ITestInterface [
					               ThatTypes.ClassWithInterface1,
					               ThatTypes.ClassWithInterface2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenAllTypesInherit_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(DerivedClass2),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).DoNotInheritFrom(baseType);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass [
					               ThatTypes.DerivedClass1,
					               ThatTypes.DerivedClass2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenAllTypesInheritIndirectly_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(GrandChildClass),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).DoNotInheritFrom(baseType);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass [
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}

			[Fact]
			public async Task WhenAtLeastOneTypeInherits_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(UnrelatedClass),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).DoNotInheritFrom(baseType);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass [
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNoTypesInherit_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass), typeof(GenericTests));
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).DoNotInheritFrom(baseType);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllTypesDoNotInherit_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(NegatedTests), typeof(UnrelatedClass));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.DoNotInheritFrom<BaseClass>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DoNotInheritFrom.NegatedTests, ThatTypes.UnrelatedClass]
					             at least one inherits from ThatTypes.BaseClass,
					             but it only contained types that do not inherit from ThatTypes.BaseClass [
					               ThatTypes.DoNotInheritFrom.NegatedTests,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Fact]
			public async Task WhenAllTypesImplementInterface_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.DoNotInheritFrom<ITestInterface>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllTypesInherit_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.DoNotInheritFrom<BaseClass>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllTypesInheritIndirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(GrandChildClass));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.DoNotInheritFrom<BaseClass>());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
