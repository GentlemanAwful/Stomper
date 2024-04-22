using System;
using System.Collections.Generic;
using System.Linq;
using Stomper.Engine;
using Stomper.Scripts.Components;

namespace Stomper.Scripts
{
    public class Walk : IECSSystem
    {
        public SystemType Type => SystemType.LOGIC;
        public Type[] RequiredComponents { get; set; } = new Type[] { typeof(Position), typeof(WalkSpeed) };
        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config)
        {

        }

        public void Dispose()
        {

        }

        /*
		public void Execute(List<IECSComponent> components, List<IGameEvent> gameEvents)
        {
            //List<IGameEvent> allInputs = gameEvents.FindAll((gameEvent) => gameEvent.GetType() == typeof(IGameEvent<Input.Action>)); // Take actions from GameEvents
            //float deltaTime = ((IGameEvent<GameTime>)(gameEvents.Find((e) => e.GetType() == typeof(IGameEvent<GameTime>)))).payload.ElapsedGameTime.Milliseconds;
            float deltaTime = ((FNAGame.DeltaTimeEvent)gameEvents.Find(ge => ge is FNAGame.DeltaTimeEvent)).gameTime.ElapsedGameTime.Milliseconds;

            var groupedComponentsByEntityID = components.GroupBy(c => c.entityID);
            foreach (Type rqt in RequiredComponents)
            {
                groupedComponentsByEntityID = groupedComponentsByEntityID.Where(g => g.Any(c => c.GetType() == rqt));
            }

            foreach (Input.InputEvent input in gameEvents.FindAll(ge => ge is Input.InputEvent))
            {
                switch(input.action)
                {
                    case Input.Action.MOVE_RIGHT:
                        foreach (var entityComponents in groupedComponentsByEntityID)
                        {
                            Position position = entityComponents.First(c => c is Position) as Position;
                            WalkSpeed walkSpeed = entityComponents.First(c => c is WalkSpeed) as WalkSpeed;
                            position.position.X += walkSpeed.Value * deltaTime;
                        }
                        break;

                    case Input.Action.MOVE_LEFT:
                        foreach (var entityComponents in groupedComponentsByEntityID)
                        {
                            Position position = entityComponents.First(c => c is Position) as Position;
                            WalkSpeed walkSpeed = entityComponents.First(c => c is WalkSpeed) as WalkSpeed;
                            position.position.X -= walkSpeed.Value * deltaTime;
                        }
                        break;
                }
            }
        }
        */

        /*
        public void Execute(IEnumerable<IGrouping<int, IECSComponent>> entities, List<IGameEvent> gameEvents)
        {
            
            foreach (Input.InputEvent input in gameEvents.FindAll(ge => ge is Input.InputEvent))
            {
                switch (input.action)
                {
                    case Input.Action.MOVE_RIGHT:
                        sign = 1;
                        foreach (var entityGroup in entities)
                        {
                            ExecuteSingle(entityGroup.ToList(), gameEvents);
                        }
                        break;

                    case Input.Action.MOVE_LEFT:
                        sign = -1;
                        foreach (var entityGroup in entities)
                        {
                            ExecuteSingle(entityGroup.ToList(), gameEvents);
                        }
                        break;
                }
            }
        }

        public void ExecuteSingle(List<IECSComponent> components, List<IGameEvent> gameEvents)
        {
            float deltaTime = ((FNAGame.DeltaTimeEvent)gameEvents.Find(ge => ge is FNAGame.DeltaTimeEvent)).gameTime.ElapsedGameTime.Milliseconds;

            Position position = components.First(c => c is Position) as Position;
            WalkSpeed walkSpeed = components.First(c => c is WalkSpeed) as WalkSpeed;
            position.position.X += walkSpeed.Value * deltaTime * sign;
        }
        */
        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents)
        {
            int sign = 0;
            float deltaTime = ((FNAGame.DeltaTimeEvent)gameEvents.Find(ge => ge is FNAGame.DeltaTimeEvent)).gameTime.ElapsedGameTime.Milliseconds;
            foreach (Input.InputEvent input in gameEvents.FindAll(ge => ge is Input.InputEvent))
            {
                switch (input.action)
                {
                    case Input.Action.MOVE_RIGHT:
                        sign = 1;
                        break;

                    case Input.Action.MOVE_LEFT:
                        sign = -1;
                        break;
                    default:
                        sign = 0;
                        break;
                }

                foreach(Entity entity in entities)
                {
                    Position position = entity.GetComponent<Position>();
                    WalkSpeed walkSpeed = entity.GetComponent<WalkSpeed>();
                    position.position.X += walkSpeed.Value * deltaTime * sign;

                    entity.UpdateComponent(position);
                }                
            }

            return (entities, new List<IGameEvent>());
        }
    }
}
