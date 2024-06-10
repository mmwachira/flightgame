using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlightGame.Utils
{
    public class TemplateType
    {
        public Type TempType;
        public Component TempComponent;

        public TemplateType()
        {
        }

        public TemplateType(Type tempType, Component tempComponent)
        {
            TempType = tempType;
            TempComponent = tempComponent;
        }
    }

    public class PoolByPrefab
    {
        //Determines if pool should expand when no object is available or just return null
        public bool AutoExpand = true;
        public Transform PoolParent { get; }

        public delegate void ObjectPoolEventHandler(Component obj);

        public event ObjectPoolEventHandler OnObjectActivated;
        public event ObjectPoolEventHandler OnObjectDeactivated;


        //Here to store the templates for autoExpand if necessary
        private readonly Dictionary<string, TemplateType> _poolTagTemplates = new();

        //Links the tag of the object with the pool
        private readonly Dictionary<string, Dictionary<Type, List<Component>>> _poolTagDict = new();

        public PoolByPrefab()
        {
        }

        public PoolByPrefab(Transform parent)
        {
            PoolParent = parent;
        }

        //Adds Prefab component to the ComponentPool
        public void AddPrefab<T>(T prefabReference, int count = 1, string tag = "")
        {
            AddComponentType(prefabReference, count, tag);
        }

        private void AddComponentType<T>(T prefabReference, int count = 1, string tag = "")
        {
            if (count <= 0)
            {
                Debug.LogError("Count cannot be <= 0");
                return;
            }

            Type compType = typeof(T);
            Component original = (Component)Convert.ChangeType(prefabReference, compType);

            Dictionary<Type, List<Component>> poolByType;
            List<Component> pool;
            if (_poolTagDict.TryGetValue(tag, out poolByType)) //Check if the tag already exist in the Dictionary
            {
                if (!poolByType.TryGetValue(compType, out pool)) //Check if the component type already exist in the pool
                {
                    pool = new List<Component>();
                }

                if (pool == null)
                    pool = new List<Component>();
            }
            else //If tag is new we add it to the dictionary along with the type
            {
                _poolTagTemplates[tag] = new TemplateType(compType, original);
                poolByType = new Dictionary<Type, List<Component>>();
                pool = new List<Component>();
            }

            //Create the type of component x times
            for (int i = 0; i < count; i++)
            {
                pool.Add(InstantiateComponent(original));
            }

            poolByType[compType] = pool; //Update dictionary with new pool
            _poolTagDict[tag] = poolByType; //Update dictionary with new tag
        }

        private Component InstantiateComponent(Component template)
        {
            Component instance = UnityEngine.Object.Instantiate(template, PoolParent);
            //De-activate each one until when needed
            instance.gameObject.SetActive(false);
            return instance;
        }

        public List<Component> GetPool<T>(string tag = "")
        {
            Type compType = typeof(T);

            Dictionary<Type, List<Component>> poolByType;
            if (_poolTagDict.TryGetValue(tag, out poolByType)) //Get all dictionaries with the requested tag
            {
                List<Component> pool;
                if (poolByType.TryGetValue(compType, out pool))
                {
                    if (pool != null)
                        return pool;
                }
            }

            return null;
        }

        public T GetAvailableObject<T>(string tag = "") where T : Component
        {
            Type compType = typeof(T);
            List<Component> pool;
            if (!_poolTagDict.TryGetValue(tag, out Dictionary<Type, List<Component>> poolByType))
            {
                poolByType = new Dictionary<Type, List<Component>>();
                _poolTagDict[tag] = poolByType;
            }

            if (!poolByType.TryGetValue(compType, out pool))
            {
                pool = new List<Component>();
                poolByType[compType] = pool;
            }

            //Get de-activated GameObject in the loop
            foreach (var component in pool)
            {
                if (!component.gameObject.activeInHierarchy)
                {
                    //Activate the GameObject then return it
                    component.gameObject.SetActive(true);
                    return ActivateAndReturnObject<T>(component, compType);
                }
            }

            //No available object in the pool. Expand array if enabled or return null
            if (AutoExpand)
            {
                //Create new component, activate the GameObject and return it
                Component instance = InstantiateComponent(_poolTagTemplates[tag].TempComponent);
                pool.Add(instance); //Added new object to the pool
                poolByType[compType] = pool; //Update dictionary with new pool
                _poolTagDict[tag] = poolByType; //Update dictionary with new tag
                instance.gameObject.SetActive(true);
                return ActivateAndReturnObject<T>(instance, compType);
            }

            return default(T);
        }

        public void RecycleObject(Component component, string tag = "")
        {
            GameObject obj = component.gameObject;
            Type compType = component.GetType();
            List<Component> pool;

            // Check if the tag exists and get the pool for this component type
            if (_poolTagDict.TryGetValue(tag, out Dictionary<Type, List<Component>> poolByType))
            {
                if (!poolByType.TryGetValue(compType, out pool))
                {
                    // If there's no pool for this component type under the given tag, create one
                    pool = new List<Component>();
                    poolByType[compType] = pool;
                }
            }
            else
            {
                Debug.LogError($"Tag '{tag}' not found in object pool.");
                return;
            }

            obj.transform.SetParent(PoolParent);
            obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            pool.Add(component);
            OnObjectDeactivated?.Invoke(component);
        }

        private T ActivateAndReturnObject<T>(Component component, Type compType) where T : Component
        {
            component.gameObject.SetActive(true);
            OnObjectActivated?.Invoke(component);
            return (T)Convert.ChangeType(component, compType);
        }
    }
}