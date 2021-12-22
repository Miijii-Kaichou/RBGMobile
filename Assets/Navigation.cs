public class Navigation<T>
{
    T[] array;
    public int Size { get; private set; } = 0;

    public Navigation()
    {
        array = new T[Size + 1];
    }

    public T Stretch(T data)
    {

        Size += 1;
        array[Size - 1] = data;
        Resize(Size);
        return array[Size - 1];
    }

    public T Condense(int distance)
    {
        Size -= distance;
        T prev = array[Size];
        Resize(Size);
        return prev;
    }

    int Resize(int newSize)
    {
        try
        {
            T[] keeper = array;

            array = new T[newSize + 1];

            if (keeper.Length > 0)
            {
                for (int i = 0; i < newSize; i++)
                {
                    array[i] = keeper[i];
                }
            }

            return newSize;
        }
        catch
        {
            return 0;
        }
    }
}
