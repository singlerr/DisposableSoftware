using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DisposableSoftware
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (input.Text.Length > 0)
            {
              String[] kws = input.Text.Split(',');
              Form1.instance.keywords.Add(kws);
                Form1.instance.BeginInvoke(new Action(delegate ()
                {
                    Form1.instance.Add(kws);
                }));
                MessageBox.Show("해당 키워드가 추가되었습니다","확인",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Close();
            }
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
