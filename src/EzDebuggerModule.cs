using System;
using System.Diagnostics;
using Leopotam.EcsProto;
using Debug = UnityEngine.Debug;

namespace ProtoEzDebugger {
    public class EzDebuggerModule : IProtoModule, IProtoEventListener {
        private readonly string _worldName;
        private readonly Type[] _types;
        private bool[] _trackedPools;
        private ProtoWorld _world;

        public EzDebuggerModule(string worldName = null, params Type[] types) {
#if DEBUG
            if (types == null) {
                throw new Exception("Массив типов null!");
            }
#endif
            _types = types;
            _worldName = worldName;
        }

        public void Init(IProtoSystems systems) {
            _world = systems.World(_worldName);
            _world.AddEventListener(this);
            _trackedPools = new bool[_world.Pools().Len()];
            for (var i = 0; i < _types.Length; i++) {
                _trackedPools[_world.Pool(_types[i]).Id()] = true;
            }
        }
        
        public void OnEntityCreated(int entity) {
        }

        public void OnEntityChanged(int entity, ushort poolId, bool added) {
            if (_trackedPools[poolId])
            {
                StackTrace stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(2);

                Debug.Log($"[ТРАССИРОВКА] компонент {_world.Pools().Get(poolId).ItemType().Name} {(added ? "добавлен" : "удален")} в классе {frame.GetMethod().ReflectedType} в методе {frame.GetMethod().Name}!");
            }
        }

        public void OnEntityDestroyed(int entity) { }

        public void OnWorldResized(int capacity) { }

        public void OnWorldDestroyed() {
            _world.RemoveEventListener(this);
        }

        public IProtoAspect[] Aspects() => null;

        public IProtoModule[] Modules() => null;
    }
}