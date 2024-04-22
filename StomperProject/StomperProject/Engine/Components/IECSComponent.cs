using System;

namespace Stomper.Engine
{
	public interface IECSComponent
	{
		int entityID { get; set; }
	}
}
