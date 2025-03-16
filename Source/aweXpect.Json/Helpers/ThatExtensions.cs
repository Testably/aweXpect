using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class ThatExtensions
{
	[ExcludeFromCodeCoverage]
	public static IExpectThat<T> Get<T>(this IThat<T> subject)
	{
		if (subject is IExpectThat<T> expectThat)
		{
			return expectThat;
		}

		throw new NotSupportedException("IThat<T> must also implement IExpectThat<T>");
	}
}
