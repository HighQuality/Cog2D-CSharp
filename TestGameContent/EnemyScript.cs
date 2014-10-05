using Cog;
using Cog.Modules.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    class EnemyScript : ObjectComponent
    {
        [ClientEdit(CanEdit = true, RequireOwner = true)]
        public Synchronized<int> TargetId;
        private KeyCapture space;

        public EnemyScript()
        {
            space = CaptureKey(Keyboard.Key.Space, 0, CaptureRelayMode.NoRelay);
        }

        public override void PhysicsUpdate(float deltaTime)
        {
            if (space.IsDown)
                TargetId.Value++;
            Console.WriteLine(TargetId.Value);

            base.Update(deltaTime);
        }
    }
}
