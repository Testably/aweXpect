using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichReturnExactly
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnExactBaseType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly<DummyBase>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
					// Should NOT include GetDummy() since Dummy inherits from DummyBase
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return exactly MethodFilters.WhichReturnExactly.DummyBase in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnExactDerivedType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly<Dummy>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return exactly MethodFilters.WhichReturnExactly.Dummy in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnExactType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly<string>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return exactly string in")
					.AsPrefix();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnExactBaseType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly(typeof(DummyBase));

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
					// Should NOT include GetDummy() since Dummy inherits from DummyBase
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return exactly MethodFilters.WhichReturnExactly.DummyBase in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnExactType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly(typeof(string));

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return exactly string in type")
					.AsPrefix();
			}
		}

		public sealed class OrReturnExactlyGenericTests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnExactlyAnyOfMultipleTypes()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly<string>().OrReturnExactly<int>().OrReturnExactly<bool>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetBool))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return exactly string or return exactly int or return exactly bool in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnExactlyAnyOfTheGivenTypes()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly<Task>().OrReturnExactly<Action>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.AsyncMethod))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetAction))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return exactly Task or return exactly Action in type")
					.AsPrefix();
			}
		}

		public sealed class OrReturnExactlyTests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnExactlyAnyOfMultipleTypes()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly(typeof(string)).OrReturnExactly(typeof(int)).OrReturnExactly(typeof(bool));

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetBool))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return exactly string or return exactly int or return exactly bool in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnExactlyAnyOfTheGivenTypes()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly<Task>().OrReturnExactly(typeof(Action));

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.AsyncMethod))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetAction))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return exactly Task or return exactly Action in type")
					.AsPrefix();
			}
		}

		public sealed class MixingWithWhichReturnTests
		{
			[Fact]
			public async Task ShouldAllowMixingWhichReturnWithOrReturnExactly()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn<DummyBase>().OrReturnExactly<string>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!, // Included because WhichReturn allows inheritance
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return MethodFilters.WhichReturnExactly.DummyBase or return exactly string in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowMixingWhichReturnExactlyWithOrReturn()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnExactly<DummyBase>().OrReturn<string>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
					// Should NOT include GetDummy() because WhichReturnExactly doesn't allow inheritance
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return exactly MethodFilters.WhichReturnExactly.DummyBase or return string in type")
					.AsPrefix();
			}
		}

		public sealed class InheritanceContrastTests
		{
			[Fact]
			public async Task WhichReturnExactly_ShouldNotIncludeInheritedTypes()
			{
				Filtered.Methods exactMethods = In.Type<TestClass>()
					.Methods().WhichReturnExactly<DummyBase>();
				
				Filtered.Methods inheritingMethods = In.Type<TestClass>()
					.Methods().WhichReturn<DummyBase>();

				// Exact should only include GetDummyBase
				await That(exactMethods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
				]).InAnyOrder();

				// Inheriting should include both GetDummyBase and GetDummy
				await That(inheritingMethods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!,
				]).InAnyOrder();
			}
		}

#pragma warning disable CA1822 // Mark members as static
		private class TestClass
		{
			public string GetString() => "test";
			public string GetStringFromObject() => "test";
			public int GetInt() => 42;
			public bool GetBool() => true;
			public DummyBase GetDummyBase() => new();
			public Dummy GetDummy() => new();
			public OtherType GetOtherType() => new();
			public async Task AsyncMethod() => await Task.CompletedTask;
			public Action GetAction() => () => { };
			public void VoidMethod() { }
		}
#pragma warning restore CA1822

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}

		private class OtherType
		{
		}
	}
}