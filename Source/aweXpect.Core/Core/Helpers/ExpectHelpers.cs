using System;

namespace aweXpect.Core.Helpers;

internal static class ExpectHelpers
{
	public static IExpectThat<T> Get<T>(this IThat<T> subject)
	{
		if (subject is IExpectThat<T> expectThat)
		{
			return expectThat;
		}

		throw Tracing.WriteException(
			new NotSupportedException("IThat<T> must also implement IExpectThat<T>"));
	}
}
