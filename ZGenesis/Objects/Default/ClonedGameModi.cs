using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
namespace ZGenesis.Objects.Default {
    internal class ClonedFIFOModus : ModdedModus {
        internal FIFOModus basedOn;

        public override int ItemCapacity { get => base.ItemCapacity; set => base.ItemCapacity = value; }

        public override void Load(ModusData data) => basedOn.Load(data);
        public override ModusData Save() => basedOn.Save();
        public override int GetHashCode() {
            Logger.Log("TESTabcdef", "{0}", basedOn);
            return basedOn.GetHashCode();
        }
        public override bool Equals(object other) => basedOn.Equals(other);
        public override string ToString() => basedOn.ToString();
        protected override void UpdatePositions() {
            basedOn.GetType().GetMethod("UpdatePositions", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        protected override bool AddItemToModus(Item toAdd) {
            return (bool) basedOn.GetType().GetMethod("AddItemToModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { toAdd });
        }
        public override int CountItem(Item item) => basedOn.CountItem(item);
        protected override bool IsRetrievable(Card item) {
            return (bool) basedOn.GetType().GetMethod("IsRetrievable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        protected override IEnumerable<Card> GetItemList() {
            return (IEnumerable<Card>) basedOn.GetType().GetMethod("GetItemList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override void Save(Stream stream) => basedOn.Save(stream);
        protected override void Load(Item[] items) {
            basedOn.GetType().GetMethod("Load", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] { typeof(Item[]) },
                new ParameterModifier[] { }).Invoke(basedOn, new object[] { items });
        }
        public override int GetAmount() => basedOn.GetAmount();
        protected override bool RemoveItemFromModus(Card item) {
            return (bool) basedOn.GetType().GetMethod("RemoveItemFromModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        public override Rect GetItemRect() {
            return (Rect) basedOn.GetType().GetMethod("GetItemRect", BindingFlags.Public | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override bool IsActive(Item item) => basedOn.IsActive(item);
        public static new Sprite Sprite => sprite;
        private static readonly Sprite sprite;
        public static new string Description => description;
        private static readonly string description;
        static ClonedFIFOModus() {
            description = "Access only the items in the front of the queue. First in, first out.";
            sprite = Resources.Load<Sprite>("Modi/QueueModus"); 
        }
        public ClonedFIFOModus(FIFOModus basedOn) {
            this.basedOn = basedOn;
        }
    }



    internal class ClonedFILOModus : ModdedModus {
        internal FILOModus basedOn;

        public override int ItemCapacity { get => base.ItemCapacity; set => base.ItemCapacity = value; }

        public override void Load(ModusData data) => basedOn.Load(data);
        public override ModusData Save() => basedOn.Save();
        public override int GetHashCode() => basedOn.GetHashCode();
        public override bool Equals(object other) => basedOn.Equals(other);
        public override string ToString() => basedOn.ToString();
        protected override void UpdatePositions() {
            basedOn.GetType().GetMethod("UpdatePositions", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        protected override bool AddItemToModus(Item toAdd) {
            return (bool) basedOn.GetType().GetMethod("AddItemToModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { toAdd });
        }
        public override int CountItem(Item item) => basedOn.CountItem(item);
        protected override bool IsRetrievable(Card item) {
            return (bool) basedOn.GetType().GetMethod("IsRetrievable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        protected override IEnumerable<Card> GetItemList() {
            return (IEnumerable<Card>) basedOn.GetType().GetMethod("GetItemList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override void Save(Stream stream) => basedOn.Save(stream);
        protected override void Load(Item[] items) {
            basedOn.GetType().GetMethod("Load", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] { typeof(Item[]) },
                new ParameterModifier[] { }).Invoke(basedOn, new object[] { items });
        }
        public override int GetAmount() => basedOn.GetAmount();
        protected override bool RemoveItemFromModus(Card item) {
            return (bool) basedOn.GetType().GetMethod("RemoveItemFromModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        public override Rect GetItemRect() {
            return (Rect) basedOn.GetType().GetMethod("GetItemRect", BindingFlags.Public | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override bool IsActive(Item item) => basedOn.IsActive(item);
        public static new Sprite Sprite => sprite;
        private static readonly Sprite sprite;
        public static new string Description => description;
        private static readonly string description;
        static ClonedFILOModus() {
            description = "Access only the items at the top of the stack. First in, last out.";
            sprite = Resources.Load<Sprite>("Modi/StackModus");
        }
        public ClonedFILOModus(FILOModus basedOn) {
            this.basedOn = basedOn;
        }
    }



    internal class ClonedTreeModus : ModdedModus {
        internal TreeModus basedOn;

        public override int ItemCapacity { get => base.ItemCapacity; set => base.ItemCapacity = value; }

        public override void Load(ModusData data) => basedOn.Load(data);
        public override ModusData Save() => basedOn.Save();
        public override int GetHashCode() => basedOn.GetHashCode();
        public override bool Equals(object other) => basedOn.Equals(other);
        public override string ToString() => basedOn.ToString();
        protected override void UpdatePositions() {
            basedOn.GetType().GetMethod("UpdatePositions", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        protected override bool AddItemToModus(Item toAdd) {
            return (bool) basedOn.GetType().GetMethod("AddItemToModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { toAdd });
        }
        public override int CountItem(Item item) => basedOn.CountItem(item);
        protected override bool IsRetrievable(Card item) {
            return (bool) basedOn.GetType().GetMethod("IsRetrievable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        protected override IEnumerable<Card> GetItemList() {
            return (IEnumerable<Card>) basedOn.GetType().GetMethod("GetItemList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override void Save(Stream stream) => basedOn.Save(stream);
        protected override void Load(Item[] items) {
            basedOn.GetType().GetMethod("Load", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] { typeof(Item[]) },
                new ParameterModifier[] { }).Invoke(basedOn, new object[] { items });
        }
        public override int GetAmount() => basedOn.GetAmount();
        protected override bool RemoveItemFromModus(Card item) {
            return (bool) basedOn.GetType().GetMethod("RemoveItemFromModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        public override Rect GetItemRect() {
            return (Rect) basedOn.GetType().GetMethod("GetItemRect", BindingFlags.Public | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override bool IsActive(Item item) => basedOn.IsActive(item);
        public static new Sprite Sprite => sprite;
        private static readonly Sprite sprite;
        public static new string Description => description;
        private static readonly string description;
        static ClonedTreeModus() {
            description = "Access only items in the 'leaves' of the tree.";
            sprite = Resources.Load<Sprite>("Modi/TreeModus");
        }
        public ClonedTreeModus(TreeModus basedOn) {
            this.basedOn = basedOn;
        }
    }



    internal class ClonedHashmapModus : ModdedModus {
        internal HashmapModus basedOn;

        public override int ItemCapacity { get => base.ItemCapacity; set => base.ItemCapacity = value; }

        public override void Load(ModusData data) => basedOn.Load(data);
        public override ModusData Save() => basedOn.Save();
        public override int GetHashCode() => basedOn.GetHashCode();
        public override bool Equals(object other) => basedOn.Equals(other);
        public override string ToString() => basedOn.ToString();
        protected override void UpdatePositions() {
            basedOn.GetType().GetMethod("UpdatePositions", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        protected override bool AddItemToModus(Item toAdd) {
            return (bool) basedOn.GetType().GetMethod("AddItemToModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { toAdd });
        }
        public override int CountItem(Item item) => basedOn.CountItem(item);
        protected override bool IsRetrievable(Card item) {
            return (bool) basedOn.GetType().GetMethod("IsRetrievable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        protected override IEnumerable<Card> GetItemList() {
            return (IEnumerable<Card>) basedOn.GetType().GetMethod("GetItemList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override void Save(Stream stream) => basedOn.Save(stream);
        protected override void Load(Item[] items) {
            basedOn.GetType().GetMethod("Load", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] { typeof(Item[]) },
                new ParameterModifier[] { }).Invoke(basedOn, new object[] { items });
        }
        public override int GetAmount() => basedOn.GetAmount();
        protected override bool RemoveItemFromModus(Card item) {
            return (bool) basedOn.GetType().GetMethod("RemoveItemFromModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        public override Rect GetItemRect() {
            return (Rect) basedOn.GetType().GetMethod("GetItemRect", BindingFlags.Public | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override bool IsActive(Item item) => basedOn.IsActive(item);
        public static new Sprite Sprite => sprite;
        private static readonly Sprite sprite;
        public static new string Description => description;
        private static readonly string description;
        static ClonedHashmapModus() {
            description = "Access any item, but items are stored using a hash function.";
            sprite = Resources.Load<Sprite>("Modi/HashmapModus");
        }
        public ClonedHashmapModus(HashmapModus basedOn) {
            this.basedOn = basedOn;
        }
    }



    internal class ClonedArrayModus : ModdedModus {
        internal ArrayModus basedOn;

        public override int ItemCapacity { get => base.ItemCapacity; set => base.ItemCapacity = value; }

        public override void Load(ModusData data) => basedOn.Load(data);
        public override ModusData Save() => basedOn.Save();
        public override int GetHashCode() => basedOn.GetHashCode();
        public override bool Equals(object other) => basedOn.Equals(other);
        public override string ToString() => basedOn.ToString();
        protected override void UpdatePositions() {
            basedOn.GetType().GetMethod("UpdatePositions", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        protected override bool AddItemToModus(Item toAdd) {
            return (bool) basedOn.GetType().GetMethod("AddItemToModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { toAdd });
        }
        public override int CountItem(Item item) => basedOn.CountItem(item);
        protected override bool IsRetrievable(Card item) {
            return (bool) basedOn.GetType().GetMethod("IsRetrievable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        protected override IEnumerable<Card> GetItemList() {
            return (IEnumerable<Card>) basedOn.GetType().GetMethod("GetItemList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override void Save(Stream stream) => basedOn.Save(stream);
        protected override void Load(Item[] items) {
            basedOn.GetType().GetMethod("Load", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] { typeof(Item[]) },
                new ParameterModifier[] { }).Invoke(basedOn, new object[] { items });
        }
        public override int GetAmount() => basedOn.GetAmount();
        protected override bool RemoveItemFromModus(Card item) {
            return (bool) basedOn.GetType().GetMethod("RemoveItemFromModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(basedOn, new object[] { item });
        }
        public override Rect GetItemRect() {
            return (Rect) basedOn.GetType().GetMethod("GetItemRect", BindingFlags.Public | BindingFlags.Instance).Invoke(basedOn, new object[] { });
        }
        public override bool IsActive(Item item) => basedOn.IsActive(item);
        public static new Sprite Sprite => sprite;
        private static readonly Sprite sprite;
        public static new string Description => description;
        private static readonly string description;
        static ClonedArrayModus() {
            description = "Access any item.";
            sprite = Resources.Load<Sprite>("Modi/ArrayModus");
        }
        public ClonedArrayModus(ArrayModus basedOn) {
            this.basedOn = basedOn;
        }
    }
}
