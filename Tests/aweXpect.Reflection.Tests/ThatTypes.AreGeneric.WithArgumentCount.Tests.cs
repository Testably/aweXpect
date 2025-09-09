using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class AreGeneric
	{
		public sealed class WithArgumentCount
		{
			public sealed class Tests
			{
				[Theory]
				[InlineData(1, true)]
				[InlineData(2, false)]
				public async Task ForTypeWithOneGenericParameter_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithOneArgument<int>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgumentCount(argumentCount);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithOneArgument<int>]
						             are all generic with 2 generic arguments,
						             but it contained not matching types [
						               ThatTypes.GenericClassWithOneArgument<int>
						             ]
						             """);
				}

				[Theory]
				[InlineData(1, false)]
				[InlineData(2, true)]
				public async Task ForTypeWithTwoGenericParameters_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgumentCount(argumentCount);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             are all generic with 1 generic argument,
						             but it contained not matching types [
						               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						             ]
						             """);
				}


				[Fact]
				public async Task WhenTypeIsNotGeneric_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(PublicClass), typeof(UnrelatedClass));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgumentCount(1);

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that in types [PublicClass, ThatTypes.UnrelatedClass]
						             are all generic with 1 generic argument,
						             but it contained not matching types [
						               PublicClass,
						               ThatTypes.UnrelatedClass
						             ]
						             """);
				}
			}
		}
	}
}
