using aweXpect.Customization;

namespace aweXpect.Reflection.Tests;

public sealed class ReflectionCustomizationExtensionsTests
{
	private const string UnusedAttributeName = "aweXpect.Reflection.Tests.UnusedAttribute";

	[Fact]
	public async Task ExcludedAttributeTypes_Get_ShouldReturnACopy()
	{
		using (Customize.aweXpect.Reflection().ExcludedAttributeTypes().Set([UnusedAttributeName,]))
		{
			string[] value = Customize.aweXpect.Reflection().ExcludedAttributeTypes().Get();
			value[0] = "changed";

			await That(Customize.aweXpect.Reflection().ExcludedAttributeTypes().Get())
				.IsEqualTo([UnusedAttributeName,])
				.Because("changing the returned array must not bypass the scoping of the setting");
		}
	}

	[Fact]
	public async Task ExcludedAttributeTypes_SetNull_ShouldThrowArgumentNullException()
	{
		void Act()
		{
			Customize.aweXpect.Reflection().ExcludedAttributeTypes().Set(null!);
		}

		await That(Act).Throws<ArgumentNullException>()
			.WithMessage("The 'value' cannot be null.*").AsWildcard();
		await That(Customize.aweXpect.Reflection().ExcludedAttributeTypes().Get()).IsEmpty();
	}

	[Fact]
	public async Task WhenDisposedOutOfOrder_ShouldKeepTheValueOfTheRemainingLifetime()
	{
		CustomizationLifetime first = Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers()
			.Set(CompilerGeneratedMembers.Types);
		CustomizationLifetime second = Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers()
			.Set(CompilerGeneratedMembers.Methods);

		first.Dispose();

		await That(Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers().Get())
			.IsEqualTo(CompilerGeneratedMembers.Methods)
			.Because("disposing the first lifetime must not remove the value of the second one");

		second.Dispose();

		await That(Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers().Get())
			.IsEqualTo(CompilerGeneratedMembers.None)
			.Because("disposing both lifetimes restores the default");
	}

	[Fact]
	public async Task WhenSetOnGlobal_ShouldApplyToAllAsyncFlows()
	{
		CustomizationLifetime lifetime = await Task.Run(() => Customize.aweXpect.Global.Reflection()
			.ExcludedAttributeTypes().Set([UnusedAttributeName,]));
		try
		{
			await That(Customize.aweXpect.Reflection().ExcludedAttributeTypes().Get())
				.IsEqualTo([UnusedAttributeName,])
				.Because("a value set on Global in another async flow must be visible in this one");
		}
		finally
		{
			lifetime.Dispose();
		}

		await That(Customize.aweXpect.Reflection().ExcludedAttributeTypes().Get()).IsEmpty()
			.Because("disposing the global lifetime restores the default");
	}

	[Fact]
	public async Task WhenSetOnGlobal_ShouldApplyToTheEvaluationInAllAsyncFlows()
	{
		CustomizationLifetime lifetime = await Task.Run(() => Customize.aweXpect.Global.Reflection()
			.ExcludedAttributeTypes().Set([typeof(GlobalMarkerAttribute).FullName!,]));
		try
		{
			await That(typeof(WithGlobalMarker)).DoesNotDependOn<GlobalMarkerAttribute>()
				.Because("an attribute excluded on Global in another async flow must not count as a dependency");
		}
		finally
		{
			lifetime.Dispose();
		}

		await That(typeof(WithGlobalMarker)).DependsOn<GlobalMarkerAttribute>()
			.Because("disposing the global lifetime makes the attribute count as a dependency again");
	}

	[Fact]
	public async Task WhenSetOnScopedAndGlobal_ShouldPreferTheScopedValue()
	{
		using CustomizationLifetime global = Customize.aweXpect.Global.Reflection().ExcludedAttributeTypes()
			.Set([UnusedAttributeName,]);
		using (Customize.aweXpect.Reflection().ExcludedAttributeTypes().Set([]))
		{
			await That(Customize.aweXpect.Reflection().ExcludedAttributeTypes().Get()).IsEmpty()
				.Because("a scoped value takes precedence over the global one");
		}

		await That(Customize.aweXpect.Reflection().ExcludedAttributeTypes().Get())
			.IsEqualTo([UnusedAttributeName,])
			.Because("disposing the scoped lifetime falls back to the global value");
	}

	[Fact]
	public async Task WhenSetOnScopedCustomization_ShouldNotApplyToTheCallingAsyncFlow()
	{
		await Task.Run(() => Customize.aweXpect.Reflection()
			.IncludedSpecialNameMembers().Set(SpecialNameMembers.Operators));

		await That(Customize.aweXpect.Reflection().IncludedSpecialNameMembers().Get())
			.IsEqualTo(SpecialNameMembers.None)
			.Because("a value set on the scoped customization stays in the async flow that set it");
	}

	[Fact]
	public async Task WhenSetOnScopedCustomization_ShouldApplyUntilDisposed()
	{
		using (Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers()
			       .Set(CompilerGeneratedMembers.Types))
		{
			await That(Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers().Get())
				.IsEqualTo(CompilerGeneratedMembers.Types);
		}

		await That(Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers().Get())
			.IsEqualTo(CompilerGeneratedMembers.None);
	}

	[AttributeUsage(AttributeTargets.Class)]
	private sealed class GlobalMarkerAttribute : Attribute
	{
	}

	[GlobalMarker]
	private sealed class WithGlobalMarker
	{
	}
}
