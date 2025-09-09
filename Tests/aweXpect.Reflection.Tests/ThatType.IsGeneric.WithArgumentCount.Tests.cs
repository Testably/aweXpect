using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed partial class IsGeneric
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
					Type subject = typeof(PublicGenericClass<int>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgumentCount(argumentCount);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with 2 generic arguments,
						             but it was generic PublicGenericClass<int>
						             """);
				}

				[Theory]
				[InlineData(1, false)]
				[InlineData(2, true)]
				public async Task ForTypeWithTwoGenericParameters_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgumentCount(argumentCount);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with 1 generic argument,
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}


				[Fact]
				public async Task WhenTypeIsNotGeneric_ShouldFail()
				{
					Type subject = typeof(PublicClass);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgumentCount(1);

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with 1 generic argument,
						             but it was non-generic PublicClass
						             """);
				}
			}
		}
	}
}
