using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class With
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task ShouldFilterForConstructorsWithAttribute()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().With<BarAttribute>();

				await That(constructors).IsEqualTo([
					typeof(Dummy).GetConstructor([])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with ConstructorFilters.With.BarAttribute")
					.AsPrefix();
			}

			[Test]
			[MethodDataSource(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForConstructorsWithAttributeMatchingPredicate(int value,
				ConstructorInfo?[] expectedConstructors)
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().With<FooAttribute>(foo => foo.Value == value);

				await That(constructors).IsEqualTo(expectedConstructors).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo(
						"constructors with ConstructorFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>().Constructors()
					.With<FooAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(constructors).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors with ConstructorFilters.With.FooAttribute matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			public static IEnumerable<Func<(int, ConstructorInfo?[])>> GetFooValues()
				=> new List<Func<(int, ConstructorInfo?[])>>
				{
					() => (
						2, [
							typeof(Dummy).GetConstructor([typeof(int),]),
						]
					),
					() => (3, [typeof(Dummy).GetConstructor([typeof(string),]),]),
				};
		}

		public sealed class OrWithAttributeTests
		{
			[Test]
			public async Task ShouldFilterForConstructorsWithAttribute()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().With<BarAttribute>().OrWith<FooAttribute>();

				await That(constructors).IsEqualTo([
					typeof(Dummy).GetConstructor([])!,
					typeof(Dummy).GetConstructor([typeof(int),])!,
					typeof(Dummy).GetConstructor([typeof(string),])!,
				]).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo(
						"constructors with ConstructorFilters.With.BarAttribute or with ConstructorFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Test]
			[MethodDataSource(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForConstructorsWithAttributeMatchingPredicate(int value,
				ConstructorInfo?[] expectedConstructors)
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().With<BarAttribute>().OrWith<FooAttribute>(foo => foo.Value == value);

				await That(constructors).IsEqualTo(expectedConstructors).InAnyOrder();
				await That(constructors.GetDescription())
					.IsEqualTo(
						"constructors with ConstructorFilters.With.BarAttribute or with ConstructorFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			public static IEnumerable<Func<(int, ConstructorInfo?[])>> GetFooValues()
				=> new List<Func<(int, ConstructorInfo?[])>>
				{
					() => (
						2, [
							typeof(Dummy).GetConstructor([]),
							typeof(Dummy).GetConstructor([typeof(int),]),
						]
					),
					() => (
						3, [
							typeof(Dummy).GetConstructor([]),
							typeof(Dummy).GetConstructor([typeof(string),]),
						]
					),
				};
		}

		[AttributeUsage(AttributeTargets.Constructor)]
		private class BarAttribute : Attribute
		{
		}

		private class Dummy
		{
			[Bar]
			public Dummy()
			{
			}

			[Foo(Value = 2)]
			public Dummy(int foo)
			{
				_ = foo;
			}

			[Foo(Value = 3)]
			public Dummy(string bar)
			{
				_ = bar;
			}
		}

		// ReSharper disable once UnusedType.Local
		private class DummyChild : Dummy
		{
			public DummyChild()
			{
			}

			public DummyChild(int foo) : base(foo)
			{
			}

			public DummyChild(string bar) : base(bar)
			{
			}
		}

		[AttributeUsage(AttributeTargets.Constructor)]
		private class FooAttribute : Attribute
		{
			public int Value { get; set; }
		}
	}
}
