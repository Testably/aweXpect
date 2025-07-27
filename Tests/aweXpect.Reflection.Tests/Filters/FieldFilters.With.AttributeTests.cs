using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class With
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForFieldsWithAttribute()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().With<BarAttribute>();

				await That(fields).IsEqualTo([
					typeof(Dummy).GetField(nameof(Dummy.MyBarField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields with FieldFilters.With.BarAttribute")
					.AsPrefix();
			}

			[Theory]
			[MemberData(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForFieldsWithAttributeMatchingPredicate(int value,
				FieldInfo?[] expectedFields)
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().With<FooAttribute>(foo => foo.Value == value);

				await That(fields).IsEqualTo(expectedFields).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields with FieldFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			public static TheoryData<int, FieldInfo?[]> GetFooValues()
				=> new()
				{
					{
						2, [
							typeof(Dummy).GetField(nameof(Dummy.MyFooField2)),
						]
					},
					{
						3, [typeof(Dummy).GetField(nameof(Dummy.MyFooField3)),]
					},
				};
		}

		public sealed class OrWithAttributeTests
		{
			[Fact]
			public async Task ShouldFilterForFieldsWithAttribute()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().With<BarAttribute>().OrWith<FooAttribute>();

				await That(fields).IsEqualTo([
					typeof(Dummy).GetField(nameof(Dummy.MyBarField))!,
					typeof(Dummy).GetField(nameof(Dummy.MyFooField2))!,
					typeof(Dummy).GetField(nameof(Dummy.MyFooField3))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields with FieldFilters.With.BarAttribute or with FieldFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Theory]
			[MemberData(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForFieldsWithAttributeMatchingPredicate(int value,
				FieldInfo?[] expectedFields)
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().With<BarAttribute>().OrWith<FooAttribute>(foo => foo.Value == value);

				await That(fields).IsEqualTo(expectedFields).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields with FieldFilters.With.BarAttribute or with FieldFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			public static TheoryData<int, FieldInfo?[]> GetFooValues()
				=> new()
				{
					{
						2, [
							typeof(Dummy).GetField(nameof(Dummy.MyBarField)),
							typeof(Dummy).GetField(nameof(Dummy.MyFooField2)),
						]
					},
					{
						3, [
							typeof(Dummy).GetField(nameof(Dummy.MyBarField)),
							typeof(Dummy).GetField(nameof(Dummy.MyFooField3)),
						]
					},
				};
		}

		[AttributeUsage(AttributeTargets.Field)]
		private class BarAttribute : Attribute
		{
		}

		private class Dummy
		{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			[Bar] public int? MyBarField;

			[Foo(Value = 2)] public int? MyFooField2;

			[Foo(Value = 3)] public int? MyFooField3;
#pragma warning restore CS0649
		}

		private class DummyChild : Dummy
		{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			public new int? MyBarField;

			public new int? MyFooField2;

			public new int? MyFooField3;
#pragma warning restore CS0649
		}

		[AttributeUsage(AttributeTargets.Field)]
		private class FooAttribute : Attribute
		{
			public int Value { get; set; }
		}
	}
}
