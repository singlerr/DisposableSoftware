using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;
namespace DisposableSoftware
{
    public partial class Form1 : Form
    {
        public static bool complete = false;
        public static Form1 instance;
        public List<string[]> keywords = new List<string[]>();
        public Form1()
        {
            InitializeComponent();
            instance = this;
            FormClosing += new FormClosingEventHandler(Form_FormClosing);
        }
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (kwlist.Items.Count > 0)
            {
                if (!complete) { 
                if (MessageBox.Show("등록된 프로그램들을 모두 삭제하고 종료하시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    e.Cancel = true;
                    ProgramManager manager = new ProgramManager(keywords);
                    manager.Start();
                }
            }

            }
        }
      
        public void Add(String[] str)
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < str.Length; i++)
            {
                if(i != str.Length-1)
                {
                    builder.Append(str[i]).Append(",");
                }
                else
                {
                    builder.Append(str[i]);
                }
            }
            kwlist.Items.Add(builder.ToString());   
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
            StartWatcher();
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = "java";
            startInfo.Arguments = "-jar " + AppDomain.CurrentDomain.BaseDirectory + @"\Exec.jar";
            process.StartInfo = startInfo;
            process.Start();
            if(MessageBox.Show("시작 프로그램으로 등록하시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                rkey.SetValue("ProgramName", Application.ExecutablePath.ToString());
            }
        }
        public void StartWatcher()
        {
            ManagementEventWatcher watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            watcher.EventArrived += new EventArrivedEventHandler(ProcessStart);
            watcher.Start();
        }
        public void ProcessStart(object sender,EventArrivedEventArgs e)
        {
           
            String process = e.NewEvent.Properties["ProcessName"].Value.ToString();
            if(process.ToLower().Contains("setup") || process.ToLower().Contains("installer"))
            {
                if(MessageBox.Show("다음과 같은 소프트웨어의 설치를 감지했습니다.\n"+process+"이 소프트웨어를 일회용으로 설정하시겠습니까?", "!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    MessageBox.Show("해당 소프트웨어에 대한 키워드를 입력해주십시오.", "!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("키워드 구분은 ',' 으로 합니다.", "!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BeginInvoke(new Action(delegate()
                    {
                        Form2 form2 = new Form2();
                        form2.Show();
                    }));
                }
                  
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }
    }
}
