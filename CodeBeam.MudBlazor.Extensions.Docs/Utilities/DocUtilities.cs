
namespace MudExtensions.Docs
{
    public static class DocUtilities
    {
        public static string GetFriendlyTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var genericTypeName = type.GetGenericTypeDefinition().Name;
                var backtickIndex = genericTypeName.IndexOf('`');
                if (backtickIndex > 0)
                    genericTypeName = genericTypeName.Substring(0, backtickIndex);

                var genericArgs = type.GetGenericArguments()
                                      .Select(GetFriendlyTypeName)
                                      .ToArray();

                return $"{genericTypeName}<{string.Join(", ", genericArgs)}>";
            }

            if (type.IsArray)
            {
                return $"{GetFriendlyTypeName(type.GetElementType()!)}[]";
            }

            return type.Name switch
            {
                "String" => "string",
                "Int32" => "int",
                "Boolean" => "bool",
                "Object" => "object",
                "Void" => "void",
                "Decimal" => "decimal",
                "Double" => "double",
                "Single" => "float",
                "Int64" => "long",
                "Int16" => "short",
                "Byte" => "byte",
                _ => type.Name
            };
        }

    }
}
