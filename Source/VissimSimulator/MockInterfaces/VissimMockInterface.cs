using System.Collections.Generic;

namespace VissimSimulator.Mock
{
    public interface IVissim
    {
        IVissimNet Net { get; }

        void LoadNet(string path, bool runBackground);

        void Exit();
    }

    public class Vissim : IVissim
    {
        public IVissimNet Net { get { return null; } }

        public ISimulation Simulation;

        public void LoadNet(string path, bool runBackground) { }

        public void Exit() { }
    }

    public interface ISimulation
    {
        void RunSingleStep();
    }

    public class Simulation : ISimulation
    {
        public void RunSingleStep()
        {
 
        }
    }

    public interface IVissimNet
    {
        IList<IVehicle> Vehicles { get; }
    }

    public interface IVehicle
    {
        string Id { get; }

        IDictionary<string, object> AttValue { get; }

        ILane Lane { get; }
    }

    public interface ILane
    {
        IDictionary<string, string> AttValue { get; }
    }
}
