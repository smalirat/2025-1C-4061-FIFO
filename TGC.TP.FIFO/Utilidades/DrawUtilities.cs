using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Utilidades;

public static class DrawUtilities
{
    // Dibuja un modelo custom (NO primitivas)
    public static void DrawCustomModel(
        Model model,
        Effect effect,
        XnaMatrix view,
        XnaMatrix projection,
        XnaMatrix? translation = null,
        XnaMatrix? scale = null,
        XnaMatrix? rotation = null,
        Color? color = null,
        XnaMatrix? globalOffset = null,
        XnaMatrix? globalRotation = null,
        Texture2D texture = null,
        float UVScaler = 1f)
    {
        // Valores por defecto
        translation ??= XnaMatrix.Identity;
        scale ??= XnaMatrix.CreateScale(1f);
        rotation ??= XnaMatrix.Identity;
        globalOffset ??= XnaMatrix.Identity;
        globalRotation ??= XnaMatrix.Identity;
        color ??= Color.Black;

        // Transformaciones del modelo base
        var baseTransforms = new XnaMatrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(baseTransforms);

        // Transformación local (modelo)
        // Esto se lee: Escala -> Rotacion -> Traslacion
        var localTransform = scale.Value * rotation.Value * translation.Value;

        // Transformacion global (escena)
        // Esto se lee: Rotacion -> Traslacion
        var worldTransform = localTransform * globalRotation.Value * globalOffset.Value;

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = effect;
            }

            var meshTransform = baseTransforms[mesh.ParentBone.Index];

            effect.Parameters["View"]?.SetValue(view);
            effect.Parameters["Projection"]?.SetValue(projection);
            effect.Parameters["World"]?.SetValue(meshTransform * worldTransform);
            effect.Parameters["DiffuseColor"]?.SetValue(color.Value.ToVector3());
            effect.Parameters["ModelTexture"]?.SetValue(texture);
            effect.Parameters["UVScale"]?.SetValue(UVScaler);

            mesh.Draw();
        }
    }
}
