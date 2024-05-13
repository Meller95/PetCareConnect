namespace PetCareConnect
{
    public class Pets
    {
        public int PetId { get; set; } // ID på kæledyret
        public string Name { get; set; } //Navn på kæledyert
        public string Species { get; set; } // Art af kældedyr
        public string Breed { get; set; } // Race af kæledyr
        public int Age { get; set; } // Alder på kæledyret
        public string Info { get; set; } // Ekstra information om kæledyret
        public int OwnerId { get; set; } // Ejerens ID
        public Pets()
        {

        }

        public Pets(int petid, string name, string species, string breed, int age, string info, int ownerId)
        {
            PetId = petid;
            Name = name;
            Species = species;
            Breed = breed;
            Age = age;
            Info = info;
            OwnerId = ownerId;
        }
    }
}
