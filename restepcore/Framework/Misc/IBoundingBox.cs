using OpenTK;

namespace restep.Framework.Misc
{
    internal interface IBoundingBox
    {
        float GetLeft();
        float GetRight();
        float GetTop();
        float GetBottom();

        Vector2 GetCenter();
        Vector2 GetHalfDims();
    }
}
