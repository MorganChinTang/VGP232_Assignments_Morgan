using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment2a;

namespace Assignment1
{
    public class WeaponCollection : List<Weapon>, IPeristence
    {
        // Gets the weapon with the highest base attack value
        public int GetHighestBaseAttack()
        {
            if (this.Count == 0)
            {
                return 0;
            }
            return this.Max(w => w.BaseAttack);
        }

        // Gets the weapon with the lowest base attack value
        public int GetLowestBaseAttack()
        {
            if (this.Count == 0)
            {
                return 0;
            }
            return this.Min(w => w.BaseAttack);
        }
        // Gets all weapons of a specific type
        public List<Weapon> GetAllWeaponsOfType(WeaponType type)
        {
            return this.Where(w => w.Type == type).ToList();
        }

        // Gets all weapons of a specific rarity
        public List<Weapon> GetAllWeaponsOfRarity(int stars)
        {
            return this.Where(w => w.Rarity == stars).ToList();
        }

        // Sorts the collection by the specified column name
        public void SortBy(string columnName)
        {
            switch (columnName.ToLower())
            {
                case "name":
                    this.Sort(Weapon.CompareByName);
                    break;
                case "type":
                    this.Sort(Weapon.CompareByType);
                    break;
                case "rarity":
                    this.Sort(Weapon.CompareByRarity);
                    break;
                case "baseattack":
                    this.Sort(Weapon.CompareByBaseAttack);
                    break;
                default:
                    throw new ArgumentException($"Unknown sort column '{columnName}'. Valid columns are: Name, Type, Rarity, BaseAttack");
            }
        }

        // Loads weapons from a CSV file
        public bool Load(string filename)
        {
            this.Clear();

            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            if (!File.Exists(filename))
            {
                return false;
            }

            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string header = reader.ReadLine();

                    while (reader.Peek() > 0)
                    {
                        string line = reader.ReadLine();

                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        if (Weapon.TryParse(line, out Weapon weapon))
                        {
                            this.Add(weapon);
                        }
                    }
                }

                return true;
            }
            catch
            {
                this.Clear();
                return false;
            }
        }
        // Saves the weapons collection to a CSV file
        public bool Save(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine("Name,Type,Image,Rarity,BaseAttack,SecondaryStat,Passive");

                    foreach (Weapon weapon in this)
                    {
                        writer.WriteLine(weapon.ToString());
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
