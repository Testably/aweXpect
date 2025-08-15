using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichReturn
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnBaseType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn<DummyBase>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return MethodFilters.WhichReturn.DummyBase in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnTask()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn<Task>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.AsyncMethod))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return Task in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn<string>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return string in")
					.AsPrefix();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnBaseType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn(typeof(DummyBase));

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return MethodFilters.WhichReturn.DummyBase in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn(typeof(string));

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return string in type")
					.AsPrefix();
			}
		}

		public sealed class OrReturnGenericTests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnAnyOfMultipleTypes()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn<string>().OrReturn<int>().OrReturn<bool>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetBool))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return string or return int or return bool in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnAnyOfTheGivenTypes()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn<Task>().OrReturn<Action>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.AsyncMethod))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetAction))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return Task or return Action in type")
					.AsPrefix();
			}
		}

		public sealed class OrReturnTests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnAnyOfMultipleTypes()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn(typeof(string)).OrReturn(typeof(int)).OrReturn(typeof(bool));

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetStringFromObject))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetBool))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return string or return int or return bool in type")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnAnyOfTheGivenTypes()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn<Task>().OrReturn(typeof(Action));

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.AsyncMethod))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetAction))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods which return Task or return Action in type")
					.AsPrefix();
			}
		}

		public sealed class InheritanceTests
		{
			[Fact]
			public async Task ShouldIncludeMethodsThatReturnDerivedTypes()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn<DummyBase>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!, // Dummy inherits from DummyBase
				]).InAnyOrder();
			}

			[Fact]
			public async Task ShouldNotIncludeMethodsThatReturnBaseTypesWhenFilteringForDerived()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturn<Dummy>();

				await That(methods).IsEqualTo([
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
