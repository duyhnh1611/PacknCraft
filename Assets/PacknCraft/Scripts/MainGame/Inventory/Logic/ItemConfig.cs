using System.Collections.Generic;
using UnityEngine;

namespace PacknCraft.Inventory
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "PacknCraft/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        // SERIALIZED FIELDS
        [SerializeField] private string id;
        [SerializeField] private List<Vector2Int> cells = new();
        [SerializeField] private Vector2 pivot;

        // PUBLIC PROPERTIES
        public string Id => id;
        public IReadOnlyList<Vector2Int> Cells => cells;
        public Vector2 Pivot => pivot;
    }
}