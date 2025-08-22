using System.Reflection;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Internal.Tests.TestHelpers;

namespace aweXpect.Reflection.Internal.Tests.Formatting;

public class MethodFormatterTests
{
	[Fact]
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

	[Fact]
	public async Task GenericMethodWithReturnValue_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyGenericMethodWithReturnValue));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo(
				"T MethodFormatterTests.MyTestClass.MyGenericMethodWithReturnValue<T>()");
	}

	[Fact]
	public async Task WithOneParameter_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithOneParameter));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("void MethodFormatterTests.MyTestClass.MyMethodWithOneParameter(int parameter1)");
	}

	[Fact]
	public async Task WithoutParameters_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyParameterlessMethod));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("void MethodFormatterTests.MyTestClass.MyParameterlessMethod()");
	}

	[Fact]
	public async Task WithoutParameters_WithReturnValue_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo =
			typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyParameterlessMethodWithReturnValue));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result).IsEqualTo("int MethodFormatterTests.MyTestClass.MyParameterlessMethodWithReturnValue()");
	}

	[Fact]
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

	[Fact]
	public async Task WithParametersWithDefaultValues_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithDefaultValues));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo(
				"void MethodFormatterTests.MyTestClass.MyMethodWithDefaultValues(string p1 = \"foo\", int p2 = 42)");
	}

	[Fact]
	public async Task WithTwoParameters_ShouldFormatCorrectly()
	{
		MethodInfo? methodInfo = typeof(MyTestClass).GetMethod(nameof(MyTestClass.MyMethodWithTwoParameters));
		MethodFormatter formatter = new();

		string result = formatter.GetString(methodInfo);

		await That(result)
			.IsEqualTo(
				"void MethodFormatterTests.MyTestClass.MyMethodWithTwoParameters(int parameter1, string parameter2)");
	}

#pragma warning disable CS1822
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
	}
	// ReSharper restore UnusedParameter.Global
#pragma warning restore CS1822
}
