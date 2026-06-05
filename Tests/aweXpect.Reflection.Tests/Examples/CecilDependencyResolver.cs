using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace aweXpect.Reflection.Tests.Examples;

/// <summary>
///     Example of a custom resolver for <c>Customize.aweXpect.Reflection().DependencyResolver()</c>, backed by
///     Mono.Cecil: in addition to the declared signature surface it reads method bodies from the IL, so
///     references that only occur inside a body (object creations, static calls, locals) are detected as
///     dependencies, too, which the built-in signature-level resolver cannot see.
/// </summary>
/// <remarks>
///     The output does not need to be unwrapped, de-duplicated or cached here: the library normalizes and
///     memoizes every resolver's output per type.
/// </remarks>
public static class CecilDependencyResolver
{
	public static IEnumerable<Type> GetUsedTypes(Type type)
	{
		if (type.FullName is null || string.IsNullOrEmpty(type.Assembly.Location))
		{
			yield break;
		}

		using AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(type.Assembly.Location);
		TypeDefinition? definition = assembly.MainModule.GetType(type.FullName.Replace('+', '/'));
		if (definition is null)
		{
			yield break;
		}

		foreach (TypeReference reference in EnumerateTypeReferences(definition))
		{
			foreach (TypeReference flattened in Flatten(reference))
			{
				if (ResolveRuntimeType(flattened) is { } resolved)
				{
					yield return resolved;
				}
			}
		}
	}

	private static IEnumerable<TypeReference> EnumerateTypeReferences(TypeDefinition definition)
	{
		if (definition.BaseType is not null)
		{
			yield return definition.BaseType;
		}

		foreach (InterfaceImplementation @interface in definition.Interfaces)
		{
			yield return @interface.InterfaceType;
		}

		foreach (FieldDefinition field in definition.Fields)
		{
			yield return field.FieldType;
		}

		foreach (PropertyDefinition property in definition.Properties)
		{
			yield return property.PropertyType;
		}

		foreach (EventDefinition @event in definition.Events)
		{
			yield return @event.EventType;
		}

		foreach (MethodDefinition method in definition.Methods)
		{
			yield return method.ReturnType;

			foreach (ParameterDefinition parameter in method.Parameters)
			{
				yield return parameter.ParameterType;
			}

			if (!method.HasBody)
			{
				continue;
			}

			foreach (VariableDefinition variable in method.Body.Variables)
			{
				yield return variable.VariableType;
			}

			foreach (Instruction instruction in method.Body.Instructions)
			{
				TypeReference? operandType = instruction.Operand switch
				{
					TypeReference typeReference => typeReference,
					MemberReference memberReference => memberReference.DeclaringType,
					_ => null,
				};
				if (operandType is not null)
				{
					yield return operandType;
				}
			}
		}
	}

	/// <summary>
	///     Strips array/by-ref/pointer wrappers and flattens generic instances into their definition and
	///     arguments, so that every part can be resolved by full name.
	/// </summary>
	private static IEnumerable<TypeReference> Flatten(TypeReference reference)
	{
		while (reference is TypeSpecification specification and not GenericInstanceType)
		{
			reference = specification.ElementType;
		}

		if (reference is GenericParameter)
		{
			yield break;
		}

		if (reference is GenericInstanceType generic)
		{
			yield return generic.ElementType;
			foreach (TypeReference argument in generic.GenericArguments)
			{
				foreach (TypeReference flattened in Flatten(argument))
				{
					yield return flattened;
				}
			}
		}
		else
		{
			yield return reference;
		}
	}

	private static Type? ResolveRuntimeType(TypeReference reference)
	{
		string assemblyName = reference.Scope switch
		{
			AssemblyNameReference assemblyReference => assemblyReference.FullName,
			ModuleDefinition module => module.Assembly.FullName,
			_ => reference.Module.Assembly.FullName,
		};

		return Type.GetType($"{reference.FullName.Replace('/', '+')}, {assemblyName}", false);
	}
}
