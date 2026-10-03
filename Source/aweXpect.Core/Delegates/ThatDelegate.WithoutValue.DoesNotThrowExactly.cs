using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Results;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	public sealed partial class WithoutValue
	{
		/// <summary>
		///     Verifies that the delegate does not throw an exception of exactly type <typeparamref name="TException" />
		///     (subtypes are allowed).
		/// </summary>
		[GuaranteesNotNull]
		public ExpectationResult DoesNotThrowExactly<TException>()
			where TException : Exception
			=> new(ExpectationBuilder.AddConstraint((it, grammars) =>
				new DoesNotThrowConstraint(it, grammars, typeof(TException), true, null)));

		/// <summary>
		///     Verifies that the delegate does not throw an exception of exactly type <paramref name="type" />
		///     (subtypes are allowed).
		/// </summary>
		[GuaranteesNotNull]
		public ExpectationResult DoesNotThrowExactly(Type type)
		{
			type.ThrowIfNotAnExceptionType();
			return new(ExpectationBuilder.AddConstraint(type, static (exceptionType, it, grammars) =>
				new DoesNotThrowConstraint(it, grammars, exceptionType, true, null)));
		}
	}
}
