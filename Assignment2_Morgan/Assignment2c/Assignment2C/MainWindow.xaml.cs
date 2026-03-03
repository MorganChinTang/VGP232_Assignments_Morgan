using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WeaponLib;

namespace Assignment2C
{
    public partial class MainWindow : Window
    {
        private readonly WeaponCollection mWeaponCollection;

        public MainWindow()
        {
            InitializeComponent();
            mWeaponCollection = new WeaponCollection();
            InitializeFilterTypeComboBox();
            RefreshWeaponList();
        }

        private void LoadClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Data Files (*.csv;*.json;*.xml)|*.csv;*.json;*.xml|CSV Files (*.csv)|*.csv|JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                if (!mWeaponCollection.Load(dialog.FileName))
                {
                    MessageBox.Show("Failed to load weapon data. Check the file format.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                FilterNameTextBox.Text = string.Empty;
                FilterTypeComboBox.SelectedIndex = 0;
                RefreshWeaponList();
            }
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                DefaultExt = ".csv"
            };

            if (dialog.ShowDialog() == true)
            {
                if (!mWeaponCollection.Save(dialog.FileName))
                {
                    MessageBox.Show("Failed to save weapon data. Check the file path.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddClicked(object sender, RoutedEventArgs e)
        {
            WeaponEditorWindow editor = new WeaponEditorWindow();
            if (editor.ShowDialog() == true)
            {
                mWeaponCollection.Add(editor.Weapon);
                RefreshWeaponList();
            }
        }

        private void EditClicked(object sender, RoutedEventArgs e)
        {
            if (WeaponListBox.SelectedItem is not Weapon selectedWeapon)
            {
                MessageBox.Show("Please select a weapon to edit.", "Edit Weapon", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Weapon copy = new Weapon
            {
                Name = selectedWeapon.Name,
                Type = selectedWeapon.Type,
                Image = selectedWeapon.Image,
                Rarity = selectedWeapon.Rarity,
                BaseAttack = selectedWeapon.BaseAttack,
                SecondaryStat = selectedWeapon.SecondaryStat,
                Passive = selectedWeapon.Passive
            };

            WeaponEditorWindow editor = new WeaponEditorWindow(copy);
            if (editor.ShowDialog() == true)
            {
                selectedWeapon.Name = editor.Weapon.Name;
                selectedWeapon.Type = editor.Weapon.Type;
                selectedWeapon.Image = editor.Weapon.Image;
                selectedWeapon.Rarity = editor.Weapon.Rarity;
                selectedWeapon.BaseAttack = editor.Weapon.BaseAttack;
                selectedWeapon.SecondaryStat = editor.Weapon.SecondaryStat;
                selectedWeapon.Passive = editor.Weapon.Passive;
                RefreshWeaponList();
            }
        }

        private void RemoveClicked(object sender, RoutedEventArgs e)
        {
            if (WeaponListBox.SelectedItem is not Weapon selectedWeapon)
            {
                MessageBox.Show("Please select a weapon to remove.", "Remove Weapon", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            mWeaponCollection.Remove(selectedWeapon);
            RefreshWeaponList();
        }

        private void SortRadioSelected(object sender, RoutedEventArgs e)
        {
            RefreshWeaponList();
        }

        private void FilterTypeOnlySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshWeaponList();
        }

        private void FilterNameTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshWeaponList();
        }

        private void InitializeFilterTypeComboBox()
        {
            List<string> types = Enum.GetNames(typeof(WeaponType)).ToList();
            types.Insert(0, "All");
            FilterTypeComboBox.ItemsSource = types;
            FilterTypeComboBox.SelectedIndex = 0;
        }

        private void RefreshWeaponList()
        {
            if (FilterTypeComboBox == null || FilterNameTextBox == null || WeaponListBox == null)
            {
                return;
            }

            ApplySort();

            IEnumerable<Weapon> filtered = mWeaponCollection;

            string selectedType = FilterTypeComboBox.SelectedItem as string ?? "All";
            if (!string.Equals(selectedType, "All", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(selectedType, out WeaponType type))
                {
                    filtered = mWeaponCollection.GetAllWeaponsOfType(type);
                }
            }

            string nameFilter = FilterNameTextBox.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(nameFilter))
            {
                filtered = filtered.Where(w => w.Name.StartsWith(nameFilter, StringComparison.OrdinalIgnoreCase));
            }

            WeaponListBox.ItemsSource = filtered.ToList();
        }

        private void ApplySort()
        {
            string sortColumn = "name";
            if (SortByBaseAttackRadioButton.IsChecked == true)
            {
                sortColumn = "baseattack";
            }
            else if (SortByRarityRadioButton.IsChecked == true)
            {
                sortColumn = "rarity";
            }
            else if (SortByPassiveRadioButton.IsChecked == true)
            {
                sortColumn = "passive";
            }
            else if (SortBySecondaryStatRadioButton.IsChecked == true)
            {
                sortColumn = "secondarystat";
            }
        }
    }
}