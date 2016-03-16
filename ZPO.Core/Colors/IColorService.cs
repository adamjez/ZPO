namespace ZPO.Core.Colors
{
    public interface IColorService<T> where T : IColor
    {
        T Add(T value1, T value2);
        int Difference(T value1, T value2);
    }
}