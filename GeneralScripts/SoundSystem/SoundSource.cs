using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class SoundSource : Node3D
{
	private Area3D area { get; set; }
	private AudioStreamPlayer3D StreamPlayer3D { get; set; }
	private GpuParticles3D particles { get; set; }
	private CollisionShape3D collisionShape3D { get; set; }

	private float falloffvalue = 0.25f;

	[Export]
	public float soundValue = 4;
	private RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();

	private PackedScene sceneCopy;

	private bool run;
    private int framesWaited = 0;
    [Export]
    public bool checkingSource = true;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		area = this.GetChildByType<Area3D>();
		StreamPlayer3D = this.GetChildByType<AudioStreamPlayer3D>();
		particles = this.GetChildByType<GpuParticles3D>();
		collisionShape3D = this.GetChildByType<CollisionShape3D>();
        sceneCopy = GD.Load<PackedScene>("res://Prefabs/sound_source.tscn");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }

    public override void _PhysicsProcess(double delta)
    {
		if (run)
		{
            if(framesWaited > 0)
            {
                run = false;
                runSound();
            }
            framesWaited++;
        }
    }
    public void PlaySound()
	{
		Node copy = sceneCopy.Instantiate();
		SoundSource soundSource = copy as SoundSource;
		if(soundSource == null)
		{
			copy.QueueFree();
			GD.PrintErr("SoundSource scene referance is not a Sound source");
			return;
		}
		GetNode("/root").AddChild(soundSource);

        soundSource.GlobalPosition = GlobalPosition;
        soundSource.SoundBehavior(StreamPlayer3D, collisionShape3D.Shape,soundValue, checkingSource);
	}

	public void SoundBehavior(AudioStreamPlayer3D stream, Shape3D shape,float soundValue,bool checking)
	{
		//Copy Variables -----
		if(StreamPlayer3D == null) { StreamPlayer3D = this.GetChildByType<AudioStreamPlayer3D>(); }
        if (area == null) { area = this.GetChildByType<Area3D>(); }
        if (particles == null) { particles = this.GetChildByType<GpuParticles3D>(); }
        this.soundValue = soundValue;
        StreamPlayer3D.Stream = stream.Stream;
        StreamPlayer3D.AttenuationModel = stream.AttenuationModel;
        StreamPlayer3D.VolumeDb = stream.VolumeDb;
        StreamPlayer3D.UnitSize =stream.UnitSize;
        StreamPlayer3D.MaxDb = stream.MaxDb;
        StreamPlayer3D.PitchScale = stream.PitchScale;
        StreamPlayer3D.MaxDistance = stream.MaxDistance;
        StreamPlayer3D.Finished += destorySelf;
        CollisionShape3D shape3D = area.GetChildByType<CollisionShape3D>();
		shape3D.Shape = shape;
        checkingSource = checking;
        //-------------------

        run = true;
    }

	private void StartRaycasting(Node3D target, uint collisionMask, ref List<Vector3> hitPositions)
	{
		if(this.RayCast3D(GlobalPosition, target.GlobalPosition, out var hit, collisionMask, false))
		{
			if(hit.collider == target)
			{
				return;
			}
			hitPositions.Add(hit.position);
			Godot.Collections.Array<Rid> exclude = new()
            {
                hit.rid
            };

			recurviseRaycasting(target, collisionMask, ref hitPositions,ref exclude);
        }
	}


	private void runSound()
	{
        StreamPlayer3D.Play();
        if (!checkingSource) { return; }
        particles.Emitting = true;
        Godot.Collections.Array<Node3D> nodes = area.GetOverlappingBodies();
        Node3D[] nodesArray = nodes.ToArray();
        foreach (Node3D node in nodesArray)
        {
            ISoundListner listner = node as ISoundListner;
            if (listner == null) { continue; }
            uint collisionMask = 0xffffffff;
            List<Vector3> hits = new List<Vector3>();
            StartRaycasting(node, collisionMask, ref hits);
            float currentSoundValue = soundValue;
            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    Vector3 start;

                    if (i == 0)
                    {
                        start = GlobalPosition;
                    }
                    else
                    {
                        start = hits[i - 1];
                    }
                    float distance = start.DistanceTo(hits[i]);
                    distance *= falloffvalue;
                    currentSoundValue -= distance;
                    if (currentSoundValue <= 0)
                    {
                        currentSoundValue = 0;
                        break;
                    }
                    currentSoundValue /= 2;
                }
            }
            else
            {
                float distance = GlobalPosition.DistanceTo(node.GlobalPosition);
                distance *= falloffvalue;
                currentSoundValue -= distance;
                if (currentSoundValue <= 0)
                {
                    currentSoundValue = 0;
                }
            }
            listner.AddSoundImpulse(currentSoundValue, GlobalPosition);
        }
    }

    private void recurviseRaycasting(Node3D target, uint collisionMask, ref List<Vector3> hitPositions, ref Godot.Collections.Array<Rid> exclude, int depth = 0)
    {
		if(depth > 5) { return; }
        if (this.RayCast3D(GlobalPosition, target.GlobalPosition, out var hit,exclude, collisionMask, false))
        {
            if (hit.collider == target)
            {
                return;
            }
            depth++;
            hitPositions.Add(hit.position);
			exclude.Add(hit.rid);

            recurviseRaycasting(target, collisionMask, ref hitPositions, ref exclude, depth);
        }
    }

	public void SetRandomPitch(float min, float max)
	{
        StreamPlayer3D.PitchScale = randomNumberGenerator.RandfRange(min, max);
    }

	private void destorySelf()
	{
		this.QueueFree();
	}
    public void SetAudio(AudioStream stream)
    {
        StreamPlayer3D.Stream = stream;
    }
}
