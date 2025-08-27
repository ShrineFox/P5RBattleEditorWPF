using ShrineFox.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5RBattleEditorWPF
{
    public partial class MainWindow : System.Windows.Window
    {
        public void UnitTab_ApplyNames()
        {
            if (string.IsNullOrEmpty(project.UnitTblData.EnemyUnits[1].PersonaName))
            {
                // Gather unit names from NAME.TBL data if not already present
                UnitTab_ApplyPersonaUnitNames();
                UnitTab_ApplyEnemyUnitNames();
            }

            // Apply unit names to comboBox
            comboBox_Units.ItemsSource = project.UnitTblData.EnemyUnits;
        }

        private void UnitTab_ApplyEnemyUnitNames()
        {
            var enemiesSectionID = Array.IndexOf(TblNamesR, "Enemies");
            for (int i = 0; i < project.NameTblData[enemiesSectionID].TblEntries.Count; i++)
                project.UnitTblData.EnemyUnits[i].ShadowName = project.NameTblData[enemiesSectionID].TblEntries[i].Name.Copy();
        }

        private void UnitTab_ApplyPersonaUnitNames()
        {
            var personaSectionID = Array.IndexOf(TblNamesR, "Personas");
            for (int i = 0; i < project.NameTblData[personaSectionID].TblEntries.Count; i++)
                project.UnitTblData.EnemyUnits[i].PersonaName = project.NameTblData[personaSectionID].TblEntries[i].Name.Copy();
        }
    }
}
