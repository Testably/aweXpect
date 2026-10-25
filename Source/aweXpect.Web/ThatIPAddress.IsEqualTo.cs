using System.Net;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatIPAddress
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IPAddress.Equals(object)" />, so an IPv4-mapped IPv6 address, e.g. <c>::ffff:10.0.0.1</c>,
	///     is not equal to the IPv4 address <c>10.0.0.1</c>, unless
	///     <see cref="IgnoringIPv4MappedToIPv6{TSelf}" /> is specified.
	/// </remarks>
	public static ObjectEqualityResult<IPAddress?, IThat<IPAddress?>, IPAddress?> IsEqualTo(
		this IThat<IPAddress?> subject,
		IPAddress? expected)
	{
		ObjectMatchType<IPAddress?> options = ObjectMatchType<IPAddress?>.Default;
		return new ObjectEqualityResult<IPAddress?, IThat<IPAddress?>, IPAddress?>(
			subject.Get().ExpectationBuilder.AddEqualityConstraint(expected, options,
			out IOptionsHolderSource<ObjectMatchType<IPAddress?>> optionsSource),
			subject,
			optionsSource);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IPAddress.Equals(object)" />, so an IPv4-mapped IPv6 address, e.g. <c>::ffff:10.0.0.1</c>,
	///     is not equal to the IPv4 address <c>10.0.0.1</c>, unless
	///     <see cref="IgnoringIPv4MappedToIPv6{TSelf}" /> is specified.
	/// </remarks>
	public static ObjectEqualityResult<IPAddress?, IThat<IPAddress?>, IPAddress?> IsNotEqualTo(
		this IThat<IPAddress?> subject,
		IPAddress? unexpected)
	{
		ObjectMatchType<IPAddress?> options = ObjectMatchType<IPAddress?>.Default;
		return new ObjectEqualityResult<IPAddress?, IThat<IPAddress?>, IPAddress?>(
			subject.Get().ExpectationBuilder.AddEqualityConstraint(unexpected, options,
			out IOptionsHolderSource<ObjectMatchType<IPAddress?>> optionsSource, isNegated: true),
			subject,
			optionsSource);
	}

	/// <summary>
	///     Maps IPv4-mapped IPv6 addresses, e.g. <c>::ffff:10.0.0.1</c>, to their IPv4 address before comparing,
	///     so that they are equal to the IPv4 address, e.g. <c>10.0.0.1</c>.
	/// </summary>
	/// <remarks>
	///     Like the other options that specify how two values are compared, e.g. <c>Using(comparer)</c>, it cannot be
	///     combined with another one.
	///     <seealso cref="IPAddress.IsIPv4MappedToIPv6" />
	/// </remarks>
	public static TSelf IgnoringIPv4MappedToIPv6<TSelf>(this IObjectEqualityResult<TSelf, IPAddress?> result)
		where TSelf : IObjectEqualityResult<TSelf, IPAddress?>
	{
		result.Options.SetMatchType(IgnoringIPv4MappedToIPv6MatchType.Instance);
		return (TSelf)result;
	}

	private sealed class IgnoringIPv4MappedToIPv6MatchType : ObjectMatchType<IPAddress?>
	{
		public static readonly IgnoringIPv4MappedToIPv6MatchType Instance = new();
		private static readonly Matcher SharedMatcher = new();

		private static readonly MatchPhrase SharedPhrase = MatchPhrase.EqualTo
			.WithSuffix(" ignoring IPv4-to-IPv6 mapping")
			.WithName(" ignoring IPv4-to-IPv6 mapping");

		private IgnoringIPv4MappedToIPv6MatchType() : base(nameof(IgnoringIPv4MappedToIPv6))
		{
		}

		public override MatchPhrase Phrase => SharedPhrase;

		public override ObjectMatcher<IPAddress?> ForEvaluation(MatchEnvironment environment)
			=> SharedMatcher;

		private sealed class Matcher : ObjectMatcher<IPAddress?, IPAddress?>
		{
			public override ValueTask<MatchResult> Match(IPAddress? actual, IPAddress? expected)
				=> new(MatchResult.From(Equals(Normalize(actual), Normalize(expected))));

			private static IPAddress? Normalize(IPAddress? address)
				=> address is { IsIPv4MappedToIPv6: true, } ? address.MapToIPv4() : address;
		}
	}
}
