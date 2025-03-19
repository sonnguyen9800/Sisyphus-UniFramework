using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SisyphusFramework.Utils
{
    public static class Utils
    {
                #region Reflection

        public static object GetFieldValue(this object o, string fieldName, bool exceptionIfNotFound = true)
        {
            var type = (o as Type) ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(fieldName) is FieldInfo fieldInfo)
                return fieldInfo.GetValue(target);


            if (exceptionIfNotFound)
                throw new System.Exception($"Field '{fieldName}' not found in type '{type.Name}' and its parent types");

            return null;

        }

        public static object GetPropertyValue(this object o, string propertyName, bool exceptionIfNotFound = true)
        {
            var type = (o as Type) ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetPropertyInfo(propertyName) is PropertyInfo propertyInfo)
                return propertyInfo.GetValue(target);


            if (exceptionIfNotFound)
                throw new System.Exception(
                    $"Property '{propertyName}' not found in type '{type.Name}' and its parent types");

            return null;

        }

        public static object GetMemberValue(this object o, string memberName, bool exceptionIfNotFound = true)
        {
            var type = (o as Type) ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(memberName) is FieldInfo fieldInfo)
                return fieldInfo.GetValue(target);

            if (type.GetPropertyInfo(memberName) is PropertyInfo propertyInfo)
                return propertyInfo.GetValue(target);


            if (exceptionIfNotFound)
                throw new System.Exception(
                    $"Member '{memberName}' not found in type '{type.Name}' and its parent types");

            return null;

        }

        public static void SetFieldValue(this object o, string fieldName, object value, bool exceptionIfNotFound = true)
        {
            var type = (o as Type) ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(fieldName) is FieldInfo fieldInfo)
                fieldInfo.SetValue(target, value);


            else if (exceptionIfNotFound)
                throw new System.Exception($"Field '{fieldName}' not found in type '{type.Name}' and its parent types");

        }

        public static void SetPropertyValue(this object o, string propertyName, object value,
            bool exceptionIfNotFound = true)
        {
            var type = (o as Type) ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetPropertyInfo(propertyName) is PropertyInfo propertyInfo)
                propertyInfo.SetValue(target, value);


            else if (exceptionIfNotFound)
                throw new System.Exception(
                    $"Property '{propertyName}' not found in type '{type.Name}' and its parent types");

        }

        public static void SetMemberValue(this object o, string memberName, object value,
            bool exceptionIfNotFound = true)
        {
            var type = (o as Type) ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(memberName) is FieldInfo fieldInfo)
                fieldInfo.SetValue(target, value);

            else if (type.GetPropertyInfo(memberName) is PropertyInfo propertyInfo)
                propertyInfo.SetValue(target, value);


            else if (exceptionIfNotFound)
                throw new System.Exception(
                    $"Member '{memberName}' not found in type '{type.Name}' and its parent types");

        }

        public static object
            InvokeMethod(this object o, string methodName,
                params object[] parameters) // todo handle null params (can't get their type)
        {
            var type = (o as Type) ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetMethodInfo(methodName, parameters.Select(r => r.GetType()).ToArray()) is MethodInfo methodInfo)
                return methodInfo.Invoke(target, parameters);


            throw new System.Exception(
                $"Method '{methodName}' not found in type '{type.Name}', its parent types and interfaces");

        }



        static FieldInfo GetFieldInfo(this Type type, string fieldName)
        {
            if (fieldInfoCache.TryGetValue(type, out var fieldInfosByNames))
                if (fieldInfosByNames.TryGetValue(fieldName, out var fieldInfo))
                    return fieldInfo;


            if (!fieldInfoCache.ContainsKey(type))
                fieldInfoCache[type] = new Dictionary<string, FieldInfo>();

            for (var curType = type; curType != null; curType = curType.BaseType)
                if (curType.GetField(fieldName, maxBindingFlags) is FieldInfo fieldInfo)
                    return fieldInfoCache[type][fieldName] = fieldInfo;


            return fieldInfoCache[type][fieldName] = null;

        }

        static Dictionary<Type, Dictionary<string, FieldInfo>> fieldInfoCache =
            new Dictionary<Type, Dictionary<string, FieldInfo>>();

        static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
            if (propertyInfoCache.TryGetValue(type, out var propertyInfosByNames))
                if (propertyInfosByNames.TryGetValue(propertyName, out var propertyInfo))
                    return propertyInfo;


            if (!propertyInfoCache.ContainsKey(type))
                propertyInfoCache[type] = new Dictionary<string, PropertyInfo>();

            for (var curType = type; curType != null; curType = curType.BaseType)
                if (curType.GetProperty(propertyName, maxBindingFlags) is PropertyInfo propertyInfo)
                    return propertyInfoCache[type][propertyName] = propertyInfo;


            return propertyInfoCache[type][propertyName] = null;

        }

        static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyInfoCache =
            new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        static MethodInfo GetMethodInfo(this Type type, string methodName, params Type[] argumentTypes)
        {
            var methodHash = methodName.GetHashCode() ^
                             argumentTypes.Aggregate(0, (hash, r) => hash ^= r.GetHashCode());


            if (methodInfoCache.TryGetValue(type, out var methodInfosByHashes))
                if (methodInfosByHashes.TryGetValue(methodHash, out var methodInfo))
                    return methodInfo;



            if (!methodInfoCache.ContainsKey(type))
                methodInfoCache[type] = new Dictionary<int, MethodInfo>();

            for (var curType = type; curType != null; curType = curType.BaseType)
                if (curType.GetMethod(methodName, maxBindingFlags, null, argumentTypes, null) is MethodInfo methodInfo)
                    return methodInfoCache[type][methodHash] = methodInfo;

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType.GetMethod(methodName, maxBindingFlags, null, argumentTypes, null) is MethodInfo
                    methodInfo)
                    return methodInfoCache[type][methodHash] = methodInfo;



            return methodInfoCache[type][methodHash] = null;

        }

        static Dictionary<Type, Dictionary<int, MethodInfo>> methodInfoCache =
            new Dictionary<Type, Dictionary<int, MethodInfo>>();



        public static T GetFieldValue<T>(this object o, string fieldName, bool exceptionIfNotFound = true) =>
            (T)o.GetFieldValue(fieldName, exceptionIfNotFound);

        public static T GetPropertyValue<T>(this object o, string propertyName, bool exceptionIfNotFound = true) =>
            (T)o.GetPropertyValue(propertyName, exceptionIfNotFound);

        public static T GetMemberValue<T>(this object o, string memberName, bool exceptionIfNotFound = true) =>
            (T)o.GetMemberValue(memberName, exceptionIfNotFound);

        public static T InvokeMethod<T>(this object o, string methodName, params object[] parameters) =>
            (T)o.InvokeMethod(methodName, parameters);






        public static List<Type> GetSubclasses(this Type t) =>
            t.Assembly.GetTypes().Where(type => type.IsSubclassOf(t)).ToList();

        public static object GetDefaultValue(this FieldInfo f, params object[] constructorVars) =>
            f.GetValue(System.Activator.CreateInstance(((MemberInfo)f).ReflectedType, constructorVars));

        public static object GetDefaultValue(this FieldInfo f) =>
            f.GetValue(System.Activator.CreateInstance(((MemberInfo)f).ReflectedType));


        public static IEnumerable<FieldInfo> GetFieldsWithoutBase(this Type t) =>
            t.GetFields().Where(r => !t.BaseType.GetFields().Any(rr => rr.Name == r.Name));

        public static IEnumerable<PropertyInfo> GetPropertiesWithoutBase(this Type t) => t.GetProperties()
            .Where(r => !t.BaseType.GetProperties().Any(rr => rr.Name == r.Name));


        public const BindingFlags maxBindingFlags = (BindingFlags)62;







        #endregion
    }
}