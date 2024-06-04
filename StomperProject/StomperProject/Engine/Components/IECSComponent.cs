using System;

namespace Stomper.Engine
{
	public interface IECSComponent
	{
		int entityID { get; }
        // bool active { get; } TODO for temporarily disabling components
    }
}
