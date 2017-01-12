using System;

namespace VpnPasswords
{
    class Program
    {
        [STAThread()]
        static void Main(string[] args)
        {
            var controller = new Controller();
            controller.Run();
        }

    }
}
