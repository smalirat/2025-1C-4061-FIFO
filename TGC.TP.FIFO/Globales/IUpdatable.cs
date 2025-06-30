using Microsoft.Xna.Framework.Input;
using TGC.TP.FIFO.Cameras;

namespace TGC.TP.FIFO.Globales;

public interface IUpdatable
{
    void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera);
}