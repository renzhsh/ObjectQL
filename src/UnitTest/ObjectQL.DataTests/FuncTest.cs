using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace ObjectQL.DataTests
{
    [TestClass]
    public class FuncTest
    {
        [TestMethod]
        public void Test_BaseType()
        {
            Debug.WriteLine("TimeSpan:" + typeof(TimeSpan).FullName);
            Debug.WriteLine("DateTime:" + typeof(DateTime).FullName);
            Debug.WriteLine("byte[]:" + typeof(byte[]).FullName);
            Debug.WriteLine("char[]:" + typeof(char[]).FullName);
            Debug.WriteLine("float:" + typeof(float).FullName);
            Debug.WriteLine("double:" + typeof(double).FullName);
            Debug.WriteLine("decimal:" + typeof(decimal).FullName);
        }

        [TestMethod]
        public void Test_String_Except()
        {
            var list1 = new string[] { "A", "B", "E" };

            var list2 = new string[] { "B", "C", "D" };

            var result = list1.Except(list2);
            Debug.WriteLine(string.Join(",", result));
        }

        [TestMethod]
        public void Test_Object_Except()
        {
            var list1 = new string[] { "A", "B", "E" }.Select(item => new ExString(item));

            var list2 = new string[] { "B", "C", "D" }.Select(item => new ExString(item));

            var result = list1.Except(list2, new ExStringNameComparer<ExString>());
            Debug.WriteLine(string.Join(",", result));
        }

        [TestMethod]
        public void Test_TimeSpan()
        {
            var zero = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var first = DateTime.UtcNow;

            var now = (long)(first - zero).TotalMilliseconds;

            Debug.WriteLine(now);
        }
    }

    public class ExString
    {
        public ExString() { }

        public ExString(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ExStringNameComparer<T> : IEqualityComparer<T> where T : ExString, new()
    {
        public bool Equals(T x, T y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(T obj)
        {
            var target = (ExString)obj;
            return target.Name.GetHashCode();
        }
    }
}
