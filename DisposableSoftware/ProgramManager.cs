using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
namespace DisposableSoftware
{
    public class ProgramManager
    {
        public List<String[]> keywords;
        public Queue<String> queues;
        public ProgramManager(List<String[]> keywords)
        {
            this.keywords = keywords;
        }
        public ProgramManager()
        {

        }
        public String GetUninstallString(String productName)
        {
            String uninstallString = null;
            String registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {

                (from a in key.GetSubKeyNames()
                 let r = key.OpenSubKey(a)
                 select new
                 {
                     Application = r.GetValue("DisplayName"),
                     RegUninstallString = r.GetValue("UninstallString"),
                     IsWindowsInstaller = r.GetValue("WindowsInstaller")
                 })
                 .Where(c => c.Application != null)
                 .Where(c => c.RegUninstallString != null)
                 .ToList()
                 .FindAll(c => c.Application.ToString().Equals(productName)).ForEach(c => uninstallString = c.RegUninstallString.ToString());
                
            }
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey))
            {

                (from a in key.GetSubKeyNames()
                 let r = key.OpenSubKey(a)
                 select new
                 {
                     Application = r.GetValue("DisplayName"),
                     RegUninstallString = r.GetValue("UninstallString"),
                     IsWindowsInstaller = r.GetValue("WindowsInstaller")
                 })
                 .Where(c => c.Application != null)
                 .Where(c => c.RegUninstallString != null)
                 .ToList()
                 .FindAll(c => c.Application.ToString().Equals(productName)).ForEach(c => uninstallString = c.RegUninstallString.ToString());

            }
            return uninstallString;
        }
        public List<String> GetInstalledPrograms()
        {
            List<String> returns = new List<string>();
            RegistryKey key;
            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach(String k in key.GetSubKeyNames())
            {
                RegistryKey sk = key.OpenSubKey(k);
                if (sk.GetValue("displayName") != null)
                    returns.Add(sk.GetValue("displayName").ToString());
            }
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach(String k in key.GetSubKeyNames())
            {
                RegistryKey sk = key.OpenSubKey(k);
                if (sk.GetValue("displayName") != null)
                returns.Add(sk.GetValue("displayName").ToString());
            }
            return returns;
        }
        public void ProcessExit(object sender,EventArgs e)
        {
     
            if (queues.Count > 0)
            {
                String program = queues.Dequeue();
                UninstallProgram(program);
            }
            else
            {
                Form1.complete = true;
                Application.Exit();
            }
        }
        public void Start()
        {
            List<String> programs = GetInstalledPrograms();
            StringBuilder builder = new StringBuilder();
            List<String> targets = new List<string>();
            foreach (String[] strs in keywords)
            {
                String program = FindProgram(programs, strs);
                targets.Add(program);
            /*    String unst = GetUninstallString(program);
                MessageBox.Show(program);
                MessageBox.Show(unst);
                if (unst.StartsWith("Msi"))
                {

                    Process process = new Process();
                    process.StartInfo.Arguments = unst;
                    process.Exited += new EventHandler(ProcessExit);
                    process.Start();
                }
                else
                {
                    Process process = new Process();
                    process.Exited += new EventHandler(ProcessExit);
                    process.StartInfo.FileName = unst;
                    process.Start();
                }*/
            }
            queues = new Queue<string>(targets);
            UninstallProgram(queues.Dequeue());
        }
        public void UninstallProgram(String program)
        {
            String unst = GetUninstallString(program);
            if (unst.StartsWith("Msi"))
            {
            
                Process process = new Process();
                process.StartInfo.Arguments = unst;
                process.EnableRaisingEvents = true;
                process.Exited += ProcessExit;
            
                process.Start();
                process.WaitForExit();
                process.Close(); 
            }
            else
            {
                Process process = new Process();
                process.StartInfo.FileName = unst;
                process.EnableRaisingEvents = true;
                process.Exited += ProcessExit;

                process.Start();
                process.WaitForExit();
                process.Close();
            }
        }
        public String FindProgram(List<String> programs,String[] keywords)
        {
            return FindMostSimilar(programs, keywords);
        }
        public String FindMostSimilar(List<String> list,String[] keywords)
        {
            int n = 0;
            String str = null;
            foreach(String t in list)
            {
                if (str == null)
                {
                  
                    str = t;
                    n = NumOfMatching(t,keywords);
                }
                else
                {
                    int temp = NumOfMatching(t, keywords);
                    if(temp > n)
                    {
                        n = temp;
                        str = t;
                    }
                }
            }
            return str;
        }
        public int NumOfMatching(String target,String[] keywords)
        {
            int i = 0;
            foreach(String key in keywords)
            {
                if (target.ToLower().Contains(key.ToLower()))
                {
                    i++;
                }
            }
            return i;
        }
    }
}
