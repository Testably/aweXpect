using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task ShouldFilterForTypesWithoutAttribute()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Without<BarAttribute>();

				await That(types).All().Satisfy(t => !Attribute.IsDefined(t!, typeof(BarAttribute), true))
					.And.IsNotEmpty();
				await That(types).DoesNotContain(typeof(BarClass)).And.DoesNotContain(typeof(BarChildClass));
				await That(types.GetDescription())
					.IsEqualTo("types without TypeFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Test]
			public async Task WhenDirectly_ShouldFilterForTypesWithoutAttributeDirectlySet()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Without<BarAttribute>().Directly();

				await That(types).DoesNotContain(typeof(BarClass)).And.Contains(typeof(BarChildClass));
				await That(types.GetDescription())
					.IsEqualTo("types without direct TypeFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Test]
			public async Task WithPredicate_ShouldFilterForTypesWithoutMatchingAttribute()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Without<FooAttribute>(foo => foo.Value == 2);

				await That(types).DoesNotContain(typeof(FooClass2)).And.DoesNotContain(typeof(FooChildClass2));
				await That(types).Contains(typeof(FooClass3)).And.Contains(typeof(BarClass))
					.Because("types with a not matching attribute or without the attribute are kept");
				await That(types.GetDescription())
					.IsEqualTo("types without TypeFilters.Without.AttributeTests.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenDirectly_ShouldFilterForTypesWithoutMatchingAttributeDirectlySet()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Without<FooAttribute>(foo => foo.Value == 2).Directly();

				await That(types).DoesNotContain(typeof(FooClass2)).And.Contains(typeof(FooChildClass2));
				await That(types.GetDescription())
					.IsEqualTo(
						"types without direct TypeFilters.Without.AttributeTests.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			[Test]
			[Arguments(1)]
			[Arguments(2)]
			public async Task WithPredicate_WhenOneOfSeveralAttributesMatches_ShouldExcludeType(int value)
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Without<MultiAttribute>(multi => multi.Value == value);
				Filtered.Types notMatchingTypes = In.AssemblyContaining<AssemblyFilters>()
					.Types().Without<MultiAttribute>(multi => multi.Value == 3);

				await That(types).DoesNotContain(typeof(MultiClass))
					.Because("every attribute of the type counts, not only the first one");
				await That(notMatchingTypes).Contains(typeof(MultiClass))
					.Because("a type whose attributes all do not match the predicate is kept");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>().Types()
					.Which(type => type == typeof(FooClass2))
					.Without<FooAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(types).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(FooClass2) without TypeFilters.Without.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[AttributeUsage(AttributeTargets.Class)]
			private class BarAttribute : Attribute
			{
			}

			[Bar]
			private class BarClass
			{
			}

			private class BarChildClass : BarClass
			{
			}

			[AttributeUsage(AttributeTargets.Class)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[Foo(Value = 2)]
			private class FooClass2
			{
			}

			[Foo(Value = 3)]
			private class FooClass3
			{
			}

			private class FooChildClass2 : FooClass2
			{
			}

			[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
			private class MultiAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[Multi(Value = 1)]
			[Multi(Value = 2)]
			private class MultiClass
			{
			}
		}
	}
}
