namespace Le0derToolkit.Toolbox
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object _lock = new object();

        // 获取单例实例
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
                return _instance;
            }
        }

        // 保护构造函数，防止外部实例化
        protected Singleton() { }
    }
}