using System.Collections.Generic;
using aweXpect.Core.Extending;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class ReflectionFallbackTests
{
	[Test]
	public async Task NotSupported_ForACompilerGeneratedType_ShouldAskForAMarkedCallSite()
	{
		var anonymous = new
		{
			Value = 1,
		};

		NotSupportedException exception = ReflectionFallback.NotSupported(anonymous.GetType(), "properties");

		await That(exception.Message)
			.Contains(
				"Let the source generator see the type as the static type of an argument at a marked call site, or of a member reached from one, in the assembly that creates it.")
			.Because("an anonymous type cannot be named in an attribute, and the generator cannot see it through object or from another assembly");
		await That(exception.Message).DoesNotContain("GenerateMetadata");
	}

	[Test]
	public async Task NotSupported_ForAGenericType_ShouldNameTheTypeAsWrittenInSource()
	{
		NotSupportedException exception =
			ReflectionFallback.NotSupported(typeof(KeyValuePair<string, int>), "properties");

		await That(exception.Message).IsEqualTo(
				"The properties of KeyValuePair<string, int> cannot be found by reflection, which is switched off when publishing with trimming or Native AOT enabled. Register the type, for example with [assembly: GenerateMetadata(typeof(KeyValuePair<string, int>))]. Alternatively, set the runtime switch 'aweXpect.ReflectionFallback.IsSupported' to true to reflect anyway.")
			.Because("the suggested attribute has to compile, so the type is spelled the way source spells it");
	}

	[Test]
	public async Task NotSupported_WhenRemedyIsNull_ShouldOnlyNameTheSwitch()
	{
		NotSupportedException exception = ReflectionFallback.NotSupported("The tracks of Album");

		await That(exception.Message).IsEqualTo(
				"The tracks of Album cannot be found by reflection, which is switched off when publishing with trimming or Native AOT enabled. Set the runtime switch 'aweXpect.ReflectionFallback.IsSupported' to true to reflect anyway.")
			.Because("without a remedy, the switch is the only way out and no longer an alternative");
	}

	[Test]
	public async Task NotSupported_WithRemedy_ShouldNameTheSwitch()
	{
		NotSupportedException exception = ReflectionFallback.NotSupported("The interfaces of Foo", "Do this.");

		await That(exception.Message).IsEqualTo(
			"The interfaces of Foo cannot be found by reflection, which is switched off when publishing with trimming or Native AOT enabled. Do this. Alternatively, set the runtime switch 'aweXpect.ReflectionFallback.IsSupported' to true to reflect anyway.");
	}

	[Test]
	public async Task UnderTheTestHost_ShouldBeSupported() => await That(ReflectionFallback.IsSupported).IsTrue()
		.Because("the tests run under the JIT without the switch, so every unregistered type is reflected over");
}
