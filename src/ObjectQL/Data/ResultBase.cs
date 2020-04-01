using System.Data;

namespace ObjectQL.Data
{
    /// <summary>
    /// 数据库命令执行产生的结果
    /// </summary>
    public abstract class ResultBase
    {
        private DataParameterCollection _parameters;
        private IDbCommand _command;
        private string _commandText;

        /// <summary>
        /// 数据库命令
        /// </summary>
        public string CommandText
        {
            get
            {
                return _commandText;
            }
        }

        private CommandType _commandType;
        /// <summary>
        /// 数据库命令的类型
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                return _commandType;
            }
        }
        /// <summary>
        /// 数据库命令参数
        /// </summary>
        public DataParameterCollection Parameters
        {
            get
            {
                return _parameters;
            }
        }

        /// <summary>
        /// 根据Command命令生成一个结果
        /// <para>
        /// 如果数据库命令的参数中有输出参数,则会记录所有的参数信息,否则不会记录参数
        /// </para>
        /// </summary>
        /// <param name="command">Command命令</param>
        public ResultBase(IDbCommand command)
        {
            if (command == null)
                return;
            _command = command;
            _commandText = command.CommandText;
            _commandType = command.CommandType;
            _parameters = new DataParameterCollection(command.Parameters);
        }

        /// <summary>
        /// 根据Command命令生成一个结果
        /// <para>
        /// 如果数据库命令的参数中有输出参数,则会记录所有的参数信息,否则不会记录参数
        /// </para>
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        public ResultBase(string commandText, CommandType commandType, IDataParameterCollection parameters)
        {
            _commandText = commandText;
            _commandType = commandType;
            _parameters = new DataParameterCollection(parameters);
        }


        /// <summary>
        /// 根据Command命令生成一个结果
        /// <para>
        /// 如果数据库命令的参数中有输出参数,则会记录所有的参数信息,否则不会记录参数
        /// </para>
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        public ResultBase(string commandText, CommandType commandType, DataParameterCollection parameters)
        {
            _commandText = commandText;
            _commandType = commandType;
            _parameters = parameters;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public abstract string Description
        {
            get;
        }

        #region IDisposable Members
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            //  commandText = null;
            if (Parameters != null)
            {
                Parameters.Dispose();
            }
        }

        #endregion
    }
}
