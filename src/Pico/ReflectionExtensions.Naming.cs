using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Pico;

partial class ReflectionExtensions
{
    #region [ Known Names and Constants ]
    
    private static readonly HashSet<Type> KnownGenericTypes =
        new()
        {
            typeof(IEnumerable<>),
            typeof(IEnumerator<>),
            typeof(ICollection<>),
            typeof(Collection<>),
            typeof(IList<>),
            typeof(List<>),
            typeof(IDictionary<,>),
            typeof(Dictionary<,>),
            typeof(IReadOnlyList<>),
            typeof(IReadOnlyCollection<>),
            typeof(IReadOnlyDictionary<,>),
            typeof(IReadOnlySet<>),
            typeof(ReadOnlyCollection<>),
            typeof(ReadOnlyDictionary<,>),
            typeof(IReadOnlyList<>),
            typeof(HashSet<>),
            typeof(Predicate<>),
            typeof(Action<>),
            typeof(Action<,>),
            typeof(Action<,,>),
            typeof(Action<,,,>),
            typeof(Action<,,,,>),
            typeof(Action<,,,,,>),
            typeof(Action<,,,,,,>),
            typeof(Action<,,,,,,,>),
            typeof(Action<,,,,,,,,>),
            typeof(Action<,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,,,,>),
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>),
            typeof(Func<,,,,>),
            typeof(Func<,,,,,>),
            typeof(Func<,,,,,,>),
            typeof(Func<,,,,,,,>),
            typeof(Func<,,,,,,,,>),
            typeof(Func<,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,,,>),
        };

    private static readonly Dictionary<Type, string> KnownTypeNames =
        new()
        {
            [typeof(string)] = "string",
            [typeof(char)] = "char",
            [typeof(byte)] = "byte",
            [typeof(sbyte)] = "sbyte",
            [typeof(short)] = "short",
            [typeof(ushort)] = "ushort",
            [typeof(int)] = "int",
            [typeof(uint)] = "uint",
            [typeof(long)] = "long",
            [typeof(ulong)] = "ulong",
            [typeof(float)] = "float",
            [typeof(double)] = "double",
            [typeof(decimal)] = "decimal",
            [typeof(bool)] = "bool",
            [typeof(DateTime)] = "DateTime",
            [typeof(DateTimeOffset)] = "DateTimeOffset",
            [typeof(DateOnly)] = "DateOnly",
            [typeof(TimeOnly)] = "TimeOnly",
            [typeof(TimeSpan)] = "TimeSpan",
            [typeof(Type)] = "Type",
            [typeof(Regex)] = "Regex",
        };

    #endregion

    public static string ToDebugString(this Type type, bool fullyQualified = false)
    {
        if (KnownTypeNames.TryGetValue(type, out var typeName))
        {
            return typeName;
        }

        if (type.IsArray)
        {
            var subType = type.GetElementType()!.ToDebugString(fullyQualified);
            var ranks = new string(',', type.GetArrayRank() - 1);
            return $"{subType}[{ranks}]";
        }

        if (type.IsPointer)
        {
            var subType = type.GetElementType()!.ToDebugString(fullyQualified);
            return $"{subType}*";
        }

        typeName = type.Name;

        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (fullyQualified && !KnownGenericTypes.Contains(genericType))
            {
                typeName = type.FullName ?? typeName;
            }

            var sb = new StringBuilder();

            sb.Append(typeName.BeforeOrAll("`")).Append('<');
            var args = type.GetGenericArguments();
            for (var i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(ToDebugString(args[i], fullyQualified));
            }
            sb.Append('>');

            return sb.ToString();
        }

        if (fullyQualified)
        {
            typeName = type.FullName ?? typeName;
        }

        return typeName;
    }
}
