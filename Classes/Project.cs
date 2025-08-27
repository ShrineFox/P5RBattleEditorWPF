namespace P5RBattleEditorWPF
{
    public partial class MainWindow : System.Windows.Window
    {
        public class Project
        {
            public EncountTableData EncountTblData { get; set; } = new EncountTableData();
            public UnitTableData UnitTblData { get; set; } = new UnitTableData();
            public SkillTableData SkillTblData { get; set; } = new SkillTableData();
            public PersonaTableData PersonaTblData { get; set; } = new PersonaTableData();
            public List<TblSection> NameTblData { get; set; } = new List<TblSection>();
        }

        public static Project project = new Project();
    }
}
