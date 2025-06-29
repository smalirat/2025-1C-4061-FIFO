﻿using BepuPhysics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Luz;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos.Boxes;

public class DynamicBox : IGameObject
{
    private const float Friction = 1f;
    private readonly XnaVector3 initialPosition;
    private readonly XnaQuaternion initialRotation;
    private readonly float sideLength;
    private readonly float mass;
    private readonly BoxPrimitive model;
    private XnaMatrix world;
    private BodyHandle boundingVolume;

    public DynamicBox(XnaVector3 initialPosition, XnaQuaternion? initialRotation = null, float sideLength = 5f)
    {
        Debug.WriteLine(initialPosition);

        this.initialPosition = initialPosition;
        this.initialRotation = initialRotation ?? XnaQuaternion.Identity;
        this.sideLength = sideLength;

        mass = MathF.Pow(sideLength / 5f, 3f);
        model = ModelManager.CreateBox(sideLength, sideLength, sideLength);
        world = XnaMatrix.CreateFromQuaternion(this.initialRotation) * XnaMatrix.CreateTranslation(initialPosition);

        CreateBoundingVolume();
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var effect = EffectManager.BlinnPhongShader;
        var material = MaterialPresets.Madera;

        effect.CurrentTechnique = effect.Techniques["Default"]; // Opciones: "Default", "Gouraud", "NormalMapping"

        effect.Parameters["WorldViewProjection"]?.SetValue(world * view * projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["InverseTransposeWorld"]?.SetValue(XnaMatrix.Transpose(XnaMatrix.Invert(world)));

        effect.Parameters["ambientColor"]?.SetValue(material.AmbientColor);
        effect.Parameters["diffuseColor"]?.SetValue(material.DiffuseColor);
        effect.Parameters["specularColor"]?.SetValue(material.SpecularColor);
        effect.Parameters["KAmbient"]?.SetValue(material.KAmbient);
        effect.Parameters["KDiffuse"]?.SetValue(material.KDiffuse);
        effect.Parameters["KSpecular"]?.SetValue(material.KSpecular);
        effect.Parameters["shininess"]?.SetValue(material.Shininess);

        effect.Parameters["lightPosition"]?.SetValue(lightPosition);
        effect.Parameters["eyePosition"]?.SetValue(eyePosition);
        effect.Parameters["Tiling"]?.SetValue(new XnaVector2(1.0f, 1.0f));

        effect.Parameters["baseTexture"]?.SetValue(TextureManager.WoodBox1Texture);

        // Solo establecer la textura de normales si estamos usando NormalMapping
        // if (effect.CurrentTechnique.Name == "NormalMapping")
        // {
        //     effect.Parameters["NormalTexture"]?.SetValue(textureManager.WoodBox1Texture); //por ahora no tiene textura normal
        // }

        model.Draw(effect);
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        world = XnaMatrix.CreateFromQuaternion(PhysicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(PhysicsManager.GetPosition(boundingVolume));
    }

    public void NotifyCollition(ICollisionable playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        AudioManager.PlayWoodBoxHitSound(contactSpeed);
    }

    public void Reset()
    {
        PhysicsManager.RemoveBoundingVolume(boundingVolume);
        CreateBoundingVolume();
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return false;
    }

    private void CreateBoundingVolume()
    {
        boundingVolume = PhysicsManager.AddDynamicBox(sideLength, sideLength, sideLength, mass, Friction, initialPosition, initialRotation, this);
    }
}