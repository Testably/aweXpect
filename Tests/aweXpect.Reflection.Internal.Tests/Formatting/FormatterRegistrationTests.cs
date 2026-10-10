using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Formatting;
using aweXpect.Reflection.Formatting;

namespace aweXpect.Reflection.Internal.Tests.Formatting;

public class FormatterRegistrationTests
{
	public FormatterRegistrationTests()
	{
		RuntimeHelpers.RunModuleConstructor(typeof(FormattableMember).Module.ModuleHandle);
	}

	[Fact]
	public async Task ModuleInitializer_ShouldNotChangeHowConstructorInfoIsFormatted()
	{
		ConstructorInfo constructorInfo = typeof(ConstructorFormatterTests.MyTestClass).GetConstructors().First();

		string result = Format.Formatter.Format(constructorInfo);

		await That(result).IsEqualTo(constructorInfo.ToString())
			.Because("the package must not register a formatter for the ConstructorInfo it does not own");
	}

	[Fact]
	public async Task ModuleInitializer_ShouldNotChangeHowEventInfoIsFormatted()
	{
		EventInfo eventInfo = typeof(EventFormatterTests.MyTestClass).GetEvents().First();

		string result = Format.Formatter.Format(eventInfo);

		await That(result).IsEqualTo(eventInfo.ToString())
			.Because("the package must not register a formatter for the EventInfo it does not own");
	}

	[Fact]
	public async Task ModuleInitializer_ShouldNotChangeHowFieldInfoIsFormatted()
	{
		FieldInfo fieldInfo = typeof(FieldFormatterTests.MyTestClass).GetFields().First();

		string result = Format.Formatter.Format(fieldInfo);

		await That(result).IsEqualTo(fieldInfo.ToString())
			.Because("the package must not register a formatter for the FieldInfo it does not own");
	}

	[Fact]
	public async Task ModuleInitializer_ShouldNotChangeHowMethodInfoIsFormatted()
	{
		MethodInfo methodInfo = typeof(MethodFormatterTests.MyTestClass).GetMethods().First();

		string result = Format.Formatter.Format(methodInfo);

		await That(result).IsEqualTo(methodInfo.ToString())
			.Because("the package must not register a formatter for the MethodInfo it does not own");
	}

	[Fact]
	public async Task ModuleInitializer_ShouldNotChangeHowPropertyInfoIsFormatted()
	{
		PropertyInfo propertyInfo = typeof(PropertyFormatterTests.MyTestClass).GetProperties().First();

		string result = Format.Formatter.Format(propertyInfo);

		await That(result).IsEqualTo(propertyInfo.ToString())
			.Because("the package must not register a formatter for the PropertyInfo it does not own");
	}
}
