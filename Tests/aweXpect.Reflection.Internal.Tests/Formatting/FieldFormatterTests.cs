using System.Reflection;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Internal.Tests.TestHelpers;

namespace aweXpect.Reflection.Internal.Tests.Formatting;

public class FieldFormatterTests
{
	[Fact]
	public async Task WithoutInitialization_ShouldFormatCorrectly()
	{
		FieldInfo? fieldInfo = typeof(MyTestClass).GetField(nameof(MyTestClass.Field1));
		FieldFormatter formatter = new();

		string result = formatter.GetString(fieldInfo);

		await That(result).IsEqualTo("int FieldFormatterTests.MyTestClass.Field1");
	}

	[Fact]
	public async Task WithInitialization_ShouldFormatCorrectly()
	{
		FieldInfo? fieldInfo = typeof(MyTestClass).GetField(nameof(MyTestClass.Field2));
		FieldFormatter formatter = new();

		string result = formatter.GetString(fieldInfo);

		await That(result).IsEqualTo("string FieldFormatterTests.MyTestClass.Field2");
	}

	internal class MyTestClass
	{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		public int Field1;
#pragma warning restore CS0649
#pragma warning disable CS0414 // Field is assigned but its value is never used
		public string Field2 = "foo";
#pragma warning restore CS0414 // Field is assigned but its value is never used
	}
}
