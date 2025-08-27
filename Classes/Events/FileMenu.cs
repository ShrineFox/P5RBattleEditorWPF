using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static P5RBattleEditorWPF.MainWindow;

namespace P5RBattleEditorWPF
{
    public partial class MainWindow : System.Windows.Window
    {
        public void NewProject_Click(object sender, EventArgs e)
        {
            FileMenu_LoadDefaultProject();
        }

        private void FileMenu_LoadDefaultProject()
        {
            // Load project from default JSON
            project = JsonConvert.DeserializeObject<Project>(File.ReadAllText("./Dependencies/Json/DataTablesP5R.json"));

            // Set up form controls
            UnitTab_ApplyNames();
        }
    }
}
