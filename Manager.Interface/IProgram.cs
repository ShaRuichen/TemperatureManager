using System;
using System.Data;
using System.Net;

using MySql.Data.MySqlClient;

namespace Manager
{
    /// <summary>
    /// 包含与本程序有关的信息与操作。
    /// </summary>
    /// <remarks>该接口的所有方法和属性都是线程安全的。</remarks>
    public interface IProgram
    {
        /// <summary>
        /// 获取与本程序连接的远程主机的端口。
        /// </summary>
        /// <remarks>若没有与远程主机与本程序有连接，则返回null。</remarks>
        IPEndPoint? RemoteEndPoint { get; }

        /// <summary>
        /// 若没有与远程主机有连接，则尝试连接指定的终端。
        /// </summary>
        /// <param name="endPoint">要连接的终端</param>
        /// <returns>是否与指定的终端建立了新连接</returns>
        bool Connect(IPEndPoint endPoint);

        /// <summary>
        /// 若与远程主机有连接，则断开与远程主机的连接。
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 若与远程主机有连接，则向远程主机发送数据。
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns>是否成功发送</returns>
        /// <exception cref="OutOfMemoryException">发送的数据大于1KB。</exception>
        bool Send(dynamic data);

        /// <summary>
        /// 执行指定的javascript语句。
        /// </summary>
        /// <param name="jsText">要执行的javascript语句</param>
        void ExecuteJs(string jsText);

        /// <summary>
        /// 在数据库中执行有指定参数的Sql语句。
        /// </summary>
        /// <param name="sqlText">要执行的Sql语句</param>
        /// <param name="args">Sql语句的参数</param>
        /// <exception cref="MySqlException">连接数据库错误或Sql语句错误。</exception>
        void ExecuteSql(string sqlText, params object[] args);

        /// <summary>
        /// 在数据库中执行有指定参数的Sql语句并返回查询结果。
        /// </summary>
        /// <param name="sqlText">要执行的Sql语句</param>
        /// <param name="args">Sql语句的参数</param>
        /// <returns>查询结果</returns>
        /// <exception cref="MySqlException">连接数据库错误或Sql语句错误。</exception>
        DataRowCollection ReadSql(string sqlText, params object[] args);
    }
}
