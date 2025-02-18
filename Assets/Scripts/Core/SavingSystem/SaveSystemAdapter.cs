using Common.DataManagement;

namespace Core.SavingSystem
{
    public class SaveSystemAdapter<T> : ISaveSystem<T> where T : new()
    {
        public T Load() => SaveSystem<T>.Instance.Load();
        public void Save(T data) => SaveSystem<T>.Instance.Save(data);
    }
}