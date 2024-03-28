using System;

namespace MP.Singleton
{
    public abstract class SingletonBase<T>
        where T : SingletonBase<T>
    {
        public static T instance
        {
            get
            {
                if (s_instance == null)
                {
                    //ConstructorInfo constructorInfo = typeof(T).GetConstructor(new Type[] { });
                    //constructorInfo.Invoke(null);

                    s_instance = (T)Activator.CreateInstance(typeof(T));
                }

                return s_instance;
            }
        }

        private static T s_instance;
    }
}
