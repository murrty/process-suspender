using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuspensionTest {
    public partial class frmSuspender : Form {
        List<string> susProc = new List<string>();
        public frmSuspender() {
            InitializeComponent();
        }

        public enum ThreadAccess : int {
            TERMINATE = (0x1),
            SUSPEND_RESUME = (0x2),
            GET_CONTEXT = (0x8),
            SET_CONTEXT = (0x10),
            SET_INFORMATION = (0x20),
            QUERY_INFORMATION = (0x40),
            SET_THREAD_TOKEN = (0x80),
            IMPERSONATE = (0x100),
            DIRECT_IMPERSONATION = (0x200)
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccess dwDesriedAccess, bool bInheritHandle, int dwThreadId);
        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hHandle);

        private void sus(Process proc) {
            foreach (ProcessThread t in proc.Threads) {
                IntPtr th;
                th = OpenThread(ThreadAccess.SUSPEND_RESUME, false, t.Id);
                if (th != IntPtr.Zero) {
                    SuspendThread(th);
                    CloseHandle(th);
                }
            }
        }

        private void res(Process proc) {
            foreach (ProcessThread t in proc.Threads) {
                IntPtr th;
                th = OpenThread(ThreadAccess.SUSPEND_RESUME, false, t.Id);
                if (th != IntPtr.Zero) {
                    ResumeThread(th);
                    CloseHandle(th);
                }
            }
        }

        private void btnSuspend_Click(object sender, EventArgs e) {
            string sproc = cbProcesses.GetItemText(cbProcesses.SelectedItem);
            if (sproc.EndsWith(".exe"))
                sproc = sproc.Substring(sproc.Length - 4);

            if (susProc.Contains(sproc)) {
                return;
            }

            foreach (Process proc in Process.GetProcessesByName(sproc)) {
                sus(proc);
            }
            susProc.Add(sproc);
        }

        private void btnResume_Click(object sender, EventArgs e) {
            string rproc = cbProcesses.GetItemText(cbProcesses.SelectedItem);
            if (rproc.EndsWith(".exe"))
                rproc = rproc.Substring(rproc.Length - 4);

            foreach (Process proc in Process.GetProcessesByName(rproc)) {
                res(proc);
            }
            susProc.Remove(rproc);
        }

        private void frmSuspender_Load(object sender, EventArgs e) {
            foreach (Process Proc in Process.GetProcesses()) {
                cbProcesses.Items.Add(Proc.ProcessName);
            }
            cbProcesses.Sorted = true;
            cbProcesses.SelectedIndex = 0;
        }
    }
}
