using aweXpect.Customization;
using aweXpect.Web;
using aweXpect.Web.ContentProcessors;

namespace aweXpect.Tests.Web;

public sealed class WebAwexpectCustomizationExtensionsTests
{
	[Fact]
	public async Task ContentProcessors_Dispose_OutOfOrder_ShouldKeepLaterValueAndThenFallBackToGlobalValue()
	{
		AwexpectCustomization customization = new();
		IContentProcessor[] first = [new StringContentProcessor(),];
		IContentProcessor[] second = [new BinaryContentProcessor(),];
		IContentProcessor[] global = [new JsonContentProcessor(),];
		CustomizationLifetime firstLifetime = customization.Web().ContentProcessors.Set(first);
		CustomizationLifetime secondLifetime = customization.Web().ContentProcessors.Set(second);

		firstLifetime.Dispose();
		IContentProcessor[] valueAfterFirstDispose = customization.Web().ContentProcessors.Get();
		secondLifetime.Dispose();
		using CustomizationLifetime globalLifetime = customization.Global.Web().ContentProcessors.Set(global);

		await That(valueAfterFirstDispose).IsSameAs(second)
			.Because("disposing a lifetime must not undo a later value that is still active");
		await That(customization.Web().ContentProcessors.Get()).IsSameAs(global)
			.Because("after all lifetimes in the current flow are disposed, the global value applies again");
	}

	[Fact]
	public async Task ContentProcessors_Get_ShouldDefaultToJsonStringAndBinaryContentProcessors()
	{
		AwexpectCustomization customization = new();

		IContentProcessor[] result = customization.Web().ContentProcessors.Get();

		await That(result).HasCount(3);
		await That(result[0]).Is<JsonContentProcessor>();
		await That(result[1]).Is<StringContentProcessor>();
		await That(result[2]).Is<BinaryContentProcessor>();
	}

	[Fact]
	public async Task ContentProcessors_SetGlobal_ShouldApplyToAsyncFlowsThatDidNotSetTheValue()
	{
		AwexpectCustomization customization = new();
		IContentProcessor[] global = [new StringContentProcessor(),];

		using CustomizationLifetime _ = await SetGlobalInAwaitedMethod(customization, global);

		await That(customization.Web().ContentProcessors.Get()).IsSameAs(global)
			.Because("a global value is visible to all async flows, also to the caller of the method that set it");
	}

	[Fact]
	public async Task ContentProcessors_SetNull_ShouldThrowArgumentNullExceptionAndKeepValue()
	{
		AwexpectCustomization customization = new();
		IContentProcessor[] processors = [new StringContentProcessor(),];
		using CustomizationLifetime _ = customization.Web().ContentProcessors.Set(processors);

		void Act()
			=> customization.Web().ContentProcessors.Set(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("value").And
			.WithMessage("The 'value' cannot be null.").AsPrefix();
		await That(customization.Web().ContentProcessors.Get()).IsSameAs(processors)
			.Because("an invalid value must not change the stored value");
	}

	private static async Task<CustomizationLifetime> SetGlobalInAwaitedMethod(
		AwexpectCustomization customization, IContentProcessor[] value)
	{
		await Task.Yield();
		return customization.Global.Web().ContentProcessors.Set(value);
	}
}
