using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WeaponLib
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
                case "secondarystat":
                    this.Sort(Weapon.CompareBySecondaryStat);
                    break;
                case "passive":
                    this.Sort(Weapon.CompareByPassive);
                    break;
                default:
                    throw new ArgumentException($"Unknown sort column '{columnName}'. Valid columns are: Name, Type, Rarity, BaseAttack, SecondaryStat, Passive");
            }
        }

        // Loads weapons from a CSV/JSON/XML file
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
                string extension = Path.GetExtension(filename).ToLowerInvariant();
                switch (extension)
                {
                    case ".csv":
                        return LoadCsv(filename);
                    case ".json":
                        return LoadJson(filename);
                    case ".xml":
                        return LoadXml(filename);
                    default:
                        return false;
                }
            }
            catch
            {
                this.Clear();
                return false;
            }
        }
        // Saves the weapons collection to a CSV/JSON/XML file
        public bool Save(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            try
            {
                string extension = Path.GetExtension(filename).ToLowerInvariant();
                switch (extension)
                {
                    case ".csv":
                        return SaveCsv(filename);
                    case ".json":
                        return SaveJson(filename);
                    case ".xml":
                        return SaveXml(filename);
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool LoadCsv(string filename)
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

        private bool SaveCsv(string filename)
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

        private bool LoadJson(string filename)
        {
            string json = File.ReadAllText(filename);
            List<Weapon>? weapons = JsonSerializer.Deserialize<List<Weapon>>(json);
            if (weapons == null)
            {
                return false;
            }

            this.AddRange(weapons);
            return true;
        }

        private bool SaveJson(string filename)
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, json);
            return true;
        }

        private bool LoadXml(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Weapon>));
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                if (serializer.Deserialize(stream) is List<Weapon> weapons)
                {
                    this.AddRange(weapons);
                    return true;
                }
            }

            return false;
        }

        private bool SaveXml(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Weapon>));
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, this.ToList());
            }

            return true;
        }
    }
}
