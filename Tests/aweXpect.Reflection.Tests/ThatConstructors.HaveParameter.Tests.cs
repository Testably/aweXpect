using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task HaveParameterByName_WhenAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).HaveParameter("value");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByName_WhenNotAllHaveParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(string),])!, // Has "name" parameter, not "value"
				};

				async Task Act()
					=> await That(constructors).HaveParameter("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter with name "value",
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task HaveParameterByType_WhenAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).HaveParameter<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByType_WhenNotAllHaveParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(string),])!, // No int parameter
				};

				async Task Act()
					=> await That(constructors).HaveParameter<int>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task HaveParameterByTypeAndName_WhenAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).HaveParameter<int>("value");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByTypeAndName_WhenNotAllHaveParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(string),])!, // Has string "name", not int "value"
				};

				async Task Act()
					=> await That(constructors).HaveParameter<int>("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with name "value",
					             but at least one did not
					             """);
			}

			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass(int value) { }
				public TestClass(string name) { }
				public TestClass(int value, string name) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task HaveParameterByName_WhenAllHaveParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).DoesNotComplyWith(they => they.HaveParameter("value"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have parameter with name "value",
					             but all did
					             """);
			}

			[Fact]
			public async Task HaveParameterByName_WhenNotAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(string),])!, // Has "name" parameter, not "value"
				};

				async Task Act()
					=> await That(constructors).DoesNotComplyWith(they => they.HaveParameter("value"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByType_WhenAllHaveParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).DoesNotComplyWith(they => they.HaveParameter<int>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have parameter of type int,
					             but all did
					             """);
			}

			[Fact]
			public async Task HaveParameterByType_WhenNotAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(string),])!, // No int parameter
				};

				async Task Act()
					=> await That(constructors).DoesNotComplyWith(they => they.HaveParameter<int>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParameterByTypeAndName_WhenAllHaveParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
					=> await That(constructors).DoesNotComplyWith(they => they.HaveParameter<int>("value"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have parameter of type int with name "value",
					             but all did
					             """);
			}

			[Fact]
			public async Task HaveParameterByTypeAndName_WhenNotAllHaveParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(string),])!, // Has string "name", not int "value"
				};

				async Task Act()
					=> await That(constructors).DoesNotComplyWith(they => they.HaveParameter<int>("value"));

				await That(Act).DoesNotThrow();
			}

			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass(int value) { }
				public TestClass(string name) { }
				public TestClass(int value, string name) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}

		public sealed class ChainingTests
		{
			[Fact]
			public async Task AsPrefix_WhenAllHaveParameterWithPrefix_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, // has "name" parameter
					typeof(TestClass).GetConstructor([typeof(string),])!, // has "name" parameter
				};

				async Task Act()
					=> await That(constructors).HaveParameter("na").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsRegex_WhenAllHaveParameterMatchingRegex_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, // has "name" parameter
					typeof(TestClass).GetConstructor([typeof(string),])!, // has "name" parameter
				};

				async Task Act()
					=> await That(constructors).HaveParameter("n.*e").AsRegex();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsSuffix_WhenAllHaveParameterWithSuffix_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, // has "name" parameter
					typeof(TestClass).GetConstructor([typeof(string),])!, // has "name" parameter
				};

				async Task Act()
					=> await That(constructors).HaveParameter("me").AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsWildcard_WhenAllHaveParameterWithWildcard_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, // has "name" parameter
					typeof(TestClass).GetConstructor([typeof(string),])!, // has "name" parameter
				};

				async Task Act()
					=> await That(constructors).HaveParameter("n*e").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndex_WhenAllHaveParameterAtSpecificIndex_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, // int at index 0
					typeof(TestClass).GetConstructor([typeof(int),])!, // int at index 0
				};

				async Task Act()
					=> await That(constructors).HaveParameter<int>().AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndex_WhenNotAllHaveParameterAtSpecificIndex_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string), typeof(int),])!, // int at index 1
					typeof(TestClass).GetConstructor([typeof(int),])!, // int at index 0
				};

				async Task Act()
					=> await That(constructors).HaveParameter<int>().AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int at index 0,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task AtIndexFromEnd_WhenAllHaveParameterAtSpecificIndexFromEnd_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([
						typeof(int), typeof(string),
					])!, // string at index 0 from end (last)
					typeof(TestClass).GetConstructor([typeof(string),])!, // string at index 0 from end (last)
				};

				async Task Act()
					=> await That(constructors).HaveParameter<string>().AtIndex(0).FromEnd();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task IgnoringCase_WhenAllHaveParameterIgnoringCase_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, // has "name" parameter
					typeof(TestClass).GetConstructor([typeof(string),])!, // has "name" parameter
				};

				async Task Act()
					=> await That(constructors).HaveParameter("NAME").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDefaultValue_WhenAllHaveParameterWithDefault_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([
						typeof(int), typeof(bool), typeof(string),
					])!, // Only testing the string parameter which has default: name = ""
				};

				async Task Act()
					=> await That(constructors).HaveParameter<string>().WithDefaultValue();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithoutDefaultValue_WhenAllHaveParameterWithoutDefault_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, // Required parameters
					typeof(TestClass).GetConstructor([typeof(int),])!, // Required parameter
				};

				async Task Act()
					=> await That(constructors).HaveParameter<int>().WithoutDefaultValue();

				await That(Act).DoesNotThrow();
			}

			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass(int value) { }
				public TestClass(string name) { }
				public TestClass(bool hasDefault = true) { }
				public TestClass(int value, string name) { }
				public TestClass(string name, int value) { }
				public TestClass(int value, bool hasDefault, string name = "") { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}
	}
}
