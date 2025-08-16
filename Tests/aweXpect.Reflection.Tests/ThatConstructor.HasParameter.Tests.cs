using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task AsPrefix_WhenParameterNameStartsWithPrefix_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<int>("val").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndex_WhenParameterDoesNotExistAtSpecificIndex_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<string>().AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type string at index 0,
					             but it did not
					             """);
			}

			[Fact]
			public async Task AtIndex_WhenParameterExistsAtSpecificIndex_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<int>().AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndexFromEnd_WhenParameterExistsAtSpecificIndexFromEnd_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<string>().AtIndex(0).FromEnd();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByName_WhenParameterDoesNotExist_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter with name "value",
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasParameterByName_WhenParameterExists_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter("value");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterDoesNotExist_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<int>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int,
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterExists_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterDoesNotExist_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<int>("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value",
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterExists_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<int>("value");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenWrongType_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<string>("value");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type string with name "value",
					             but it did not
					             """);
			}

			[Fact]
			public async Task IgnoringCase_WhenParameterNameDiffersInCase_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<int>("VALUE").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDefaultValue_WhenParameterHasDefault_ShouldSucceed()
			{
				ConstructorInfo constructorInfo =
					typeof(TestClass).GetConstructor([typeof(int), typeof(bool), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<string>().WithDefaultValue();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithoutDefaultValue_WhenParameterHasNoDefault_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).HasParameter<int>().WithoutDefaultValue();

				await That(Act).DoesNotThrow();
			}

			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass() { }
				public TestClass(string name) { }
				public TestClass(int value, string name) { }
				public TestClass(int value, bool hasDefault = true, string name = "") { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task HasParameterByName_WhenParameterDoesNotExist_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter("value"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByName_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter("value"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter with name "value",
					             but it did
					             """);
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterDoesNotExist_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter<int>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter<int>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter of type int,
					             but it did
					             """);
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterDoesNotExist_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter<int>("value"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
					=> await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter<int>("value"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter of type int with name "value",
					             but it did
					             """);
			}

			// ReSharper disable UnusedParameter.Local
			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass() { }
				public TestClass(string name) { }
				public TestClass(int value, string name) { }
				public TestClass(int value, bool hasDefault = true, string name = "") { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}
	}
}
