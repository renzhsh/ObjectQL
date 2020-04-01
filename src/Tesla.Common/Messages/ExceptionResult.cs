/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ExceptionResult
 * 命名空间：Jinhe.Messages
 * 文 件 名：ExceptionResult
 * 创建时间：2017/2/28 19:49:08
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/


using Jinhe.Messages;
using System;
using System.Linq;
using System.Net; 

namespace Jinhe.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionResult : ResponseData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public ExceptionResult(System.Exception ex)
        {
            Message = ex.Message;
            if (ex is NotImplementedException)
            { 
                throw ex;
            }
            else if (ex is Exceptions.ASoftException)
            {
                this.Code = (ex as ASoftException).Code;
            }
            else if (ex.GetType()?.BaseType.FullName == "System.Data.Common.DbException")
            {
                MapDbExceptionRespose(ex);
            }
            else if (ex is Exceptions.ASoftException)
            {
                var exception = ex as ASoftException;
                this.Code = exception.Code;
            }
            else if (ex is System.FormatException)
            {
                this.Code = (int)RetCode.C1002;
                this.Message = "输入的参数格式错误"; 
            }
            else
            {
                this.Code = (int)RetCode.C1111; 
            }
        }

        /// <summary>
        /// 根据异常信息，映射数据库操作错误代码和信息（暂时针对ORACLE）
        /// </summary>
        /// <param name="ex">异常</param> 
        protected virtual void MapDbExceptionRespose(Exception ex)
        {
            if (ex.Message.StartsWith("ORA-00001"))
            {
                this.Code = (int)RetCode.DB9010;
                this.Message = "重复数据，违反唯一性约束";
            }
            else if (ex.Message.StartsWith("ORA-02291"))
            {
                this.Code = (int)RetCode.DB9011;
                this.Message = "违反数据完整性约束";
            }
            else
            {
                this.Code = (int)RetCode.DB9001;
                this.Message = "内部错误：数据库操作异常";
            }
        }
    }
}
