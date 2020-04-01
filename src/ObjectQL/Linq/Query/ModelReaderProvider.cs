/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ModelReaderProvider
 * 命名空间：ObjectQL.Data
 * 文 件 名：ModelReaderProvider
 * 创建时间：2017/3/22 20:42:28
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using Jinhe.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Jinhe;
using ObjectQL.Mapping;
using ObjectQL.Data;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    class ModelReaderProvider : IModelReaderProvider
    {
        readonly Func<string, string> _getSelectNameFun;
        public ModelReaderProvider()
        { }

        public ModelReaderProvider(Func<string, string> getSelectNameFun)
        {
            _getSelectNameFun = getSelectNameFun;
        }

        public T SetObjectProperty<T>(T model, IResultReader dr)
                where T : class, new()
        {
            var ea = OrmContext.OrmProvider.GetEntityInfo<T>();
            if (ea == null) return model;
            foreach (var item in ea.PropertyInfos)
            {
                if (!item.Value.HasColumn || item.Value.IsComplex) continue;
                var value = GetPropertyValue(item.Value, dr, "");
                if (value != null)
                    item.Value.SetValue(model, value, null);
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAnonymous"></typeparam>
        /// <param name="dr"></param>
        /// <param name="getAnonymousType"></param>
        /// <returns></returns>
        public dynamic GetAnonymousTypeObject<TAnonymous>(DataReader dr, Func<string, TAnonymous> getAnonymousType)
        {
            //var fields = dr.Fields;
            var paramesterNames = getAnonymousType.Method.GetParameters().Select(item => item.Name);
            var dict = new Dictionary<string, object>();
            foreach (var name in paramesterNames)
            {
                dict[name] = dr[name];
            }
            dict.ToAnonymousType(getAnonymousType);
            return dict;
        }

        public dynamic GetDynamicObject(DataReader dr)
        {
            var fields = dr.Fields;
            System.Dynamic.ExpandoObject result = new System.Dynamic.ExpandoObject();
            var dict = (IDictionary<string, object>)result;
            foreach (var name in fields)
            {
                if (dict.Keys.Contains(name))
                {
                    dict.Add(name, dr[name]);
                }
            }
            return result;
        }

        /// <summary>
        /// 从DataReader中读取值并设置其Model对应的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dr"></param>
        /// <param name="tableKey"></param> 
        /// <returns></returns>
        protected object SetObjectProperty(Type type, IResultReader dr, string tableKey)
        {
            var entityInfo = OrmContext.OrmProvider.GetEntityInfo(type.FullName);
            if (entityInfo == null) return null;
            object propertyModel = null;
            foreach (var item in entityInfo.PropertyInfos)
            {
                if (!item.Value.HasColumn || item.Value.IsComplex) continue;
                var value = GetPropertyValue(item.Value, dr, tableKey);
                if (value == null) continue;
                if (propertyModel == null)
                {
                    var propertyCreateExp = Expression.New(type);
                    var lambdaExp = Expression.Lambda<Func<object>>(propertyCreateExp, null);
                    propertyModel = lambdaExp.Compile()();
                }
                item.Value.SetValue(propertyModel, value, null);
            }
            return propertyModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="entityPropertyInfo"></param>
        /// <param name="dr"></param> 
        /// <returns></returns>
        public object SetObjectProperty<T>(T model, EntityPropertyInfo entityPropertyInfo, IResultReader dr)
               where T : class, new()
        {
            var propertyModel = entityPropertyInfo.GetValue(model, null);
            var type = entityPropertyInfo.PropertyType;
            if (type.IsIEnumerable())
            {
                var itemType = type.GetGenericArguments().FirstOrDefault();
                if (itemType == null) return model;
                var itemModel = SetObjectProperty(itemType, dr, $"{itemType.FullName}.{entityPropertyInfo.PropetyName}");
                if (propertyModel == null)
                {
                    var listType = typeof(List<>).MakeGenericType(itemType);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var newListExpression = Expression.New(listType.GetConstructor(new Type[0]));
                    var newListLambdaExpression = Expression.Lambda<Func<object>>(newListExpression, null);
                    propertyModel = newListLambdaExpression.Compile()();
                    entityPropertyInfo.SetValue(model, propertyModel, null);
                }
                if (itemModel != null)
                {
                    (propertyModel as IList)?.Add(itemModel);
                }
            }
            else if (propertyModel == null)
            {
                propertyModel = SetObjectProperty(type, dr, $"{type.FullName}.{entityPropertyInfo.PropetyName}");
                entityPropertyInfo.SetValue(model, propertyModel, null);
            }
            return propertyModel;
        }

        /// <summary>
        /// 获取从属性的值
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="dr"></param>
        /// <param name="tableKey"></param>
        /// <returns></returns>
        public object GetPropertyValue(EntityPropertyInfo fieldInfo, IResultReader dr, string tableKey)
        {
            if (string.IsNullOrEmpty(tableKey))
                tableKey = $"{fieldInfo.EntityInfo.EntityType.FullName}";
            string fieldName = "";
            if (_getSelectNameFun != null)
            {
                fieldName = _getSelectNameFun($"{tableKey}.{fieldInfo.PropetyName}");
            }
            else
            {
                fieldName = fieldInfo.DbFieldInfo.ColumnName;
            }

            var flag = dr.HasField(fieldName);
            if (!flag)
                return null;

            var value = ToEntityValue(fieldInfo.PropertyType, dr[fieldName]);
            return value;
        }

        /// <summary>
        /// 把数据库的数据转换为实体的数据
        /// </summary>
        /// <param name="type">实体的数据类型</param>
        /// <param name="data">数据库字段的值</param>
        /// <returns>实体的数据</returns>
        protected virtual object ToEntityValue(Type type, object data)
        {
            if (Convert.IsDBNull(data) || data == null)
                return null;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = new System.ComponentModel.NullableConverter(type).UnderlyingType;
            }
            if (type.IsEnum)
                return Enum.Parse(type, data.ToString());

            if (data is Guid)
            {
                if (type.FullName == "System.String")
                    return Convert.ChangeType(((Guid)data).ToString(), type);
            }
            if (Convert.IsDBNull(data))
                return null;

            #region Array
            if (type.IsArray && type == typeof(char[]))
            {
                var result = Convert.ChangeType(data, typeof(string)).ToString().ToCharArray();
                return result;
            }
            if (type.IsArray && type == typeof(byte[]))
            {
                var result = data;
                return result;
            }
            if ((!type.IsArray || type == typeof(byte[])) &&
                (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)))
                return Convert.ChangeType(data, type);
            {
                if (type == typeof(int[]) || type == typeof(List<int>))
                {
                    var result = StringUtils.SplitIntNumber(data.ToString());
                    if (type == typeof(int[]))
                        return result.ToArray();
                    return result;
                }
                if (type == typeof(long[]) || type == typeof(List<long>))
                {
                    var result = StringUtils.SplitLongNumber(data.ToString());
                    if (type == typeof(long[]))
                        return result.ToArray();
                    return result;
                }
                if (type == typeof(string[]) || type == typeof(List<string>))
                {
                    var result = StringUtils.SplitStrictString(data.ToString());
                    if (type == typeof(string[]))
                        return result.ToArray();
                    return result;
                }
            }

            #endregion
            return Convert.ChangeType(data, type);
        }
    }
}
