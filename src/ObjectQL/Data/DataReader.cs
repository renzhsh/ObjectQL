/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DataReader
 * 命名空间：ObjectQL.Data
 * 文 件 名：DataReader
 * 创建时间：2016/10/19 11:00:32
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ObjectQL.Data
{
    /// <summary>
    /// 数据阅读器
    /// </summary>
    public class DataReader : ResultBase, IResultReader
    {
        private IDataReader _reader;
        private int _rowIndex = -1;
        private readonly List<string> columns = new List<string>();
        private DataTable _dataTable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="command"></param>
        public DataReader(IDataReader dr, IDbCommand command)
            : base(command)
        {
            _reader = dr;
            this.RefreshSchema();
        }

        private void RefreshSchema()
        {
            columns.Clear();
            _dataTable = this._reader.GetSchemaTable();
            foreach (DataColumn dc in _dataTable.Columns)
            {
                columns.Add(dc.ColumnName.ToUpper());
            }
        }

        /// <summary>
        /// 返回指定列的序号
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>列的序号</returns>
        public int GetOrdinal(string name)
        {
            try
            {
                return (_reader as DbDataReader).GetOrdinal(name);
            }
            catch
            {
                throw new Exception(string.Format("未找到数据读取器字段:{1}{0}{2}", Environment.NewLine, name, Description));
            }
        }

          /// <summary>
        /// 检查数据读取器是否包含某个指定的字段
        /// </summary>
        /// <param name="filedName">要检查的字段的名称</param>
        /// <returns>返回一个值,指示是否包含这个字段</returns>
        public bool HasField(string filedName)
        {
            return this.Fields.Contains(filedName); 
        }

        /// <summary>
        /// 数据读取器的字段个数
        /// </summary>
        public int FieldCount => _reader.FieldCount;

        private List<string> _fields;
        /// <summary>
        /// 当前读取器返回的列
        /// </summary>
        public IEnumerable<string> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = new List<string>();
                    for (int i = 0; i < this.FieldCount; i++)
                    {
                        _fields.Add(this.GetName(i));
                    }
                }
                return _fields;
            }
        }

        /// <summary>
        /// 返回指定列的名称
        /// </summary>
        /// <param name="index">列的序号</param>
        /// <returns>指定列的名称</returns>
        public string GetName(int index)
        {
            return this._reader.GetName(index);
        }

        #region 行纪录及游标的处理
        /// <summary>
        /// 指示数据读取器的游标向前移动指定长度
        /// </summary>
        /// <param name="skip">游标移动长度</param>
        public void Skip(int skip)
        {
            for (; skip > 0 && _reader.Read(); _rowIndex++, skip--) ;
        }

        /// <summary>
        /// 执行记取命令
        /// </summary>
        /// <returns>返回一个值,指示DataReader是否成功读取了一条数据</returns>
        public bool Read()
        {
            if (this._reader.Read())
            {
                _rowIndex++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 行索引值(默认为-1,当读取了第一行行,变为0)
        /// </summary>
        public int CurrentRowIndex
        {
            get
            {
                return this._rowIndex;
            }
        }

        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        public int RecordsAffected
        {
            get
            {
                return _reader.RecordsAffected;
            }
        }

        /// <summary>
        /// 返回一个值,指示检索到的结果中是否包含行结果.
        /// <para>
        /// 如果数据读取器不支持此操作,则抛出一个异常
        /// </para>
        /// </summary>
        public bool HasRows
        {
            get
            {
                var dbdr = _reader as DbDataReader;
                if (dbdr != null)
                {
                    return dbdr.HasRows;
                }
                return false;
            }
        }  
        #endregion

        #region 空值检查
        /// <summary>
        /// 检查数据读取器的某个字段是否为DBNull
        /// </summary>
        /// <param name="name"></param>
        private void CheckFieldNotNull(string name)
        {
            if (IsDBNull(name))
            {
                throw new Exception(string.Format("要获取的列{1}的为DBNull{0}{2}", Environment.NewLine, name, Description));
            }
        }

        /// <summary>
        /// 检查数据读取器的某个字段是否为DBNull
        /// </summary>
        /// <param name="ordinal">列索引</param>
        private void CheckFieldNotNull(int ordinal)
        {
            if (_reader.IsDBNull(ordinal))
            {
                throw new Exception(string.Format("要获取的列{1}的为DBNull{0}{2}", Environment.NewLine, ordinal, Description));
            }
        }


        /// <summary>
        /// 检查某一列是否是DBNull值
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>返回一个值,指示获取的列是否为NULL</returns>
        public bool IsDBNull(string name)
        {
            return _reader.IsDBNull(this.GetOrdinal(name));
        }

        /// <summary>
        /// 检查某一列是否是DBNull值
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>返回一个值,指示获取的列是否为NULL</returns>
        public bool IsDBNull(int ordinal)
        {
            return _reader.IsDBNull(ordinal);
        }

        #endregion

        #region 根据列名称或者索引来获取列的值
        /// <summary>
        /// 获取具有指定名称的列。
        /// </summary>
        /// <param name="name">要查找的列的名称。</param>
        /// <returns>名称指定为 Object 的列。</returns>
        public object this[string name]
        {
            get
            {
                return _reader[this.GetOrdinal(name)];
            }
        }

        /// <summary>
        /// 获取位于指定索引处的列。
        /// </summary>
        /// <param name="index">要获取的列的从零开始的索引</param>
        /// <returns>作为 Object 位于指定索引处的列</returns>
        public object this[int index]
        {
            get
            {
                return _reader[index];
            }
        }
        #endregion

        #region 获取字段的值（根据类型）
        #region GetBoolean

        /// <summary>
        /// 返回bool
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public bool GetBoolean(string name)
        {
            CheckFieldNotNull(name);
            string o = this._reader[name].ToString().Trim();
            return o == "1" || o.Equals("T", StringComparison.OrdinalIgnoreCase) || o.Equals("True", StringComparison.OrdinalIgnoreCase) || o.Equals("Y", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 返回bool
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public bool GetBoolean(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            string o = this._reader[ordinal].ToString().Trim();
            return o == "1" || o.Equals("T", StringComparison.OrdinalIgnoreCase) || o.Equals("True", StringComparison.OrdinalIgnoreCase) || o.Equals("Y", StringComparison.OrdinalIgnoreCase);

        }



        /// <summary>
        /// 返回bool
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public bool GetBoolean(string name, bool defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetBoolean(name);
        }

        /// <summary>
        /// 返回bool
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public bool GetBoolean(int ordinal, bool defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetBoolean(ordinal);
        }

        /// <summary>
        /// 返回指定列的Boolean值,如果指定列为DBNull,则返回null
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public bool? GetBooleanNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            string o = this._reader[name].ToString().Trim();
            return o == "1" || o.Equals("T", StringComparison.OrdinalIgnoreCase) || o.Equals("True", StringComparison.OrdinalIgnoreCase) || o.Equals("Y", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 返回指定列的Boolean值,如果指定列为DBNull,则返回null
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public bool? GetBooleanNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            string o = this._reader[ordinal].ToString().Trim();
            return o == "1" || o.Equals("T", StringComparison.OrdinalIgnoreCase) || o.Equals("True", StringComparison.OrdinalIgnoreCase) || o.Equals("Y", StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region GetByte

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public byte GetByte(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetByte(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public byte GetByte(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetByte(ordinal);
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public byte GetByte(string name, byte defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetByte(name);
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public byte GetByte(int ordinal, byte defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetByte(ordinal);
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public byte? GetByteNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this._reader.GetByte(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public byte? GetByteNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetByte(ordinal);
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="fieldoffset">偏移量</param>
        /// <param name="buffer">缓冲区</param>
        /// <param name="bufferoffset">缓冲区偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>实际读取的内容的长度</returns>
        public long GetBytes(string name, long fieldoffset, byte[] buffer, int bufferoffset, int length)
        {
            CheckFieldNotNull(name);
            return this._reader.GetBytes(this.GetOrdinal(name), fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="fieldoffset">偏移量</param>
        /// <param name="buffer">缓冲区</param>
        /// <param name="bufferoffset">缓冲区偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>实际读取的内容的长度</returns>
        public long GetBytes(int ordinal, long fieldoffset, byte[] buffer, int bufferoffset, int length)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetBytes(ordinal, fieldoffset, buffer, bufferoffset, length);
        }


        #endregion

        #region GetChars

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public char GetChar(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetChar(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public char GetChar(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetChar(ordinal);
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public char GetChar(string name, char defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetChar(name);
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public char GetChar(int ordinal, char defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetChar(ordinal);
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public char? GetCharNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this._reader.GetChar(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public char? GetCharNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetChar(ordinal);
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="fieldoffset">偏移量</param>
        /// <param name="buffer">目标</param>
        /// <param name="bufferoffset">目标偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>读取的字节长度</returns>
        public long GetChars(string name, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            CheckFieldNotNull(name);
            return this._reader.GetChars(this.GetOrdinal(name), fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="fieldoffset">偏移量</param>
        /// <param name="buffer">目标</param>
        /// <param name="bufferoffset">目标偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>读取的字节长度</returns>
        public long GetChars(int ordinal, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetChars(ordinal, fieldoffset, buffer, bufferoffset, length);
        }

        #endregion

        #region GetInt16

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetInt16(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetInt16(ordinal);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToInt16来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToInt16(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetInt16(name);
            }
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToInt16来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToInt16(this[ordinal]);
            }
            else
            {
                return GetInt16(ordinal);
            }
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(string name, short defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt16(name);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(int ordinal, short defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt16(ordinal);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt16来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(string name, short defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt16(name, convert);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt16来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(int ordinal, short defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt16(ordinal, convert);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public short? GetInt16Null(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this._reader.GetInt16(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public short? GetInt16Null(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetInt16(ordinal);
        }
        #endregion

        #region GetInt32

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetInt32(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetInt32(ordinal);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToInt32来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToInt32(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetInt32(name);
            }
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToInt32来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToInt32(this[ordinal]);
            }
            else
            {
                return GetInt32(ordinal);
            }
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(string name, int defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt32(name);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(int ordinal, int defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt32(ordinal);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt32来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(string name, int defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt32(name, convert);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt32来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(int ordinal, int defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt32(ordinal, convert);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public int? GetInt32Null(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this._reader.GetInt32(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public int? GetInt32Null(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetInt32(ordinal);
        }
        #endregion

        #region GetInt64

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetInt64(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetInt64(ordinal);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToInt64来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToInt64(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetInt64(name);
            }
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToInt64来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToInt64(this[ordinal]);
            }
            else
            {
                return GetInt64(ordinal);
            }
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(string name, long defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt64(name);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(int ordinal, long defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt64(ordinal);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt64来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(string name, long defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt64(name, convert);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt64来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(int ordinal, long defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt64(ordinal, convert);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public long? GetInt64Null(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this._reader.GetInt64(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public long? GetInt64Null(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetInt64(ordinal);
        }
        #endregion

        #region GetFloat

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetFloat(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetFloat(ordinal);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToSingle来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToSingle(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetFloat(name);
            }
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToSingle来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToSingle(this[ordinal]);
            }
            else
            {
                return GetFloat(ordinal);
            }
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(string name, float defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetFloat(name);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(int ordinal, float defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetFloat(ordinal);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToSingle来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(string name, float defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetFloat(name, convert);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToSingle来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(int ordinal, float defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetFloat(ordinal, convert);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public float? GetFloatNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this._reader.GetFloat(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public float? GetFloatNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetFloat(ordinal);
        }

        #endregion

        #region GetDouble

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetDouble(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetDouble(ordinal);
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToDouble来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToDouble(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetDouble(name);
            }
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToDouble来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToDouble(this[ordinal]);
            }
            else
            {
                return GetDouble(ordinal);
            }
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(string name, double defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetDouble(name);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(int ordinal, double defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDouble(ordinal);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(string name, double defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetDouble(name, convert);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(int ordinal, double defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDouble(ordinal, convert);
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public double? GetDoubleNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return GetDouble(this.GetOrdinal(name), true);
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public double? GetDoubleNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetDouble(ordinal);
        }
        #endregion

        #region GetDecimal

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetDecimal(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetDecimal(ordinal);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToDecimal(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetDecimal(name);
            }
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToDecimal(this[ordinal]);
            }
            else
            {
                return GetDecimal(ordinal);
            }
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(string name, decimal defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetDecimal(name);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(int ordinal, decimal defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDecimal(ordinal);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(string name, decimal defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetDecimal(name, convert);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(int ordinal, decimal defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDecimal(ordinal, convert);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public decimal? GetDecimalNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            };
            return this._reader.GetDecimal(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public decimal? GetDecimalNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetDecimal(ordinal);
        }
        #endregion

        #region GetDateTime

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetDateTime(this._reader.GetOrdinal(name));
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetDateTime(ordinal);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToDateTime来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToDateTime(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetDateTime(name);
            }
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToDateTime来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToDateTime(this[ordinal]);
            }
            else
            {
                return GetDateTime(ordinal);
            }
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(string name, DateTime defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetDateTime(name);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(int ordinal, DateTime defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDateTime(ordinal);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDateTime来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(string name, DateTime defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetDateTime(name, convert);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDateTime来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(int ordinal, DateTime defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDateTime(ordinal, convert);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public DateTime? GetDateTimeNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this._reader.GetDateTime(this._reader.GetOrdinal(name));
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public DateTime? GetDateTimeNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetDateTime(ordinal);
        }
        #endregion

        #region GetGuid

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public Guid GetGuid(string name)
        {
            CheckFieldNotNull(name);
            return this._reader.GetGuid(this._reader.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public Guid GetGuid(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this._reader.GetGuid(ordinal);
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public Guid GetGuid(string name, Guid defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : this._reader.GetGuid(this._reader.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public Guid GetGuid(int ordinal, Guid defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : this._reader.GetGuid(ordinal);
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public Guid? GetGuidNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this._reader.GetGuid(this._reader.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public Guid? GetGuidNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetGuid(ordinal);
        }

        #endregion

        #region GetString

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this._reader.GetString(this._reader.GetOrdinal(name));
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetString(ordinal);
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToString来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(string name, bool convert)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            if (convert)
            {
                return Convert.ToString(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetString(name);
            }
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToString来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(int ordinal, bool convert)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            if (convert)
            {
                return Convert.ToString(this[ordinal]);
            }
            else
            {
                return GetString(ordinal);
            }
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(string name, string defaultValue)
        {
            return (this.IsDBNull(name) ? defaultValue : this.GetString(name));
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(int ordinal, string defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetString(ordinal);
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToString来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(string name, string defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetString(name, convert);
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToString来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(int ordinal, string defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetString(ordinal, convert);
        }

        #endregion

        #region GetValue

        /// <summary>
        /// 返回指定列的值
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns></returns>
        public object GetValue(string name)
        {
            CheckFieldNotNull(name);
            return _reader.GetValue(_reader.GetOrdinal(name));
        }

        /// <summary>
        /// 返回指定列的值
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns></returns>
        public object GetValue(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return _reader.GetValue(ordinal);
        }

        #endregion

        #region GetFieldType

        /// <summary>
        /// 获取指定列的数据类型
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns></returns>
        public Type GetFieldType(int ordinal)
        {
            return this._reader.GetFieldType(ordinal);
        }

        /// <summary>
        /// 获取指定列的数据类型
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns></returns>
        public Type GetFieldType(string name)
        {
            return this._reader.GetFieldType(this._reader.GetOrdinal(name));
        }

        #endregion
        #endregion

        #region 阅读器状态检查和销毁
        /// <summary>
        /// 检查数据读取器是否已关闭
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return _reader.IsClosed;
            }
        }

        private StringBuilder sbdesc;
         /// <summary>
         /// 结果描述
         /// </summary>
        public override string Description
        {
            get
            {
                if (sbdesc == null)
                {
                    sbdesc = new System.Text.StringBuilder();

                    sbdesc.AppendLine("数据库返回结果信息如下");
                    sbdesc.AppendLine("类型类型:" + CommandType);
                    sbdesc.AppendLine("命令文本:" + CommandText);
                    if (Parameters != null && Parameters.Count > 0)
                    {
                        sbdesc.AppendLine("参数列表:");
                        int max = 0;
                        foreach (IDataParameter p in Parameters)
                        {
                            max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                        }
                        foreach (IDataParameter p in Parameters)
                        {
                            sbdesc.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                        }
                    }
                    // sbdesc.AppendLine("查询结果:" + value);
                }
                return sbdesc.ToString();
            }
        }


        /// <summary>
        /// 关闭DataReader
        /// </summary>
        public void Close()
        {
            this._reader.Close();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (!this._reader.IsClosed)
            {
                this._reader.Close();
            }
            this._reader.Dispose();
            base.Dispose();
        }
        #endregion

         
    }
}
