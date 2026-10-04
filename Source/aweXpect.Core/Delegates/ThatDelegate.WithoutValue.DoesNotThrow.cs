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
		///     Verifies that the delegate does not throw any exception.
		/// </summary>
		[GuaranteesNotNull]
		public ExpectationResult DoesNotThrow()
			=> new(_expectationBuilder.AddConstraint((it, grammars)
				=> new DoesNotThrowConstraint(it, grammars, typeof(Exception), false, null)));

		/// <summary>
		///     Verifies that the delegate does not throw an exception of type <typeparamref name="TException" />.
		/// </summary>
		/// <remarks>
		///     Only an exception of type <typeparamref name="TException" /> or of a derived type fails the expectation,
		///     while any other exception is ignored. Use <see cref="DoesNotThrowExactly{TException}()" /> to ignore
		///     derived types as well.
		/// </remarks>
		[GuaranteesNotNull]
		public ExpectationResult DoesNotThrow<TException>()
			where TException : Exception
			=> new(_expectationBuilder.AddConstraint((it, grammars) =>
				new DoesNotThrowConstraint(it, grammars, typeof(TException), false, null)));

		/// <summary>
		///     Verifies that the delegate does not throw an exception of type <paramref name="type" />.
		/// </summary>
		/// <remarks>
		///     Only an exception of the <paramref name="type" /> or of a derived type fails the expectation, while any
		///     other exception is ignored. Use <see cref="DoesNotThrowExactly(Type)" /> to ignore derived types as well.
		/// </remarks>
		[GuaranteesNotNull]
		public ExpectationResult DoesNotThrow(Type type)
		{
			type.ThrowIfNotAnExceptionType();
			return new(_expectationBuilder.AddConstraint(type, static (exceptionType, it, grammars) =>
				new DoesNotThrowConstraint(it, grammars, exceptionType, false, null)));
		}
	}
}
