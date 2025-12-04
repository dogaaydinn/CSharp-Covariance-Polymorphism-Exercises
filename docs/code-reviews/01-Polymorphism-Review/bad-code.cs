// ❌ BAD CODE - Written by Junior Developer
// Pull Request: "Add animal sound feature"
// Reviewer: Sarah Chen (Senior Engineer)

using System;
using System.Collections.Generic;

namespace AnimalSoundSystem
{
    // Animal class with type enum
    public class Animal
    {
        public string Name { get; set; }
        public string Type { get; set; }  // "Dog", "Cat", "Bird"
        public int Age { get; set; }
        
        // Method to make sound based on type
        public void MakeSound()
        {
            if (Type == "Dog")
            {
                Console.WriteLine("Woof!");
            }
            else if (Type == "Cat")
            {
                Console.WriteLine("Meow!");
            }
            else if (Type == "Bird")
            {
                Console.WriteLine("Chirp!");
            }
            else if (Type == "Cow")
            {
                Console.WriteLine("Moo!");
            }
            else
            {
                Console.WriteLine("Unknown animal sound");
            }
        }
        
        // Method to get animal info
        public void PrintInfo()
        {
            Console.WriteLine($"Name: {Name}, Type: {Type}, Age: {Age}");
            
            // Different logic based on type
            if (Type == "Dog")
            {
                Console.WriteLine("Dogs are loyal companions");
            }
            else if (Type == "Cat")
            {
                Console.WriteLine("Cats are independent");
            }
            else if (Type == "Bird")
            {
                Console.WriteLine("Birds can fly");
            }
        }
    }
    
    // Service class
    public class AnimalService
    {
        private List<Animal> animals = new List<Animal>();
        
        public void AddAnimal(string name, string type, int age)
        {
            var animal = new Animal 
            { 
                Name = name, 
                Type = type, 
                Age = age 
            };
            animals.Add(animal);
        }
        
        public void MakeAllAnimalsSounds()
        {
            foreach (var animal in animals)
            {
                // Check type before calling
                if (animal.Type == "Dog" || animal.Type == "Cat" || 
                    animal.Type == "Bird" || animal.Type == "Cow")
                {
                    animal.MakeSound();
                }
            }
        }
        
        public void FeedAnimal(Animal animal)
        {
            // Different feeding logic per type
            if (animal.Type == "Dog")
            {
                Console.WriteLine("Feeding dog with dog food");
            }
            else if (animal.Type == "Cat")
            {
                Console.WriteLine("Feeding cat with cat food");
            }
            else if (animal.Type == "Bird")
            {
                Console.WriteLine("Feeding bird with seeds");
            }
        }
        
        public List<Animal> GetAnimalsByType(string type)
        {
            var result = new List<Animal>();
            foreach (var animal in animals)
            {
                if (animal.Type == type)
                {
                    result.Add(animal);
                }
            }
            return result;
        }
    }
    
    // Usage example
    class Program
    {
        static void Main(string[] args)
        {
            var service = new AnimalService();
            
            service.AddAnimal("Buddy", "Dog", 5);
            service.AddAnimal("Whiskers", "Cat", 3);
            service.AddAnimal("Tweety", "Bird", 2);
            
            service.MakeAllAnimalsSounds();
            
            var dogs = service.GetAnimalsByType("Dog");
            foreach (var dog in dogs)
            {
                service.FeedAnimal(dog);
            }
        }
    }
}
