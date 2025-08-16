using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task AsPrefix_WhenParameterNameStartsWithPrefix_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<int>("val").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndex_WhenParameterDoesNotExistAtSpecificIndex_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<string>().AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type string at index 0,
					             but it did not
					             """);
			}

			[Fact]
			public async Task AtIndex_WhenParameterExistsAtSpecificIndex_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<int>().AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndexFromEnd_WhenParameterExistsAtSpecificIndexFromEnd_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<string>().AtIndex(0).FromEnd();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByName_WhenParameterDoesNotExist_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!;

				async Task Act()
					=> await That(methodInfo).HasParameter("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter with name "value",
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasParameterByName_WhenParameterExists_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter("value");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterDoesNotExist_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<int>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int,
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterExists_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterDoesNotExist_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<int>("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with name "value",
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterExists_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<int>("value");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenWrongType_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<string>("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type string with name "value",
					             but it did not
					             """);
			}

			[Fact]
			public async Task IgnoringCase_WhenParameterNameDiffersInCase_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<int>("VALUE").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDefaultValue_WhenParameterHasDefault_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDefaults))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<string>().WithDefaultValue();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithoutDefaultValue_WhenParameterHasNoDefault_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).HasParameter<int>().WithoutDefaultValue();

				await That(Act).DoesNotThrow();
			}

#pragma warning disable CA1822
			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public void MethodWithoutParameters() { }
				public void MethodWithString(string name) { }
				public void MethodWithIntAndString(int value, string name) { }
				public void MethodWithDefaults(int value, string name = "default") { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task HasParameterByName_WhenParameterDoesNotExist_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasParameter("value"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByName_WhenParameterExists_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasParameter("value"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have parameter with name "value",
					             but it did
					             """);
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterDoesNotExist_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasParameter<int>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterExists_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasParameter<int>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have parameter of type int,
					             but it did
					             """);
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterDoesNotExist_ShouldSucceed()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasParameter<int>("value"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterExists_ShouldFail()
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!;

				async Task Act()
					=> await That(methodInfo).DoesNotComplyWith(it => it.HasParameter<int>("value"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             does not have parameter of type int with name "value",
					             but it did
					             """);
			}

#pragma warning disable CA1822
			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public void MethodWithoutParameters() { }
				public void MethodWithString(string name) { }
				public void MethodWithIntAndString(int value, string name) { }
				public void MethodWithDefaults(int value, string name = "default") { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
		}
	}
}
