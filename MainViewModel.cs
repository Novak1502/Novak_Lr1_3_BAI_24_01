using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Threading;

namespace NovakLR_4_1_BAI_24_01
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ArraySorter _sorter;
        private readonly SynchronizationContext _uiContext;
        private int[]? _originalArray;

        [ObservableProperty]
        private int _arraySize = 1000;

        [ObservableProperty]
        private string? _originalArrayString;

        [ObservableProperty]
        private string? _bubbleSortResult;

        [ObservableProperty]
        private string? _quickSortResult;

        [ObservableProperty]
        private string? _insertionSortResult;

        [ObservableProperty]
        private string _totalComparisons = "Общее число сравнений: 0";

        [ObservableProperty]
        private bool _canGenerate = true;

        public MainViewModel()
        {
            _sorter = new ArraySorter();
            _uiContext = SynchronizationContext.Current ?? new SynchronizationContext();

            _sorter.BubbleSortCompleted += OnBubbleSortCompleted;
            _sorter.QuickSortCompleted += OnQuickSortCompleted;
            _sorter.InsertionSortCompleted += OnInsertionSortCompleted;
        }

        [RelayCommand(CanExecute = nameof(CanGenerateArray))]
        private void GenerateArray()
        {
            _originalArray = _sorter.GenerateRandomArray(ArraySize);

            OriginalArrayString = "Исходный массив: "
                + string.Join(", ", _originalArray.Take(20))
                + (ArraySize > 20 ? "..." : "");

            BubbleSortResult = QuickSortResult = InsertionSortResult = null;
            TotalComparisons = "Общее число сравнений: 0";

            BubbleSortCommand.NotifyCanExecuteChanged();
            QuickSortCommand.NotifyCanExecuteChanged();
            InsertionSortCommand.NotifyCanExecuteChanged();
        }

        private bool CanGenerateArray() => CanGenerate;

        private bool CanSortBubble() =>
            _originalArray != null && BubbleSortResult != "Сортируется...";

        [RelayCommand(CanExecute = nameof(CanSortBubble))]
        private void BubbleSort()
        {
            BubbleSortResult = "Сортируется...";
            BubbleSortCommand.NotifyCanExecuteChanged();

            Thread thread = new Thread(() =>
            {
                Thread.Sleep(3000); // Временная задержка для проверки
                _sorter.BubbleSort(_originalArray!);
            });
            thread.Start();
        }

        private bool CanSortQuick() =>
            _originalArray != null && QuickSortResult != "Сортируется...";

        [RelayCommand(CanExecute = nameof(CanSortQuick))]
        private void QuickSort()
        {
            QuickSortResult = "Сортируется...";
            QuickSortCommand.NotifyCanExecuteChanged();

            Thread thread = new Thread(() => _sorter.QuickSort(_originalArray!));
            thread.Start();
        }

        private bool CanSortInsertion() =>
            _originalArray != null && InsertionSortResult != "Сортируется...";

        [RelayCommand(CanExecute = nameof(CanSortInsertion))]
        private void InsertionSort()
        {
            InsertionSortResult = "Сортируется...";
            InsertionSortCommand.NotifyCanExecuteChanged();

            Thread thread = new Thread(() => _sorter.InsertionSort(_originalArray!));
            thread.Start();
        }

        private void OnBubbleSortCompleted(
            int[] sortedArray, long comparisons, double elapsedMs)
        {
            _uiContext.Post(_ =>
            {
                BubbleSortResult =
                    $"Пузырьковая: {FormatArray(sortedArray)}, " +
                    $"время: {elapsedMs:F2} мс, сравнений: {comparisons}";
                UpdateTotalComparisons();
                BubbleSortCommand.NotifyCanExecuteChanged();
            }, null);
        }

        private void OnQuickSortCompleted(
            int[] sortedArray, long comparisons, double elapsedMs)
        {
            _uiContext.Post(_ =>
            {
                QuickSortResult =
                    $"Быстрая: {FormatArray(sortedArray)}, " +
                    $"время: {elapsedMs:F2} мс, сравнений: {comparisons}";
                UpdateTotalComparisons();
                QuickSortCommand.NotifyCanExecuteChanged();
            }, null);
        }

        private void OnInsertionSortCompleted(
            int[] sortedArray, long comparisons, double elapsedMs)
        {
            _uiContext.Post(_ =>
            {
                InsertionSortResult =
                    $"Вставками: {FormatArray(sortedArray)}, " +
                    $"время: {elapsedMs:F2} мс, сравнений: {comparisons}";
                UpdateTotalComparisons();
                InsertionSortCommand.NotifyCanExecuteChanged();
            }, null);
        }

        private void UpdateTotalComparisons()
        {
            TotalComparisons = $"Общее число сравнений: {_sorter.TotalComparisons}";
        }

        private string FormatArray(int[] arr)
        {
            if (arr.Length <= 10)
                return string.Join(", ", arr);

            return string.Join(", ", arr.Take(5)) + "...";
        }
    }
}