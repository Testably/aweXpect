using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class DoesNotHaveParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task AtIndex_WhenParameterIsAtIndex_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<string>().AtIndex(0);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtIndexFromEnd_WhenParameterIsAtIndexFromEnd_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>().AtIndex(1).FromEnd();
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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter("other");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByName_WhenParameterExists_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<long>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByType_WhenParameterExists_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithDerivedParameter))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have parameter of type ThatMethod.DoesNotHaveParameter.DummyBase,
					             but it did
					             """);
			}

			[Test]
			public async Task ByTypeAndName_WhenNameDiffers_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<int>("text");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ByTypeAndName_WhenParameterExists_ShouldFail()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter<string>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Type_ByType_WhenParameterDoesNotExist_ShouldSucceed()
			{
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;
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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;
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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;
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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoesNotHaveParameter(unexpectedType, "text");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

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
				MethodInfo subject = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParameters))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveParameter<int>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		private abstract class TestClass
		{
			public abstract void MethodWithParameters(int value, string text);
			public abstract void MethodWithDerivedParameter(Dummy dummy);
			public abstract void MethodWithOptionalParameter(int value = 42);
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
