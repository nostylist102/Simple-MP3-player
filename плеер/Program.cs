using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace плеер
{
    internal static class Program
    {
        private static Form1 form1;
        private static Form2 form2;
        private static Form3 form3;
        private static Form8 form8;
        private static Form7 form7;



        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form1 = new Form1();
            
            form1.FormClosed += (sender, e) =>
            {
                //exiting form  form8 = new Form8(form1);
                //  form8.ShowDialog();
            };

            form7 = new Form7();
            form7.FormClosed += (sender, e) =>
            {
                form1.ShowDialog();
            };

            Timer timer = new Timer();
            timer.Interval = 2777; // 2 seconds
            timer.Tick += (sender, e) =>
            {
                form7.Hide();
                form7.Close();

            };
            timer.Start();

            form7.ShowDialog();



        }

        public static void ShowForm8(Form1 form1)
        {
            form8 = new Form8(form1);
            form8.ShowDialog();
        }
        public static void StartTimer()
        {

        }
        public static void GoBackToForm1()
        {
            if (form1 != null)
            {

            }
        }
    }
}