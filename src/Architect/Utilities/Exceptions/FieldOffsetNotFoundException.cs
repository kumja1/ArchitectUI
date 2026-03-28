namespace Architect.Utilities.Exceptions;

public class FieldOffsetNotFoundException(string propertyName, string typeName)
    : Exception($"Field offset of property '{propertyName}' could not be found in type '{typeName}'.");