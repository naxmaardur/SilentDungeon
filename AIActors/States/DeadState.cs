using Godot;
using System;

namespace AIStates
{
	public partial class DeadState : State<ActorControler>
	{
        public override void OnEnter()
        {
            ctx.tree.Set("parameters/conditions/Dead", true);
            Node scene = ctx.LootScene.Instantiate();
            Node3D node3D = scene as Node3D;
            if (node3D != null)
            {
                ctx.GetParent().AddChild(node3D);
                node3D.GlobalPosition = ctx.visuals.GlobalPosition;
                ctx.visuals.Reparent(node3D);
                ContainerInteractable containerInteractable = node3D as ContainerInteractable;
                if (containerInteractable != null)
                {
                    int rolls = ctx.randomNumberGenerator.RandiRange(ctx.minLootRolls, ctx.maxLootRolls);
                    for(int i = 0; i < rolls; i++)
                    {
                        containerInteractable.container.AddItem(ctx.lootTable.RollDrop(),ctx.
                            randomNumberGenerator.RandiRange(0,35));
                    }
                    containerInteractable.AddMesh(ctx.visuals);
                }
                ctx.QueueFree();
            }
        }


        public override void OnUpdate(double delta)
        {
            ctx.Velocity = Vector3.Zero;
        }
    }
}
