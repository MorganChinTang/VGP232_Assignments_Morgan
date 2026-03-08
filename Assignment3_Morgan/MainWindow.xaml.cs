using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Microsoft.Win32;
using TextureAtlasLib;

namespace Assignment3_Morgan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly RoutedUICommand RemoveCommand = new("Remove", "Remove", typeof(MainWindow));
        public static readonly RoutedUICommand RemoveAllCommand = new("Remove All", "RemoveAll", typeof(MainWindow));

        private const string DefaultProjectName = "SpriteSheet.xml";
        private const string DefaultOutputFile = "SpriteSheet.png";
        private const string DefaultColumns = "1";

        private readonly ObservableCollection<ImageItem> _images = new();
        private readonly Stack<IUndoableAction> _undoStack = new();
        private readonly Stack<IUndoableAction> _redoStack = new();
        private string? _currentProjectPath;

        public MainWindow()
        {
            InitializeComponent();
            lbSprites.ItemsSource = _images;
            tbOutputFile.Text = DefaultOutputFile;
            tbColumnsInput.Text = DefaultColumns;
            txtProjectName.Text = DefaultProjectName;

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OnNewExecuted));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OnOpenExecuted));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, OnSaveExecuted, OnSaveCanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, OnSaveAsExecuted));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopyExecuted, OnCopyCanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPasteExecuted, OnPasteCanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUndoExecuted, OnUndoCanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, OnRedoExecuted, OnRedoCanExecute));
            CommandBindings.Add(new CommandBinding(RemoveCommand, OnRemoveExecuted, OnRemoveCanExecute));
            CommandBindings.Add(new CommandBinding(RemoveAllCommand, OnRemoveAllExecuted, OnRemoveAllCanExecute));
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                FileName = string.IsNullOrWhiteSpace(tbOutputFile.Text) ? DefaultOutputFile : tbOutputFile.Text,
                InitialDirectory = string.IsNullOrWhiteSpace(tbOutputDir.Text) ? null : tbOutputDir.Text
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            tbOutputDir.Text = Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
            tbOutputFile.Text = Path.GetFileName(dialog.FileName);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                Multiselect = true
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            AddImages(dialog.FileNames, true);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelected(true);
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetColumns(out int columns))
            {
                return;
            }

            if (_images.Count == 0)
            {
                MessageBox.Show("Please add at least one PNG image.", "Missing Images", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbOutputDir.Text))
            {
                MessageBox.Show("Please select an output directory.", "Missing Output Directory", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbOutputFile.Text))
            {
                MessageBox.Show("Please enter an output filename.", "Missing Output Filename", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!tbOutputFile.Text.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("The output filename must end with .png.", "Invalid Output Filename", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string outputDirectory = tbOutputDir.Text.Trim();
            if (!Directory.Exists(outputDirectory))
            {
                var createResult = MessageBox.Show("The output directory does not exist. Create it?", "Create Directory", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (createResult != MessageBoxResult.Yes)
                {
                    return;
                }

                Directory.CreateDirectory(outputDirectory);
            }

            string outputPath = Path.Combine(outputDirectory, tbOutputFile.Text.Trim());
            bool overwrite = false;
            if (File.Exists(outputPath))
            {
                var overwriteResult = MessageBox.Show("The output file already exists. Overwrite it?", "Confirm Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (overwriteResult != MessageBoxResult.Yes)
                {
                    return;
                }

                overwrite = true;
            }

            try
            {
                var sheet = new Spritesheet
                {
                    Columns = columns,
                    OutputDirectory = outputDirectory,
                    OutputFile = tbOutputFile.Text.Trim(),
                    IncludeMetaData = cbIncludeMetaData.IsChecked == true,
                    InputPaths = _images.Select(item => item.Path).ToList()
                };

                sheet.Generate(overwrite);

                string message = cbIncludeMetaData.IsChecked == true
                    ? "Sprite sheet and metadata were generated successfully. View the output folder?"
                    : "Sprite sheet was generated successfully. View the output folder?";
                var result = MessageBox.Show(message, "Generate Successful", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    OpenOutputDirectory(outputDirectory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Generate Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewMenu_Click(object sender, RoutedEventArgs e)
        {
            ExecuteNewProject();
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ExecuteOpenProject();
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSaveProject();
        }

        private void SaveAsMenu_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSaveProjectAs();
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ConfirmSaveIfNeeded())
            {
                e.Cancel = true;
            }
        }

        private void OnNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteNewProject();
        }

        private void OnOpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteOpenProject();
        }

        private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteSaveProject();
        }

        private void OnSaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteSaveProjectAs();
        }

        private void OnSaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrWhiteSpace(_currentProjectPath);
        }

        private void OnCopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (lbSprites.SelectedItem is ImageItem selected)
            {
                Clipboard.SetText(selected.Path);
            }
        }

        private void OnCopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lbSprites.SelectedItem is ImageItem;
        }

        private void OnPasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Clipboard.ContainsFileDropList())
            {
                var list = Clipboard.GetFileDropList();
                AddImages(list.Cast<string>(), true);
                return;
            }

            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                var paths = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                AddImages(paths, true);
            }
        }

        private void OnPasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsFileDropList() || Clipboard.ContainsText();
        }

        private void OnUndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_undoStack.Count == 0)
            {
                return;
            }

            IUndoableAction action = _undoStack.Pop();
            action.Undo();
            _redoStack.Push(action);
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnUndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _undoStack.Count > 0;
        }

        private void OnRedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_redoStack.Count == 0)
            {
                return;
            }

            IUndoableAction action = _redoStack.Pop();
            action.Redo();
            _undoStack.Push(action);
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnRedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _redoStack.Count > 0;
        }

        private void OnRemoveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveSelected(true);
        }

        private void OnRemoveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lbSprites.SelectedItem is ImageItem;
        }

        private void OnRemoveAllExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAll(true);
        }

        private void OnRemoveAllCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _images.Count > 0;
        }

        private void ExecuteNewProject()
        {
            if (!ConfirmSaveIfNeeded())
            {
                return;
            }

            _currentProjectPath = null;
            txtProjectName.Text = DefaultProjectName;
            tbOutputDir.Text = string.Empty;
            tbOutputFile.Text = DefaultOutputFile;
            tbColumnsInput.Text = DefaultColumns;
            cbIncludeMetaData.IsChecked = false;
            _images.Clear();
            ClearUndoRedo();
            UpdateSaveState();
        }

        private void ExecuteOpenProject()
        {
            if (!ConfirmSaveIfNeeded())
            {
                return;
            }

            var dialog = new OpenFileDialog
            {
                Filter = "Sprite Sheet Project (*.xml)|*.xml|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            LoadProject(dialog.FileName);
        }

        private bool ExecuteSaveProject()
        {
            if (string.IsNullOrWhiteSpace(_currentProjectPath))
            {
                return ExecuteSaveProjectAs();
            }

            return SaveProjectToPath(_currentProjectPath);
        }

        private bool ExecuteSaveProjectAs()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Sprite Sheet Project (*.xml)|*.xml|All files (*.*)|*.*",
                FileName = string.IsNullOrWhiteSpace(_currentProjectPath) ? DefaultProjectName : Path.GetFileName(_currentProjectPath)
            };

            if (dialog.ShowDialog() != true)
            {
                return false;
            }

            string targetPath = dialog.FileName;
            bool saved = SaveProjectToPath(targetPath);
            if (saved)
            {
                _currentProjectPath = targetPath;
                UpdateSaveState();
                UpdateProjectTitle();
            }

            return saved;
        }

        private bool ConfirmSaveIfNeeded()
        {
            if (!HasProjectData() && string.IsNullOrWhiteSpace(_currentProjectPath))
            {
                return true;
            }

            var result = MessageBox.Show("Would you like to save your project first?", "Save Project", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
            {
                return false;
            }

            if (result == MessageBoxResult.Yes)
            {
                return ExecuteSaveProject();
            }

            return true;
        }

        private bool HasProjectData()
        {
            return _images.Count > 0
                   || !string.IsNullOrWhiteSpace(tbOutputDir.Text)
                   || !string.Equals(tbOutputFile.Text, DefaultOutputFile, StringComparison.Ordinal)
                   || !string.Equals(tbColumnsInput.Text, DefaultColumns, StringComparison.Ordinal)
                   || cbIncludeMetaData.IsChecked == true;
        }

        private bool SaveProjectToPath(string path)
        {
            try
            {
                var project = new SpriteSheetProject
                {
                    OutputDirectory = tbOutputDir.Text.Trim(),
                    OutputFile = tbOutputFile.Text.Trim(),
                    ImagePaths = _images.Select(item => item.Path).ToList(),
                    IncludeMetaData = cbIncludeMetaData.IsChecked == true,
                    Columns = GetColumnsForSave()
                };

                var serializer = new XmlSerializer(typeof(SpriteSheetProject));
                using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                serializer.Serialize(stream, project);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void LoadProject(string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(SpriteSheetProject));
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var project = serializer.Deserialize(stream) as SpriteSheetProject;
                if (project == null)
                {
                    MessageBox.Show("The selected project file could not be loaded.", "Open Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ApplyProject(project);
                _currentProjectPath = path;
                UpdateProjectTitle();
                UpdateSaveState();
                ClearUndoRedo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Open Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyProject(SpriteSheetProject project)
        {
            tbOutputDir.Text = project.OutputDirectory ?? string.Empty;
            tbOutputFile.Text = string.IsNullOrWhiteSpace(project.OutputFile) ? DefaultOutputFile : project.OutputFile;
            cbIncludeMetaData.IsChecked = project.IncludeMetaData;
            tbColumnsInput.Text = project.Columns > 0 ? project.Columns.ToString() : DefaultColumns;

            _images.Clear();
            var missingFiles = new List<string>();
            if (project.ImagePaths != null)
            {
                foreach (string path in project.ImagePaths)
                {
                    if (File.Exists(path))
                    {
                        _images.Add(new ImageItem(path));
                    }
                    else
                    {
                        missingFiles.Add(path);
                    }
                }
            }

            if (missingFiles.Count > 0)
            {
                string message = "The following images were missing and removed:\n" + string.Join("\n", missingFiles);
                MessageBox.Show(message, "Missing Images", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private int GetColumnsForSave()
        {
            return int.TryParse(tbColumnsInput.Text, out int columns) && columns > 0
                ? columns
                : int.Parse(DefaultColumns);
        }

        private void UpdateSaveState()
        {
            menuItemSave.IsEnabled = !string.IsNullOrWhiteSpace(_currentProjectPath);
            CommandManager.InvalidateRequerySuggested();
        }

        private void UpdateProjectTitle()
        {
            txtProjectName.Text = string.IsNullOrWhiteSpace(_currentProjectPath)
                ? DefaultProjectName
                : Path.GetFileName(_currentProjectPath);
        }

        private bool TryGetColumns(out int columns)
        {
            if (!int.TryParse(tbColumnsInput.Text, out columns) || columns < 1)
            {
                MessageBox.Show("Columns must be a whole number greater than 0.", "Invalid Columns", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void AddImages(IEnumerable<string> paths, bool trackHistory)
        {
            var newItems = new List<ImageItem>();
            var existing = new HashSet<string>(_images.Select(item => item.Path), StringComparer.OrdinalIgnoreCase);

            foreach (string path in paths)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                if (!File.Exists(path))
                {
                    continue;
                }

                if (!path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!existing.Add(path))
                {
                    continue;
                }

                newItems.Add(new ImageItem(path));
            }

            foreach (ImageItem item in newItems)
            {
                _images.Add(item);
            }

            if (trackHistory && newItems.Count > 0)
            {
                PushUndoAction(new AddImagesAction(_images, newItems));
            }
        }

        private void RemoveSelected(bool trackHistory)
        {
            if (lbSprites.SelectedItem is not ImageItem selected)
            {
                return;
            }

            int index = _images.IndexOf(selected);
            if (index < 0)
            {
                return;
            }

            _images.RemoveAt(index);

            if (trackHistory)
            {
                PushUndoAction(new RemoveImagesAction(_images, new List<RemoveImagesAction.RemovedItem>
                {
                    new(index, selected)
                }));
            }
        }

        private void RemoveAll(bool trackHistory)
        {
            if (_images.Count == 0)
            {
                return;
            }

            var removedItems = _images.Select((item, index) => new RemoveImagesAction.RemovedItem(index, item)).ToList();
            _images.Clear();

            if (trackHistory)
            {
                PushUndoAction(new RemoveImagesAction(_images, removedItems));
            }
        }

        private void PushUndoAction(IUndoableAction action)
        {
            _undoStack.Push(action);
            _redoStack.Clear();
            CommandManager.InvalidateRequerySuggested();
        }

        private void ClearUndoRedo()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            CommandManager.InvalidateRequerySuggested();
        }

        private void OpenOutputDirectory(string outputDirectory)
        {
            try
            {
                Process.Start(new ProcessStartInfo("explorer.exe", outputDirectory)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Open Folder Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private interface IUndoableAction
        {
            void Undo();
            void Redo();
        }

        private sealed class AddImagesAction : IUndoableAction
        {
            private readonly ObservableCollection<ImageItem> _collection;
            private readonly List<ImageItem> _items;

            public AddImagesAction(ObservableCollection<ImageItem> collection, List<ImageItem> items)
            {
                _collection = collection;
                _items = items;
            }

            public void Undo()
            {
                foreach (ImageItem item in _items)
                {
                    _collection.Remove(item);
                }
            }

            public void Redo()
            {
                foreach (ImageItem item in _items)
                {
                    _collection.Add(item);
                }
            }
        }

        private sealed class RemoveImagesAction : IUndoableAction
        {
            private readonly ObservableCollection<ImageItem> _collection;
            private readonly List<RemovedItem> _items;

            public RemoveImagesAction(ObservableCollection<ImageItem> collection, List<RemovedItem> items)
            {
                _collection = collection;
                _items = items;
            }

            public void Undo()
            {
                foreach (RemovedItem item in _items.OrderBy(entry => entry.Index))
                {
                    _collection.Insert(item.Index, item.Item);
                }
            }

            public void Redo()
            {
                foreach (RemovedItem item in _items.OrderByDescending(entry => entry.Index))
                {
                    _collection.Remove(item.Item);
                }
            }

            public readonly struct RemovedItem
            {
                public RemovedItem(int index, ImageItem item)
                {
                    Index = index;
                    Item = item;
                }

                public int Index { get; }
                public ImageItem Item { get; }
            }
        }

        private sealed class ImageItem
        {
            public ImageItem(string path)
            {
                Path = path;
                Preview = LoadPreview(path);
            }

            public string Path { get; }
            public ImageSource Preview { get; }

            private static ImageSource LoadPreview(string path)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path, UriKind.Absolute);
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
    }
}