using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class OfType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFilterForFieldsWhichImplementType()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields of type FieldFilters.OfType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldFilterForFieldsWhichImplementType()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfType(typeof(DummyBase));

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields of type FieldFilters.OfType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		public sealed class OrOfTypeGenericTests
		{
			[Fact]
			public async Task FirstCheckIsOfTypeExactly_ShouldAllowDerivedTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfExactType(typeof(OtherDummy)).OrOfType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of exact type FieldFilters.OfType.OtherDummy or of type FieldFilters.OfType.DummyBase in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForFieldsWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().OfType<OtherDummy>().OrOfType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of type FieldFilters.OfType.OtherDummy or of type FieldFilters.OfType.DummyBase in assembly")
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
						"fields of exact type FieldFilters.OfType.OtherDummy or of type FieldFilters.OfType.DummyBase in assembly")
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
						"fields of type FieldFilters.OfType.OtherDummy or of type FieldFilters.OfType.DummyBase in assembly")
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
