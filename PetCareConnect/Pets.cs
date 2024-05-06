﻿namespace PetCareConnect
{
    public class Pets
    {
        public string Name { get; set; } //Navn på kæledyert
        public string Species { get; set; } // Art af kældedyr
        public string Breed { get; set; } // Race af kæledyr
        public int Age { get; set; } // Alder på kæledyret
        public string Info { get; set; } // Ekstra information om kæledyret

        public Pets()
        {

        }

        public Pets(string name, string species, string breed, int age, string info)
        {
            Name = name;
            Species = species;
            Breed = breed;
            Age = age;
            Info = info;
        }
    }
}
