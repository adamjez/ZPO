using ZPO.Core.Colors;

namespace ZPO.Core.Conditions
{
    public interface IColorCondition
    {
        /// <summary>
        /// Compares given color with colors given in constructor
        /// </summary>
        /// <param name="pixelColor">Color to compare with</param>
        /// <param name="neighborCount">Number of pixel neighbor, which was flagged</param>
        /// <param name="row">Ratio of current row to height of image in rows. Can be ignored.</param>
        /// <returns>Returns True if pixelColor meets given conditions</returns>
        bool Compare(IColor pixelColor, int neighborCount, double row = -1);
    }
}