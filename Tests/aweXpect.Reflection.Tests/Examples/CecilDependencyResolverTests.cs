using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Synthetic;

namespace aweXpect.Reflection.Tests.Examples;

public sealed class CecilDependencyResolverTests
{
	[Fact]
	public async Task BodyOnlyReference_IsOnlyDetectedWithTheCecilResolver()
	{
		// Layer1's TargetA is referenced only inside a method body of ViaMethodBodyOnly, with no
		// trace in its signature.
		Type subject = typeof(ViaMethodBodyOnly);

		// The default resolver is signature-level and cannot see the body-only reference, …
		await That(subject).DoesNotDependOn(Layer1Namespace);

		// … the Mono.Cecil-backed example resolver reads the IL and detects it, …
		using (Customize.aweXpect.Reflection().DependencyResolver()
			       .Set(CecilDependencyResolver.GetUsedTypes))
		{
			await That(subject).DependsOn(Layer1Namespace);
		}

		// … and outside the scope the default applies again.
		await That(subject).DoesNotDependOn(Layer1Namespace);
	}

	private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
}
