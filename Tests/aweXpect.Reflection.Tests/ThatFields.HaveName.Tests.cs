using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class HaveName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldInfosContainFieldInfoWithDifferentName_ShouldFail()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithFields>().Fields();

				async Task Act()
				{
					await That(subject).HaveName("PublicField");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that fields in types matching t => t == typeof(T) in assembly containing type ThatField.ClassWithFields
					             all have name equal to "PublicField",
					             but it contained not matching items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFieldInfosHaveName_ShouldSucceed()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithSingleField>().Fields();

				async Task Act()
				{
					await That(subject).HaveName("MyField");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfosMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithSingleField>().Fields();

				async Task Act()
				{
					await That(subject).HaveName("mYfIELD").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfosMatchSuffix_ShouldSucceed()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithFields>().Fields();

				async Task Act()
				{
					await That(subject).HaveName("Field").AsSuffix();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldsDoNotHaveName_ShouldSucceed()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithFields>().Fields();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("NonExistentField"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldsHaveName_ShouldFail()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithSingleField>().Fields();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("MyField"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that fields in types matching t => t == typeof(T) in assembly containing type ThatField.ClassWithSingleField
					             not all have name equal to "MyField",
					             but it only contained matching items*
					             """).AsWildcard();
			}
		}
	}
}
