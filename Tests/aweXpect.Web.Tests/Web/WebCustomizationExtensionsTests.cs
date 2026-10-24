using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Web;
using aweXpect.Web.ContentProcessors;

namespace aweXpect.Tests.Web;

public sealed class WebCustomizationExtensionsTests
{
#if NET8_0_OR_GREATER
	[Test]
	public async Task ContentProcessors_ChangingTheReturnedList_ShouldThrowNotSupportedException()
	{
		WebCustomizationExtensions.WebCustomization web = new AwexpectCustomization().Web();
		IReadOnlyList<IContentProcessor> processors = web.ContentProcessors.Get();
		IContentProcessor original = processors[0];

		void Act() => ((IList<IContentProcessor>)processors)[0] = new StringContentProcessor();

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Collection is read-only.");
		await That(web.ContentProcessors.Get()[0]).IsSameAs(original)
			.Because("changing the returned list must not bypass the scoping of the setting");
	}
#endif

	[Test]
	public async Task ContentProcessors_Dispose_OutOfOrder_ShouldKeepLaterValueAndThenFallBackToGlobalValue()
	{
		AwexpectCustomization customization = new();
		IContentProcessor[] first = [new StringContentProcessor(),];
		IContentProcessor[] second = [new BinaryContentProcessor(),];
		IContentProcessor[] global = [new JsonContentProcessor(),];
		CustomizationLifetime firstLifetime = customization.Web().ContentProcessors.Set(first);
		CustomizationLifetime secondLifetime = customization.Web().ContentProcessors.Set(second);

		firstLifetime.Dispose();
		IReadOnlyList<IContentProcessor> valueAfterFirstDispose = customization.Web().ContentProcessors.Get();
		secondLifetime.Dispose();
		using CustomizationLifetime globalLifetime = customization.Global.Web().ContentProcessors.Set(global);

		await That(valueAfterFirstDispose).IsEqualTo(second)
			.Because("disposing a lifetime must not undo a later value that is still active");
		await That(customization.Web().ContentProcessors.Get()).IsEqualTo(global)
			.Because("after all lifetimes in the current flow are disposed, the global value applies again");
	}

	[Test]
	public async Task ContentProcessors_Get_ShouldDefaultToJsonStringAndBinaryContentProcessors()
	{
		AwexpectCustomization customization = new();

		IReadOnlyList<IContentProcessor> result = customization.Web().ContentProcessors.Get();

		await That(result).HasCount(3);
		await That(result[0]).Is<JsonContentProcessor>();
		await That(result[1]).Is<StringContentProcessor>();
		await That(result[2]).Is<BinaryContentProcessor>();
	}

	[Test]
	public async Task ContentProcessors_SetGlobal_ShouldApplyToAsyncFlowsThatDidNotSetTheValue()
	{
		AwexpectCustomization customization = new();
		IContentProcessor[] global = [new StringContentProcessor(),];

		using CustomizationLifetime _ = await SetGlobalInAwaitedMethod(customization, global);

		await That(customization.Web().ContentProcessors.Get()).IsEqualTo(global)
			.Because("a global value is visible to all async flows, also to the caller of the method that set it");
	}

	[Test]
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
		await That(customization.Web().ContentProcessors.Get()).IsEqualTo(processors)
			.Because("an invalid value must not change the stored value");
	}

	[Test]
	public async Task ContentProcessors_WhenTheListIsChangedAfterSetting_ShouldKeepTheSetting()
	{
		AwexpectCustomization customization = new();
		StringContentProcessor processor = new();
		List<IContentProcessor> processors = [processor,];
		using CustomizationLifetime _ = customization.Web().ContentProcessors.Set(processors);

		processors.Add(new BinaryContentProcessor());

		await That(customization.Web().ContentProcessors.Get()).IsEqualTo([processor,])
			.Because("changing the list afterwards must not change the setting");
	}

	private static async Task<CustomizationLifetime> SetGlobalInAwaitedMethod(
		AwexpectCustomization customization, IContentProcessor[] value)
	{
		await Task.Yield();
		return customization.Global.Web().ContentProcessors.Set(value);
	}
}
