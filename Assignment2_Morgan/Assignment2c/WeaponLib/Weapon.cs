using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponLib
{
    public enum WeaponType
    {
        Sword,
        Polearm,
        Claymore,
        Catalyst,
        Bow,
        None
    }

    public class Weapon
    {
        // Name,Type,Rarity,BaseAttack,Image,SecondaryStat,Passive
        public string Name { get; set; }
        public WeaponType Type { get; set; }
        public int Rarity { get; set; }
        public int BaseAttack { get; set; }
        public string Image { get; set; }
        public string SecondaryStat { get; set; }
        public string Passive { get; set; }

        /// <summary>
        /// The Comparator function to check for name
        /// </summary>
        /// <param name="left">Left side Weapon</param>
        /// <param name="right">Right side Weapon</param>
        /// <returns> -1 (or any other negative value) for "less than", 0 for "equals", or 1 (or any other positive value) for "greater than"</returns>
        public static int CompareByName(Weapon left, Weapon right)
        {
            return left.Name.CompareTo(right.Name);
        }

        // TODO: add sort for each property:
        /// <summary>
        /// The Comparator function to check for type
        /// </summary>
        /// <param name="left">Left side Weapon</param>
        /// <param name="right">Right side Weapon</param>
        /// <returns> -1 (or any other negative value) for "less than", 0 for "equals", or 1 (or any other positive value) for "greater than"</returns>
        // CompareByType
        public static int CompareByType(Weapon left, Weapon right)
        {
            return left.Type.ToString().CompareTo(right.Type.ToString());
        }

        /// <summary>
        /// The Comparator function to check for rarity
        /// </summary>
        /// <param name="left">Left side Weapon</param>
        /// <param name="right">Right side Weapon</param>
        /// <returns> -1 (or any other negative value) for "less than", 0 for "equals", or 1 (or any other positive value) for "greater than"</returns>
        // CompareByRarity
        public static int CompareByRarity(Weapon left, Weapon right)
        {
            return left.Rarity.CompareTo(right.Rarity);
        }

        /// <summary>
        /// The Comparator function to check for base attack
        /// </summary>
        /// <param name="left">Left side Weapon</param>
        /// <param name="right">Right side Weapon</param>
        /// <returns> -1 (or any other negative value) for "less than", 0 for "equals", or 1 (or any other positive value) for "greater than"</returns>
        // CompareByBaseAttack
        public static int CompareByBaseAttack(Weapon left, Weapon right)
        {
            return left.BaseAttack.CompareTo(right.BaseAttack);
        }

        /// <summary>
        /// The Comparator function to check for secondary stat
        /// </summary>
        /// <param name="left">Left side Weapon</param>
        /// <param name="right">Right side Weapon</param>
        /// <returns> -1 (or any other negative value) for "less than", 0 for "equals", or 1 (or any other positive value) for "greater than"</returns>
        // CompareBySecondaryStat
        public static int CompareBySecondaryStat(Weapon left, Weapon right)
        {
            return string.Compare(left.SecondaryStat, right.SecondaryStat, StringComparison.Ordinal);
        }

        /// <summary>
        /// The Comparator function to check for passive
        /// </summary>
        /// <param name="left">Left side Weapon</param>
        /// <param name="right">Right side Weapon</param>
        /// <returns> -1 (or any other negative value) for "less than", 0 for "equals", or 1 (or any other positive value) for "greater than"</returns>
        // CompareByPassive
        public static int CompareByPassive(Weapon left, Weapon right)
        {
            return string.Compare(left.Passive, right.Passive, StringComparison.Ordinal);
        }

        /// <summary>
        /// The Weapon string with all the properties
        /// </summary>
        /// <returns>The Weapon formated string</returns>
        public override string ToString()
        {
            // TODO: construct a comma seperated value string
            // Name,Type,Image,Rarity,BaseAttack,SecondaryStat,Passive
            return string.Format("{0},{1},{2},{3},{4},{5},{6}", Name, Type, Image, Rarity, BaseAttack, SecondaryStat, Passive);
        }

        /// <summary>
        /// Attempts to parse a CSV line into a Weapon object
        /// </summary>
        /// <param name="rawData">The CSV line to parse</param>
        /// <param name="weapon">The output Weapon object if successful</param>
        /// <returns>True if parsing was successful, false otherwise</returns>
        public static bool TryParse(string rawData, out Weapon weapon)
        {
            weapon = null;

            if (string.IsNullOrEmpty(rawData))
            {
                return false;
            }

            string[] values = rawData.Split(',');

            if (values.Length != 7)
            {
                return false;
            }

            try
            {
                weapon = new Weapon();
                weapon.Name = values[0];

                if (!Enum.TryParse<WeaponType>(values[1], ignoreCase: true, out WeaponType type))
                {
                    weapon = null;
                    return false;
                }
                weapon.Type = type;

                weapon.Image = values[2];

                if (!int.TryParse(values[3], out int rarity))
                {
                    weapon = null;
                    return false;
                }
                weapon.Rarity = rarity;

                if (!int.TryParse(values[4], out int baseAttack))
                {
                    weapon = null;
                    return false;
                }
                weapon.BaseAttack = baseAttack;

                weapon.SecondaryStat = values[5];
                weapon.Passive = values[6];

                return true;
            }
            catch
            {
                weapon = null;
                return false;
            }
        }
    }
}
