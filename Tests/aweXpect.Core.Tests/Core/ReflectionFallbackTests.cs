using System.Collections.Generic;

namespace aweXpect.Core.Tests.Core;

public sealed class ReflectionFallbackTests
{
	[Fact]
	public async Task NotSupported_ForACompilerGeneratedType_ShouldAskForAMarkedCallSite()
	{
		var anonymous = new
		{
			Value = 1,
		};

		NotSupportedException exception = ReflectionFallback.NotSupported(anonymous.GetType(), "properties");

		await That(exception.Message).Contains("Let the source generator see the type at a marked call site.")
			.Because("an anonymous type cannot be named in an attribute");
		await That(exception.Message).DoesNotContain("GenerateMetadata");
	}

	[Fact]
	public async Task NotSupported_ForAGenericType_ShouldNameTheTypeAsWrittenInSource()
	{
		NotSupportedException exception =
			ReflectionFallback.NotSupported(typeof(KeyValuePair<string, int>), "properties");

		await That(exception.Message).IsEqualTo(
				"The properties of KeyValuePair<string, int> cannot be found by reflection, which is switched off when publishing with trimming or Native AOT enabled. Register the type, for example with [assembly: GenerateMetadata(typeof(KeyValuePair<string, int>))]. Alternatively, set the runtime switch 'aweXpect.ReflectionFallback.IsSupported' to true to reflect anyway.")
			.Because("the suggested attribute has to compile, so the type is spelled the way source spells it");
	}

	[Fact]
	public async Task NotSupported_WithRemedy_ShouldNameTheSwitch()
	{
		NotSupportedException exception = ReflectionFallback.NotSupported("The interfaces of Foo", "Do this.");

		await That(exception.Message).IsEqualTo(
			"The interfaces of Foo cannot be found by reflection, which is switched off when publishing with trimming or Native AOT enabled. Do this. Alternatively, set the runtime switch 'aweXpect.ReflectionFallback.IsSupported' to true to reflect anyway.");
	}

	[Fact]
	public async Task UnderTheTestHost_ShouldBeSupported()
	{
		await That(ReflectionFallback.IsSupported).IsTrue()
			.Because("the tests run under the JIT without the switch, so every unregistered type is reflected over");
	}
}
