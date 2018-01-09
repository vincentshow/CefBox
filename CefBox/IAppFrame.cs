using CefBox.Models;
using CefSharp;
using System;
using System.Collections.Generic;

namespace CefBox
{
    public interface IAppFrame : IDisposable
    {
        IWebBrowser Browser { get; set; }

        IEnumerable<string> DraggedFiles { get; set; }

        /// <summary>
        /// show sub frame by specified option
        /// </summary>
        /// <param name="options">the frame options used to create sub frame</param>
        /// <returns></returns>
        IAppFrame ShowSubForm(FrameOptions options);

        void ShowFrame();

        void Maximum();

        void Minimum();

        void ResetFrame(FrameOptions options);

        void CloseFrame(CloseTypes type = CloseTypes.CloseSelf);

        void MoveFrame();

        void ShowMsg(string msg, ShowMsgTypes type = ShowMsgTypes.Info);

        void Reload();

        void ShowDevTools();

        void BeforeCloing();
    }

    public enum CloseTypes
    {
        CloseSelf,
        Hide2Tray,
        MainFormChanged,
        CloseApp
    }

    public class FrameOptions
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Width { get; set; }

        public int MinWidth { get; set; }

        public int Height { get; set; }

        public int MinHeight { get; set; }

        public bool Resizable { get; set; } = true;

        public bool CanBeMinimized { get; set; } = true;

        public bool IsModal { get; set; }

        public bool ShowHeaderBar { get; set; }

        public bool IsMain { get; set; }

        public string ContentPath { get; set; }

        public FramePositions Position { get; set; } = FramePositions.CenterScreen;
        /// <summary>
        /// 是否允许拖动文件
        /// </summary>
        public bool Draggable { get; set; }

        /// <summary>
        /// 是否每次请求都新建窗口
        /// </summary>
        public bool NewFormPerRequest { get; set; }

    }

    public enum FramePositions
    {
        /// <summary>
        /// 居中
        /// </summary>
        CenterScreen,
        /// <summary>
        /// 右下
        /// </summary>
        BottomRight,
        /// <summary>
        /// 参考上一个位置，中心点不变
        /// </summary>
        RefPreLocation
    }

    public enum ShowMsgTypes
    {
        Info,
        Warn,
        Error
    }
}
