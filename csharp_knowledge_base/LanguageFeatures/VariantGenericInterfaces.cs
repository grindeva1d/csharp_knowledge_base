using System;

namespace LanguageFeatures
{
    public class CovariantInterface : ICovariantInterface<Derived>
    {
        public Derived DoSomething()
        {
            throw new NotImplementedException();
        }

        public Derived DoSomething(Action<Derived> action)
        {
            throw new NotImplementedException();
        }
    }

    // Covariance allows interface methods to have more derived return types than that defined by the generic type parameters.
    public interface ICovariantInterface<out T>
    {
        T DoSomething();
        T DoSomething(Action<T> action);
    }

    // Contravariance allows interface methods to have argument types that are less derived than that specified by the generic parameters.
    public interface IContravarianceInterface<in T>
    {
        void DoSomething(T t);
    }

    public class Base
    {
        
    }

    public class Derived : Base
    {
        
    }
}