using System.Globalization;
using System.Reflection;

namespace aweXpect.Xml.Tests.Helpers;

/// <summary>
///     Runs the tests with an English UI culture.
/// </summary>
/// <remarks>
///     The failure messages contain the messages of the <see cref="System.Xml.XmlException" />, which .NET Framework
///     localizes.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class UseEnglishMessagesAttribute : BeforeAfterTestAttribute
{
	private CultureInfo? _originalUICulture;

	public override void Before(MethodInfo methodUnderTest)
	{
		_originalUICulture = CultureInfo.CurrentUICulture;
		CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
	}

	public override void After(MethodInfo methodUnderTest)
		=> CultureInfo.CurrentUICulture = _originalUICulture!;
}
