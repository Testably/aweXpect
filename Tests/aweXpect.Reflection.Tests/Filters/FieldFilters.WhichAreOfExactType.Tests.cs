using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreOfExactType
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task ShouldFilterForFieldsWithType()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreOfExactType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields of exact type FieldFilters.WhichAreOfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class TypeTests
		{
			[Test]
			public async Task ShouldFilterForFieldsWithType()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreOfExactType(typeof(DummyBase));

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields of exact type FieldFilters.WhichAreOfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}
#pragma warning restore CA2263

		public sealed class OrOfTypeGenericTests
		{
			[Test]
			public async Task FirstCheckIsOfType_ShouldNotAllowDerivedTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreOfType<OtherDummy>().OrOfExactType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of type FieldFilters.WhichAreOfExactType.OtherDummy or of exact type FieldFilters.WhichAreOfExactType.DummyBase in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldFilterForFieldsWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreOfExactType<OtherDummy>().OrOfExactType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of exact type FieldFilters.WhichAreOfExactType.OtherDummy or of exact type FieldFilters.WhichAreOfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class OrOfTypeTests
		{
			[Test]
			public async Task FirstCheckIsOfTypeExactly_ShouldAllowDerivedTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreOfExactType(typeof(OtherDummy)).OrOfType(typeof(DummyBase));

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of exact type FieldFilters.WhichAreOfExactType.OtherDummy or of type FieldFilters.WhichAreOfExactType.DummyBase in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldFilterForFieldsWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreOfType<OtherDummy>().OrOfType(typeof(DummyBase));

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields of type FieldFilters.WhichAreOfExactType.OtherDummy or of type FieldFilters.WhichAreOfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}
#pragma warning restore CA2263

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
