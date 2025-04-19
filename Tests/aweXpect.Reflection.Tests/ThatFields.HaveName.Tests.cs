using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class HaveName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldInfosContainFieldInfoWithDifferentName_ShouldFail()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithFields>().Fields();

				async Task Act()
					=> await That(subject).HaveName("PublicField");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fields in types matching t => t == typeof(T) in assembly containing type ThatField.ClassWithFields
					             all have name equal to "PublicField",
					             but it contained not matching types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFieldInfosHaveName_ShouldSucceed()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithSingleField>().Fields();

				async Task Act()
					=> await That(subject).HaveName("MyField");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfosMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithSingleField>().Fields();

				async Task Act()
					=> await That(subject).HaveName("mYfIELD").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfosMatchSuffix_ShouldSucceed()
			{
				Filtered.Fields subject = GetTypes<ThatField.ClassWithFields>().Fields();

				async Task Act()
					=> await That(subject).HaveName("Field").AsSuffix();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
