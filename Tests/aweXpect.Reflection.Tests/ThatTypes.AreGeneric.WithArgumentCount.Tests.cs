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
				[Test]
				[Arguments(1, true)]
				[Arguments(2, false)]
				public async Task ForTypeWithOneGenericParameter_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithOneArgument<int>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgumentCount(argumentCount);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithOneArgument<int>]
						             all are generic with 2 generic arguments,
						             but none of 1 were

						             Not matching items:
						             [
						               ThatTypes.GenericClassWithOneArgument<int>
						             ]

						             Collection:
						             [
						               ThatTypes.GenericClassWithOneArgument<int>
						             ]
						             """);
				}

				[Test]
				[Arguments(1, false)]
				[Arguments(2, true)]
				public async Task ForTypeWithTwoGenericParameters_ShouldSucceedWhenArgumentCountMatches(
					int argumentCount, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgumentCount(argumentCount);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             all are generic with 1 generic argument,
						             but none of 1 were

						             Not matching items:
						             [
						               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						             ]

						             Collection:
						             [
						               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						             ]
						             """);
				}


				[Test]
				public async Task WhenTypeIsNotGeneric_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(PublicClass), typeof(UnrelatedClass));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgumentCount(1);
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that in types [PublicClass, ThatTypes.UnrelatedClass]
						             all are generic with 1 generic argument,
						             but none of 2 were

						             Not matching items:
						             [
						               PublicClass,
						               ThatTypes.UnrelatedClass
						             ]

						             Collection:
						             [
						               PublicClass,
						               ThatTypes.UnrelatedClass
						             ]
						             """);
				}
			}
		}
	}
}
