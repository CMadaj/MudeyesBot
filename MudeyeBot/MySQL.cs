using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MudeyeBot
{
    class MySQL
    {
        public MySqlConnection Mud = new MySqlConnection();

        public MySQL()
        {
            string StrMud = "server='localhost';user id='root';password='REDACTED';database='sys'";
            Mud = new MySqlConnection(StrMud);
        }
    }
}
