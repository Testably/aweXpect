using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichReturnVoid
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWhichReturnVoid()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnVoid();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return void in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowChainingAlternativeReturnType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichReturnVoid().OrReturn<string>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which return void or return string in")
					.AsPrefix();
			}
		}

#pragma warning disable CA1822 // Mark members as static
		private class TestClass
		{
			public string GetString() => "test";
			public int GetInt() => 42;
			public void VoidMethod() { }
		}
#pragma warning restore CA1822
	}
}
