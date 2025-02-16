using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class ThatExtensions
{
	[ExcludeFromCodeCoverage]
	public static IThatIs<T> ThatIs<T>(this IThat<T> subject)
	{
		if (subject is IThatIs<T> thatIs)
		{
			return thatIs;
		}

		if (subject is IThatVerb<T> thatVerb)
		{
			return new ThatSubject<T>(thatVerb.ExpectationBuilder);
		}

		throw new NotSupportedException("IThat<T> must also implement IThatIs<T>");
	}
}
