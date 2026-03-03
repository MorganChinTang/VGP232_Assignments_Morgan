using System;
using System.Linq;
using System.Windows;
using WeaponLib;

namespace Assignment2C
{
    /// <summary>
    /// Interaction logic for WeaponEditorWindow.xaml
    /// </summary>
    public partial class WeaponEditorWindow : Window
    {
        public Weapon Weapon { get; private set; }

        public WeaponEditorWindow()
            : this(new Weapon())
        {
        }

        public WeaponEditorWindow(Weapon weapon)
        {
            InitializeComponent();
            Weapon = weapon;
            InitializeTypeComboBox();
            PopulateFields();
        }

        private void InitializeTypeComboBox()
        {
            TypeComboBox.ItemsSource = Enum.GetNames(typeof(WeaponType)).Where(t => t != WeaponType.None.ToString()).ToList();
            TypeComboBox.SelectedIndex = 0;
        }

        private void PopulateFields()
        {
            NameTextBox.Text = Weapon.Name ?? string.Empty;
            ImageTextBox.Text = Weapon.Image ?? string.Empty;
            RarityTextBox.Text = Weapon.Rarity > 0 ? Weapon.Rarity.ToString() : string.Empty;
            BaseAttackTextBox.Text = Weapon.BaseAttack > 0 ? Weapon.BaseAttack.ToString() : string.Empty;
            SecondaryStatTextBox.Text = Weapon.SecondaryStat ?? string.Empty;
            PassiveTextBox.Text = Weapon.Passive ?? string.Empty;

            if (Weapon.Type != WeaponType.None)
            {
                TypeComboBox.SelectedItem = Weapon.Type.ToString();
            }
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Enum.TryParse(TypeComboBox.SelectedItem?.ToString(), out WeaponType type))
            {
                MessageBox.Show("Type is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(RarityTextBox.Text, out int rarity))
            {
                MessageBox.Show("Rarity must be a number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(BaseAttackTextBox.Text, out int baseAttack))
            {
                MessageBox.Show("Base Attack must be a number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Weapon.Name = NameTextBox.Text.Trim();
            Weapon.Type = type;
            Weapon.Image = ImageTextBox.Text.Trim();
            Weapon.Rarity = rarity;
            Weapon.BaseAttack = baseAttack;
            Weapon.SecondaryStat = SecondaryStatTextBox.Text.Trim();
            Weapon.Passive = PassiveTextBox.Text.Trim();

            DialogResult = true;
        }
    }
}
