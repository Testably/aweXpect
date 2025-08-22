using System.Linq;
using System.Reflection;
using aweXpect.Formatting;
using aweXpect.Reflection.Formatting;

namespace aweXpect.Reflection.Internal.Tests.Formatting;

public class FormatterRegistrationTests
{
	[Fact]
	public async Task Constructor_WhenNotDisposed_ShouldThrowInvalidOperationException()
	{
		void Act() => _ = new FormatterRegistration();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"A FormatterRegistration instance is already initialized. Dispose the existing instance before creating a new one.");
	}

	[Fact]
	public async Task Initialize_ShouldRegisterFormatterForConstructorInfo()
	{
		ConstructorInfo constructorInfo = typeof(ConstructorFormatterTests.MyTestClass).GetConstructors().First();
		await That(true).IsTrue();
		string result1 = Format.Formatter.Format(constructorInfo);

		FormatterRegistration.Instance.Dispose();
		string result2 = Format.Formatter.Format(constructorInfo);
		FormatterRegistration.Instance.Initialize();

		string result3 = Format.Formatter.Format(constructorInfo);

		await That(result1).IsEqualTo(result3);
		await That(result2).IsNotEqualTo(result3);
	}

	[Fact]
	public async Task Initialize_ShouldRegisterFormatterForEventInfo()
	{
		EventInfo eventInfo = typeof(EventFormatterTests.MyTestClass).GetEvents().First();
		await That(true).IsTrue();
		string result1 = Format.Formatter.Format(eventInfo);

		FormatterRegistration.Instance.Dispose();
		string result2 = Format.Formatter.Format(eventInfo);
		FormatterRegistration.Instance.Initialize();

		string result3 = Format.Formatter.Format(eventInfo);

		await That(result1).IsEqualTo(result3);
		await That(result2).IsNotEqualTo(result3);
	}

	[Fact]
	public async Task Initialize_ShouldRegisterFormatterForFieldInfo()
	{
		FieldInfo fieldInfo = typeof(FieldFormatterTests.MyTestClass).GetFields().First();
		await That(true).IsTrue();
		string result1 = Format.Formatter.Format(fieldInfo);

		FormatterRegistration.Instance.Dispose();
		string result2 = Format.Formatter.Format(fieldInfo);
		FormatterRegistration.Instance.Initialize();

		string result3 = Format.Formatter.Format(fieldInfo);

		await That(result1).IsEqualTo(result3);
		await That(result2).IsNotEqualTo(result3);
	}

	[Fact]
	public async Task Initialize_ShouldRegisterFormatterForPropertyInfo()
	{
		PropertyInfo propertyInfo = typeof(PropertyFormatterTests.MyTestClass).GetProperties().First();
		await That(true).IsTrue();
		string result1 = Format.Formatter.Format(propertyInfo);

		FormatterRegistration.Instance.Dispose();
		string result2 = Format.Formatter.Format(propertyInfo);
		FormatterRegistration.Instance.Initialize();

		string result3 = Format.Formatter.Format(propertyInfo);

		await That(result1).IsEqualTo(result3);
		await That(result2).IsNotEqualTo(result3);
	}
}
