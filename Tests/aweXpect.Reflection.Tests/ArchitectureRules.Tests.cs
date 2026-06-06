using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;

namespace aweXpect.Reflection.Tests;

/// <summary>
///     End-to-end tests for the architecture-rules pattern: layers are reusable <see cref="Filtered.Types" />
///     selections, combined into one verification with <c>Expect.ThatAll(…)</c>.
/// </summary>
public sealed class ArchitectureRulesTests
{
	private const string ConsumersNamespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers";
	private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
	private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

	[Fact]
	public async Task ShouldVerifyMultipleRulesWithThatAll()
	{
		// A "layer" is just a reusable Filtered.Types selection.
		Filtered.Types layer1 = Types.InNamespace(Layer1Namespace);
		Filtered.Types layer2 = Types.InNamespace(Layer2Namespace);

		async Task Act()
			=> await ThatAll(
				// Filtered.Types as dependency target:
				That(layer2).DoNotDependOn(layer1),
				// Namespace as dependency target on the same selection:
				That(layer2).DoNotDependOn(Layer1Namespace),
				// Any other assertion works on the same selection:
				That(layer2).AreWithinNamespace(Layer2Namespace));

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenARuleIsViolated_ShouldReportAggregatedFailure()
	{
		Filtered.Types layer1 = Types.InNamespace(Layer1Namespace);
		Filtered.Types layer2 = Types.InNamespace(Layer2Namespace);

		async Task Act()
			=> await ThatAll(
				// Fails: Layer1's TargetSeverityAttribute references Layer2's TargetSeverity.
				That(layer1).DoNotDependOn(layer2),
				// Succeeds:
				That(layer2).AreWithinNamespace(Layer2Namespace));

		await That(Act).ThrowsException()
			.WithMessage("*TargetSeverityAttribute*").AsWildcard();
	}

	[Fact]
	public async Task ShouldSupportExemptionsViaExcept()
	{
		Filtered.Types layer2 = Types.InNamespace(Layer2Namespace);

		async Task Act()
			=> await ThatAll(
				// The consumers that intentionally reference Layer2 are exempted from the rule:
				That(Types.InNamespace(ConsumersNamespace)
						.Except<OnlyLayer2>()
						.Except<Layer1AndLayer2>()
						.Except<ViaAttributeArgument>())
					.DoNotDependOn(layer2));

		await That(Act).DoesNotThrow();
	}
}
