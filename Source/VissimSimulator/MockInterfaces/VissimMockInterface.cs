//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace VissimSimulator
//{
//    public interface IVissim 
//    {
//        IVissimNet Net { get; }

//        void LoadNet();

//        void Exit();
//    }

//    public class Vissim : IVissim
//    {
//        public IVissimNet Net { get { return null; } }

//        public void LoadNet() { }

//        public void Exit() { }
//    }

//    public interface IVissimNet
//    {
//        IList<IVehicle> Vehicles { get; }
//    }

//    public interface IVehicle
//    {
//        string Id { get; }
//    }
//}
