using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class With
	{
		public sealed class AttributeTests
		{
			[Fact]
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

			[Theory]
			[MemberData(nameof(GetFooValues))]
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

			public static TheoryData<int, ConstructorInfo?[]> GetFooValues()
				=> new()
				{
					{
						2, [
							typeof(Dummy).GetConstructor([typeof(int),]),
						]
					},
					{
						3, [typeof(Dummy).GetConstructor([typeof(string),]),]
					},
				};
		}

		public sealed class OrWithAttributeTests
		{
			[Fact]
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

			[Theory]
			[MemberData(nameof(GetFooValues))]
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

			public static TheoryData<int, ConstructorInfo?[]> GetFooValues()
				=> new()
				{
					{
						2, [
							typeof(Dummy).GetConstructor([]),
							typeof(Dummy).GetConstructor([typeof(int),]),
						]
					},
					{
						3, [
							typeof(Dummy).GetConstructor([]),
							typeof(Dummy).GetConstructor([typeof(string),]),
						]
					},
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
