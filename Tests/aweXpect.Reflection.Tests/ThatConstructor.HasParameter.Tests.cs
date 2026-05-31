using System.IO;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

// The System.Type overloads are deliberately exercised here alongside the generic ones.
#pragma warning disable CA2263 // Prefer generic overload when type is known

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
				{
					await That(constructorInfo).HasParameter<int>("val").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndex_WhenParameterDoesNotExistAtSpecificIndex_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter<string>().AtIndex(0);
				}

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
				{
					await That(constructorInfo).HasParameter<int>().AtIndex(0);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AtIndexFromEnd_WhenParameterExistsAtSpecificIndexFromEnd_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter<string>().AtIndex(0).FromEnd();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByName_WhenParameterDoesNotExist_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter("value");
				}

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
				{
					await That(constructorInfo).HasParameter("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterDoesNotExist_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter<int>();
				}

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
				{
					await That(constructorInfo).HasParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterIsSubtype_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter<IDisposable>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeArgument_WhenParameterDoesNotExist_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int,
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasParameterByTypeArgument_WhenParameterExists_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeArgument_WhenParameterIsSubtype_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(Stream),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(IDisposable));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterDoesNotExist_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter<int>("value");
				}

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
				{
					await That(constructorInfo).HasParameter<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenWrongType_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter<string>("value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type string with name "value",
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasParameterByTypeArgumentAndName_WhenParameterDoesNotExist_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type int with name "value",
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasParameterByTypeArgumentAndName_WhenParameterExists_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeArgumentAndName_WhenWrongType_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(string), "value");
				}

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
				{
					await That(constructorInfo).HasParameter<int>("VALUE").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDefaultValue_WhenParameterHasDefault_ShouldSucceed()
			{
				ConstructorInfo constructorInfo =
					typeof(TestClass).GetConstructor([typeof(int), typeof(bool), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter<string>().WithDefaultValue();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithoutDefaultValue_WhenParameterHasNoDefault_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter<int>().WithoutDefaultValue();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_AsPrefix_WhenParameterNameStartsWithPrefix_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(int), "val").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_AsSuffix_WhenParameterNameEndsWithSuffix_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(string), "ame").AsSuffix();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_AsRegex_WhenParameterNameMatchesRegex_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(string), "n[a-z]*e").AsRegex();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_AsWildcard_WhenParameterNameMatchesWildcard_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(string), "n??e").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_AtIndex_WhenParameterDoesNotExistAtSpecificIndex_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(string)).AtIndex(0);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has parameter of type string at index 0,
					             but it did not
					             """);
			}

			[Fact]
			public async Task Type_AtIndex_WhenParameterExistsAtSpecificIndex_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(int)).AtIndex(0);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_AtIndexFromEnd_WhenParameterExistsAtSpecificIndexFromEnd_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(string)).AtIndex(0).FromEnd();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_IgnoringCase_WhenParameterNameDiffersInCase_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(int), "VALUE").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_Using_WhenParameterNameMatchesWithCustomComparer_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(string), "nAmE")
						.Using(new IgnoreCaseForVocalsComparer());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_WithDefaultValue_WhenParameterHasDefault_ShouldSucceed()
			{
				ConstructorInfo constructorInfo =
					typeof(TestClass).GetConstructor([typeof(int), typeof(bool), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(string)).WithDefaultValue();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Type_WithoutDefaultValue_WhenParameterHasNoDefault_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameter(typeof(int)).WithoutDefaultValue();
				}

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
				public TestClass(Stream stream) { }
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
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter("value"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByName_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter("value"));
				}

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
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter<int>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByType_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter<int>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter of type int,
					             but it did
					             """);
			}

			[Fact]
			public async Task HasParameterByTypeArgument_WhenParameterDoesNotExist_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter(typeof(int)));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeArgument_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter(typeof(int)));
				}

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
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter<int>("value"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeAndName_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter<int>("value"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter of type int with name "value",
					             but it did
					             """);
			}

			[Fact]
			public async Task HasParameterByTypeArgumentAndName_WhenParameterDoesNotExist_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter(typeof(int), "value"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParameterByTypeArgumentAndName_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameter(typeof(int), "value"));
				}

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
				public TestClass(Stream stream) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}
	}
}
