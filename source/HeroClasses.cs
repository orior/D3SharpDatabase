using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3SharpDatabase
{
    //TODO 
    //Implementation of hero_class and SQL-Lite functions.
    public class HeroClasses
    {
        public int Id_Wizard { get; set; }
        public int Id_Witch_Doctor { get; set; }
        public int Id_Demon_Hunter { get; set; }
        public int Id_Monk { get; set; }
        public int Id_Barbarian { get; set; }
        
        public HeroClasses (int id_wizard, 
                             int id_witch_doctor, 
                             int id_demon_hunter, 
                             int id_monk, 
                             int id_barbarian)
        {
            Id_Wizard = id_wizard;
            Id_Witch_Doctor = id_witch_doctor;
            Id_Demon_Hunter = id_demon_hunter;
            Id_Monk = id_monk;
            Id_Barbarian = id_barbarian;
        }    
    }
}
