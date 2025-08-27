using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5RBattleEditorWPF
{
    public partial class MainWindow : System.Windows.Window
    {
        public void SetupFormControls()
        {
            ApplyUnitNames();
            UpdateUnitListComboBox();
        }
    }
}
