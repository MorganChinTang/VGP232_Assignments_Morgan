using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Assignment1;

namespace Assignment2a
{
    [TestFixture]
    public class UnitTests
    {
        private WeaponCollection weaponCollection;
        private string inputPath;
        private string outputPath;

        const string INPUT_FILE = "data2.csv";
        const string OUTPUT_FILE = "output.csv";

        // A helper function to get the directory of where the actual path is.
        private string CombineToAppPath(string filename)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
        }

        [SetUp]
        public void SetUp()
        {
            inputPath = CombineToAppPath(INPUT_FILE);
            outputPath = CombineToAppPath(OUTPUT_FILE);
            weaponCollection = new WeaponCollection();
        }

        [TearDown]
        public void CleanUp()
        {
            // We remove the output file after we are done.
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }

        // WeaponCollection Unit Tests
        [Test]
        public void WeaponCollection_GetHighestBaseAttack_HighestValue()
        {
            // Expected Value: 48
            Assert.That(weaponCollection.Load(inputPath), Is.True);
            Assert.That(weaponCollection.GetHighestBaseAttack(), Is.EqualTo(48));
        }

        [Test]
        public void WeaponCollection_GetLowestBaseAttack_LowestValue()
        {
            // Expected Value: 23
            Assert.That(weaponCollection.Load(inputPath), Is.True);
            Assert.That(weaponCollection.GetLowestBaseAttack(), Is.EqualTo(23));
        }

        [TestCase(WeaponType.Sword, 21)]
        public void WeaponCollection_GetAllWeaponsOfType_ListOfWeapons(WeaponType type, int expectedValue)
        {
            Assert.That(weaponCollection.Load(inputPath), Is.True);
            List<Weapon> weapons = weaponCollection.GetAllWeaponsOfType(type);
            Assert.That(weapons.Count, Is.EqualTo(expectedValue));
        }

        [TestCase(5, 10)]
        public void WeaponCollection_GetAllWeaponsOfRarity_ListOfWeapons(int stars, int expectedValue)
        {
            Assert.That(weaponCollection.Load(inputPath), Is.True);
            List<Weapon> weapons = weaponCollection.GetAllWeaponsOfRarity(stars);
            Assert.That(weapons.Count, Is.EqualTo(expectedValue));
        }

        [Test]
        public void WeaponCollection_LoadThatExistAndValid_True()
        {
            // load returns true, expect WeaponCollection with count of 95
            Assert.That(weaponCollection.Load(inputPath), Is.True);
            Assert.That(weaponCollection.Count, Is.EqualTo(95));
        }

        [Test]
        public void WeaponCollection_LoadThatDoesNotExist_FalseAndEmpty()
        {
            // load returns false, expect an empty WeaponCollection
            string nonExistentPath = CombineToAppPath("nonexistent.csv");
            Assert.That(weaponCollection.Load(nonExistentPath), Is.False);
            Assert.That(weaponCollection.Count, Is.EqualTo(0));
        }

        [Test]
        public void WeaponCollection_SaveWithValuesCanLoad_TrueAndNotEmpty()
        {
            // save returns true, load returns true, and WeaponCollection is not empty
            Assert.That(weaponCollection.Load(inputPath), Is.True);
            Assert.That(weaponCollection.Save(outputPath), Is.True);
            
            WeaponCollection loadedCollection = new WeaponCollection();
            Assert.That(loadedCollection.Load(outputPath), Is.True);
            Assert.That(loadedCollection.Count, Is.GreaterThan(0));
        }

        [Test]
        public void WeaponCollection_SaveEmpty_TrueAndEmpty()
        {
            // After saving an empty WeaponCollection, load the file and expect WeaponCollection to be empty.
            weaponCollection.Clear();
            Assert.That(weaponCollection.Save(outputPath), Is.True);
            
            WeaponCollection loadedCollection = new WeaponCollection();
            Assert.That(loadedCollection.Load(outputPath), Is.True);
            Assert.That(loadedCollection.Count, Is.EqualTo(0));
        }

        // Weapon Unit Tests
        [Test]
        public void Weapon_TryParseValidLine_TruePropertiesSet()
        {
            Weapon expected = new Weapon()
            {
                Name = "Skyward Blade",
                Type = WeaponType.Sword,
                Image = "https://vignette.wikia.nocookie.net/gensin-impact/images/0/03/Weapon_Skyward_Blade.png",
                Rarity = 5,
                BaseAttack = 46,
                SecondaryStat = "Energy Recharge",
                Passive = "Sky-Piercing Fang"
            };

            string line = "Skyward Blade,Sword,https://vignette.wikia.nocookie.net/gensin-impact/images/0/03/Weapon_Skyward_Blade.png,5,46,Energy Recharge,Sky-Piercing Fang";
            Weapon actual = null;

            Assert.That(Weapon.TryParse(line, out actual), Is.True);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Type, Is.EqualTo(expected.Type));
            Assert.That(actual.BaseAttack, Is.EqualTo(expected.BaseAttack));
            Assert.That(actual.Rarity, Is.EqualTo(expected.Rarity));
            Assert.That(actual.Image, Is.EqualTo(expected.Image));
            Assert.That(actual.SecondaryStat, Is.EqualTo(expected.SecondaryStat));
            Assert.That(actual.Passive, Is.EqualTo(expected.Passive));
        }

        [Test]
        public void Weapon_TryParseInvalidLine_FalseNull()
        {
            string line = "Skyward Blade,InvalidType,https://example.com/image.png,NotANumber,46,Energy Recharge,Sky-Piercing Fang";
            Weapon actual = null;

            Assert.That(Weapon.TryParse(line, out actual), Is.False);
            Assert.That(actual, Is.Null);
        }
    }
}
