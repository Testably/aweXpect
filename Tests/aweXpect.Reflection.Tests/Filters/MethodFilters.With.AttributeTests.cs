using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class With
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task ShouldFilterForMethodsWithAttribute()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().With<BarAttribute>();

				await That(methods).IsEqualTo([
					typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod))!,
					typeof(DummyChild).GetMethod(nameof(DummyChild.MyBarMethod))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods with MethodFilters.With.BarAttribute")
					.AsPrefix();
			}

			[Test]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForMethodsWithAttributeDirectlySet()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().With<BarAttribute>(false);

				await That(methods).HasSingle().Which.IsEqualTo(typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod)));
				await That(methods.GetDescription())
					.IsEqualTo("methods with direct MethodFilters.With.BarAttribute")
					.AsPrefix();
			}

			[Test]
			[MethodDataSource(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForMethodsWithAttributeMatchingPredicate(int value,
				MethodInfo?[] expectedMethods)
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().With<FooAttribute>(foo => foo.Value == value);

				await That(methods).IsEqualTo(expectedMethods).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with MethodFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldFilterForMethodsWithAttributeDirectlySet()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().With<FooAttribute>(foo => foo.Value == 2, false);

				await That(methods).HasSingle().Which
					.IsEqualTo(typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod2)));
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with direct MethodFilters.With.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>().Methods()
					.With<FooAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(methods).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods with MethodFilters.With.FooAttribute matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			public static IEnumerable<Func<(int, MethodInfo?[])>> GetFooValues()
				=> new List<Func<(int, MethodInfo?[])>>
				{
					() => (
						2, [
							typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod2)),
							typeof(DummyChild).GetMethod(nameof(DummyChild.MyFooMethod2)),
						]
					),
					() => (3, [typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod3)),]),
				};
		}

		public sealed class OrWithAttributeTests
		{
			[Test]
			public async Task ShouldFilterForMethodsWithAttribute()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().With<BarAttribute>().OrWith<FooAttribute>();

				await That(methods).IsEqualTo([
					typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod))!,
					typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod2))!,
					typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod3))!,
					typeof(DummyChild).GetMethod(nameof(DummyChild.MyBarMethod))!,
					typeof(DummyChild).GetMethod(nameof(DummyChild.MyFooMethod2))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with MethodFilters.With.BarAttribute or with MethodFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Test]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForMethodsWithAttributeDirectlySet()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().With<BarAttribute>().OrWith<FooAttribute>(false);

				await That(methods).IsEqualTo([
					typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod))!,
					typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod2))!,
					typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod3))!,
					typeof(DummyChild).GetMethod(nameof(DummyChild.MyBarMethod))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with MethodFilters.With.BarAttribute or with direct MethodFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Test]
			[MethodDataSource(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForMethodsWithAttributeMatchingPredicate(int value,
				MethodInfo?[] expectedMethods)
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().With<BarAttribute>(false).OrWith<FooAttribute>(foo => foo.Value == value);

				await That(methods).IsEqualTo(expectedMethods).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with direct MethodFilters.With.BarAttribute or with MethodFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldFilterForMethodsWithAttributeDirectlySet()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().With<BarAttribute>(_ => false)
					.OrWith<FooAttribute>(foo => foo.Value == 2, false);

				await That(methods).HasSingle().Which
					.IsEqualTo(typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod2)));
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with MethodFilters.With.BarAttribute matching _ => false or with direct MethodFilters.With.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			public static IEnumerable<Func<(int, MethodInfo?[])>> GetFooValues()
				=> new List<Func<(int, MethodInfo?[])>>
				{
					() => (
						2, [
							typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod)),
							typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod2)),
							typeof(DummyChild).GetMethod(nameof(DummyChild.MyFooMethod2)),
						]
					),
					() => (
						3, [
							typeof(Dummy).GetMethod(nameof(Dummy.MyBarMethod)),
							typeof(Dummy).GetMethod(nameof(Dummy.MyFooMethod3)),
						]
					),
				};
		}

		[AttributeUsage(AttributeTargets.Method)]
		private class BarAttribute : Attribute
		{
		}

		private class Dummy
		{
			[Bar]
			public virtual void MyBarMethod()
			{
			}

			[Foo(Value = 2)]
			public virtual void MyFooMethod2()
			{
			}

			[Foo(Value = 3)]
			public virtual void MyFooMethod3()
			{
			}
		}

		private class DummyChild : Dummy
		{
			public override void MyBarMethod()
			{
			}

			public override void MyFooMethod2()
			{
			}
		}

		[AttributeUsage(AttributeTargets.Method)]
		private class FooAttribute : Attribute
		{
			public int Value { get; set; }
		}
	}
}
