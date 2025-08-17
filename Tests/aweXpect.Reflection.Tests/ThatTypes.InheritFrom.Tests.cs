using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class InheritFrom
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenAllTypesImplementInterface_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(ClassWithInterface1) || type == typeof(ClassWithInterface2));

				async Task Act()
					=> await That(subject).InheritFrom<ITestInterface>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllTypesInheritFromBaseClass_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(DerivedClass1) || type == typeof(DerivedClass2));

				async Task Act()
					=> await That(subject).InheritFrom<BaseClass>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypesDoNotInherit_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(DerivedClass1) || type == typeof(UnrelatedClass));

				async Task Act()
					=> await That(subject).InheritFrom<BaseClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(DerivedClass1) || type == typeof(UnrelatedClass) in assembly containing type ThatTypes.InheritFrom.GenericTests
					             all inherit from ThatTypes.BaseClass,
					             but it contained types that do not inherit from ThatTypes.BaseClass [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypesInheritDirectly_WithForceDirect_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(DerivedClass1) || type == typeof(DerivedClass2));

				async Task Act()
					=> await That(subject).InheritFrom<BaseClass>(true);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesInheritIndirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(GrandChildClass));

				async Task Act()
					=> await That(subject).InheritFrom<BaseClass>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesInheritIndirectly_WithForceDirect_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(GrandChildClass));

				async Task Act()
					=> await That(subject).InheritFrom<BaseClass>(true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(GrandChildClass) in assembly containing type ThatTypes.InheritFrom.GenericTests
					             all inherit directly from ThatTypes.BaseClass,
					             but it contained types that do not inherit directly from ThatTypes.BaseClass [
					               ThatTypes.GrandChildClass
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
					=> await That(subject).InheritFrom(interfaceType);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllTypesInheritFromBaseClass_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(DerivedClass2),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).InheritFrom(baseType);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypesDoNotInherit_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(UnrelatedClass),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).InheritFrom(baseType);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all inherit from ThatTypes.BaseClass,
					             but it contained types that do not inherit from ThatTypes.BaseClass [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypesInheritDirectly_WithForceDirect_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(DerivedClass2),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).InheritFrom(baseType, true);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesInheritIndirectly_WithForceDirect_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(GrandChildClass),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
					=> await That(subject).InheritFrom(baseType, true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all inherit directly from ThatTypes.BaseClass,
					             but it contained types that do not inherit directly from ThatTypes.BaseClass [
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllTypesImplementInterface_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<NegatedTests>()
					.Types()
					.Which(type => type == typeof(ClassWithInterface1) || type == typeof(ClassWithInterface2));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.InheritFrom<ITestInterface>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(ClassWithInterface1) || type == typeof(ClassWithInterface2) in assembly containing type ThatTypes.InheritFrom.NegatedTests
					             not all inherit from ThatTypes.ITestInterface,
					             but it only contained types that inherit from ThatTypes.ITestInterface [
					               ThatTypes.ClassWithInterface1,
					               ThatTypes.ClassWithInterface2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenAllTypesInheritFromBaseClass_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<NegatedTests>()
					.Types()
					.Which(type => type == typeof(DerivedClass1) || type == typeof(DerivedClass2));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.InheritFrom<BaseClass>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(DerivedClass1) || type == typeof(DerivedClass2) in assembly containing type ThatTypes.InheritFrom.NegatedTests
					             not all inherit from ThatTypes.BaseClass,
					             but it only contained types that inherit from ThatTypes.BaseClass [
					               ThatTypes.DerivedClass1,
					               ThatTypes.DerivedClass2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSomeTypesDoNotInherit_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<NegatedTests>()
					.Types()
					.Which(type => type == typeof(DerivedClass1) || type == typeof(UnrelatedClass));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.InheritFrom<BaseClass>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesInheritIndirectly_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<NegatedTests>()
					.Types()
					.Which(type => type == typeof(GrandChildClass));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.InheritFrom<BaseClass>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(GrandChildClass) in assembly containing type ThatTypes.InheritFrom.NegatedTests
					             not all inherit from ThatTypes.BaseClass,
					             but it only contained types that inherit from ThatTypes.BaseClass [
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}
		}
	}
}
