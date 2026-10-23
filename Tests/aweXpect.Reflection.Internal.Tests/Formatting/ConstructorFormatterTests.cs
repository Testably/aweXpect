using System.Reflection;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Internal.Tests.TestHelpers;

namespace aweXpect.Reflection.Internal.Tests.Formatting;

public class ConstructorFormatterTests
{
	[Test]
	public async Task WithOneParameter_ShouldFormatCorrectly()
	{
		ConstructorInfo? constructorInfo = typeof(MyTestClass).GetConstructor([typeof(int),]);
		ConstructorFormatter formatter = new();

		string result = formatter.GetString(constructorInfo);

		await That(result).IsEqualTo("ConstructorFormatterTests.MyTestClass(int parameter1)");
	}

	[Test]
	public async Task WithoutParameters_ShouldFormatCorrectly()
	{
		ConstructorInfo? constructorInfo = typeof(MyTestClass).GetConstructor(Type.EmptyTypes);
		ConstructorFormatter formatter = new();

		string result = formatter.GetString(constructorInfo);

		await That(result).IsEqualTo("ConstructorFormatterTests.MyTestClass()");
	}

	[Test]
	public async Task WithParametersWithDefaultValues_ShouldFormatCorrectly()
	{
		ConstructorInfo? constructorInfo = typeof(MyTestClass).GetConstructor([typeof(string), typeof(int),]);
		ConstructorFormatter formatter = new();

		string result = formatter.GetString(constructorInfo);

		await That(result).IsEqualTo("ConstructorFormatterTests.MyTestClass(string p1 = \"foo\", int p2 = 42)");
	}

	[Test]
	public async Task WithRefAndParamsParameters_ShouldFormatModifiers()
	{
		ConstructorInfo? constructorInfo =
			typeof(MyTestClass).GetConstructor([typeof(int).MakeByRefType(), typeof(int[]),]);
		ConstructorFormatter formatter = new();

		string result = formatter.GetString(constructorInfo);

		await That(result)
			.IsEqualTo("ConstructorFormatterTests.MyTestClass(ref int value, params int[] values)");
	}

	[Test]
	public async Task WithTwoParameters_ShouldFormatCorrectly()
	{
		ConstructorInfo? constructorInfo = typeof(MyTestClass).GetConstructor([typeof(int), typeof(string),]);
		ConstructorFormatter formatter = new();

		string result = formatter.GetString(constructorInfo);

		await That(result).IsEqualTo("ConstructorFormatterTests.MyTestClass(int parameter1, string parameter2)");
	}

	// ReSharper disable UnusedMember.Local
	// ReSharper disable UnusedParameter.Local
	internal class MyTestClass
	{
		public MyTestClass()
		{
		}

		public MyTestClass(int parameter1)
		{
		}

		public MyTestClass(int parameter1, string parameter2)
		{
		}

		public MyTestClass(string p1 = "foo", int p2 = 42)
		{
		}

		public MyTestClass(ref int value, params int[] values)
		{
		}
	}
	// ReSharper restore UnusedParameter.Local
	// ReSharper restore UnusedMember.Local
}
