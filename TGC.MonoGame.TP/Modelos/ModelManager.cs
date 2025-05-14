using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Modelos.Primitivas;

namespace TGC.MonoGame.TP.Modelos;

public class ModelManager
{
    public const string ContentFolderModels = "Models/";

    public Model BoxModel { get; private set; }
    public Model CurveModel { get; private set; }
    public Model SlantLongAModel { get; private set; }
    public Model SlantLongCModel { get; private set; }
    public Model SlantLongDModel { get; private set; }
    public Model BumpAModel { get; private set; }
    public Model BumpDModel { get; private set; }
    public Model BumpSolidBModel { get; private set; }
    public Model RampLongAModel { get; private set; }
    public Model RampLongBModel { get; private set; }
    public Model RampLongCModel { get; private set; }
    public Model RampLongDModel { get; private set; }
    public Model CurveLargeModel { get; private set; }
    public Model StraightModel { get; private set; }
    public Model SplitModel { get; private set; }
    public Model SplitLeftModel { get; private set; }
    public Model SplitRightModel { get; private set; }
    public Model SplitDoubleModel { get; private set; }
    public Model SplitDoubleSidesModel { get; private set; }
    public Model TunnelModel { get; private set; }
    public Model HelixLeftModel { get; private set; }
    public Model HelixRightModel { get; private set; }
    public Model HelixHalfLeftModel { get; private set; }
    public Model HelixHalfRightModel { get; private set; }
    public Model HelixLargeHalfLeftModel { get; private set; }
    public Model HelixLargeHalfRightModel { get; private set; }
    public Model HelixLargeLeftModel { get; private set; }
    public Model HelixLargeRightModel { get; private set; }
    public Model HelixLargeQuarterLeftModel { get; private set; }
    public Model HelixLargeQuarterRightModel { get; private set; }
    public Model WaveAModel { get; private set; }
    public Model WaveBModel { get; private set; }
    public Model WaveCModel { get; private set; }
    public Model FunnelModel { get; private set; }
    public Model FunnelLongModel { get; private set; }
    public Model WallHalfModel { get; private set; }
    public Model NormalTreeModel { get; private set; }
    public Model TallTreeModel { get; private set; }
    public Model EndSquareModel { get; private set; }
    public Model SupportModel { get; private set; }
    public Model BannerHighModel { get; private set; }
    public Model BendModel { get; private set; }
    public Model Pelota { get; set; }

    public void Load(ContentManager content)
    {
        BoxModel = LoadModel(content, "skybox/cube");
        CurveModel = LoadModel(content, "curves/curve");
        SlantLongAModel = LoadModel(content, "slants/slant_long_A");
        SlantLongCModel = LoadModel(content, "slants/slant_long_C");
        SlantLongDModel = LoadModel(content, "slants/slan_long_D");
        BumpAModel = LoadModel(content, "bump/bump_A");
        BumpDModel = LoadModel(content, "bump/bump_D");
        BumpSolidBModel = LoadModel(content, "bump/bump_solid_B");
        RampLongAModel = LoadModel(content, "ramps/ramp_long_A");
        RampLongBModel = LoadModel(content, "ramps/ramp_long_B");
        RampLongCModel = LoadModel(content, "ramps/ramp_long_C");
        RampLongDModel = LoadModel(content, "ramps/ramp_long_D");
        CurveLargeModel = LoadModel(content, "curves/curve_large");
        StraightModel = LoadModel(content, "straights/straight");
        SplitModel = LoadModel(content, "splits/split");
        SplitLeftModel = LoadModel(content, "splits/split_left");
        SplitRightModel = LoadModel(content, "splits/split_right");
        SplitDoubleModel = LoadModel(content, "splits/split_double");
        SplitDoubleSidesModel = LoadModel(content, "splits/split_double_sides");
        TunnelModel = LoadModel(content, "extras/tunnel");
        HelixLeftModel = LoadModel(content, "helixs/helix_left");
        HelixRightModel = LoadModel(content, "helixs/helix_right");
        HelixHalfLeftModel = LoadModel(content, "helixs/helix_half_left");
        HelixHalfRightModel = LoadModel(content, "helixs/helix_half_right");
        HelixLargeHalfLeftModel = LoadModel(content, "helixs/helix_large_half_left");
        HelixLargeHalfRightModel = LoadModel(content, "helixs/helix_large_half_right");
        HelixLargeLeftModel = LoadModel(content, "helixs/helix_large_left");
        HelixLargeRightModel = LoadModel(content, "helixs/helix_large_right");
        HelixLargeQuarterLeftModel = LoadModel(content, "helixs/helix_large_quarter_left");
        HelixLargeQuarterRightModel = LoadModel(content, "helixs/helix_large_quarter_right");
        WaveAModel = LoadModel(content, "waves/wave_A");
        WaveBModel = LoadModel(content, "waves/wave_B");
        WaveCModel = LoadModel(content, "waves/wave_C");
        FunnelModel = LoadModel(content, "funnels/funnel");
        FunnelLongModel = LoadModel(content, "funnels/funnel_long");
        WallHalfModel = LoadModel(content, "extras/wallHalf");
        NormalTreeModel = LoadModel(content, "tree/tree");
        TallTreeModel = LoadModel(content, "tree/tree_tall");
        EndSquareModel = LoadModel(content, "endHoles/end_square");
        SupportModel = LoadModel(content, "supports/support_base");
        BannerHighModel = LoadModel(content, "banners/banner_high");
        BendModel = LoadModel(content, "bend/bend_medium");
        Pelota = LoadModel(content, "marble/marble_high");
    }

    private Model LoadModel(ContentManager content, string path)
    {
        return content.Load<Model>(ContentFolderModels + path);
    }

    public SpherePrimitive CreateSphere(GraphicsDevice graphicsDevice, float diameter)
    {
        return new SpherePrimitive(graphicsDevice, diameter);
    }

    public QuadPrimitive CreateQuad(GraphicsDevice graphicsDevice)
    {
        return new QuadPrimitive(graphicsDevice);
    }
}
