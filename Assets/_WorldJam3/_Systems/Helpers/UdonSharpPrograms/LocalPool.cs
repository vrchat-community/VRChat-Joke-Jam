using UdonSharp;
using UnityEngine;

namespace VRC.Examples.Helpers
{
    /// <summary>
    /// A simple local pool script that can instantiate a single prefab. With support
    /// for pre-populating a list of instances. NOTE: In order to pool different prefab
    /// types you must create a separate LocalPool component for every unique prefab.
    /// </summary>
    public class LocalPool : UdonSharpBehaviour
    {

        /// <summary>
        /// The single prefab type that this pool can instantiate.
        /// </summary>
        public GameObject prefab;

        /// <summary>
        /// The amount of objects to fill the pool with in advance. The objects will be
        /// deactivated until 'Spawn' is called.
        /// </summary>
        [Range(5, 50)] public int prePopulateCount;

        public GameObject[] _instances;
        private bool _prePopulated = false;


        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            if (!_prePopulated)
            {
                PrePopulate();
            }
        }


        /// <summary>
        /// Activates and returns a disabled gameobject in the _instances array.
        /// If no activated object can be found, instantiates a new one and adds
        /// it to the array.
        /// </summary>
        public GameObject Spawn()
        {
            GameObject instance = null;

            for (int i = 0; i < _instances.Length; i++)
            {
                if (!_instances[i].activeInHierarchy)
                {
                    instance = _instances[i];
                }
            }

            if (instance == null)
            {
                instance = Object.Instantiate(prefab);
                _instances = Add(_instances, instance);
            }

            instance.SetActive(true);

            return instance;
        }

        [HideInInspector] public GameObject SpawnGraphResult;

        /// <summary>
        /// The graph cannot accept a return object, so we instead save the result to a public variable which can be retrieved.
        /// </summary>
        public void SpawnGraph()
        {
            SpawnGraphResult = Spawn();
        }


        /// <summary>
        /// Spawns an instance with a specified position and rotation.
        /// </summary>
        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject instance = Spawn();
            instance.transform.SetPositionAndRotation(position, rotation);
            return instance;
        }


        /// <summary>
        /// If the passed gameobject is present in the _instances array,
        /// deactivates it (in effect returning it to the pool).
        /// </summary>
        public void Despawn(GameObject instance)
        {
            if (this.Contains(instance))
            {
                instance.SetActive(false);
            }
        }


        /// <summary>
        /// Returns true if the passed gameobject is present in the _instances array.
        /// </summary>
        public bool Contains(GameObject prefab)
        {
            for (int i = 0; i < _instances.Length; i++)
            {
                if (_instances[i] == prefab)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Expands 'oldArray' by one slot and adds the prefab to the last index.
        /// </summary>
        private GameObject[] Add(GameObject[] oldArray, GameObject prefab)
        {
            GameObject[] result = new GameObject[oldArray.Length + 1];
            oldArray.CopyTo(result, 0);
            result[result.Length - 1] = prefab;
            return result;
        }


        /// <summary>
        /// Creates a set number of instances of the spawnable prefab and adds
        /// them (deactivated) to the _instances list for later spawning.
        /// </summary>
        private void PrePopulate()
        {
            _instances = new GameObject[prePopulateCount];
            for (int i = 0; i < _instances.Length; i++)
            {
                GameObject instance = Object.Instantiate(prefab);
                _instances[i] = instance;
                instance.SetActive(false);
            }

            _prePopulated = true;
        }

    }
}