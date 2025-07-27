using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class OfExactType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFilterForFieldsWithType()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfExactType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields of exact type FieldFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldFilterForFieldsWithType()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfExactType(typeof(DummyBase));

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields of exact type FieldFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		public sealed class OrOfTypeGenericTests
		{
			[Fact]
			public async Task FirstCheckIsOfType_ShouldNotAllowDerivedTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfType(typeof(OtherDummy)).OrOfExactType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of type FieldFilters.OfExactType.OtherDummy or of exact type FieldFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForFieldsWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfExactType<OtherDummy>().OrOfExactType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of exact type FieldFilters.OfExactType.OtherDummy or of exact type FieldFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Fact]
			public async Task FirstCheckIsOfTypeExactly_ShouldAllowDerivedTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfExactType(typeof(OtherDummy)).OrOfType(typeof(DummyBase));

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of exact type FieldFilters.OfExactType.OtherDummy or of type FieldFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForFieldsWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfType<OtherDummy>().OrOfType(typeof(DummyBase));

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of type FieldFilters.OfExactType.OtherDummy or of type FieldFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		private class TestClass
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			public DummyBase DummyBaseField;
			public Dummy DummyField;
			public OtherDummy OtherDummyField;
#pragma warning restore CS0649
#pragma warning restore CS8618
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}

		private class OtherDummy
		{
		}
	}
}
