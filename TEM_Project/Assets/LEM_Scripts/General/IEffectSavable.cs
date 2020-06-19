#if UNITY_EDITOR
namespace LEM_Effects
{
    public interface IEffectSavable
    {
        void SetUp();

        void UnPack();

    }

    public interface IEffectSavable<T1>
    {
        void SetUp(T1 t1);

        void UnPack(out T1 t1);

    }

    public interface IEffectSavable<T1, T2>
    {
        void SetUp(T1 t1, T2 t2);

        void UnPack(out T1 t1, out T2 t2);

    }

    public interface IEffectSavable<T1, T2, T3>
    {
        void SetUp(T1 t1, T2 t2, T3 t3);

        void UnPack(out T1 t1, out T2 t2, out T3 t3);

    }

    public interface IEffectSavable<T1, T2, T3, T4>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4);

    }

    public interface IEffectSavable<T1, T2, T3, T4, T5>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5);

    }

    public interface IEffectSavable<T1, T2, T3, T4, T5, T6>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6);

    }

    public interface IEffectSavable<T1, T2, T3, T4, T5, T6, T7>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7);

    }

    public interface IEffectSavable<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8);

    }

} 
#endif