using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Utilidades;

public static class DrawUtilities
{
    // Dibuja un modelo custom (NO primitivas)
    public static void DrawCustomModel(
        Model model,
        Effect effect,
        Matrix view,
        Matrix projection,
        Matrix? translation = null,
        Matrix? scale = null,
        Matrix? rotation = null,
        Color? color = null,
        Matrix? globalOffset = null,
        Matrix? globalRotation = null)
    {
        // Valores por defecto
        translation ??= Matrix.Identity;
        scale ??= Matrix.CreateScale(1f);
        rotation ??= Matrix.Identity;
        globalOffset ??= Matrix.Identity;
        globalRotation ??= Matrix.Identity;
        color ??= Color.Black;

        // Transformaciones del modelo base
        var baseTransforms = new Matrix[model.Bones.Count];
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

            mesh.Draw();
        }
    }
}
