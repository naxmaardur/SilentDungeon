using Godot;
using System;

public partial class ContainerInteractable : Node3D, Iinteractable
{
    MeshInstance3D[] meshInstances;

    public ItemContainer container;

    [Export]
    private Shader shader;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (container == null)
        {
            container = new ItemContainer(false);
        }
        meshInstances = this.GetAllChildrenByType<MeshInstance3D>();
        foreach (MeshInstance3D child in meshInstances)
        {
            ShaderMaterial material = new ShaderMaterial();
            material.Shader = shader;

            child.MaterialOverlay = material;
        }
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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void EnableGlow()
    {
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
        UIItemContainer UI = player.uIContainer;
        UI.Visible = true;
        UI.ProcessMode = ProcessModeEnum.Inherit;
        UI.OpenContainer(container);
        player.OpenInventory();
        return true;
    }
}
