namespace ZPO.Core.Colors
{
    public interface IColor
    {
        int ToInt();
        IColor FromInt(int value);       
    }
}