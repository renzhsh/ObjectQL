using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.DataAnnotation;

namespace ObjectQL.DataExtTests.Models
{
    [Table("MyStudent", Schema = "objectql")]
    [Unique(Property = "Name,Age")]
    public class Student
    {
        [PrimaryKey]
        public string ID { get; set; }

        [Column(MaxLength = 200, Unicode = false)]
        public string Name { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }
    }

    public class TestColumn
    {
        [PrimaryKey]
        public string FID { get; set; }

        public sbyte FSByte { get; set; }

        public byte FByte { get; set; }

        public int FInt { get; set; }

        public float FFloat { get; set; }

        public double FDouble { get; set; }

        public decimal FDecimal { get; set; }

        public bool FBoolean { get; set; }

        public char FChar { get; set; }

        public char[] FChars { get; set; }

        public DateTime FDate { get; set; }

        public byte[] FBytes { get; set; }

        public TestEnum FEnum { get; set; }
    }

    public enum TestEnum
    {
        Enum1 = 0,
        Enum2 = 1,
        Enum12 = 12
    }
}
