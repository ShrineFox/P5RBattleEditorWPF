﻿using System.Windows;
using ShrineFox.IO;
using System.IO;

namespace P5RBattleEditorWPF
{
    public partial class MainWindow : System.Windows.Window
    {
        const int UNIT_SEGMENT0_ENTRY_SIZE = 68;
        const int UNIT_SEGMENT1_ENTRY_SIZE = 40;
        const int UNIT_SEGMENT2_ENTRY_SIZE = 40;
        const int UNIT_SEGMENT3_ENTRY_SIZE = 24;
        const int UNIT_SEGMENT4_ENTRY_SIZE = 6;

        private UnitTableData ReadP5RUnitTbl(string path)
        {
            UnitTableData tblData = new UnitTableData();

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (EndianBinaryReader br = new EndianBinaryReader(fs, Endianness.BigEndian))
                {
                    // Segment 0: Enemy Unit Stats
                    uint segment0Size = br.ReadUInt32();
                    for (int i = 0; i < (segment0Size / UNIT_SEGMENT0_ENTRY_SIZE); i++)
                    {
                        EnemyUnit enemy = new EnemyUnit() { Id = i };

                        // Bit flags
                        enemy.EnemyStats.Flags = ReadUnitFlags(br);
                        // Misc
                        enemy.EnemyStats.Arcana = br.ReadByte();
                        enemy.EnemyStats.RESERVE = br.ReadByte();
                        enemy.EnemyStats.Level = br.ReadUInt16();
                        enemy.EnemyStats.HP = br.ReadUInt32();
                        enemy.EnemyStats.SP = br.ReadUInt32();
                        // Enemy Stats
                        enemy.EnemyStats.Stats.Strength = br.ReadByte();
                        enemy.EnemyStats.Stats.Magic = br.ReadByte();
                        enemy.EnemyStats.Stats.Endurance = br.ReadByte();
                        enemy.EnemyStats.Stats.Agility = br.ReadByte();
                        enemy.EnemyStats.Stats.Luck = br.ReadByte();
                        enemy.EnemyStats.RESERVE2 = br.ReadByte();
                        // Enemy Skills
                        for (int x = 0; x < 8; x++)
                            enemy.EnemyStats.Skills[x] = br.ReadUInt16();
                        // Rewards
                        enemy.EnemyStats.EXPReward = br.ReadUInt16();
                        enemy.EnemyStats.MoneyReward = br.ReadUInt16();
                        for (int x = 0; x < 4; x++)
                            enemy.EnemyStats.ItemDrops[x] = new ItemDrop() { ItemID = br.ReadUInt16(), Probability = br.ReadUInt16() };
                        enemy.EnemyStats.EventItemDrop = new ItemDrop() { EventID = br.ReadUInt16(), ItemID = br.ReadUInt16(), Probability = br.ReadUInt16() };
                        // Attack Attributes
                        enemy.EnemyStats.AttackAttributes.AttackType = br.ReadByte();
                        enemy.EnemyStats.AttackAttributes.AttackAccuracy = br.ReadByte();
                        enemy.EnemyStats.AttackAttributes.AttackDamage = br.ReadUInt16();

                        tblData.EnemyUnits.Add(enemy);
                    }

                    br.BaseStream.Position += Get16ByteAlignmentPadding(br);

                    // Segment 1: Elemental Affinities (Enemies)
                    uint segment1Size = br.ReadUInt32();
                    for (int i = 0; i < (segment1Size / UNIT_SEGMENT1_ENTRY_SIZE); i++)
                    {
                        for (int x = 0; x < 20; x++)
                        {
                            tblData.EnemyUnits[i].Affinities.Add(
                            new Affinity()
                            {
                                Attributes = ConvertByteToBools(br.ReadByte()),
                                Multiplier = br.ReadByte()
                            });
                        }
                    }

                    // Segment 2: Elemental Affinities (Personas)

                    br.BaseStream.Position += Get16ByteAlignmentPadding(br);

                    uint segment2Size = br.ReadUInt32();
                    for (int i = 0; i < (segment2Size / UNIT_SEGMENT2_ENTRY_SIZE); i++)
                    {
                        var personaUnit = new PersonaUnit();

                        for (int x = 0; x < 20; x++)
                        {
                            personaUnit.Affinities.Add(
                            new Affinity()
                            {
                                Attributes = ConvertByteToBools(br.ReadByte()),
                                Multiplier = br.ReadByte()
                            });
                        }

                        tblData.PersonaUnits.Add(personaUnit);
                    }

                    br.BaseStream.Position += Get16ByteAlignmentPadding(br);

                    // Segment 3: Voice Data (Enemies)
                    uint segment3Size = br.ReadUInt32();
                    for (int i = 0; i < (segment3Size / UNIT_SEGMENT3_ENTRY_SIZE); i++)
                    {
                        tblData.EnemyUnits[i].VoiceData.VoiceID = br.ReadByte();
                        tblData.EnemyUnits[i].VoiceData.TALK_PERSON = br.ReadByte();
                        tblData.EnemyUnits[i].VoiceData.VoiceAcbValue = br.ReadByte();
                        tblData.EnemyUnits[i].VoiceData.Padding = br.ReadByte();
                        tblData.EnemyUnits[i].VoiceData.TALK_MONEY_MIN = br.ReadUInt16();
                        tblData.EnemyUnits[i].VoiceData.TALK_MONEY_MAX = br.ReadUInt16();
                        for (int x = 0; x < 4; x++)
                            tblData.EnemyUnits[i].VoiceData.TALK_ITEM[x] = new ItemDrop() { ItemID = br.ReadUInt16() };
                        for (int x = 0; x < 4; x++)
                            tblData.EnemyUnits[i].VoiceData.TALK_ITEM_RARE[x] = new ItemDrop() { ItemID = br.ReadUInt16() };
                    }

                    br.BaseStream.Position += Get16ByteAlignmentPadding(br);

                    // Segment 4: Visual Data (Enemies)
                    uint segment4Size = br.ReadUInt32();
                    for (int i = 0; i < (segment4Size / UNIT_SEGMENT4_ENTRY_SIZE); i++)
                    {
                        tblData.EnemyUnits[i].VisualData = new VisualData() { PersonaID = br.ReadUInt16(), ModelID = br.ReadUInt16(), UnknownR = br.ReadUInt16() };
                    }

                    br.BaseStream.Position += Get16ByteAlignmentPadding(br);

                    // Segment 5: Unknown
                    uint segment5Size = br.ReadUInt32();
                    tblData.Segment5 = br.ReadBytes(Convert.ToInt32(segment5Size));
                }
            }

            return tblData;
        }

        private void WriteP5RUnitTbl(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (EndianBinaryWriter bw = new EndianBinaryWriter(fs, Endianness.BigEndian))
                {
                    // Segment 0: Enemy Unit Stats
                    uint segment0Size = Convert.ToUInt32(UNIT_SEGMENT0_ENTRY_SIZE * project.UnitTblData.EnemyUnits.Count);
                    bw.Write(segment0Size);
                    foreach (var unit in project.UnitTblData.EnemyUnits)
                    {
                        // Bit flags
                        WriteUnitFlags(bw, unit.EnemyStats.Flags);
                        // Misc
                        bw.Write(unit.EnemyStats.Arcana);
                        bw.Write(unit.EnemyStats.RESERVE);
                        bw.Write(unit.EnemyStats.Level);
                        bw.Write(unit.EnemyStats.HP);
                        bw.Write(unit.EnemyStats.SP);
                        // Enemy Stats
                        bw.Write(unit.EnemyStats.Stats.Strength);
                        bw.Write(unit.EnemyStats.Stats.Magic);
                        bw.Write(unit.EnemyStats.Stats.Endurance);
                        bw.Write(unit.EnemyStats.Stats.Agility);
                        bw.Write(unit.EnemyStats.Stats.Luck);
                        bw.Write(unit.EnemyStats.RESERVE2);
                        // Enemy Skills
                        foreach (var skill in unit.EnemyStats.Skills)
                            bw.Write(skill);
                        // Rewards
                        bw.Write(unit.EnemyStats.EXPReward);
                        bw.Write(unit.EnemyStats.MoneyReward);
                        foreach(var itemDrop in unit.EnemyStats.ItemDrops)
                        {
                            bw.Write(itemDrop.ItemID);
                            bw.Write(itemDrop.Probability);
                        }
                        bw.Write(unit.EnemyStats.EventItemDrop.EventID);
                        bw.Write(unit.EnemyStats.EventItemDrop.ItemID);
                        bw.Write(unit.EnemyStats.EventItemDrop.Probability);
                        // Attack Attributes
                        bw.Write(unit.EnemyStats.AttackAttributes.AttackType);
                        bw.Write(unit.EnemyStats.AttackAttributes.AttackAccuracy);
                        bw.Write(unit.EnemyStats.AttackAttributes.AttackDamage);
                    }

                    Add16ByteAlignmentPadding(bw);

                    // Segment 1: Elemental Affinities (Enemies)
                    uint segment1Size = Convert.ToUInt32(UNIT_SEGMENT1_ENTRY_SIZE * project.UnitTblData.EnemyUnits.Count);
                    bw.Write(segment1Size);
                    foreach (var unit in project.UnitTblData.EnemyUnits)
                    {
                        foreach (var affinity in unit.Affinities)
                        {
                            bw.Write(ConvertBoolsToByte(affinity.Attributes));
                            bw.Write(affinity.Multiplier);
                        }
                    }

                    Add16ByteAlignmentPadding(bw);

                    // Segment 2: Elemental Affinities (Personas)
                    uint segment2Size = Convert.ToUInt32(UNIT_SEGMENT2_ENTRY_SIZE * project.UnitTblData.PersonaUnits.Count);
                    bw.Write(segment2Size);
                    foreach (var unit in project.UnitTblData.PersonaUnits)
                    {
                        foreach (var affinity in unit.Affinities)
                        {
                            bw.Write(ConvertBoolsToByte(affinity.Attributes));
                            bw.Write(affinity.Multiplier);
                        }
                    }

                    Add16ByteAlignmentPadding(bw);

                    // Segment 3: Voice Data (Enemies)
                    uint segment3Size = Convert.ToUInt32(UNIT_SEGMENT3_ENTRY_SIZE * project.UnitTblData.EnemyUnits.Count);
                    bw.Write(segment3Size);
                    foreach (var unit in project.UnitTblData.EnemyUnits)
                    {
                        bw.Write(unit.VoiceData.VoiceID);
                        bw.Write(unit.VoiceData.TALK_PERSON);
                        bw.Write(unit.VoiceData.VoiceAcbValue);
                        bw.Write(unit.VoiceData.Padding);
                        bw.Write(unit.VoiceData.TALK_MONEY_MIN);
                        bw.Write(unit.VoiceData.TALK_MONEY_MAX);
                        foreach (var itemDrop in unit.VoiceData.TALK_ITEM)
                            bw.Write(itemDrop.ItemID);
                        foreach (var itemDrop in unit.VoiceData.TALK_ITEM_RARE)
                            bw.Write(itemDrop.ItemID);
                    }

                    Add16ByteAlignmentPadding(bw);

                    // Segment 4: Visual Data (Enemies)
                    uint segment4Size = Convert.ToUInt32(UNIT_SEGMENT4_ENTRY_SIZE * project.UnitTblData.EnemyUnits.Count);
                    bw.Write(segment4Size);
                    foreach (var unit in project.UnitTblData.EnemyUnits)
                    {
                        bw.Write(unit.VisualData.PersonaID);
                        bw.Write(unit.VisualData.ModelID);
                        bw.Write(unit.VisualData.UnknownR);
                    }

                    Add16ByteAlignmentPadding(bw);

                    // Segment 5: Unknown
                    bw.Write(Convert.ToUInt32(project.UnitTblData.Segment5.Length));
                    bw.Write(project.UnitTblData.Segment5);

                    Add16ByteAlignmentPadding(bw);
                }
            }
        }

        private void WriteUnitFlags(EndianBinaryWriter bw, UnitFlags flags)
        {
            List<bool[]> boolSets = new List<bool[]>
            {
                new bool[] { flags.Bit0, flags.Bit1, flags.Bit2, flags.Bit3, flags.Bit4, flags.Bit5, flags.Bit6, flags.Bit7 },
                new bool[] { flags.Bit8, flags.Bit9, flags.Bit10, flags.Bit11, flags.Bit12, flags.Bit13, flags.Bit14, flags.Bit15 },
                new bool[] { flags.NoBeggingShadows, flags.HidingStatus, flags.Bit18, flags.Bit19, flags.GuaranteePersonaMask, flags.NotNegotiable, flags.Bit22, flags.Bit23 },
                new bool[] { flags.Bit24, flags.Bit25, flags.Bit26, flags.Bit27, flags.Bit28, flags.HidingStatusBoss, flags.InfiniteSP, flags.Bit31 }
            };

            foreach (var flagCollection in boolSets)
                bw.Write(ConvertBoolsToByte(flagCollection));
        }

        private UnitFlags ReadUnitFlags(EndianBinaryReader br)
        {
            var unitFlags = new UnitFlags();

            var flags = ConvertBytesToBools(br.ReadBytes(4));

            unitFlags.Bit0 = flags[0]; unitFlags.Bit8 = flags[8];
            unitFlags.Bit1 = flags[1]; unitFlags.Bit9 = flags[9];
            unitFlags.Bit2 = flags[2]; unitFlags.Bit10 = flags[10];
            unitFlags.Bit3 = flags[3]; unitFlags.Bit11 = flags[11];
            unitFlags.Bit4 = flags[4]; unitFlags.Bit12 = flags[12];
            unitFlags.Bit5 = flags[5]; unitFlags.Bit13 = flags[13];
            unitFlags.Bit6 = flags[6]; unitFlags.Bit14 = flags[14];
            unitFlags.Bit7 = flags[7]; unitFlags.Bit15 = flags[15];

            unitFlags.NoBeggingShadows = flags[16]; unitFlags.Bit24 = flags[24];
            unitFlags.HidingStatus = flags[17]; unitFlags.Bit25 = flags[25];
            unitFlags.Bit18 = flags[18]; unitFlags.Bit26 = flags[26];
            unitFlags.Bit19 = flags[19]; unitFlags.Bit27 = flags[27];
            unitFlags.GuaranteePersonaMask = flags[20]; unitFlags.Bit28 = flags[28];
            unitFlags.NotNegotiable = flags[21]; unitFlags.HidingStatusBoss = flags[29];
            unitFlags.Bit22 = flags[22]; unitFlags.InfiniteSP = flags[30];
            unitFlags.Bit23 = flags[23]; unitFlags.Bit31 = flags[31];

            return unitFlags;
        }

    }
}
