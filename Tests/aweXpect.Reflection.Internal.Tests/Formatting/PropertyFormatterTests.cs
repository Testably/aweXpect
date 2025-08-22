using System.Reflection;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Internal.Tests.TestHelpers;

namespace aweXpect.Reflection.Internal.Tests.Formatting;

public class PropertyFormatterTests
{
	[Theory]
	[MemberData(nameof(GetTestCases))]
	public async Task ShouldFormatCorrectly(PropertyInfo? propertyInfo, string expectedResult)
	{
		PropertyFormatter formatter = new();

		string result = formatter.GetString(propertyInfo);

		await That(result).IsEqualTo(expectedResult);
	}

	[Fact]
	public async Task WithOnlyGetter_ShouldFormatCorrectly()
	{
		PropertyInfo? propertyInfo = typeof(MyTestClass).GetProperty(nameof(MyTestClass.PropertyWithOnlyGetter),
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		PropertyFormatter formatter = new();

		string result = formatter.GetString(propertyInfo);

		await That(result)
			.IsEqualTo("internal string PropertyFormatterTests.MyTestClass.PropertyWithOnlyGetter { get; }");
	}

	[Fact]
	public async Task WithOnlySetter_ShouldFormatCorrectly()
	{
		PropertyInfo? propertyInfo = typeof(MyTestClass).GetProperty(nameof(MyTestClass.PropertyWithOnlySetter),
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		PropertyFormatter formatter = new();

		string result = formatter.GetString(propertyInfo);

		await That(result).IsEqualTo("public int PropertyFormatterTests.MyTestClass.PropertyWithOnlySetter { set; }");
	}

	[Fact]
	public async Task WithPrivateGetter_ShouldFormatCorrectly()
	{
		PropertyInfo? propertyInfo = typeof(MyTestClass).GetProperty(nameof(MyTestClass.PropertyWithPrivateGetter),
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		PropertyFormatter formatter = new();

		string result = formatter.GetString(propertyInfo);

		await That(result)
			.IsEqualTo("public int PropertyFormatterTests.MyTestClass.PropertyWithPrivateGetter { private get; set; }");
	}

	[Fact]
	public async Task WithPrivateSetter_ShouldFormatCorrectly()
	{
		PropertyInfo? propertyInfo = typeof(MyTestClass).GetProperty(nameof(MyTestClass.PropertyWithPrivateSetter),
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		PropertyFormatter formatter = new();

		string result = formatter.GetString(propertyInfo);

		await That(result)
			.IsEqualTo("public int PropertyFormatterTests.MyTestClass.PropertyWithPrivateSetter { get; private set; }");
	}

	public static TheoryData<PropertyInfo?, string> GetTestCases()
	{
		static PropertyInfo? GetProperty(string name) => typeof(MyTestClass).GetProperty(name,
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		return new TheoryData<PropertyInfo?, string>
		{
			{
				GetProperty("MyProperty"),
				"public int PropertyFormatterTests.MyTestClass.MyProperty { get; private set; }"
			},
			{
				GetProperty("InternalProperty"),
				"internal int PropertyFormatterTests.MyTestClass.InternalProperty { get; private set; }"
			},
			{
				GetProperty("ProtectedProperty"),
				"protected int PropertyFormatterTests.MyTestClass.ProtectedProperty { get; private protected set; }"
			},
			{
				GetProperty("ProtectedInternalProperty"),
				"protected internal int PropertyFormatterTests.MyTestClass.ProtectedInternalProperty { get; internal set; }"
			},
			{
				GetProperty("PrivateProtectedProperty"),
				"private protected int PropertyFormatterTests.MyTestClass.PrivateProtectedProperty { get; private set; }"
			},
			{
				GetProperty("PrivateProperty"),
				"private int PropertyFormatterTests.MyTestClass.PrivateProperty { get; set; }"
			}
		};
	}

	// ReSharper disable UnusedMember.Local
	private class MyTestClass
	{
		public int MyProperty { get; private set; }
		internal int InternalProperty { get; private set; }
		protected int ProtectedProperty { get; private protected set; }
		protected internal int ProtectedInternalProperty { get; internal set; }
		private protected int PrivateProtectedProperty { get; private set; }
		private int PrivateProperty { get; set; }
		// ReSharper disable once UnassignedGetOnlyAutoProperty
#pragma warning disable CS8618
		internal string? PropertyWithOnlyGetter { get; }
#pragma warning restore CS8618

		public int PropertyWithOnlySetter
		{
			set => PropertyWithPrivateSetter = value;
		}

		public int PropertyWithPrivateSetter { get; private set; }

		public int PropertyWithPrivateGetter { private get; set; }
	}
	// ReSharper restore UnusedMember.Local
}
