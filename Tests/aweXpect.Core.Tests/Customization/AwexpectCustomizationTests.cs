using System.Threading;
using aweXpect.Customization;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Customization;

public class AwexpectCustomizationTests
{
	[Fact]
	public async Task Dispose_OutOfOrder_ShouldKeepLaterValueAndThenFallBackToGlobalValue()
	{
		AwexpectCustomization customization = new();
		CustomizationLifetime firstLifetime = customization.MyConfiguration().Set("first");
		CustomizationLifetime secondLifetime = customization.MyConfiguration().Set("second");

		firstLifetime.Dispose();
		string valueAfterFirstDispose = customization.MyConfiguration().Get();
		secondLifetime.Dispose();
		using CustomizationLifetime globalLifetime = customization.Global.MyConfiguration().Set("global");

		await That(valueAfterFirstDispose).IsEqualTo("second")
			.Because("disposing a lifetime must not undo a later value that is still active");
		await That(customization.MyConfiguration().Get()).IsEqualTo("global")
			.Because("after all lifetimes in the current flow are disposed, the global value applies again");
	}

	[Fact]
	public async Task DoubleDispose_ShouldNotResetLaterValue()
	{
		CustomizationLifetime firstLifetime = Customize.aweXpect.MyConfiguration().Set("first");
		firstLifetime.Dispose();
		using (Customize.aweXpect.MyConfiguration().Set("second"))
		{
			firstLifetime.Dispose();

			await That(Customize.aweXpect.MyConfiguration().Get()).IsEqualTo("second")
				.Because("disposing a lifetime a second time must not reset a value that was set afterwards");
		}
	}

	[Fact]
	public async Task Equivalency_Dispose_OutOfOrder_ShouldKeepLaterValueAndThenFallBackToGlobalValue()
	{
		AwexpectCustomization customization = new();
		EquivalencyOptions firstOptions = new();
		EquivalencyOptions secondOptions = new();
		EquivalencyOptions globalOptions = new();
		CustomizationLifetime firstLifetime =
			customization.Equivalency().DefaultEquivalencyOptions.Set(firstOptions);
		CustomizationLifetime secondLifetime =
			customization.Equivalency().DefaultEquivalencyOptions.Set(secondOptions);

		firstLifetime.Dispose();
		EquivalencyOptions valueAfterFirstDispose = customization.Equivalency().DefaultEquivalencyOptions.Get();
		secondLifetime.Dispose();
		using CustomizationLifetime globalLifetime =
			customization.Global.Equivalency().DefaultEquivalencyOptions.Set(globalOptions);

		await That(valueAfterFirstDispose).IsSameAs(secondOptions)
			.Because("disposing a lifetime must not undo a later value of the same property that is still active");
		await That(customization.Equivalency().DefaultEquivalencyOptions.Get()).IsSameAs(globalOptions)
			.Because("after all lifetimes of the group in the current flow are disposed, the global value applies again");
	}

	[Fact]
	public async Task Formatting_PropertyLifetime_Dispose_OutOfOrder_ShouldFallBackToGlobalValue()
	{
		AwexpectCustomization customization = new();
		CustomizationLifetime lengthLifetime = customization.Formatting().MaximumStringLength.Set(5);
		CustomizationLifetime itemsLifetime = customization.Formatting().MaximumNumberOfCollectionItems.Set(3);

		lengthLifetime.Dispose();
		itemsLifetime.Dispose();
		using CustomizationLifetime globalLifetime = customization.Global.Formatting().MaximumStringLength.Set(20);

		await That(customization.Formatting().MaximumStringLength.Get()).IsEqualTo(20)
			.Because("after all lifetimes of the group in the current flow are disposed, the global value applies again");
	}

	[Fact]
	public async Task Formatting_PropertyLifetime_Dispose_OutOfOrder_ShouldKeepLaterValueOfSameProperty()
	{
		AwexpectCustomization customization = new();
		CustomizationLifetime firstLifetime = customization.Formatting().MaximumStringLength.Set(5);
		using CustomizationLifetime secondLifetime = customization.Formatting().MaximumStringLength.Set(7);

		firstLifetime.Dispose();

		await That(customization.Formatting().MaximumStringLength.Get()).IsEqualTo(7)
			.Because("disposing a lifetime must not undo a later value of the same property that is still active");
	}

	[Fact]
	public async Task Formatting_PropertyLifetime_Dispose_ShouldOnlyRestoreThatProperty()
	{
		CustomizationLifetime itemsLifetime =
			Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(5);
		CustomizationLifetime lengthLifetime = Customize.aweXpect.Formatting().MaximumStringLength.Set(20);

		itemsLifetime.Dispose();
		AwexpectCustomization.FormattingCustomizationValue afterFirstDispose = Customize.aweXpect.Formatting().Get();
		lengthLifetime.Dispose();
		AwexpectCustomization.FormattingCustomizationValue afterSecondDispose = Customize.aweXpect.Formatting().Get();

		await That(afterFirstDispose.MaximumNumberOfCollectionItems).IsEqualTo(10)
			.Because("disposing the lifetime restores its own property");
		await That(afterFirstDispose.MaximumStringLength).IsEqualTo(20)
			.Because("disposing a lifetime must not undo another property that is still active");
		await That(afterSecondDispose.MaximumNumberOfCollectionItems).IsEqualTo(10)
			.Because("disposing a lifetime must not bring back a property of an already disposed lifetime");
		await That(afterSecondDispose.MaximumStringLength).IsEqualTo(100)
			.Because("disposing the lifetime restores its own property");
	}

	[Fact]
	public async Task Formatting_PropertyLifetime_DoubleDispose_ShouldNotResetLaterValue()
	{
		CustomizationLifetime firstLifetime = Customize.aweXpect.Formatting().MaximumStringLength.Set(5);
		firstLifetime.Dispose();
		using (Customize.aweXpect.Formatting().MaximumStringLength.Set(7))
		{
			firstLifetime.Dispose();

			await That(Customize.aweXpect.Formatting().MaximumStringLength.Get()).IsEqualTo(7)
				.Because("disposing a lifetime a second time must not reset a value that was set afterwards");
		}
	}

	[Fact]
	public async Task Formatting_Set_InParallelFlows_ShouldNotInfluenceEachOther()
	{
		using CustomizationLifetime parentLifetime = Customize.aweXpect.Formatting().MaximumStringLength.Set(50);
		using SemaphoreSlim firstHasSet = new(0);
		using SemaphoreSlim secondHasSet = new(0);
		using SemaphoreSlim firstHasRead = new(0);

		Task<int> first = Task.Run(async () =>
		{
			using CustomizationLifetime lifetime = Customize.aweXpect.Formatting().MaximumStringLength.Set(5);
			firstHasSet.Release();
			await secondHasSet.WaitAsync();
			int value = Customize.aweXpect.Formatting().MaximumStringLength.Get();
			firstHasRead.Release();
			return value;
		});
		Task<int> second = Task.Run(async () =>
		{
			await firstHasSet.WaitAsync();
			int value = Customize.aweXpect.Formatting().MaximumStringLength.Get();
			using CustomizationLifetime lifetime = Customize.aweXpect.Formatting().MaximumStringLength.Set(20);
			secondHasSet.Release();
			await firstHasRead.WaitAsync();
			return value;
		});

		int valueInFirst = await first;
		int valueInSecond = await second;

		await That(valueInFirst).IsEqualTo(5)
			.Because("the value set in a parallel flow must not leak into this flow");
		await That(valueInSecond).IsEqualTo(50)
			.Because("the value set in a parallel flow must not leak into this flow");
		await That(Customize.aweXpect.Formatting().MaximumStringLength.Get()).IsEqualTo(50)
			.Because("a value set in a child flow must not leak into the parent flow");
	}

	[Fact]
	public async Task Formatting_ShouldReturnTenAsDefaultMaximumNumberOfCollectionItems()
	{
		int defaultValue1 = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		int defaultValue2 = Customize.aweXpect.Formatting().Get().MaximumNumberOfCollectionItems;

		await That(defaultValue1).IsEqualTo(10);
		await That(defaultValue2).IsEqualTo(10);
	}

	[Fact]
	public async Task Formatting_ShouldUpdate()
	{
		int defaultValue = 10;
		int value = 42;
		using (Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(value))
		{
			await That(Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()).IsEqualTo(value);
			await That(Customize.aweXpect.Formatting().Get().MaximumNumberOfCollectionItems).IsEqualTo(value);
		}

		await That(Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()).IsEqualTo(defaultValue);
		await That(Customize.aweXpect.Formatting().Get().MaximumNumberOfCollectionItems).IsEqualTo(defaultValue);
	}

	[Fact]
	public async Task Formatting_ShouldUpdate2()
	{
		int defaultValue = 10;
		int value = 42;
		// ReSharper disable once WithExpressionModifiesAllMembers
		using (Customize.aweXpect.Formatting().Update(p => p with
		       {
			       MaximumNumberOfCollectionItems = value,
		       }))
		{
			await That(Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()).IsEqualTo(value);
			await That(Customize.aweXpect.Formatting().Get().MaximumNumberOfCollectionItems).IsEqualTo(value);
		}

		await That(Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()).IsEqualTo(defaultValue);
		await That(Customize.aweXpect.Formatting().Get().MaximumNumberOfCollectionItems).IsEqualTo(defaultValue);
	}

	[Fact]
	public async Task Formatting_Update_DoubleDispose_ShouldNotResetLaterValue()
	{
		// ReSharper disable once WithExpressionModifiesAllMembers
		CustomizationLifetime firstLifetime = Customize.aweXpect.Formatting().Update(p => p with
		{
			MaximumStringLength = 5,
		});
		firstLifetime.Dispose();
		// ReSharper disable once WithExpressionModifiesAllMembers
		using (Customize.aweXpect.Formatting().Update(p => p with
		       {
			       MaximumStringLength = 7,
		       }))
		{
			firstLifetime.Dispose();

			await That(Customize.aweXpect.Formatting().MaximumStringLength.Get()).IsEqualTo(7)
				.Because("disposing a lifetime a second time must not reset a value that was set afterwards");
		}
	}

	[Fact]
	public async Task Global_Get_ShouldIgnoreValueOfCurrentFlow()
	{
		AwexpectCustomization customization = new();
		using CustomizationLifetime globalLifetime = customization.Global.MyConfiguration().Set("global");
		using CustomizationLifetime scopedLifetime = customization.MyConfiguration().Set("scoped");

		await That(customization.Global.MyConfiguration().Get()).IsEqualTo("global")
			.Because("the global customization only reads the global values");
	}

	[Fact]
	public async Task Global_Global_ShouldReturnTheSameGlobalCustomization()
	{
		AwexpectCustomization customization = new();
		using CustomizationLifetime lifetime = customization.Global.Global.MyConfiguration().Set("global");

		await That(customization.Global.Global).IsSameAs(customization.Global)
			.Because("the global customization is its own global customization");
		await That(customization.MyConfiguration().Get()).IsEqualTo("global")
			.Because("a value set on the global customization of the global customization is a global value");
	}
	[Fact]
	public async Task Global_PropertyLifetime_Dispose_ShouldOnlyRestoreThatProperty()
	{
		AwexpectCustomization customization = new();
		CustomizationLifetime itemsLifetime =
			customization.Global.Formatting().MaximumNumberOfCollectionItems.Set(5);
		CustomizationLifetime lengthLifetime = customization.Global.Formatting().MaximumStringLength.Set(20);

		itemsLifetime.Dispose();
		AwexpectCustomization.FormattingCustomizationValue afterFirstDispose = customization.Formatting().Get();
		lengthLifetime.Dispose();
		AwexpectCustomization.FormattingCustomizationValue afterSecondDispose = customization.Formatting().Get();

		await That(afterFirstDispose.MaximumNumberOfCollectionItems).IsEqualTo(10)
			.Because("disposing the lifetime restores its own property");
		await That(afterFirstDispose.MaximumStringLength).IsEqualTo(20)
			.Because("disposing a lifetime must not undo another property that is still active");
		await That(afterSecondDispose.MaximumNumberOfCollectionItems).IsEqualTo(10)
			.Because("disposing a lifetime must not bring back a property of an already disposed lifetime");
		await That(afterSecondDispose.MaximumStringLength).IsEqualTo(100)
			.Because("disposing the lifetime restores its own property");
	}

	[Fact]
	public async Task Global_ScopedPropertyLifetime_Dispose_ShouldApplyGlobalValuesSetInTheMeantime()
	{
		AwexpectCustomization customization = new();
		CustomizationLifetime scopedLifetime = customization.Formatting().MaximumNumberOfCollectionItems.Set(5);
		using CustomizationLifetime globalLifetime = customization.Global.Formatting().MaximumStringLength.Set(20);

		scopedLifetime.Dispose();

		await That(customization.Formatting().MaximumStringLength.Get()).IsEqualTo(20)
			.Because("disposing the scoped lifetime removes the group from the current flow again");
	}

	[Fact]
	public async Task Global_ScopedValue_ShouldTakePrecedence()
	{
		AwexpectCustomization customization = new();
		using CustomizationLifetime globalLifetime = customization.Global.Formatting().MaximumStringLength.Set(20);
		string valueInScope;
		using (customization.Formatting().MaximumNumberOfCollectionItems.Set(5))
		using (customization.Formatting().MaximumStringLength.Set(30))
		{
			valueInScope = $"{customization.Formatting().MaximumNumberOfCollectionItems.Get()}/{customization.Formatting().MaximumStringLength.Get()}";
		}

		string valueAfterScope = $"{customization.Formatting().MaximumNumberOfCollectionItems.Get()}/{customization.Formatting().MaximumStringLength.Get()}";

		await That(valueInScope).IsEqualTo("5/30")
			.Because("a value set in the current flow takes precedence over the global value");
		await That(valueAfterScope).IsEqualTo("10/20")
			.Because("after the scope the global value applies again");
	}

	[Fact]
	public async Task Global_Set_ShouldApplyToOtherFlows()
	{
		AwexpectCustomization customization = new();

		CustomizationLifetime globalLifetime =
			await Task.Run(() => customization.Global.MyConfiguration().Set("global"));
		string valueWhileSet = customization.MyConfiguration().Get();
		await Task.Run(globalLifetime.Dispose);
		string valueAfterDispose = customization.MyConfiguration().Get();

		await That(valueWhileSet).IsEqualTo("global")
			.Because("a global value applies to all flows, also to one that did not start from the setting flow");
		await That(valueAfterDispose).IsEqualTo("foo")
			.Because("disposing the global lifetime restores the global value in all flows");
	}

	[Fact]
	public async Task Global_Update_WhileAnotherUpdateIsComputed_ShouldApplyBothUpdates()
	{
		AwexpectCustomization customization = new();
		using ManualResetEventSlim concurrentUpdateDone = new();
		Task<CustomizationLifetime>? concurrentUpdate = null;

		using CustomizationLifetime lengthLifetime = customization.Global.Formatting().Update(p =>
		{
			concurrentUpdate ??= Task.Run(() =>
			{
				CustomizationLifetime lifetime =
					customization.Global.Formatting().MaximumNumberOfCollectionItems.Set(5);
				concurrentUpdateDone.Set();
				return lifetime;
			});
			concurrentUpdateDone.Wait(TimeSpan.FromMilliseconds(200));
			return p with
			{
				MaximumStringLength = 20,
			};
		});
		using CustomizationLifetime itemsLifetime = await concurrentUpdate!;

		await That(customization.Global.Formatting().MaximumStringLength.Get()).IsEqualTo(20);
		await That(customization.Global.Formatting().MaximumNumberOfCollectionItems.Get()).IsEqualTo(5)
			.Because("a global update that completes while another one computes its value must not be lost");
	}

	[Fact]
	public async Task NestedLifetimes_ShouldSetPreviousValue()
	{
		string valueInLifetime1;
		string valueInLifetime2;
		string valueInLifetime1AfterLifetime2;
		string valueBeforeLifetime1 = Customize.aweXpect.MyConfiguration().Get();
		using (Customize.aweXpect.MyConfiguration().Set("l1"))
		{
			valueInLifetime1 = Customize.aweXpect.MyConfiguration().Get();
			using (Customize.aweXpect.MyConfiguration().Set("l2"))
			{
				valueInLifetime2 = Customize.aweXpect.MyConfiguration().Get();
			}

			valueInLifetime1AfterLifetime2 = Customize.aweXpect.MyConfiguration().Get();
		}

		string valueAfterLifetime1 = Customize.aweXpect.MyConfiguration().Get();

		await ThatAll(
			That(valueBeforeLifetime1).IsEqualTo("foo"),
			That(valueInLifetime1).IsEqualTo("l1"),
			That(valueInLifetime2).IsEqualTo("l2"),
			That(valueInLifetime1AfterLifetime2).IsEqualTo("l1"),
			That(valueAfterLifetime1).IsEqualTo("foo")
		);
	}

	[Fact]
	public async Task Set_InAwaitedAsyncMethod_ShouldNotBeVisibleToCaller()
	{
		using CustomizationLifetime outerLifetime = Customize.aweXpect.MyConfiguration().Set("outer");

		await SetInAsyncMethod("inner");

		await That(Customize.aweXpect.MyConfiguration().Get()).IsEqualTo("outer")
			.Because("changes to an async local value in an awaited method do not flow back to the caller");

		static async Task SetInAsyncMethod(string value)
		{
			await Task.Yield();
			Customize.aweXpect.MyConfiguration().Set(value);
		}
	}

	[Fact]
	public async Task Set_InParallelFlows_ShouldNotInfluenceEachOther()
	{
		using CustomizationLifetime parentLifetime = Customize.aweXpect.MyConfiguration().Set("parent");
		using SemaphoreSlim firstHasSet = new(0);
		using SemaphoreSlim secondHasSet = new(0);
		using SemaphoreSlim firstHasRead = new(0);

		Task<string> first = Task.Run(async () =>
		{
			using CustomizationLifetime lifetime = Customize.aweXpect.MyConfiguration().Set("first");
			firstHasSet.Release();
			await secondHasSet.WaitAsync();
			string value = Customize.aweXpect.MyConfiguration().Get();
			firstHasRead.Release();
			return value;
		});
		Task<string> second = Task.Run(async () =>
		{
			await firstHasSet.WaitAsync();
			string value = Customize.aweXpect.MyConfiguration().Get();
			using CustomizationLifetime lifetime = Customize.aweXpect.MyConfiguration().Set("second");
			secondHasSet.Release();
			await firstHasRead.WaitAsync();
			return value;
		});

		string valueInFirst = await first;
		string valueInSecond = await second;

		await That(valueInFirst).IsEqualTo("first")
			.Because("the value set in a parallel flow must not leak into this flow");
		await That(valueInSecond).IsEqualTo("parent")
			.Because("the value set in a parallel flow must not leak into this flow");
		await That(Customize.aweXpect.MyConfiguration().Get()).IsEqualTo("parent")
			.Because("a value set in a child flow must not leak into the parent flow");
	}

	[Fact]
	public async Task Set_InSynchronousMethod_ShouldBeVisibleToCaller()
	{
		using CustomizationLifetime outerLifetime = Customize.aweXpect.MyConfiguration().Set("outer");

		using CustomizationLifetime innerLifetime = SetInSynchronousMethod("inner");

		await That(Customize.aweXpect.MyConfiguration().Get()).IsEqualTo("inner")
			.Because("a synchronous method shares the async flow of its caller");

		static CustomizationLifetime SetInSynchronousMethod(string value)
			=> Customize.aweXpect.MyConfiguration().Set(value);
	}

	[Fact]
	public async Task Settings_PropertyLifetime_Dispose_OutOfOrder_ShouldFallBackToGlobalValue()
	{
		AwexpectCustomization customization = new();
		CustomizationLifetime intervalLifetime =
			customization.Settings().DefaultCheckInterval.Set(TimeSpan.FromSeconds(1));
		CustomizationLifetime timeoutLifetime =
			customization.Settings().DefaultEventuallyTimeout.Set(TimeSpan.FromSeconds(2));

		intervalLifetime.Dispose();
		timeoutLifetime.Dispose();
		using CustomizationLifetime globalLifetime =
			customization.Global.Settings().DefaultCheckInterval.Set(TimeSpan.FromSeconds(3));

		await That(customization.Settings().DefaultCheckInterval.Get()).IsEqualTo(TimeSpan.FromSeconds(3))
			.Because("after all lifetimes of the group in the current flow are disposed, the global value applies again");
	}
}

public static class DummyExtensions
{
	private static readonly string MyKey = Guid.NewGuid().ToString();

	public static ICustomizationValueSetter<string> MyConfiguration(this AwexpectCustomization awexpectCustomization)
		=> new CustomizationValue<string>(awexpectCustomization, MyKey, "foo");

	private sealed class CustomizationValue<TValue>(
		IAwexpectCustomization customization,
		string key,
		TValue defaultValue)
		: ICustomizationValueSetter<TValue>
	{
		public TValue Get()
			=> customization.Get(key, defaultValue);

		public CustomizationLifetime Set(TValue value)
			=> customization.Set(key, value);
	}
}
