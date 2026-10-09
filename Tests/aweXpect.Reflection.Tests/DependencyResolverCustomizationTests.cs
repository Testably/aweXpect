using System.Collections.Generic;
using System.Linq;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2;

// The unwrap test intentionally exercises the non-generic DependsOn(Type) overload with an open generic
// construction, so the "prefer generic overload" hint does not apply here.
#pragma warning disable CA2263

namespace aweXpect.Reflection.Tests;

public sealed class DependencyResolverCustomizationTests
{
	[Fact]
	public async Task WhenComposingOnGet_ShouldAugmentTheBuiltInResolver()
	{
		Type subject = typeof(OnlyLayer1);

		ICustomizationValueSetter<Func<Type, IEnumerable<Type>>?> resolver =
			Customize.aweXpect.Reflection().DependencyResolver();
		Func<Type, IEnumerable<Type>> builtin = resolver.Get()!;
		using (resolver.Set(type => builtin(type).Concat([typeof(TargetB),])))
		{
			// The built-in signature dependencies are kept and the composed ones are added.
			await That(subject).DependsOn(Layer1Namespace);
			await That(subject).DependsOn(Layer2Namespace);
		}
	}

	[Fact]
	public async Task WhenResolverIsReplaced_ShouldUseItAndRevertAfterTheScope()
	{
		// OnlyLayer1's signature references Layer1, but never Layer2.
		Type subject = typeof(OnlyLayer1);

		using (Customize.aweXpect.Reflection().DependencyResolver()
			       .Set(_ => [typeof(TargetB),]))
		{
			// The custom resolver replaces the built-in: only its output counts.
			await That(subject).DependsOn(Layer2Namespace);
			await That(subject).DoesNotDependOn(Layer1Namespace);
		}

		// After the scope is disposed, the built-in signature-level resolver applies again.
		await That(subject).DependsOn(Layer1Namespace);
		await That(subject).DoesNotDependOn(Layer2Namespace);
	}

	[Fact]
	public async Task WhenResolverReturnsWrappedTypes_ShouldUnwrapThemLikeTheBuiltIn()
	{
		Type subject = typeof(OnlyLayer2);

		using (Customize.aweXpect.Reflection().DependencyResolver()
			       .Set(_ => [typeof(List<TargetA>),]))
		{
			// The generic argument is unwrapped from the custom resolver's output, …
			await That(subject).DependsOn<TargetA>();
			// … and the constructed generic itself is kept (matching an open generic target).
			await That(subject).DependsOn(typeof(List<>));
		}
	}

	[Fact]
	public async Task WhenResolvingTheSameTypeTwiceWithinAScope_ShouldInvokeTheResolverOnlyOnce()
	{
		Type subject = typeof(OnlyLayer1);
		int invocationCount = 0;

		using (Customize.aweXpect.Reflection().DependencyResolver()
			       .Set(_ =>
			       {
				       invocationCount++;
				       return [typeof(TargetA),];
			       }))
		{
			await That(subject).DependsOn(Layer1Namespace);
			await That(subject).DependsOn(Layer1Namespace);
		}

		await That(invocationCount).IsEqualTo(1);
	}

	[Fact]
	public async Task WhenSettingANewResolver_ShouldNotReadTheCacheOfThePreviousScope()
	{
		Type subject = typeof(OnlyLayer1);
		int secondInvocationCount = 0;

		using (Customize.aweXpect.Reflection().DependencyResolver()
			       .Set(_ => [typeof(TargetA),]))
		{
			await That(subject).DependsOn(Layer1Namespace);
		}

		using (Customize.aweXpect.Reflection().DependencyResolver()
			       .Set(_ =>
			       {
				       secondInvocationCount++;
				       return [typeof(TargetB),];
			       }))
		{
			// The previous scope's cached result (Layer1) must not leak into this scope.
			await That(subject).DependsOn(Layer2Namespace);
			await That(subject).DoesNotDependOn(Layer1Namespace);
		}

		await That(secondInvocationCount).IsEqualTo(1);
	}

	[Fact]
	public async Task WhenSettingNull_ShouldRevertToTheBuiltInDefaultWithinTheScope()
	{
		Type subject = typeof(OnlyLayer1);

		using (Customize.aweXpect.Reflection().DependencyResolver()
			       .Set(_ => [typeof(TargetB),]))
		{
			await That(subject).DependsOn(Layer2Namespace);

			using (Customize.aweXpect.Reflection().DependencyResolver().Set(null))
			{
				// Within the null scope the built-in signature-level default applies again,
				// even though an outer scope configured a custom resolver.
				await That(subject).DependsOn(Layer1Namespace);
				await That(subject).DoesNotDependOn(Layer2Namespace);
			}

			// After the null scope is disposed, the outer custom resolver applies again.
			await That(subject).DependsOn(Layer2Namespace);
		}
	}

	private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
	private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";
}
