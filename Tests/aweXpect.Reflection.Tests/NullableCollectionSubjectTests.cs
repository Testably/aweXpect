using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed class NullableCollectionSubjectTests
{
	[Fact]
	public async Task WhenChainedAfterCoreCollectionExpectation_ShouldVerifyTheSubject()
	{
		IEnumerable<Type?> subject = [typeof(string), typeof(object),];

		async Task Act()
		{
			await That(subject).All().Satisfy(type => type is not null).And.AreSealed();
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             satisfies type => type is not null for all items and are all sealed,
			             but it contained non-sealed types

			             Not matching items:
			             [
			               object
			             ]
			             """);
	}

	[Fact]
	public async Task WhenReturnIsChainedAfterAnotherMethodsExpectation_ShouldVerifyTheSubject()
	{
		IEnumerable<MethodInfo> subject = [typeof(string).GetMethod(nameof(string.IsNullOrEmpty))!,];

		async Task Act()
		{
			await That(subject).AreStatic().And.Return<int>();
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             are all static and all return int,
			             but it contained not matching methods

			             Not matching items:
			             [
			               bool string.IsNullOrEmpty(string value)
			             ]
			             """);
	}

	[Fact]
	public async Task WhenReturnIsUsedOnNullableMethodItems_ShouldVerifyTheSubject()
	{
		IEnumerable<MethodInfo?> subject = [typeof(string).GetMethod(nameof(string.IsNullOrEmpty)),];

		async Task Act()
		{
			await That(subject).Return<bool>().And.ReturnExactly<bool>().And.ReturnVoid();
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             all return bool and all return exactly bool and all return void,
			             but it contained not matching methods

			             Not matching items:
			             [
			               bool string.IsNullOrEmpty(string value)
			             ]
			             """);
	}

	[Fact]
	public async Task WhenUsedInAllComplyWith_ShouldVerifyEachItem()
	{
		IEnumerable<Type?>?[] subject = [[typeof(string),], [typeof(object),],];

		async Task Act()
		{
			await That(subject).All().ComplyWith(types => types.AreSealed());
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             are all sealed for all items,
			             but only 1 of 2 were

			             Not matching items:
			             [
			               [
			                 object
			               ]
			             ]

			             Collection:
			             [
			               [
			                 string
			               ],
			               [
			                 object
			               ]
			             ]

			             Not matching items (item [1]):
			             [
			               object
			             ]
			             """);
	}

	[Fact]
	public async Task WhenUsedInWhoseForArrayMember_ShouldVerifyTheMember()
	{
		Holder subject = new([typeof(object),]);

		async Task Act()
		{
			await That(subject).Whose(h => h.Array, types => types.AreSealed());
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             whose Array are all sealed,
			             but Array contained non-sealed types

			             Not matching items (Array):
			             [
			               object
			             ]
			             """);
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task WhenUsedInWhoseForAsyncEnumerableMember_ShouldVerifyTheMember()
	{
		Holder subject = new([typeof(object),]);

		async Task Act()
		{
			await That(subject).Whose(h => h.AsyncEnumerable, types => types.AreSealed());
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             whose AsyncEnumerable are all sealed,
			             but AsyncEnumerable contained non-sealed types

			             Not matching items (AsyncEnumerable):
			             [
			               object
			             ]
			             """);
	}
#endif

	[Fact]
	public async Task WhenUsedInWhoseForEnumerableMember_ShouldVerifyTheMember()
	{
		Holder subject = new([typeof(object),]);

		async Task Act()
		{
			await That(subject).Whose(h => h.Enumerable, types => types.AreSealed());
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             whose Enumerable are all sealed,
			             but Enumerable contained non-sealed types

			             Not matching items (Enumerable):
			             [
			               object
			             ]
			             """);
	}

	private sealed class Holder(Type[] types)
	{
		public Type[] Array { get; } = types;
		public IEnumerable<Type?> Enumerable { get; } = types;
#if NET8_0_OR_GREATER
		public IAsyncEnumerable<Type?> AsyncEnumerable { get; } = ToAsyncEnumerable(types);

		private static async IAsyncEnumerable<Type?> ToAsyncEnumerable(Type[] types)
		{
			foreach (Type type in types)
			{
				await Task.Yield();
				yield return type;
			}
		}
#endif
	}
}
