using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Formatting;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Internal.Tests.TestHelpers;

namespace aweXpect.Reflection.Internal.Tests.Formatting;

public class FormattableMemberTests
{
	[Fact]
	public async Task From_WithConstructorInfo_ShouldFormatItWithTheConstructorFormatter()
	{
		ConstructorInfo constructorInfo = typeof(ConstructorFormatterTests.MyTestClass).GetConstructors().First();

		string result = Format.Formatter.Format(FormattableMember.From(constructorInfo));

		await That(result).IsEqualTo(new ConstructorFormatter().GetString(constructorInfo));
	}

	[Fact]
	public async Task From_WithEventInfo_ShouldFormatItWithTheEventFormatter()
	{
		EventInfo eventInfo = typeof(EventFormatterTests.MyTestClass).GetEvents().First();

		string result = Format.Formatter.Format(FormattableMember.From(eventInfo));

		await That(result).IsEqualTo(new EventFormatter().GetString(eventInfo));
	}

	[Fact]
	public async Task From_WithFieldInfo_ShouldFormatItWithTheFieldFormatter()
	{
		FieldInfo fieldInfo = typeof(FieldFormatterTests.MyTestClass).GetFields().First();

		string result = Format.Formatter.Format(FormattableMember.From(fieldInfo));

		await That(result).IsEqualTo(new FieldFormatter().GetString(fieldInfo));
	}

	[Fact]
	public async Task From_WithMethodInfo_ShouldFormatItWithTheMethodFormatter()
	{
		MethodInfo methodInfo = typeof(MethodFormatterTests.MyTestClass).GetMethods().First();

		string result = Format.Formatter.Format(FormattableMember.From(methodInfo));

		await That(result).IsEqualTo(new MethodFormatter().GetString(methodInfo));
	}

	[Fact]
	public async Task From_WithPropertyInfo_ShouldFormatItWithThePropertyFormatter()
	{
		PropertyInfo propertyInfo = typeof(PropertyFormatterTests.MyTestClass).GetProperties().First();

		string result = Format.Formatter.Format(FormattableMember.From(propertyInfo));

		await That(result).IsEqualTo(new PropertyFormatter().GetString(propertyInfo));
	}

	[Fact]
	public async Task From_WithNull_ShouldReturnNull()
	{
		object? result = FormattableMember.From(null);

		await That(result).IsNull();
	}

	[Fact]
	public async Task From_WithType_ShouldReturnTheType()
	{
		Type type = typeof(MethodFormatterTests.MyTestClass);

		object? result = FormattableMember.From(type);

		await That(result).IsSameAs(type)
			.Because("a type is formatted by aweXpect.Core itself");
	}

	[Fact]
	public async Task FromAll_ShouldFormatEachMember()
	{
		MethodInfo[] methods = typeof(MethodFormatterTests.MyTestClass).GetMethods().Take(2).ToArray();

		string result = Format.Formatter.Format(FormattableMember.FromAll(methods));

		await That(result).IsEqualTo(
			$"[{new MethodFormatter().GetString(methods[0])}, {new MethodFormatter().GetString(methods[1])}]");
	}

	[Fact]
	public async Task FromAll_WithCollection_ShouldKeepTheNumberOfItems()
	{
		MethodInfo methodInfo = typeof(MethodFormatterTests.MyTestClass).GetMethods().First();
		List<MethodInfo> methods = Enumerable.Repeat(methodInfo, 25).ToList();

		string result = Format.Formatter.Format(FormattableMember.FromAll(methods));

		await That(result).IsEqualTo(Format.Formatter.Format(methods.Select(FormattableMember.From).ToList()))
			.Because("a truncated collection names the number of remaining items");
	}

	[Fact]
	public async Task FromAll_WithLazySequence_ShouldNotKnowTheNumberOfItems()
	{
		MethodInfo methodInfo = typeof(MethodFormatterTests.MyTestClass).GetMethods().First();
		IEnumerable<MethodInfo> methods = Enumerable.Repeat(methodInfo, 25).Where(_ => true);

		string result = Format.Formatter.Format(FormattableMember.FromAll(methods));

		await That(result).IsEqualTo(Format.Formatter.Format(methods.Select(FormattableMember.From)))
			.Because("a lazy sequence does not know the number of remaining items");
	}
}
