using System.Reflection;

namespace Pico;

partial class ReflectionExtensions
{
    #region [ CreateVoidDispatcher ]

    public static IMethodDispatcher<Type> CreateVoidDispatcher(
        this Type targetType,
        Predicate<MethodInfo> isValidMethod,
        Func<MethodInfo, Type> getIdentifierType)
    {
        var builder = MethodDispatcher.Builder<Type>();

        foreach (var method in targetType.GetMethods())
        {
            if (!isValidMethod(method)) continue;
            var identifierType = getIdentifierType(method);
            var parameters = method.GetParameters();
            builder.Add(identifierType, (target, resolveParameter) =>
            {
                var arguments = parameters
                    .Select(info => resolveParameter(info))
                    .ToArray();

                method.Invoke(target, arguments);
            });
        }

        return builder.Build();
    }

    public static IMethodDispatcher<Type> CreateVoidDispatcher(
        this Type targetType,
        Predicate<MethodInfo> isValidMethod) =>
        CreateVoidDispatcher(
            targetType,
            HaveAtLeastOneParameter(isValidMethod),
            GetFirstParameterType);

    public static IMethodDispatcher<Type> CreateVoidDispatcher(
        this Type targetType,
        string methodName) =>
        CreateVoidDispatcher(
            targetType,
            IsMethodNamed(methodName));

    #endregion

    #region [ CreateDispatcher ]

    public static IMethodDispatcher<Type, TResult> CreateDispatcher<TResult>(
        this Type targetType,
        Predicate<MethodInfo> isValidMethod,
        Func<MethodInfo, Type> getIdentifierType,
        Converter<object?, TResult> convertResult)
    {
        var builder = MethodDispatcher.Builder<Type, TResult>();

        foreach (var method in targetType.GetMethods())
        {
            if (!isValidMethod(method)) continue;
            var identifierType = getIdentifierType(method);
            var parameters = method.GetParameters();
            builder.Add(identifierType, (target, resolveParameter) =>
            {
                var arguments = parameters
                    .Select(info => resolveParameter(info))
                    .ToArray();

                return convertResult(method.Invoke(target, arguments));
            });
        }

        return builder.Build();
    }

    public static IMethodDispatcher<Type, TResult> CreateDispatcher<TResult>(
        this Type targetType,
        Predicate<MethodInfo> isValidMethod,
        Converter<object?, TResult> convertResult) =>
        targetType.CreateDispatcher(
            HaveAtLeastOneParameter(isValidMethod),
            GetFirstParameterType,
            convertResult);

    public static IMethodDispatcher<Type, TResult> CreateDispatcher<TResult>(
        this Type targetType,
        Predicate<MethodInfo> isValidMethod,
        Func<MethodInfo, Type> getIdentifierType) =>
        targetType.CreateDispatcher(
            isValidMethod,
            getIdentifierType,
            ConvertResultOrThrow<TResult>);

    public static IMethodDispatcher<Type, TResult> CreateDispatcher<TResult>(
        this Type targetType,
        Predicate<MethodInfo> isValidMethod) =>
        targetType.CreateDispatcher(
            isValidMethod,
            ConvertResultOrThrow<TResult>);

    public static IMethodDispatcher<Type, TResult> CreateDispatcher<TResult>(
        this Type targetType,
        string methodName,
        Converter<object?, TResult> convertResult) =>
        targetType.CreateDispatcher(
            IsMethodNamed(methodName),
            GetFirstParameterType,
            convertResult);

    public static IMethodDispatcher<Type, TResult> CreateDispatcher<TResult>(
        this Type targetType,
        string methodName) =>
        targetType.CreateDispatcher(
            IsMethodNamed(methodName),
            ConvertResultOrThrow<TResult>);

    #endregion

    #region [ CreateTaskDispatcher ]

    public static IMethodDispatcher<Type, Task> CreateTaskDispatcher(
        this Type targetType,
        Predicate<MethodInfo> isValidMethod,
        Func<MethodInfo, Type> getIdentifierType) =>
        targetType.CreateDispatcher(
            isValidMethod,
            getIdentifierType,
            result => result as Task ?? Task.CompletedTask);

    public static IMethodDispatcher<Type, Task> CreateTaskDispatcher(
        this Type targetType,
        Predicate<MethodInfo> isValidMethod) =>
        targetType.CreateTaskDispatcher(
            HaveAtLeastOneParameter(isValidMethod),
            GetFirstParameterType);

    public static IMethodDispatcher<Type, Task> CreateTaskDispatcher(
        this Type targetType,
        string methodName) =>
        targetType.CreateTaskDispatcher(
            IsMethodNamed(methodName));

    #endregion

    private static Predicate<MethodInfo> HaveAtLeastOneParameter(
        Predicate<MethodInfo> isValidMethod) =>
        info => isValidMethod(info) &&
                info.GetParameters().Length > 0;

    private static Predicate<MethodInfo> IsMethodNamed(
        string methodName) =>
        info => info.Name == methodName;

    private static Type GetFirstParameterType(MethodInfo method) =>
        method.GetParameters()[0].ParameterType;

    private static TResult ConvertResultOrThrow<TResult>(object? result) =>
        result is TResult r
            ? r
            : throw new InvalidCastException(
                $"Expected {typeof(TResult).Name} but got {result?.GetType()?.Name ?? "null"}");
}

public delegate object? ResolveParameter(ParameterInfo parameter);

public delegate void DispatchMethod(object target, ResolveParameter resolveParameter);
public delegate void DispatchMethodFallback<in TMethod>(object target, TMethod method, ResolveParameter resolveParameter) where TMethod : notnull;

public delegate TResult DispatchMethod<out TResult>(object target, ResolveParameter resolveParameter);
public delegate TResult DispatchMethodFallback<in TMethod, out TResult>(object target, TMethod method, ResolveParameter resolveParameter) where TMethod : notnull;

public interface IMethodDispatcher<TMethod>
    where TMethod : notnull
{
    IEnumerable<TMethod> Methods { get; }

    bool CanDispatch(TMethod method);

    void Dispatch(
        object target,
        TMethod method,
        ResolveParameter resolveParameter);
}

public interface IMethodDispatcher<TMethod, out TResult>
    where TMethod : notnull
{
    IEnumerable<TMethod> Methods { get; }

    bool CanDispatch(TMethod method);

    TResult Dispatch(
        object target,
        TMethod method,
        ResolveParameter resolveParameter);
}

public static class MethodDispatcher
{
    public static MethodDispatcherBuilder<TMethod> Builder<TMethod>(
        IEqualityComparer<TMethod>? comparer = null)
        where TMethod : notnull
        => new(comparer);

    public static MethodDispatcherBuilder<TMethod, TResult> Builder<TMethod, TResult>(
        IEqualityComparer<TMethod>? comparer = null)
        where TMethod : notnull
        => new(comparer);

    public class MethodDispatcherBuilder<TMethod>
        where TMethod : notnull
    {
        private readonly IEqualityComparer<TMethod>? comparer;
        private readonly Dictionary<TMethod, DispatchMethod> dispatchers;
        private DispatchMethodFallback<TMethod>? fallback;

        internal MethodDispatcherBuilder(
            IEqualityComparer<TMethod>? comparer = null)
        {
            this.comparer = comparer;
            dispatchers = new Dictionary<TMethod, DispatchMethod>(comparer);
        }

        public MethodDispatcherBuilder<TMethod> Add(
            TMethod method,
            DispatchMethod dispatch)
        {
            dispatchers.Add(method, dispatch);
            return this;
        }

        public IMethodDispatcher<TMethod> Build()
        {
            var methods = new Dictionary<TMethod, DispatchMethod>(dispatchers, comparer);

            var methodFallback = fallback ?? ((_, method, _) => throw new KeyNotFoundException($"No dispatcher found for method {method}"));

            dispatchers.Clear();
            fallback = null;

            return new MethodDispatcherImpl<TMethod>(methods, methodFallback);
        }
    }

    public class MethodDispatcherBuilder<TMethod, TResult>
        where TMethod : notnull
    {
        private readonly IEqualityComparer<TMethod>? comparer;
        private readonly Dictionary<TMethod, DispatchMethod<TResult>> dispatchers;
        private DispatchMethodFallback<TMethod, TResult>? fallback;

        internal MethodDispatcherBuilder(
            IEqualityComparer<TMethod>? comparer = null)
        {
            this.comparer = comparer;
            dispatchers = new Dictionary<TMethod, DispatchMethod<TResult>>(comparer);
        }

        public MethodDispatcherBuilder<TMethod, TResult> Add(
            TMethod method,
            DispatchMethod<TResult> dispatch)
        {
            dispatchers.Add(method, dispatch);
            return this;
        }

        public IMethodDispatcher<TMethod, TResult> Build()
        {
            var methods = new Dictionary<TMethod, DispatchMethod<TResult>>(dispatchers, comparer);

            var methodFallback = fallback ?? ((_, method, _) => throw new KeyNotFoundException($"No dispatcher found for method {method}"));

            dispatchers.Clear();
            fallback = null;

            return new MethodDispatcherImpl<TMethod, TResult>(methods, methodFallback);
        }
    }

    private class MethodDispatcherImpl<TMethod> :
        IMethodDispatcher<TMethod>
        where TMethod : notnull
    {
        private readonly IReadOnlyDictionary<TMethod, DispatchMethod> dispatchers;
        private readonly DispatchMethodFallback<TMethod> fallback;

        public MethodDispatcherImpl(
            IReadOnlyDictionary<TMethod, DispatchMethod> dispatchers,
            DispatchMethodFallback<TMethod> fallback)
        {
            this.dispatchers = dispatchers;
            this.fallback = fallback;
        }

        public IEnumerable<TMethod> Methods => dispatchers.Keys;

        public bool CanDispatch(TMethod method) => dispatchers.ContainsKey(method);

        public void Dispatch(
            object target,
            TMethod method,
            ResolveParameter resolveParameter)
        {
            if (!dispatchers.TryGetValue(method, out var dispatcher))
            {
                fallback(target, method, resolveParameter);
            }
            else
            {
                dispatcher(target, resolveParameter);
            }
        }
    }

    private class MethodDispatcherImpl<TMethod, TResult> :
        IMethodDispatcher<TMethod, TResult>
        where TMethod : notnull
    {
        private readonly IReadOnlyDictionary<TMethod, DispatchMethod<TResult>> dispatchers;
        private readonly DispatchMethodFallback<TMethod, TResult> fallback;

        public MethodDispatcherImpl(
            IReadOnlyDictionary<TMethod, DispatchMethod<TResult>> dispatchers,
            DispatchMethodFallback<TMethod, TResult> fallback)
        {
            this.dispatchers = dispatchers;
            this.fallback = fallback;
        }

        public IEnumerable<TMethod> Methods => dispatchers.Keys;
        public bool CanDispatch(TMethod method) => dispatchers.ContainsKey(method);

        public TResult Dispatch(
            object target,
            TMethod method,
            ResolveParameter resolveParameter)
        {
            if (!dispatchers.TryGetValue(method, out var dispatcher))
            {
                return fallback(target, method, resolveParameter);
            }

            return dispatcher(target, resolveParameter);
        }
    }
}

public static class ParameterResolver
{
    public static ParameterResolverBuilder Builder() => new();

    public class ParameterResolverBuilder
    {
        private List<Func<ParameterInfo, (bool, object?)>> customs = new();
        private ResolveParameter? fallback;
        private Func<ParameterInfo, string>? notFoundMessage;

        internal ParameterResolverBuilder()
        {

        }

        #region [ ForInstance ]

        public ParameterResolverBuilder ForInstance(Type type, object instance) =>
            Custom(info => info.ParameterType == type
                ? (true, instance)
                : (false, null));

        public ParameterResolverBuilder ForInstance<T>(T instance) =>
            ForInstance(typeof(T), instance);

        #endregion

        #region [ ForInstances ]

        public ParameterResolverBuilder ForInstances(
            params object[] instances) =>
            ForInstances((IEnumerable<object>)instances);

        public ParameterResolverBuilder ForInstances(
            IEnumerable<object> instances) =>
            ForInstances(instances.Select(instance =>
                (instance.GetType(), instance)));

        public ParameterResolverBuilder ForInstances(
            IEnumerable<(Type key, object instance)> instances) =>
            ForInstances(instances.ToDictionary(e => e.key, e => e.instance));

        public ParameterResolverBuilder ForInstances(
            IDictionary<Type, object> instances) =>
            ForInstances((IReadOnlyDictionary<Type, object>)instances);

        public ParameterResolverBuilder ForInstances(
            Dictionary<Type, object> instances) =>
            ForInstances((IReadOnlyDictionary<Type, object>)instances);

        public ParameterResolverBuilder ForInstances(
            IReadOnlyDictionary<Type, object> instances) =>
            Custom(info =>
            {
                return instances.TryGetValue(info.ParameterType, out var instance)
                    ? (true, instance)
                    : (false, null);
            });

        #endregion

        #region [ ForFactory ]

        public ParameterResolverBuilder ForFactory(Type type, Func<object> factory) =>
            Custom(info => info.ParameterType == type ? (true, factory()) : (false, null));

        public ParameterResolverBuilder ForFactory<T>(Func<T> factory) =>
            ForFactory(typeof(T), () => factory()!);

        #endregion

        #region [ WithNotFoundMessage ]

        public ParameterResolverBuilder WithNotFoundMessage(Func<ParameterInfo, string> notFoundMessage)
        {
            if (notFoundMessage is null) throw new ArgumentNullException(nameof(notFoundMessage));

            if (this.notFoundMessage is not null)
            {
                throw new InvalidOperationException("Not-Found message is already set");
            }

            this.notFoundMessage = notFoundMessage;

            return this;
        }

        public ParameterResolverBuilder WithNotFoundMessage(string notFoundMessage)
        {
            if (notFoundMessage == null) throw new ArgumentNullException(nameof(notFoundMessage));

            return WithNotFoundMessage(_ => notFoundMessage);
        }

        #endregion

        public ParameterResolverBuilder Custom(Func<ParameterInfo, (bool, object?)> custom)
        {
            if (custom == null) throw new ArgumentNullException(nameof(custom));

            if (this.fallback is not null)
            {
                throw new InvalidOperationException("Fallback is already set");
            }

            this.customs.Add(custom);

            return this;
        }

        public ParameterResolverBuilder WithFallback(ResolveParameter fallback)
        {
            if (fallback is null) throw new ArgumentNullException(nameof(fallback));

            if (this.fallback is not null)
            {
                throw new InvalidOperationException("Fallback is already set");
            }

            this.fallback = fallback;

            return this;
        }

        public ResolveParameter Build()
        {
            var notFoundMessage = this.notFoundMessage ?? (info =>
                $"No resolver found for parameter {info.Name} of type {info.ParameterType}");
            var fallback = this.fallback ?? (info =>
                throw new ArgumentException(notFoundMessage(info)));
            var customs = this.customs.ToArray();

            this.notFoundMessage = null;
            this.fallback = null;
            this.customs.Clear();

            return info =>
            {
                foreach (var custom in customs)
                {
                    var (success, result) = custom(info);
                    if (success) return result;
                }
                return fallback(info);
            };
        }
    }
}
