using UnityEngine;

namespace PacknCraft.Inventory
{
    [CreateAssetMenu(menuName = "PacknCraft/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        // SERIALIZED FIELDS
        public string Id;

        public ShapeData Shape;
    }
}