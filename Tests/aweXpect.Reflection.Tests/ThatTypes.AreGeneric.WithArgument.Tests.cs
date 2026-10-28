using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class AreGeneric
	{
		public sealed class WithArgument
		{
			public sealed class GenericTests
			{
				[Test]
				[Arguments(0, false)]
				[Arguments(1, true)]
				[Arguments(2, false)]
				public async Task AtIndex_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>().AtIndex(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument of type ThatTypes.BaseClass at index {{index}},
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
				[Arguments(0, true)]
				[Arguments(1, false)]
				[Arguments(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>().AtIndexFromEnd(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument of type ThatTypes.BaseClass at index {{index}} from end,
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
				public async Task ForTypeWithRestrictedMatchingArgument_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ForTypeWithRestrictedNotMatchingArgument_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<ThatMethod.DerivedClass>();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             all are generic with argument of type ThatMethod.DerivedClass,
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
				public async Task ForTypeWithUnrestrictedArgument_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithOneArgument<int>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithOneArgument<int>]
						             all are generic with argument of type ThatTypes.BaseClass,
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
			}

			public sealed class GenericNamedTests
			{
				[Test]
				[Arguments(0, false)]
				[Arguments(1, true)]
				[Arguments(2, false)]
				public async Task AtIndex_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>("TBar").AtIndex(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument of type ThatTypes.BaseClass and name equal to "TBar" at index {{index}},
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
				[Arguments(0, true)]
				[Arguments(1, false)]
				[Arguments(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>("TBar").AtIndexFromEnd(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument of type ThatTypes.BaseClass and name equal to "TBar" at index {{index}} from end,
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
				public async Task ForTypeWithRestrictedMatchingArgumentAndName_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>("TBar");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ForTypeWithRestrictedMatchingArgumentAndNotMatchingName_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>("Tbar");
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             all are generic with argument of type ThatTypes.BaseClass and name equal to "Tbar",
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
				public async Task ForTypeWithRestrictedNotMatchingArgumentAndMatchingName_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<ThatMethod.DerivedClass>("TBar");
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             all are generic with argument of type ThatMethod.DerivedClass and name equal to "TBar",
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
				public async Task ForTypeWithUnrestrictedArgument_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithOneArgument<int>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>("TBar");
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithOneArgument<int>]
						             all are generic with argument of type ThatTypes.BaseClass and name equal to "TBar",
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
				[Arguments("TBa", true)]
				[Arguments("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>(prefix).AsPrefix();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument of type ThatTypes.BaseClass and name starting with "{{prefix}}",
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
				[Arguments("T[a-zA-Z]*r", true)]
				[Arguments("T[a-zA-Z]*o", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>(regex).AsRegex();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument of type ThatTypes.BaseClass and name matching regex "{{regex}}",
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
				[Arguments("Bar", true)]
				[Arguments("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>(suffix).AsSuffix();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument of type ThatTypes.BaseClass and name ending with "{{suffix}}",
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
				[Arguments("T??r", true)]
				[Arguments("T??o", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>(wildcard).AsWildcard();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument of type ThatTypes.BaseClass and name matching "{{wildcard}}",
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
				[Arguments(true, true)]
				[Arguments(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>("TBAR")
							.IgnoringCase(ignoreCase);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             all are generic with argument of type ThatTypes.BaseClass and name equal to "TBAR",
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
				public async Task ShouldSupportUsingCustomComparer()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument<BaseClass>("TBAr")
							.Using(new IgnoreCaseForVocalsComparer());
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NamedTests
			{
				[Test]
				[Arguments(0, false)]
				[Arguments(1, true)]
				[Arguments(2, false)]
				public async Task AtIndex_ForTypeWithMatchingArgumentName_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument("TBar").AtIndex(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument name equal to "TBar" at index {{index}},
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
				[Arguments(0, true)]
				[Arguments(1, false)]
				[Arguments(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgumentName_ShouldSucceedWhenIndexMatches(
					int index,
					bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument("TBar").AtIndexFromEnd(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument name equal to "TBar" at index {{index}} from end,
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
				public async Task ForTypeWithMatchingArgumentName_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument("TBar");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ForTypeWithNotMatchingArgumentName_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument("Tbar");
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             all are generic with argument name equal to "Tbar",
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
				[Arguments("TBa", true)]
				[Arguments("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument(prefix).AsPrefix();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument name starting with "{{prefix}}",
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
				[Arguments("T[a-zA-Z]*r", true)]
				[Arguments("T[a-zA-Z]*s", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument(regex).AsRegex();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument name matching regex "{{regex}}",
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
				[Arguments("Bar", true)]
				[Arguments("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument(suffix).AsSuffix();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument name ending with "{{suffix}}",
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
				[Arguments("T??r", true)]
				[Arguments("T??s", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument(wildcard).AsWildcard();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               all are generic with argument name matching "{{wildcard}}",
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
				[Arguments(true, true)]
				[Arguments(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument("TBAR").IgnoringCase(ignoreCase);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             all are generic with argument name equal to "TBAR",
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
				public async Task ShouldSupportUsingCustomComparer()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
					{
						await That(subject).AreGeneric().WithArgument("TBAr")
							.Using(new IgnoreCaseForVocalsComparer());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
