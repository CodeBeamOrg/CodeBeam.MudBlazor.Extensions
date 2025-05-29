using System.Reflection;
using System.Xml.Linq;

namespace MudExtensions.Docs.Services
{
    public class SimpleXmlDocReader
    {
        private readonly Dictionary<string, string> _summaries;

        // XML içeriği string olarak alır, parse edip member açıklamalarını hazırlar
        public SimpleXmlDocReader(string xmlContent)
        {
            var doc = XDocument.Parse(xmlContent);

            _summaries = doc.Descendants("member")
                .Where(m => m.Attribute("name") != null)
                .ToDictionary(
                    m => m.Attribute("name")!.Value,
                    m => CleanSummaryText(m.Element("summary")?.Value)
                );
        }

        // Summary'den gereksiz boşlukları ve satır sonlarını temizler
        private string CleanSummaryText(string? summary)
        {
            if (string.IsNullOrWhiteSpace(summary))
                return "";

            // Satır başı ve sonu boşlukları temizle, iç satır boşluklarını tek boşluk yap
            return string.Join(" ", summary.Trim().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
        }

        // Reflection MemberInfo'dan XML dosyasındaki member name formatını oluşturur
        //public static string GetMemberName(MemberInfo member)
        //{
        //    if (member == null) throw new ArgumentNullException(nameof(member));

        //    string prefix = member.MemberType switch
        //    {
        //        MemberTypes.Constructor => "M",
        //        MemberTypes.Method => "M",
        //        MemberTypes.Property => "P",
        //        MemberTypes.Event => "E",
        //        MemberTypes.Field => "F",
        //        _ => throw new ArgumentOutOfRangeException(nameof(member), $"Unsupported member type: {member.MemberType}")
        //    };

        //    var typeName = member.DeclaringType?.FullName ?? "";

        //    if (member is MethodInfo method)
        //    {
        //        var parameters = method.GetParameters();
        //        if (parameters.Length == 0)
        //            return $"{prefix}:{typeName}.{member.Name}";

        //        // Parametre tiplerini yaz (tam tip isimleri ile)
        //        var paramTypeNames = parameters.Select(p => GetParameterTypeName(p.ParameterType));
        //        return $"{prefix}:{typeName}.{member.Name}({string.Join(",", paramTypeNames)})";
        //    }
        //    else
        //    {
        //        return $"{prefix}:{typeName}.{member.Name}";
        //    }
        //}

        // Parametre tipi için XML formatında isim oluşturur
        private static string GetParameterTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var mainType = type.GetGenericTypeDefinition().FullName;
                var genericArgs = type.GetGenericArguments().Select(GetParameterTypeName);
                // Generic format: Namespace.Type`1[System.String]
                // XML doc için: Namespace.Type{System.String}
                mainType = mainType?.Split('`')[0] ?? "";
                return $"{mainType}{{{string.Join(",", genericArgs)}}}";
            }

            // Array tipi için []
            if (type.IsArray)
            {
                return $"{GetParameterTypeName(type.GetElementType()!)}[]";
            }

            return type.FullName ?? type.Name;
        }

        // MemberInfo üzerinden özet (summary) alır
        public string? GetSummary(MemberInfo member)
        {
            var memberName = GetMemberName(member);
            return _summaries.TryGetValue(memberName, out var summary) ? summary : null;
        }

        // Direkt memberName ile de özet çekilebilir
        public string? GetSummary(string memberName)
        {
            return _summaries.TryGetValue(memberName, out var var) ? var : null;
        }

        public static string GetMemberName(MemberInfo member)
        {
            return member.MemberType switch
            {
                MemberTypes.TypeInfo or MemberTypes.NestedType => $"T:{FormatTypeName((Type)member)}",
                MemberTypes.Property => $"P:{FormatTypeName(member.DeclaringType!)}.{member.Name}",
                MemberTypes.Method => $"M:{FormatTypeName(member.DeclaringType!)}.{member.Name}{FormatMethodParameters((MethodInfo)member)}",
                _ => member.Name
            };
        }

        private static string FormatTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var baseName = type.FullName?.Split('`')[0]; // Remove `1, `2 etc.
                var argCount = type.GetGenericArguments().Length;
                return $"{baseName}`{argCount}";
            }

            return type.FullName!;
        }

        private static string FormatMethodParameters(MethodInfo method)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0) return "";

            var formatted = string.Join(",", parameters.Select(p => FormatParameterType(p.ParameterType)));
            return $"({formatted})";
        }

        private static string FormatParameterType(Type type)
        {
            if (type.IsGenericType)
            {
                var genericTypeName = type.GetGenericTypeDefinition().FullName!.Split('`')[0];
                var args = string.Join(",", type.GetGenericArguments().Select(FormatParameterType));
                return $"{genericTypeName}{{{args}}}";
            }

            if (type.IsArray)
                return $"{FormatParameterType(type.GetElementType()!)}[]";

            return type.FullName!;
        }

    }
}
