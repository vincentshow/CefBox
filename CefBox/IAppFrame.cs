using CefSharp;
using System;
using System.Collections.Generic;

namespace CefBox
{
    public interface IAppFrame : IDisposable
    {
        IWebBrowser Browser { get; set; }

        IEnumerable<string> DraggedFiles { get; set; }

        void ShowForm();

        void Maximum();

        void Minimum();

        void ResetForm(FrameOptions options);

        void CloseForm(CloseTypes type = CloseTypes.CloseSelf);

        void MoveForm();

        void LoadCEF(AppOptions options, object extendParam = null);

        void ShowMsg(string msg, ShowMsgTypes type = ShowMsgTypes.Info);

        void InvokeActionOnUIThread(Action action);
        
        void Reload();

        void ShowDevTools();
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

        /// <summary>
        /// 是否允许拖动文件
        /// </summary>
        public bool Draggable { get; set; }

        /// <summary>
        /// 是否每次请求都新建窗口
        /// </summary>
        public bool NewFormPerRequest { get; set; }

    }

    public enum ShowMsgTypes
    {
        Info,
        Warn,
        Error
    }
}
