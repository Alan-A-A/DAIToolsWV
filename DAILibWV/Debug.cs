using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAILibWV
{
    /// <summary>
    /// Static class for debug output.
    /// </summary>
    public static class Debug
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        private static readonly object _sync = new object();

        public static RichTextBox box = null;

        /// <summary>
        /// Setter method to set the <see cref="RichTextBox"/> member field.
        /// </summary>
        /// <param name="rtb">The RichTextBox to use.</param>
        public static void SetBox(RichTextBox rtb)
        {
            box = rtb;
        }

        /// <summary>
        /// Logs the given string into the <see cref="Debug.box"/>.
        /// </summary>
        /// <param name="s">The text to log.</param>
        /// <param name="update">True to update the window after appending the text.</param>
        public static void Log(string s, bool update = true)
        {
            lock (_sync)
            {
                if (box == null)
                    return;
                LockWindowUpdate(box.Parent.Handle);
                box.AppendText(s);
                if (update)
                {
                    LockWindowUpdate(IntPtr.Zero);
                    Application.DoEvents();
                }
            }
        }

        /// <summary>
        /// Same as <see cref="Log(string, bool)"/> but logs line-wise.
        /// </summary>
        public static void LogLn(string s, bool update = true)
        {
            Log(s + "\n", update);
        }
    }
}
