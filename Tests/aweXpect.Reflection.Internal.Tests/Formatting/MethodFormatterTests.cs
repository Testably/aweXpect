using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Internal.Tests.TestHelpers;

namespace aweXpect.Reflection.Internal.Tests.Formatting;

public class MethodFormatterTests
{
	[Test]
	public async Task GenericMethodWithParameterAndReturnValue_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo =
			typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyGenericMethodWithParameterAndReturnValue));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo(
				"T1 MethodFormatterTests.MyTestClass.MyGenericMethodWithParameterAndReturnValue<T1, T2>(MethodFormatterTests.MyTestClass p1, T2 p2)");
	}

	[Test]
	public async Task GenericMethodWithRefParameter_ShouldFormatRefModifier()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyGenericMethodWithRefParameter));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo("void MethodFormatterTests.MyTestClass.MyGenericMethodWithRefParameter<T>(ref T value)");
	}

	[Test]
	public async Task GenericMethodWithReturnValue_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyGenericMethodWithReturnValue));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo(
				"T MethodFormatterTests.MyTestClass.MyGenericMethodWithReturnValue<T>()");
	}

	[Test]
	public async Task WithByRefReturnValue_ShouldFormatRefModifier()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithRefReturnValue));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("ref int MethodFormatterTests.MyTestClass.MyMethodWithRefReturnValue()");
	}

	[Test]
	public async Task WithInParameter_ShouldFormatInModifier()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithInParameter));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("void MethodFormatterTests.MyTestClass.MyMethodWithInParameter(in int value)");
	}

	[Test]
	public async Task WithOneParameter_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithOneParameter));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("void MethodFormatterTests.MyTestClass.MyMethodWithOneParameter(int parameter1)");
	}

	[Test]
	public async Task WithOutParameter_ShouldFormatOutModifier()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithOutParameter));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("void MethodFormatterTests.MyTestClass.MyMethodWithOutParameter(out int value)");
	}

	[Test]
	public async Task WithoutParameters_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyParameterlessMethod));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("void MethodFormatterTests.MyTestClass.MyParameterlessMethod()");
	}

	[Test]
	public async Task WithoutParameters_WithReturnValue_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo =
			typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyParameterlessMethodWithReturnValue));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("int MethodFormatterTests.MyTestClass.MyParameterlessMethodWithReturnValue()");
	}

	[Test]
	public async Task WithParameters_WithReturnValue_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo =
			typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithParametersAndReturnValue));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo(
				"string MethodFormatterTests.MyTestClass.MyMethodWithParametersAndReturnValue(string p1, int p2 = 42)");
	}

	[Test]
	public async Task WithParametersWithDefaultValues_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithDefaultValues));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo(
				"void MethodFormatterTests.MyTestClass.MyMethodWithDefaultValues(string p1 = \"foo\", int p2 = 42)");
	}

	[Test]
	public async Task WithParamsArray_ShouldFormatParamsModifier()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithParamsArray));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo("void MethodFormatterTests.MyTestClass.MyMethodWithParamsArray(int first, params int[] values)");
	}

	[Test]
	public async Task WithParamsCollection_ShouldFormatParamsModifier()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithParamsCollection));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo("void MethodFormatterTests.MyTestClass.MyMethodWithParamsCollection(params List<int> values)");
	}

	[Test]
	public async Task WithReadOnlyRefReturnValue_ShouldFormatRefReadonlyModifier()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithRefReadonlyReturnValue));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo("ref readonly int MethodFormatterTests.MyTestClass.MyMethodWithRefReadonlyReturnValue()");
	}

	[Test]
	public async Task WithRefParameter_ShouldFormatRefModifier()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithRefParameter));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("void MethodFormatterTests.MyTestClass.MyMethodWithRefParameter(ref int value)");
	}

	[Test]
	public async Task WithRefReadonlyParameter_ShouldFormatModifier()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithRefReadonlyParameter));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo("void MethodFormatterTests.MyTestClass.MyMethodWithRefReadonlyParameter(ref readonly int value)");
	}

	[Test]
	public async Task WithTwoParameters_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithTwoParameters));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo(
				"void MethodFormatterTests.MyTestClass.MyMethodWithTwoParameters(int parameter1, string parameter2)");
	}

#pragma warning disable CA1822
	// ReSharper disable UnusedParameter.Global
	internal abstract class MyTestClass
	{
		public void MyParameterlessMethod()
		{
		}

		public void MyMethodWithOneParameter(int parameter1)
		{
		}

		public void MyMethodWithTwoParameters(int parameter1, string parameter2)
		{
		}

		public void MyMethodWithDefaultValues(string p1 = "foo", int p2 = 42)
		{
		}

		public int MyParameterlessMethodWithReturnValue()
			=> 42;

		public abstract T MyGenericMethodWithReturnValue<T>();

		public abstract T1 MyGenericMethodWithParameterAndReturnValue<T1, T2>(MyTestClass p1, T2 p2);

		public string MyMethodWithParametersAndReturnValue(string p1, int p2 = 42)
			=> p1 + p2;

		public abstract void MyGenericMethodWithRefParameter<T>(ref T value);

		public abstract void MyMethodWithRefParameter(ref int value);

		public abstract void MyMethodWithRefReadonlyParameter(ref readonly int value);

		public abstract void MyMethodWithOutParameter(out int value);

		public abstract void MyMethodWithInParameter(in int value);

		public abstract void MyMethodWithParamsArray(int first, params int[] values);

		public abstract void MyMethodWithParamsCollection(params List<int> values);

		public abstract ref int MyMethodWithRefReturnValue();

		public abstract ref readonly int MyMethodWithRefReadonlyReturnValue();
	}
	// ReSharper restore UnusedParameter.Global
#pragma warning restore CA1822
}
