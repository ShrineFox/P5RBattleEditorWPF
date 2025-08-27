using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5RBattleEditorWPF
{
    public partial class MainWindow : System.Windows.Window
    {
        public static void ApplyUnitNames()
        {
            GetPersonaUnitNames();
            GetEnemyUnitNames();
        }

        public static List<string> EnemyUnitNames = new List<string>();
        private static void GetEnemyUnitNames()
        {
            EnemyUnitNames.Clear();
            var enemiesSectionID = Array.IndexOf(TblNamesR, "Enemies");
            foreach (var entry in project.NameTblData[enemiesSectionID].TblEntries)
                EnemyUnitNames.Add(entry.Name);
        }

        public static List<string> PersonaUnitNames = new List<string>();
        private static void GetPersonaUnitNames()
        {
            PersonaUnitNames.Clear();
            var personaSectionID = Array.IndexOf(TblNamesR, "Personas");
            foreach (var entry in project.NameTblData[personaSectionID].TblEntries)
                PersonaUnitNames.Add(entry.Name);
        }

        private static void UpdateUnitListComboBox()
        {
            if (project.UnitTblData.EnemyUnits != null && project.UnitTblData.EnemyUnits.Count > 0)
            {
                //bindingSource_UnitList.DataSource = project.UnitTblData.EnemyUnits;
                //comboBox_Units.DataSource = bindingSource_UnitList;
                //comboBox_Units.FormattingEnabled = true;
                //comboBox_Units.Format += UnitListFormat;
            }
        }
    }
}
