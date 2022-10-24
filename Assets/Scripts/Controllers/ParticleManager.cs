using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance;
        public ParticleData[] particleData;
        Dictionary<string, ParticleData> _particleDic;

        private void Awake()
        {
            Instance = this;
            _particleDic = new Dictionary<string, ParticleData>();
            for (int i = 0; i < particleData.Length; i++)
            {
                _particleDic.Add(particleData[i].name, particleData[i]);
            }
        }

        public static Transform PlayParticle(string n)
        {
            return Instantiate(Instance._particleDic[n].GetParticle());
        }

        public static Transform PlayParticle(string n, Vector3 pos, Quaternion rot)
        {
            return Instantiate(Instance._particleDic[n].GetParticle(), pos, rot);
        }

        public static Transform PlayParticle(string n, Vector3 pos, Quaternion rot, Transform parent)
        {
            return Instantiate(Instance._particleDic[n].GetParticle(), pos, rot, parent);
        }
    }

    [System.Serializable]
    public class ParticleData
    {
        public string name;
        public Transform[] particleObj;

        public Transform GetParticle()
        {
            return particleObj[Random.Range(0, particleObj.Length)];
        }
    }
}
