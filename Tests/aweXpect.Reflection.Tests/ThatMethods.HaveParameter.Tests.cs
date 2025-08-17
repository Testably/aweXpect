using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task HaveParameterByName_WhenAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
					=> await That(methods).HaveParameter("value");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByName_WhenNotAllHaveParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, typeof(TestClass).GetMethod(
						nameof(TestClass.MethodWithString))!, // Has "name" parameter, not "value"
				};

				async Task Act()
					=> await That(methods).HaveParameter("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter with name "value",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task HaveParameterByType_WhenAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
					=> await That(methods).HaveParameter<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByType_WhenNotAllHaveParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithString))!, // No int parameter
				};

				async Task Act()
					=> await That(methods).HaveParameter<int>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task HaveParameterByTypeAndName_WhenAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
					=> await That(methods).HaveParameter<int>("value");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByTypeAndName_WhenNotAllHaveParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, typeof(TestClass).GetMethod(
						nameof(TestClass.MethodWithString))!, // Has string "name", not int "value"
				};

				async Task Act()
					=> await That(methods).HaveParameter<int>("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with name "value",
					             but at least one did not
					             """);
			}

#pragma warning disable CA1822
			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public void MethodWithInt(int value) { }
				public void MethodWithString(string name) { }
				public void MethodWithIntAndString(int value, string name) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task HaveParameterByName_WhenAllHaveParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
					=> await That(methods).DoesNotComplyWith(they => they.HaveParameter("value"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have parameter with name "value",
					             but all did
					             """);
			}

			[Fact]
			public async Task HaveParameterByName_WhenNotAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, typeof(TestClass).GetMethod(
						nameof(TestClass.MethodWithString))!, // Has "name" parameter, not "value"
				};

				async Task Act()
					=> await That(methods).DoesNotComplyWith(they => they.HaveParameter("value"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByType_WhenAllHaveParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
					=> await That(methods).DoesNotComplyWith(they => they.HaveParameter<int>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have parameter of type int,
					             but all did
					             """);
			}

			[Fact]
			public async Task HaveParameterByType_WhenNotAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithString))!, // No int parameter
				};

				async Task Act()
					=> await That(methods).DoesNotComplyWith(they => they.HaveParameter<int>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByTypeAndName_WhenAllHaveParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
					=> await That(methods).DoesNotComplyWith(they => they.HaveParameter<int>("value"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have parameter of type int with name "value",
					             but all did
					             """);
			}

			[Fact]
			public async Task HaveParameterByTypeAndName_WhenNotAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, typeof(TestClass).GetMethod(
						nameof(TestClass.MethodWithString))!, // Has string "name", not int "value"
				};

				async Task Act()
					=> await That(methods).DoesNotComplyWith(they => they.HaveParameter<int>("value"));

				await That(Act).DoesNotThrow();
			}

#pragma warning disable CA1822
			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public void MethodWithInt(int value) { }
				public void MethodWithString(string name) { }
				public void MethodWithIntAndString(int value, string name) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
		}

		public sealed class ChainingTests
		{
			[Fact]
			public async Task AsPrefix_WhenAllHaveParameterWithPrefix_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, // has "name" parameter
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithString))!, // has "name" parameter
				};

				async Task Act()
					=> await That(methods).HaveParameter("na").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsRegex_WhenAllHaveParameterMatchingRegex_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, // has "name" parameter
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithString))!, // has "name" parameter
				};

				async Task Act()
					=> await That(methods).HaveParameter("n.*e").AsRegex();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsSuffix_WhenAllHaveParameterWithSuffix_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, // has "name" parameter
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithString))!, // has "name" parameter
				};

				async Task Act()
					=> await That(methods).HaveParameter("me").AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsWildcard_WhenAllHaveParameterWithWildcard_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, // has "name" parameter
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithString))!, // has "name" parameter
				};

				async Task Act()
					=> await That(methods).HaveParameter("n*e").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndex_WhenAllHaveParameterAtSpecificIndex_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, // int at index 0
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!, // int at index 0
				};

				async Task Act()
					=> await That(methods).HaveParameter<int>().AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndex_WhenNotAllHaveParameterAtSpecificIndex_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStringAndInt))!, // int at index 1
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!, // int at index 0
				};

				async Task Act()
					=> await That(methods).HaveParameter<int>().AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int at index 0,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AtIndexFromEnd_WhenAllHaveParameterAtSpecificIndexFromEnd_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(
						nameof(TestClass.MethodWithIntAndString))!, // string at index 0 from end (last)
					typeof(TestClass).GetMethod(
						nameof(TestClass.MethodWithString))!, // string at index 0 from end (last)
				};

				async Task Act()
					=> await That(methods).HaveParameter<string>().AtIndex(0).FromEnd();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task IgnoringCase_WhenAllHaveParameterIgnoringCase_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, // has "name" parameter
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithString))!, // has "name" parameter
				};

				async Task Act()
					=> await That(methods).HaveParameter("NAME").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDefaultValue_WhenAllHaveParameterWithDefault_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDefaults))!, // bool has default
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDefaultBool))!, // bool has default
				};

				async Task Act()
					=> await That(methods).HaveParameter<bool>().WithDefaultValue();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithoutDefaultValue_WhenAllHaveParameterWithoutDefault_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, // Required parameters
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!, // Required parameter
				};

				async Task Act()
					=> await That(methods).HaveParameter<int>().WithoutDefaultValue();

				await That(Act).DoesNotThrow();
			}

#pragma warning disable CA1822
			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public void MethodWithInt(int value) { }
				public void MethodWithString(string name) { }
				public void MethodWithIntAndString(int value, string name) { }
				public void MethodWithStringAndInt(string name, int value) { }
				public void MethodWithDefaults(int value, bool hasDefault = true, string name = "default") { }
				public void MethodWithDefaultBool(bool hasDefault = false) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
		}
	}
}
