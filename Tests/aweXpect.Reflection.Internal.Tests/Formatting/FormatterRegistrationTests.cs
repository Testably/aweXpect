using System.Linq;
using System.Reflection;
using aweXpect.Formatting;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Internal.Tests.TestHelpers;

namespace aweXpect.Reflection.Internal.Tests.Formatting;

public class FormatterRegistrationTests
{
	[Fact]
	public async Task ModuleInitializer_ShouldRegisterFormatterForConstructorInfo()
	{
		ConstructorInfo constructorInfo = typeof(ConstructorFormatterTests.MyTestClass).GetConstructors().First();

		string result = Format.Formatter.Format(constructorInfo);

		await That(result).IsEqualTo(new ConstructorFormatter().GetString(constructorInfo))
			.Because("the module initializer must have registered the ConstructorFormatter globally");
	}

	[Fact]
	public async Task ModuleInitializer_ShouldRegisterFormatterForEventInfo()
	{
		EventInfo eventInfo = typeof(EventFormatterTests.MyTestClass).GetEvents().First();

		string result = Format.Formatter.Format(eventInfo);

		await That(result).IsEqualTo(new EventFormatter().GetString(eventInfo))
			.Because("the module initializer must have registered the EventFormatter globally");
	}

	[Fact]
	public async Task ModuleInitializer_ShouldRegisterFormatterForFieldInfo()
	{
		FieldInfo fieldInfo = typeof(FieldFormatterTests.MyTestClass).GetFields().First();

		string result = Format.Formatter.Format(fieldInfo);

		await That(result).IsEqualTo(new FieldFormatter().GetString(fieldInfo))
			.Because("the module initializer must have registered the FieldFormatter globally");
	}

	[Fact]
	public async Task ModuleInitializer_ShouldRegisterFormatterForMethodInfo()
	{
		MethodInfo methodInfo = typeof(MethodFormatterTests.MyTestClass).GetMethods().First();

		string result = Format.Formatter.Format(methodInfo);

		await That(result).IsEqualTo(new MethodFormatter().GetString(methodInfo))
			.Because("the module initializer must have registered the MethodFormatter globally");
	}

	[Fact]
	public async Task ModuleInitializer_ShouldRegisterFormatterForPropertyInfo()
	{
		PropertyInfo propertyInfo = typeof(PropertyFormatterTests.MyTestClass).GetProperties().First();

		string result = Format.Formatter.Format(propertyInfo);

		await That(result).IsEqualTo(new PropertyFormatter().GetString(propertyInfo))
			.Because("the module initializer must have registered the PropertyFormatter globally");
	}
}
