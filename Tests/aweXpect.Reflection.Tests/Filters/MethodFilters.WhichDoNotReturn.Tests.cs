using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichDoNotReturn
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForMethodsWhichDoNotReturnType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichDoNotReturn<DummyBase>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.DoSomething))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which do not return MethodFilters.WhichDoNotReturn.DummyBase in")
					.AsPrefix();
			}

			[Test]
			public async Task WithType_ShouldFilterForMethodsWhichDoNotReturnType()
			{
				Type unexpectedType = typeof(DummyBase);
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichDoNotReturn(unexpectedType);

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.DoSomething))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which do not return MethodFilters.WhichDoNotReturn.DummyBase in")
					.AsPrefix();
			}
		}

		public sealed class ExactlyTests
		{
			[Test]
			public async Task ShouldFilterForMethodsWhichDoNotReturnExactlyType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichDoNotReturnExactly<DummyBase>();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.DoSomething))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which do not return exactly MethodFilters.WhichDoNotReturn.DummyBase in")
					.AsPrefix();
			}

			[Test]
			public async Task WithType_ShouldFilterForMethodsWhichDoNotReturnExactlyType()
			{
				Type unexpectedType = typeof(DummyBase);
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichDoNotReturnExactly(unexpectedType);

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.DoSomething))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which do not return exactly MethodFilters.WhichDoNotReturn.DummyBase in")
					.AsPrefix();
			}
		}

		public sealed class VoidTests
		{
			[Test]
			public async Task ShouldFilterForMethodsWhichDoNotReturnVoid()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().WhichDoNotReturnVoid();

				await That(methods).IsEqualTo([
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
				]).InAnyOrder();
				await That(methods.GetDescription())
					.IsEqualTo("methods which do not return void in")
					.AsPrefix();
			}
		}

		private abstract class TestClass
		{
			public abstract DummyBase GetDummyBase();
			public abstract Dummy GetDummy();
			public abstract string GetString();
			public abstract void DoSomething();
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
