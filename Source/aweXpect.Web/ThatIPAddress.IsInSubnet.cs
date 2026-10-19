using System;
using System.Net;
using System.Net.Sockets;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatIPAddress
{
	/// <summary>
	///     Verifies that the subject is in the <paramref name="subnet" /> given in CIDR notation, e.g. "10.0.0.0/8".
	/// </summary>
	/// <remarks>
	///     An IPv4 subject is compared as IPv4-mapped IPv6 address against an IPv6 subnet,
	///     and an IPv4-mapped IPv6 subject is compared as IPv4 address against an IPv4 subnet.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IPAddress, IThat<IPAddress?>> IsInSubnet(this IThat<IPAddress?> subject, string subnet)
		=> new(subject.Get().ExpectationBuilder.AddConstraint(Subnet.Parse(subnet, nameof(subnet)),
				static (subnet, it, grammars) =>
					new IsKindConstraint(it, grammars, subnet.Contains, subnet.Expectation, subnet.Expectation)),
			subject);

	/// <summary>
	///     Verifies that the subject is not in the <paramref name="subnet" /> given in CIDR notation, e.g. "10.0.0.0/8".
	/// </summary>
	/// <remarks>
	///     An IPv4 subject is compared as IPv4-mapped IPv6 address against an IPv6 subnet,
	///     and an IPv4-mapped IPv6 subject is compared as IPv4 address against an IPv4 subnet.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IPAddress, IThat<IPAddress?>> IsNotInSubnet(this IThat<IPAddress?> subject, string subnet)
		=> new(subject.Get().ExpectationBuilder.AddConstraint(Subnet.Parse(subnet, nameof(subnet)),
				static (subnet, it, grammars) =>
					new IsKindConstraint(it, grammars, subnet.Contains, subnet.Expectation, subnet.Expectation)
						.Invert()),
			subject);

	private sealed class Subnet(IPAddress network, byte[] networkBytes, int prefixLength)
	{
		public string Expectation { get; } = $"in subnet {network}/{prefixLength}";

		public static Subnet Parse(string subnet, string paramName)
		{
			ThrowHelper.ThrowIfNull(subnet, paramName);
			int slashIndex = subnet.IndexOf('/');
			string address = slashIndex > 0 ? subnet.Substring(0, slashIndex) : "";
			if (IsAddress(address)
			    && IPAddress.TryParse(address, out IPAddress? network)
			    && (network.AddressFamily != AddressFamily.InterNetwork || network.ToString() == address)
			    && TryParsePrefix(subnet, slashIndex + 1, out int prefixLength))
			{
				byte[] networkBytes = network.GetAddressBytes();
				if (prefixLength <= networkBytes.Length * 8 && HasNoHostBits(networkBytes, prefixLength))
				{
					return new Subnet(network, networkBytes, prefixLength);
				}
			}

			throw Tracing.WriteException(new ArgumentException(
				$"The '{paramName}' must be a valid CIDR notation, e.g. \"10.0.0.0/8\".", paramName));
		}

		public bool Contains(IPAddress address)
		{
			if (address.AddressFamily != network.AddressFamily)
			{
				if (address.AddressFamily == AddressFamily.InterNetwork)
				{
					address = address.MapToIPv6();
				}
				else if (address.IsIPv4MappedToIPv6)
				{
					address = address.MapToIPv4();
				}
				else
				{
					return false;
				}
			}

			byte[] bytes = address.GetAddressBytes();
			for (int i = 0; i < bytes.Length; i++)
			{
				if ((bytes[i] & Mask(prefixLength, i)) != networkBytes[i])
				{
					return false;
				}
			}

			return true;
		}

		private static bool IsAddress(string address)
		{
			foreach (char c in address)
			{
				if (c != '.' && c != ':' && !Uri.IsHexDigit(c))
				{
					return false;
				}
			}

			return address.Length > 0;
		}

		private static bool TryParsePrefix(string subnet, int startIndex, out int prefixLength)
		{
			prefixLength = 0;
			int length = subnet.Length - startIndex;
			if (length is < 1 or > 3 || (length > 1 && subnet[startIndex] == '0'))
			{
				return false;
			}

			for (int i = startIndex; i < subnet.Length; i++)
			{
				char c = subnet[i];
				if (c is < '0' or > '9')
				{
					return false;
				}

				prefixLength = (prefixLength * 10) + (c - '0');
			}

			return true;
		}

		private static bool HasNoHostBits(byte[] bytes, int prefixLength)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				if ((bytes[i] & ~Mask(prefixLength, i)) != 0)
				{
					return false;
				}
			}

			return true;
		}

		private static int Mask(int prefixLength, int byteIndex)
		{
			int bits = Math.Max(0, Math.Min(8, prefixLength - (byteIndex * 8)));
			return (0xFF << (8 - bits)) & 0xFF;
		}
	}
}
