using UnityEngine;

// Fizik malzemeleri (sürtünme / sekme). Çalışma anında bir kez oluşturulur.
public static class PhysMats
{
    static PhysicsMaterial2D bird, wood, stone, ice, fox, ground;

    static PhysicsMaterial2D Make(ref PhysicsMaterial2D m, string name, float friction, float bounce)
    {
        if (m == null) m = new PhysicsMaterial2D(name) { friction = friction, bounciness = bounce };
        return m;
    }

    public static PhysicsMaterial2D Bird => Make(ref bird, "Bird", 0.5f, 0.25f);
    public static PhysicsMaterial2D Wood => Make(ref wood, "Wood", 0.7f, 0.05f);
    public static PhysicsMaterial2D Stone => Make(ref stone, "Stone", 0.8f, 0.02f);
    public static PhysicsMaterial2D Ice => Make(ref ice, "Ice", 0.15f, 0.1f);
    public static PhysicsMaterial2D Fox => Make(ref fox, "Fox", 0.6f, 0.2f);
    public static PhysicsMaterial2D Ground => Make(ref ground, "Ground", 0.9f, 0f);

    public static PhysicsMaterial2D For(BlockMaterial m)
    {
        switch (m)
        {
            case BlockMaterial.Stone: return Stone;
            case BlockMaterial.Ice: return Ice;
            case BlockMaterial.Fox: return Fox;
            default: return Wood;
        }
    }
}
