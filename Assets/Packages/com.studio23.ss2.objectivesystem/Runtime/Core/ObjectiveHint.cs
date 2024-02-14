﻿using System;
using NaughtyAttributes;
using Newtonsoft.Json;
using Studio23.SS2.InventorySystem.Data;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Studio23.SS2.ObjectiveSystem.Core
{
    [CreateAssetMenu(menuName = "Studio-23/Objective System/Hint", fileName = "objective Hint")]
    [Serializable]
    public class ObjectiveHint: ItemBase
    {
        [SerializeField] ObjectiveBase _parentObjective;
        public ObjectiveBase ParentObjective => _parentObjective;
        [SerializeField] int _priority;
        public int Priority => _priority;

        [NonSerialized][ShowNonSerializedField] private bool _isActive = false;
        public bool IsActive => _isActive;
        public event Action<ObjectiveHint> OnHintActivationToggled;
        public bool ObjectiveManagerExists => ObjectiveManager.Instance != null;

        public void SetObjective(ObjectiveBase objective)
        {
            this._parentObjective = objective;
        }

        internal void SetActive(bool shouldBeActive)
        {
            if (shouldBeActive == IsActive)
                return;
            _isActive = shouldBeActive;
            OnHintActivationToggled?.Invoke(this);
        }
        [ShowIf("ObjectiveManagerExists")]
        [Button]
        public void Add()
        {
            if (!ObjectiveManager.Instance.IsObjectiveActiveAndValid(_parentObjective))
            {
                Debug.LogWarning($"can't add hint {this} because parent objective is not active and valid");
                return;
            }
            SetActive(true);
        }

        public void FullReset()
        {
            Remove();
        }
        [ShowIf("ObjectiveManagerExists")]
        [Button]
        public void Remove()
        {
            if (!ObjectiveManager.Instance.IsObjectiveActiveAndValid(_parentObjective))
            {
                Debug.LogWarning($"can't remove hint {this} because parent objective is not active and valid");
                return;
            }
            SetActive(false);
        }
        public override void AssignSerializedData(string data)
        {
            _isActive = JsonConvert.DeserializeObject<bool>(data);
        }

        public override string GetSerializedData()
        {
            return JsonConvert.SerializeObject(_isActive);
        }
#if UNITY_EDITOR     
        [Button]
        public void Rename()
        {
            Rename(_parentObjective.getFullHintAssetName(Name));
        }
        
                
        [Button]
        public void DestroyHint()
        {
            if (_parentObjective != null) 
            {
                Undo.RecordObject(_parentObjective, "Hint");
                _parentObjective.Hints.Remove(this);
            }

            EditorUtility.SetDirty(_parentObjective);
            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssetIfDirty(_parentObjective);
        }
        
        
        public void Rename(string newName)
        {
            this.name = newName;

            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}

