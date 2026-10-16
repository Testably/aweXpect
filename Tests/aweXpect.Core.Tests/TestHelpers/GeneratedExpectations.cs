using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using aweXpect.Core.Extending;

namespace aweXpect.Core.Tests.TestHelpers;

internal sealed class Account(string name, int level)
{
	public string Name { get; } = name;
	public int Level { get; } = level;
	public CancellationToken ReceivedToken { get; set; }

	public override string ToString() => Name;
}

internal static partial class ThatAccount
{
	[GenerateCollectionExpectation("HaveOwner", "have owner {owner}")]
	[GenerateExpectation("HasOwner", "has owner {owner}",
		NegatedName = "DoesNotHaveOwner", NegatedExpectationText = "does not have owner {owner}")]
	private static bool HasOwner(Account account, string owner) => account.Name == owner;

	[GenerateCollectionExpectation("AreActive", "is active", "are active")]
	[GenerateExpectation("Is{Not}Active", "is {not} active")]
	private static bool IsActive(Account account, CancellationToken cancellationToken)
	{
		account.ReceivedToken = cancellationToken;
		return account.Level > 0;
	}

	[GenerateExpectation("Is{Not}AtLevel", "is {not} at level {level}", Result = nameof(DescribeLevel))]
	private static bool IsAtLevel(Account account, int level) => account.Level == level;

	[GenerateExpectation("Is{Not}Named", "is {not} named {name}")]
	private static bool IsNamed(Account account, string name) => account.Name == name;

	[GenerateCollectionExpectation("AreNullOrUnnamed", "is null or unnamed", "are null or unnamed")]
	[GenerateExpectation("Is{Not}NullOrUnnamed", "is {not} null or unnamed")]
	private static bool IsNullOrUnnamed([NotNullWhen(false)] Account? account)
		=> account is null || account.Name.Length == 0;

	[GenerateCollectionExpectation("ArePremium", "is premium", "are premium")]
	[GenerateExpectation("Is{Not}Premium", "is {not} premium")]
	private static async ValueTask<bool> IsPremium(Account account)
	{
		await Task.Yield();
		return account.Level > 5;
	}

	[GenerateExpectation("IsTheOwner", "is {not} the owner")]
	private static bool IsTheOwner(Account account) => account.Name == "owner";

	[GenerateCollectionExpectation("AreVerified", "is verified", "are verified")]
	[GenerateExpectation("Is{Not}Verified", "is {not} verified")]
	private static async Task<bool> IsVerified(Account account, CancellationToken cancellationToken)
	{
		await Task.Yield();
		account.ReceivedToken = cancellationToken;
		return account.Level > 0;
	}

	[GenerateExpectation("Is{Not}AtLeastLevel", "is {not} at least level {minimum}",
		Result = nameof(DescribeMinimumLevel))]
	private static bool IsAtLeastLevel(Account account, int minimum) => account.Level >= minimum;

	private static string DescribeLevel(Account account) => $"was at level {account.Level}";

	private static string DescribeMinimumLevel(Account account, int minimum, string it, ExpectationGrammars grammars)
		=> $"{it}{grammars.SubjectVerb(it, " was ", " were ")}at level {account.Level} instead of {minimum}";
}

internal static partial class ThatLuckyNumber
{
	[GenerateExpectation("Is{Not}AnyOf", "is {not} any of {values}")]
	private static bool IsAnyOf(int value, params int[] values) => values.Contains(value);

	[GenerateCollectionExpectation("AreBetween", "is between {minimum} and {maximum}", "are between {minimum} and {maximum}",
		NullableOverload = false)]
	[GenerateExpectation("Is{Not}Between", "is {not} between {minimum} and {maximum}", NullableOverload = false)]
	private static bool IsBetween(int value, int minimum, int maximum) => value >= minimum && value <= maximum;

	[GenerateCollectionExpectation("AreLucky", "are lucky")]
	[GenerateExpectation("Is{Not}Lucky", "is {not} lucky")]
	private static bool IsLucky(int value) => value == 7;

	[GenerateCollectionExpectation("AreOdd", "is odd", "are odd")]
	[GenerateExpectation("Is{Not}Odd", "is {not} odd")]
	private static bool IsOdd([DisallowNull] int? value) => value.GetValueOrDefault() % 2 != 0;

	[GenerateCollectionExpectation("AreSingleDigits", "is a single digit", "are single digits")]
	private static bool IsSingleDigit(int value)
		=> value >= 0 ? value < 10 : throw new ArgumentOutOfRangeException(nameof(value));
}

internal static partial class ThatWord
{
	[GenerateCollectionExpectation("AreShouted", "is shouted", "are shouted")]
	private static bool IsShouted(string value)
		=> string.Equals(value, value.ToUpperInvariant(), StringComparison.Ordinal);
}
