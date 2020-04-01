/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：UpdateCriteria
 * 命名空间：ObjectQL.Data
 * 文 件 名：UpdateCriteria
 * 创建时间：2016/10/24 13:58:15
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UpdateCriteria<TTableObject> : IDictionary<string, object>
       where TTableObject : class, new()
    {
        #region Nested Internal Classes
        private class DumpMemberAccessNameVisitor : ExpressionVisitor
        {
            private List<string> nameList = new List<string>();
            protected override Expression VisitMember(MemberExpression node)
            {
                var expression = base.VisitMember(node);
                nameList.Add(node.Member.Name);
                return expression;
            }

            public string MemberAccessName => string.Join(".", nameList);

            public override string ToString() => MemberAccessName;
        }
        #endregion 

        private readonly Dictionary<string, object> updateCriterias = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                return updateCriterias[key];
            }

            set
            {
                updateCriterias[key] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return updateCriterias.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return updateCriterias.Keys;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<object> Values
        {
            get
            {
                return updateCriterias.Values;
            }
        }

        private static Expression<Func<TTableObject, object>> CreateLambdaExpression(string propertyName)
        {
            var param = Expression.Parameter(typeof(TTableObject), "x");
            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.Property(body, member);
            }
            return Expression.Lambda<Func<TTableObject, object>>(Expression.Convert(body, typeof(object)), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            updateCriterias.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateCriteria"></param>
        /// <param name="value"></param>
        public void Add(Expression<Func<TTableObject, object>> updateCriteria, object value)
        {
            var visitor = new DumpMemberAccessNameVisitor();
            visitor.Visit(updateCriteria);
            var memberAccessName = visitor.MemberAccessName;
            if (!ContainsKey(memberAccessName))
            {
                Add(memberAccessName, value);
            }
        }

        public UpdateCriteria<TTableObject> Set(Expression<Func<TTableObject, object>> updateCriteria, object value)
        {
            Add(updateCriteria, value);
            return this;
        }

        public static UpdateCriteria<TTableObject> Builder
        {
            get
            {
                return new UpdateCriteria<TTableObject>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Tuple<Expression<Func<TTableObject, object>>, object>> UpdateCriterias
        {
            get
            {
                foreach (var kvp in updateCriterias)
                {
                    yield return new Tuple<Expression<Func<TTableObject, object>>, object>(CreateLambdaExpression(kvp.Key), kvp.Value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            updateCriterias.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return updateCriterias.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return updateCriterias.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection)updateCriterias).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return updateCriterias.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return updateCriterias.Remove(item.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return updateCriterias.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value)
        {
            return updateCriterias.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return updateCriterias.GetEnumerator();
        }
    }
}
