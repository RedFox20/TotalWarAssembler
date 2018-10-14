using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyKitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new AssemblyKit.Game("Attila", @"\Games\steamapps\common\Total War Attila\");
            game.LoadTables();
        }
    }
}
