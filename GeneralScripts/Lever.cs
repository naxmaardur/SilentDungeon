using Godot;
using System;

public partial class Lever : Node3D, Iinteractable
{
    private AnimationTree tree;
    MeshInstance3D[] meshInstances;

    [Export]
    private Shader shader;
    private bool opened;

    [Export]
    private AnimationTree door;

    [Export]
    private Node[] ToEnable;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        tree = this.GetChildByType<AnimationTree>();
        meshInstances = this.GetChild(0).GetAllChildrenByType<MeshInstance3D>();
        foreach (MeshInstance3D child in meshInstances)
        {
            ShaderMaterial material = new ShaderMaterial();
            material.Shader = shader;

            child.MaterialOverlay = material;
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void AddMesh(Node3D node)
    {
        meshInstances = node.GetAllChildrenByType<MeshInstance3D>();
        foreach (MeshInstance3D child in meshInstances)
        {
            ShaderMaterial material = new ShaderMaterial();
            material.Shader = shader;

            child.MaterialOverlay = material;
        }
    }
    public void EnableGlow()
    {
        if (opened) { return; }
        foreach (MeshInstance3D meshInstance in meshInstances)
        {
            ShaderMaterial material = meshInstance.MaterialOverlay as ShaderMaterial;
            material.SetShaderParameter("strenght", 0.2f);
        }
    }

    public void DisableGlow()
    {

        foreach (MeshInstance3D meshInstance in meshInstances)
        {
            ShaderMaterial material = meshInstance.MaterialOverlay as ShaderMaterial;
            material.SetShaderParameter("strenght", 0);
        }
    }


    public bool Interact(PlayerController player)
    {
        if(opened) { return true; }
        opened = true;
        tree.Set("parameters/conditions/Open", true);
        door.Set("parameters/conditions/Open", true);

        foreach(Node n in ToEnable)
        {
            n.ProcessMode = ProcessModeEnum.Inherit;
        }
        return true;
    }
}
