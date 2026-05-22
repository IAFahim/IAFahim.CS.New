        private static void QuickSort(PointWithIdx* arr, int left, int right)
        {
            if (left < right)
            {
                int pivot = Partition(arr, left, right);
                QuickSort(arr, left, pivot - 1);
                QuickSort(arr, pivot + 1, right);
            }
        }
        private static int Partition(PointWithIdx* arr, int left, int right)
        {
            ulong pivotValue = arr[right].Morton;
            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                if (arr[j].Morton <= pivotValue)
                {
                    i++;
                    PointWithIdx temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }
            PointWithIdx temp1 = arr[i + 1];
            arr[i + 1] = arr[right];
            arr[right] = temp1;
            return i + 1;
        }
