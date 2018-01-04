using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox
{
    public enum ExceptionCode
    {
        /// <summary>
        /// 什么都没有，占位符
        /// </summary>
        None = 0,
        /// <summary>
        /// 方法未实现
        /// </summary>
        NotImplemented,
        /// <summary>
        /// 登录失败
        /// </summary>
        LoginFailed,
        /// <summary>
        /// 数据更新时禁止访问
        /// </summary>
        ForbiddenWhenUpdating,
        /// <summary>
        /// 离线时禁用
        /// </summary>
        ForbiddenWhenOffline,
        /// <summary>
        /// 用户被禁言
        /// </summary>
        UserIsBanned,
        /// <summary>
        /// 非法操作
        /// </summary>
        InvalidUser,
        /// <summary>
        /// 操作失败
        /// </summary>
        OperationFailed,
        /// <summary>
        /// 无法获取可用的TCP端口
        /// </summary>
        CannotGetUsableTcpPort,
        /// <summary>
        /// 无法获取内网ip
        /// </summary>
        CannotGetIntranetIP,
        /// <summary>
        /// 网络异常
        /// </summary>
        NetworkError,
        /// <summary>
        /// 非法参数
        /// </summary>
        InvalidArgument,
        /// <summary>
        /// 无法找到用户
        /// </summary>
        NoUserFound = 100,
        NoChatFound,
        NoGroupFound,
        NoMsgFound,
        NoDeptFound,
        /// <summary>
        /// 无播音设备
        /// </summary>
        NoBroadcasterFound,
        /// <summary>
        /// 未找到录音设备
        /// </summary>
        NoMicrophoneFound,
        NoFileFound,
        NoDirFound,

        /// <summary>
        /// 群成员人数溢出
        /// </summary>
        MemberCountOverflow = 200,
        /// <summary>
        /// 未知错误
        /// </summary>
        UnknownError = 99999,
    }
}
