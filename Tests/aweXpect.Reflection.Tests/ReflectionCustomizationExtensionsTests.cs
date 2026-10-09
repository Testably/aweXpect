using aweXpect.Customization;

namespace aweXpect.Reflection.Tests;

public sealed class ReflectionCustomizationExtensionsTests
{
	private const string UnusedAttributeName = "aweXpect.Reflection.Tests.UnusedAttribute";

	[Fact]
	public async Task ExcludedAttributeTypes_Get_ShouldReturnACopy()
	{
		using (Customize.aweXpect.ReflectionSettings().ExcludedAttributeTypes.Set([UnusedAttributeName,]))
		{
			string[] value = Customize.aweXpect.ReflectionSettings().ExcludedAttributeTypes.Get();
			value[0] = "changed";

			await That(Customize.aweXpect.ReflectionSettings().ExcludedAttributeTypes.Get())
				.IsEqualTo([UnusedAttributeName,])
				.Because("changing the returned array must not bypass the scoping of the setting");
		}
	}

	[Fact]
	public async Task ExcludedAttributeTypes_SetNull_ShouldThrowArgumentNullException()
	{
		void Act()
		{
			Customize.aweXpect.ReflectionSettings().ExcludedAttributeTypes.Set(null!);
		}

		await That(Act).Throws<ArgumentNullException>()
			.WithMessage("The 'value' cannot be null.*").AsWildcard();
		await That(Customize.aweXpect.ReflectionSettings().ExcludedAttributeTypes.Get()).IsEmpty();
	}

	[Fact]
	public async Task WhenSetOnGlobal_ShouldApplyToAllAsyncFlows()
	{
		CustomizationLifetime lifetime = await Task.Run(() => Customize.aweXpect.Global.ReflectionSettings()
			.ExcludedAttributeTypes.Set([UnusedAttributeName,]));
		try
		{
			await That(Customize.aweXpect.ReflectionSettings().ExcludedAttributeTypes.Get())
				.IsEqualTo([UnusedAttributeName,])
				.Because("a value set on Global in another async flow must be visible in this one");
		}
		finally
		{
			lifetime.Dispose();
		}

		await That(Customize.aweXpect.ReflectionSettings().ExcludedAttributeTypes.Get()).IsEmpty()
			.Because("disposing the global lifetime restores the default");
	}

	[Fact]
	public async Task WhenSetOnScopedCustomization_ShouldNotApplyToTheCallingAsyncFlow()
	{
		await Task.Run(() => Customize.aweXpect.ReflectionSettings()
			.IncludedSpecialNameMembers.Set(SpecialNameMembers.Operators));

		await That(Customize.aweXpect.ReflectionSettings().IncludedSpecialNameMembers.Get())
			.IsEqualTo(SpecialNameMembers.None)
			.Because("a value set on the scoped customization stays in the async flow that set it");
	}

	[Fact]
	public async Task WhenSetOnScopedCustomization_ShouldApplyUntilDisposed()
	{
		using (Customize.aweXpect.ReflectionSettings().IncludedCompilerGeneratedMembers
			       .Set(CompilerGeneratedMembers.Types))
		{
			await That(Customize.aweXpect.ReflectionSettings().IncludedCompilerGeneratedMembers.Get())
				.IsEqualTo(CompilerGeneratedMembers.Types);
		}

		await That(Customize.aweXpect.ReflectionSettings().IncludedCompilerGeneratedMembers.Get())
			.IsEqualTo(CompilerGeneratedMembers.None);
	}
}
