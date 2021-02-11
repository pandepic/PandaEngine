﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementEngine.ECS
{
    public class Registry
    {
        protected const int _defaultMaxComponents = 100;
        protected int _nextEntityID = 0;

        public Dictionary<Type, IComponentStore> ComponentData = new Dictionary<Type, IComponentStore>();
        public List<Group> RegisteredGroups = new List<Group>();

        public Registry()
        {
        }

        public ComponentStore<T> GetComponentStore<T>() where T : struct
        {
            var type = typeof(T);

            if (ComponentData.TryGetValue(type, out var componentStore))
                return (ComponentStore<T>)componentStore;

            var newStore = new ComponentStore<T>(_defaultMaxComponents);
            ComponentData.Add(type, newStore);
            return newStore;
        }

        public int CreateEntity()
        {
            return _nextEntityID++;
        }

        public void DestroyEntity(int entityID)
        {
            foreach (var (_, componentStore) in ComponentData)
                componentStore.TryRemove(entityID);

            foreach (var view in RegisteredGroups)
                view.Entities.TryRemove(entityID);
        }

        public bool TryAddComponent<T>(int entityID, T component) where T : struct
        {
            if (GetComponentStore<T>().TryAdd(component, entityID))
            {
                for (var i = 0; i < RegisteredGroups.Count; i++)
                {
                    var view = RegisteredGroups[i];
                    var matchesView = true;

                    foreach (var viewType in view.Types)
                    {
                        if (ComponentData.TryGetValue(viewType, out var componentStore))
                        {
                            if (!componentStore.Contains(entityID))
                                matchesView = false;
                        }
                        else
                        {
                            matchesView = false;
                        }
                    }

                    if (matchesView)
                        view.Entities.TryAdd(entityID, out var _);
                }

                return true;
            }

            return false;
        } // TryAddComponent

        public bool TryRemoveComponent<T>(int entityID) where T : struct
        {
            if (GetComponentStore<T>().TryRemove(entityID))
            {
                for (var i = 0; i < RegisteredGroups.Count; i++)
                {
                    var type = typeof(T);
                    var view = RegisteredGroups[i];

                    if (view.Types.Contains(type))
                        view.Entities.TryRemove(entityID);
                }

                return true;
            }

            return false;
        } // TryRemoveComponent

        public ref T GetComponent<T>(int entityID) where T : struct
        {
            var componentStore = GetComponentStore<T>();
            if (!componentStore.Contains(entityID))
                throw new ArgumentException("This entity doesn't have the component requested.", "T");

            return ref componentStore.GetRef(entityID);
        }

        public Group RegisterGroup(Type[] componentTypes)
        {
            if (_nextEntityID > 0)
                throw new Exception("Must register groups before creating entities");

            var group = new Group(componentTypes);
            RegisteredGroups.Add(group);

            return group;
        }

        public Group RegisterGroup<T>() where T : struct
        {
            return RegisterGroup(new Type[] {
                typeof(T)
            });
        }

        public Group RegisterGroup<T, U>() where T : struct where U : struct
        {
            return RegisterGroup(new Type[] {
                typeof(T),
                typeof(U)
            });
        }

        public Group RegisterGroup<T, U, V>() where T : struct where U : struct where V : struct
        {
            return RegisterGroup(new Type[] {
                typeof(T),
                typeof(U),
                typeof(V)
            });
        }

        public View<T> View<T>() where T : struct
            => new View<T>(this);

        public View<T, U> View<T, U>() where T : struct where U : struct
            => new View<T, U>(this);

        public View<T, U, V> View<T, U, V>() where T : struct where U : struct where V : struct
            => new View<T, U, V>(this);

    } // Registry
}