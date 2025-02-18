namespace Core.SavingSystem
{
    public interface ISaveSystem<T>
    {
        T Load();
        void Save(T data);
    }
}