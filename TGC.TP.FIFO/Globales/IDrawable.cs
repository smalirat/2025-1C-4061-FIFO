namespace TGC.TP.FIFO.Globales;

public interface IDrawable
{
    void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition);
}
