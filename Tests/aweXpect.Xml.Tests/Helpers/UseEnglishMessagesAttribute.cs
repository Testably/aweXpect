using TUnit.Core.Executors;

namespace aweXpect.Xml.Tests.Helpers;

/// <summary>
///     Runs the tests with an English culture.
/// </summary>
/// <remarks>
///     The failure messages contain the messages of the <see cref="System.Xml.XmlException" />, which .NET Framework
///     localizes.
/// </remarks>
public sealed class UseEnglishMessagesAttribute() : CultureAttribute("en-US");
