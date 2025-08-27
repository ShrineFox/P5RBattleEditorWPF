using ShrineFox.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace P5RBattleEditorWPF
{
    public partial class MainWindow : System.Windows.Window
    {
        public bool valueChanged = false;
        public bool isUpdating = false;
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
            // Apply misc names
            comboBox_Arcana.ItemsSource = Enum.GetValues(typeof(ArcanaName)).Cast<ArcanaName>();
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

        private void UnitTab_SelectedUnitChanged(object sender, SelectionChangedEventArgs e)
        {
            isUpdating = true;
            UnitTab_UpdateUnitControls();
            isUpdating = false;
        }

        private void UnitTab_PersonaNameChanged(object sender, TextChangedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (isUpdating)
                return;

            valueChanged = true;

            ((EnemyUnit)comboBox_Units.SelectedItem).PersonaName = txtBox.Text;
        }

        private void UnitTab_ShadowNameChanged(object sender, TextChangedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (isUpdating)
                return;

            valueChanged = true;

            ((EnemyUnit)comboBox_Units.SelectedItem).ShadowName = txtBox.Text;
        }

        private void UnitTab_NotesChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdating)
                return;

            var txtBox = sender as TextBox;

            ((EnemyUnit)comboBox_Units.SelectedItem).Comment = txtBox.Text;
        }

        private void UnitTab_ControlLostFocus(object sender, EventArgs e)
        {
            if (valueChanged)
            {
                valueChanged = false;
                UnitTab_RefreshUnitComboBox();
            }
        }

        private void UnitTab_RefreshUnitComboBox()
        {
            comboBox_Units.Items.Refresh();
        }

        private void UnitTab_ArcanaChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdating)
                return;

            ComboBox cmbBox = sender as ComboBox;

            ((EnemyUnit)comboBox_Units.SelectedItem).EnemyStats.Arcana = Convert.ToByte(cmbBox.SelectedIndex);
        }

        private void UnitTab_UpdateUnitControls()
        {
            // Unit Name
            txt_PersonaName.Text = ((EnemyUnit)comboBox_Units.SelectedItem).PersonaName;
            txt_ShadowName.Text = ((EnemyUnit)comboBox_Units.SelectedItem).ShadowName;

            // Unit Arcana
            comboBox_Arcana.SelectedIndex = ((EnemyUnit)comboBox_Units.SelectedItem).EnemyStats.Arcana;

            // Notes
            txt_UnitNotes.Text = ((EnemyUnit)comboBox_Units.SelectedItem).Comment;

            // Unit Stats
            NumUpDwn_UnitEXP.Value = ((EnemyUnit)comboBox_Units.SelectedItem).EnemyStats.EXPReward;
        }
    }
}
