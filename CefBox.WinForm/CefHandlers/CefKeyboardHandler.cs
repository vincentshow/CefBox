using CefSharp;
using System;

namespace CefBox.WinForm.CefHandlers
{
    public class CefKeyboardHandler : IKeyboardHandler
    {
        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            return false;
        }

        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            try
            {
                if (type == KeyType.KeyUp)
                {
                    return false;
                }

                var selectedKey = WinApi.ModifierKeys.None;
                switch (modifiers)
                {
                    case CefEventFlags.None:
                        selectedKey = WinApi.ModifierKeys.None;
                        break;
                    case CefEventFlags.ShiftDown:
                        selectedKey = WinApi.ModifierKeys.Shift;
                        break;
                    case CefEventFlags.ControlDown:
                        selectedKey = WinApi.ModifierKeys.Ctrl;
                        break;
                    case CefEventFlags.AltDown:
                        selectedKey = WinApi.ModifierKeys.Alt;
                        break;
                    case CefEventFlags.ControlDown | CefEventFlags.AltDown:
                        selectedKey = WinApi.ModifierKeys.Ctrl | WinApi.ModifierKeys.Alt;
                        break;
                    case CefEventFlags.ControlDown | CefEventFlags.ShiftDown:
                        selectedKey = WinApi.ModifierKeys.Ctrl | WinApi.ModifierKeys.Shift;
                        break;
                    case CefEventFlags.ShiftDown | CefEventFlags.AltDown:
                        selectedKey = WinApi.ModifierKeys.Shift | WinApi.ModifierKeys.Alt;
                        break;
                    default:
                        return false;
                }

                //只有功能键的时候跳过
                if (type == KeyType.RawKeyDown && windowsKeyCode >= 16 && windowsKeyCode <= 18)
                {
                    return false;
                }
                //modifier为空且按下了字符键
                if (selectedKey == WinApi.ModifierKeys.None && type == KeyType.Char)
                {
                    return false;
                }

                //selectedKey windowsKeyCode
            }
            catch (Exception ex)
            {
                //GTLogger.Instance.Log($"error when OnKeyEvent", ex);
            }

            return false;
        }
    }
}
