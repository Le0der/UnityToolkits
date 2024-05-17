using SQLite4Unity3d;
using System;

namespace Le0der.ArchiveSystem
{

    public class DemoTestData
    {
        public string Name = "Le0der";
        public int Age = 20;
        public float Height = 1.7f;
        public bool IsMale = true;
    }


    public class DemoTestPersonData : IDBDataBase
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        public DemoTestPersonData()
        {
            Name = "Le0der";
            Surname = "Le0der";
            Age = 20;
            Birthday = new DateTime(1998, 10, 10);
            Height = 170;
            Weight = 60;
            Address = "Shanghai";
            Description = "I am a good man.";
        }
    }
}
