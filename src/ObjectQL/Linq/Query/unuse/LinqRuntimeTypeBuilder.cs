/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：LinqRuntimeTypeBuilder
 * 命名空间：ObjectQL.Data
 * 文 件 名：LinqRuntimeTypeBuilder
 * 创建时间：2017/5/6 9:32:48
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Jinhe;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class LinqRuntimeTypeBuilder
    {
        private static readonly ILogger log = Jinhe.LogAdapter.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static AssemblyName assemblyName = new AssemblyName() { Name = "DynamicLinqTypes" };
        private static ModuleBuilder moduleBuilder = null;
        private static Dictionary<string, Type> builtTypes = new Dictionary<string, Type>();

        static LinqRuntimeTypeBuilder()
        {
            moduleBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(assemblyName.Name);
        }

        private static string GetTypeKey(Dictionary<string, Type> fields)
        { 
            string key = string.Empty;
            foreach (var field in fields)
                key += field.Key + ";" + field.Value.Name + ";";

            return key;
        }

        public static Type GetDynamicType(Dictionary<string, Type> fields)
        {
            if (null == fields)
                throw new ArgumentNullException("fields");
            if (0 == fields.Count)
                throw new ArgumentOutOfRangeException("fields", "fields must have at least 1 field definition");

            try
            {
                Monitor.Enter(builtTypes);
                string className = GetTypeKey(fields);

                if (builtTypes.ContainsKey(className))
                    return builtTypes[className];

                TypeBuilder typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                foreach (var field in fields)
                    typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);

                builtTypes[className] = typeBuilder.CreateType();

                return builtTypes[className];
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                Monitor.Exit(builtTypes);
            }

            return null;
        }


        private static string GetTypeKey(IEnumerable<PropertyInfo> fields)
        {
            return GetTypeKey(fields.ToDictionary(f => f.Name, f => f.PropertyType));
        }

        public static Type GetDynamicType(IEnumerable<PropertyInfo> fields)
        {
            return GetDynamicType(fields.ToDictionary(f => f.Name, f => f.PropertyType));
        }

        private static AssemblyBuilder asmBuilder = null;
        private static ModuleBuilder modBuilder = null;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ModuleBuilder GenerateAssemblyAndModule()
        {
            if (asmBuilder == null)
            {
                AssemblyName assemblyName = new AssemblyName();
                assemblyName.Name = "ObjectQL.Data.Dynamic";
                AppDomain thisDomain = Thread.GetDomain();
                asmBuilder = thisDomain.DefineDynamicAssembly(assemblyName,
                             AssemblyBuilderAccess.Run);
                modBuilder = asmBuilder.DefineDynamicModule(
                             asmBuilder.GetName().Name, false);
            }
            return modBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modBuilder"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static TypeBuilder CreateType<T>(this ModuleBuilder modBuilder, string typeName)
        {
            TypeBuilder typeBuilder = modBuilder.DefineType(typeName,
                        TypeAttributes.Public |
                        TypeAttributes.Class |
                        TypeAttributes.AutoClass |
                        TypeAttributes.AnsiClass |
                        TypeAttributes.BeforeFieldInit |
                        TypeAttributes.AutoLayout,
                        typeof(object),
                        new Type[] { typeof(T) });
            return typeBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeBuilder"></param>
        public static void CreateConstructor(this TypeBuilder typeBuilder)
        {
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(
                                MethodAttributes.Public |
                                MethodAttributes.SpecialName |
                                MethodAttributes.RTSpecialName,
                                CallingConventions.Standard,
                                new Type[0]); 
            ConstructorInfo conObj = typeof(object).GetConstructor(new Type[0]);            
            ILGenerator il = constructor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, conObj);
            il.Emit(OpCodes.Ret);
        }

        public static MethodInfo GetGenericMethod(this Type targetType, string name, BindingFlags flags,   Type[] genericTypes, params Type[] parameterTypes)
        {
            var methods = targetType.GetMethods(flags).Where(m => m.Name == name && m.IsGenericMethod);
            foreach (MethodInfo method in methods)
            {
                var parameters = method.GetParameters();
                var typeArgs = method.GetGenericArguments();
                if (parameters.Length != parameterTypes.Length)
                    continue;
                if (typeArgs.Length != genericTypes.Length)
                    continue;

                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        break;
                }
                return method;
            }
            return null;
        }
    }
}
