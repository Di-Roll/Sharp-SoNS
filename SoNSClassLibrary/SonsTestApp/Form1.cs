using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SoNSClassLibrary;

namespace SonsTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var loop = new UpdateLoop(100);

            Summator sum1 = new Summator();
            SynapseDirect daSynapse = new SynapseDirect(sum1, true, 5);
            Summator sum2 = new Summator();
            sum2.ConnectToSynapse(daSynapse);            

            loop.Enabled = true;
            sum1.Sum = 29;
        }
    }
}
