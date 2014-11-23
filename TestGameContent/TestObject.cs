using Cog;
using Cog.Modules.Animation;
using Cog.Modules.Content;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using Cog.Modules.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    [Resource(ContainerName = "main", Filename = "valid.png", Key = "texture")]
    [Resource(ContainerName = "main", Filename = "test_sound.wav", Key = "Test Sound")]
    public class TestObject : GameObject, IAnimated
    {
        public SpriteComponent Sprite;
        public MovementComponent Movement;
        public Synchronized<int> SynchronizedValue = new Synchronized<int>(1234);
        public SynchronizedDR<GameObject> SynchronizedTarget;
        public IAnimationComponent AnimationComponent { get; set; }

        public TestObject()
        {
            Movement = MovementComponent.RegisterOn(this);
            Movement.Left = new KeyCapture(this, Keyboard.Key.A, 0, CaptureRelayMode.NoRelay);
            Movement.Right = new KeyCapture(this, Keyboard.Key.D, 0, CaptureRelayMode.NoRelay);
            Movement.Up = new KeyCapture(this, Keyboard.Key.W, 0, CaptureRelayMode.NoRelay);
            Movement.Down = new KeyCapture(this, Keyboard.Key.S, 0, CaptureRelayMode.NoRelay);

            AnimationComponent<TestObject>.RegisterOn(this);
            AnimationComponent.Animation = new AnimationInstance(new TestAnimation());
            Sprite = SpriteComponent.RegisterOn(this, Resources.GetTexture("texture"));

            RegisterEvent<UpdateEvent>(0, e => LocalRotation += Angle.FromRadian(e.DeltaTime));

            if (Engine.IsServer)
            {
                SynchronizedValue.Value = 1010;
                RegisterEvent<UpdateEvent>(0, ev => { if (Engine.RandomFloat() >= 0.95f) SynchronizedValue.Value++; });
                SynchronizedTarget.Value = this;
            }
            else
                RegisterEvent<KeyDownEvent>(Keyboard.Key.Space, 1000, ev => { var sound = Resources.GetSound("Test Sound").Play(); Console.WriteLine(SynchronizedValue.Value); });

            Console.WriteLine(ObjectName + ":");

            Console.WriteLine("SynchronizedValue is " + SynchronizedValue.Value);

            if (SynchronizedTarget.Value != null)
                Console.WriteLine("SynchronizedTarget is " + SynchronizedTarget.Value.ObjectName);
            else
                Console.WriteLine("SynchronizedTarget is null");

            Console.WriteLine("---");
        }
    }
}
