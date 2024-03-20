using System;
using System.Reflection;

namespace BLMMX.Helpers
{
    public static class ReflectionExtensions
    {
        public static T GetFieldValue<T>(this object obj, string name) {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }
        
        public static MethodInfo GetMethodInfo(this object obj, string methodName)
        {
            return obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static MethodInfo GetStaticMethodInfo(this Type type, string methodName)
        {
            return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        }
    }
}