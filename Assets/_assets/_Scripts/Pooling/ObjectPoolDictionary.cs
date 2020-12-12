using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace Arashmup
{
    [CreateAssetMenu]
    public class ObjectPoolDictionary : RuntimeDictionary<string, ObjectPool>
    {
    }
}