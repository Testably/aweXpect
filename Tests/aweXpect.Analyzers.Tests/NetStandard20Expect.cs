namespace aweXpect.Analyzers.Tests;

/// <summary>
///     The <c>Expect.That</c> overloads of the netstandard2.0 build of aweXpect.Core, which has no overloads for
///     delegates returning a <c>ValueTask</c>, so that the binding of such delegates can be tested with the reference
///     assemblies of any target framework.
/// </summary>
internal static class NetStandard20Expect
{
	public const string Source =
		"""
		using System;
		using System.Threading;
		using System.Threading.Tasks;

		namespace aweXpect
		{
		    public static class Expect
		    {
		        public static void That<T>(T subject, string doNotPopulateThisValue = "") { }
		        public static void That<T>(T[] subject, string doNotPopulateThisValue = "") { }
		        public static void That<T>(Task<T> subject, string doNotPopulateThisValue = "") { }
		        public static void That<T>(ValueTask<T> subject, string doNotPopulateThisValue = "") { }
		        public static void That(Action @delegate, string doNotPopulateThisValue = "") { }
		        public static void That(Action<CancellationToken> @delegate, string doNotPopulateThisValue = "") { }
		        public static void That(Func<Task> @delegate, string doNotPopulateThisValue = "") { }
		        public static void That(Func<CancellationToken, Task> @delegate, string doNotPopulateThisValue = "") { }
		        public static void That(Task subject, string doNotPopulateThisValue = "") { }
		        public static void That(ValueTask subject, string doNotPopulateThisValue = "") { }
		        public static void That<TValue>(Func<TValue> @delegate, string doNotPopulateThisValue = "") { }
		        public static void That<TValue>(Func<CancellationToken, TValue> @delegate, string doNotPopulateThisValue = "") { }
		        public static void That<TValue>(Func<Task<TValue>> @delegate, string doNotPopulateThisValue = "") { }
		        public static void That<TValue>(Func<CancellationToken, Task<TValue>> @delegate, string doNotPopulateThisValue = "") { }
		        public static void That(bool subject, string doNotPopulateThisValue = "") { }
		    }
		}
		""";
}
