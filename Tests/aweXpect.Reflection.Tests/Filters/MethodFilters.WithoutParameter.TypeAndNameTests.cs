using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithoutParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task AtIndex_ShouldOnlyFilterOutMethodsWithParameterAtIndex()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithoutParameter<int>().AtIndex(0);

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameter of type int at index 0 in")
					.AsPrefix();
			}

			[Test]
			public async Task ByName_ShouldFilterOutMethodsWithParameterName()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithoutParameter("value");

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameter with name equal to \"value\" in")
					.AsPrefix();
			}

			[Test]
			public async Task ByName_ShouldSupportAsPrefix()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithoutParameter("coun").AsPrefix();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameter with name starting with \"coun\" in")
					.AsPrefix();
			}

			[Test]
			public async Task ByType_ShouldFilterOutMethodsWithParameterOfType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithoutParameter<string>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameter of type string in")
					.AsPrefix();
			}

			[Test]
			public async Task ByType_WithRefModifier_ShouldOnlyFilterOutMethodsWithRefParameter()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithoutParameter<int>().WithRefModifier();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameter of type int and with ref modifier in")
					.AsPrefix();
			}

			[Test]
			public async Task ByTypeAndName_ShouldFilterOutMethodsWithMatchingParameter()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithoutParameter<int>("count");

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameter of type int and name equal to \"count\" in")
					.AsPrefix();
			}

			[Test]
			public async Task Type_ByType_ShouldFilterOutMethodsWithParameterOfType()
			{
				Type unexpectedType = typeof(string);
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithoutParameter(unexpectedType);

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameter of type string in")
					.AsPrefix();
			}

			[Test]
			public async Task Type_ByTypeAndName_ShouldFilterOutMethodsWithMatchingParameter()
			{
				Type unexpectedType = typeof(int);
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WithoutParameter(unexpectedType, "count");

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithReversedParameters))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods without parameter of type int and name equal to \"count\" in")
					.AsPrefix();
			}
		}

		private abstract class TestClass
		{
			public abstract void MethodWithParameters(int value, string text);
			public abstract void MethodWithReversedParameters(string text, int value);
			public abstract void MethodWithRefParameter(ref int count);
			public abstract void MethodWithoutParameters();
		}
	}
}
