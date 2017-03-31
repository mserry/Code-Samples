public abstract class Singleton<T> where T : class, new()
{
    public static T Instance
    {
        get { return instance ?? (instance = new T()); }
    }

    protected static T instance;
}