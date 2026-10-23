using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class DoesNotHaveParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task AtIndex_WhenParameterIsAtIndex_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<string>().AtIndex(1);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type string at index 1,
					             but it did
					             """);
			}

			[Test]
			public async Task AtIndex_WhenParameterIsAtOtherIndex_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<string>().AtIndex(0);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtIndexFromEnd_WhenParameterIsAtIndexFromEnd_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>().AtIndexFromEnd(1);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type int at index 1 from end,
					             but it did
					             """);
			}

			[Test]
			public async Task ByName_AsPrefix_WhenParameterNameStartsWithPrefix_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter("val").AsPrefix();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter with name "val",
					             but it did
					             """);
			}

			[Test]
			public async Task ByName_WhenParameterDoesNotExist_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter("other");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByName_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter with name "value",
					             but it did
					             """);
			}

			[Test]
			public async Task ByType_WhenParameterDoesNotExist_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<long>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type int,
					             but it did
					             """);
			}

			[Test]
			public async Task ByType_WhenParameterIsSubtype_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(Dummy),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type ThatConstructor.DoesNotHaveParameter.DummyBase,
					             but it did
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenNameDiffers_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>("text");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByTypeAndName_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>("value");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type int with name "value",
					             but it did
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenParameterMatchesIgnoringCase_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>("VALUE").IgnoringCase();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type int with name "VALUE",
					             but it did
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenTypeDiffers_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<string>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Type_ByType_WhenParameterDoesNotExist_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;
				Type unexpectedType = typeof(long);

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter(unexpectedType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Type_ByType_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type int,
					             but it did
					             """);
			}

			[Test]
			public async Task Type_ByTypeAndName_WhenParameterExists_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter(unexpectedType, "text");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type string with name "text",
					             but it did
					             """);
			}

			[Test]
			public async Task Type_ByTypeAndName_WhenTypeDiffers_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter(unexpectedType, "text");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithDefaultValue_WhenParameterHasDefaultValue_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(bool), typeof(int),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>().WithDefaultValue(42);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type int,
					             but it did
					             """);
			}

			[Test]
			public async Task WithDefaultValue_WhenParameterHasNoDefaultValue_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>().WithDefaultValue();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenParameterDoesNotExist_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveParameter<long>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has parameter of type long,
					             but it did not
					             """);
			}

			[Test]
			public async Task WhenParameterExists_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveParameter<int>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		private class TestClass
		{
			public TestClass(int value, string text) { }
			public TestClass(Dummy dummy) { }
			public TestClass(bool flag, int value = 42) { }
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
