using BEAUTYLIFE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandevuSistemi
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Lisans.MUSCLEMAN_Lisanslama();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logo logoForm = new Logo();
            logoForm.Show();
             Application.Run(); 
        }
    }
}
