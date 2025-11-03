using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using static Ghost;

namespace Shop
{
    [Serializable]
    public struct ShopItem
    {
        public SongInfo song;
        public int price;

        public static ShopItem None => new ShopItem() { song = SongInfo.None, price = 0 };
        public bool IsEmpty => song == SongInfo.None;
    }

    [ExecuteAlways]
    public class Shop : MonoBehaviour
    {
        [Header("Components")]
        public ShopShelfDisplay[] shelfDisplays;
        [Header("Contents")]
        public List<SongInfo> songPool;
        public ShopItem[] inventory;

        public TMP_Text moneyField;
        [Header("Patron")]
        public int cartCapacity;
        public int budget;
        public bool canBuyMinigames;
        public int minimumPartyLevel;
        [Header("Setlist reading")]
        public string emptySetlistSlotTitle = "AUDIENCE_CHOICE";
        public string skipSlotsAfterTitle = "Mii";
        [Header("Web")]
        public HttpRequestLoop songPoolRequest = new(HttpRequest.RequestType.GET, "songs/available/{setlist_id}", HttpRequestLoop.ParameterFormat.Query);
        public SocketIOClientScriptable socketClient;
        public string cartContentMessagePrefix = "Panier : ";
        public string buyingMessage = "Achat en cours...";
        [Header("Events")]
        public UnityEvent<ShopItem> onBuyItem;
        [Header("Editor Tools")]
        public ObjectMethodCaller editorButtons = new ObjectMethodCaller("RefreshSongPool", "FillInventory", "ClearInventory", "InitCart");

        private List<ShopItem> cart;

        private InventoryTracker playerInventory;

        protected void OnEnable()
        {
            playerInventory = CurrentAssetsManager.GetCurrent<InventoryTracker>();
            socketClient = CurrentAssetsManager.GetCurrent<SocketIOClientScriptable>();
            SetPlayerMoney(playerInventory.Data.Money);
            ClearInventory();
            RefreshSongPool();
            InitCart();
        }

        protected void OnDisable()
        {
            SetShelfListenersActive(false);
        }

        private void Update()
        {
            UpdateShelves();
        }

        private void SetShelfListenersActive(bool active)
        {
            if (shelfDisplays == null) return;
            foreach(ShopShelfDisplay shelf in shelfDisplays)
            {
                if (shelf == null) continue;
                if (active) shelf.onChoseItem.AddListener(OnItemChoice);
                else shelf.onChoseItem.RemoveListener(OnItemChoice);
            }
        }

        private void OnItemChoice(ShopItem item)
        {
            ResetShelves();
            BuyItem(item);
        }

        public void ResetShelves()
        {
            if (shelfDisplays == null) return;
            foreach (ShopShelfDisplay shelf in shelfDisplays)
            {
                if (shelf == null) continue;
                shelf.ResetShelf();
            }
        }

        public void Open()
        {
            ResetShelves();
            FillInventory();
            SetShelfListenersActive(true);
            if (shelfDisplays != null) foreach(ShopShelfDisplay shelf in shelfDisplays) if (shelf != null) shelf.gameObject.SetActive(true);
        }

        public void Close()
        {
            SetShelfListenersActive(false);
            if (shelfDisplays != null) foreach (ShopShelfDisplay shelf in shelfDisplays) if (shelf != null) shelf.gameObject.SetActive(false);
        }

        public void RefreshSongPool()
        {
            if (songPoolRequest != null)
            {
                int setlistId = ConcertAdmin.Current != null ? ConcertAdmin.Current.state.setlist.databaseID : -1;
                songPoolRequest.parameters = new string[] { setlistId.ToString() };
                songPoolRequest.onRequestEnd.AddListener(OnSongPoolRequestEnd);
                songPoolRequest.StartRequestCoroutine(this, restart: true);
            }
        }

        private void OnSongPoolRequestEnd(HttpRequest request)
        {
            songPool = null;
            if (songPoolRequest != null)
            {
                songPoolRequest.onRequestEnd.RemoveListener(OnSongPoolRequestEnd);
                if (songPoolRequest.RequestStatus != HttpRequest.RequestStatus.Success)
                {
                    SongInfo[] getSongs = songPoolRequest.DeserializeResponse<SongInfo[]>();
                    songPool = getSongs != null ? new(getSongs) : new();
                }
            }
        }

        public void RemoveSongFromPool(SongInfo song)
        {
            if (songPool == null) return;
            songPool.Remove(song);
        }

        public int InventorySize
        {
            get => inventory != null ? inventory.Length : 0;
            set
            {
                int size = Mathf.Max(0, value);
                if (inventory != null) Array.Resize(ref inventory, size);
                else inventory = new ShopItem[size];
            }
        }

        public void ClearInventory()
        {
            if (InventorySize > 0) Array.Clear(inventory, 0, InventorySize);
        }

        public void RemoveItemFromInventory(int index)
        {
            if (index < 0 || index >= InventorySize) return;
            inventory[index] = new ShopItem() { song = SongInfo.None, price = -1 };
        }

        public void RemoveItemFromInventory(ShopItem item)
        {
            if (InventorySize == 0) return;
            RemoveItemFromInventory(Array.IndexOf(inventory, item));
        }

        public void FillInventory()
        {
            for (int i = 0, iend = InventorySize; i < iend; i++)
            {
                if (inventory[i].IsEmpty) inventory[i] = CreateShopItem();
            }
        }

        private ShopItem CreateShopItem()
        {
            // Pick an available song from pool, that is not already in inventory
            int songPoolSize = songPool != null ? songPool.Count : 0;
            List<int> availableSongIndices = new List<int>(songPoolSize);
            SongInfo song;
            for (int i = 0; i < songPoolSize; i++)
            {
                song = songPool[i];
                if (IsInInventory(song)) continue;
                if (!canBuyMinigames && song.hasMinigame) continue;
                if (song.partyLevel < minimumPartyLevel) continue;
                availableSongIndices.Add(i);
            }
            if (availableSongIndices.Count == 0) return ShopItem.None;
            song = songPool[availableSongIndices[UnityEngine.Random.Range(0, availableSongIndices.Count)]];
            // Set a semi-random price
            GetPriceRange(cartCapacity, budget, out int minPrice, out int maxPrice);
            return new ShopItem() { song = song, price = UnityEngine.Random.Range(minPrice, maxPrice) };
        }

        private bool IsInInventory(SongInfo song)
        {
            if (inventory == null) return false;
            return Array.FindIndex(inventory, item => item.song == song) != -1;
        }

        private void GetPriceRange(int buyableItems, int maxBudget, out int minPrice, out int maxPrice)
        {
            if (buyableItems > 0)
            {
                minPrice = Mathf.CeilToInt(maxBudget / (buyableItems + 1)) + 1;
                maxPrice = Mathf.FloorToInt(maxBudget / buyableItems);
            }
            else
            {
                minPrice = maxBudget + 1;
                maxPrice = int.MaxValue;
            }
        }

        private void UpdateShelves()
        {
            int shelfCount = shelfDisplays != null ? shelfDisplays.Length : 0,
                inventoryCount = inventory != null ? inventory.Length : 0;
            if (shelfCount == 0) return;
            int inventory_index = 0;
            foreach (ShopShelfDisplay shelf in shelfDisplays)
            {
                if (shelf == null) continue;
                if (inventory_index < inventoryCount) shelf.item = inventory[inventory_index];
                else shelf.EmptyShelf();
                inventory_index++;
            }
        }

        private void BuyItem(ShopItem item)
        {
            MessengerAdmin.Send(buyingMessage);
            // Update cart
            if (cart == null) cart = new List<ShopItem>(cartCapacity);
            cart.Add(item);
            // Update inventory
            RemoveItemFromInventory(item);
            RemoveSongFromPool(item.song);
            int newPlayerMoney = playerInventory.Data.Money - item.price;
            SetPlayerMoney(newPlayerMoney);
            FillInventory();
            // Update setlist
            if (socketClient != null)
            {
                socketClient.Emit("choice", item.song.databaseID);
                MessengerAdmin.Send(CartContentMessage);
            }
            // Notify purchase
            onBuyItem.Invoke(item);
        }

        private string CartContentMessage
        {
            get
            {
                string message = cartContentMessagePrefix;
                if (cart != null)
                {
                    foreach (ShopItem item in cart) message += item.song.Title + " | ";
                    message += cart.Count + "/" + cartCapacity;
                }
                return message;
            }
        }

        public bool CartIsFull => cart != null && cart.Count >= cartCapacity;

        public void InitCart()
        {
            SetlistInfo currentSetlist = ConcertAdmin.Current.state.setlist;
            int currentPosition = ConcertAdmin.Current.state.songPosition;
            string shopSongTitle = currentSetlist.GetSong(currentPosition).title;
            cartCapacity = 0;
            SongInfo song;
            bool skipSlots = false;
            for (int i = currentPosition + 1; i < currentSetlist.Length; i++)
            {
                song = currentSetlist.GetSong(i);
                if (song.title == shopSongTitle) break;
                else if (song.title == skipSlotsAfterTitle) skipSlots = true;
                else if (song.title == emptySetlistSlotTitle && skipSlots == false) cartCapacity++;
                else skipSlots = false;
            }
            cart = new List<ShopItem>(cartCapacity);
        }

        public void SetPlayerMoney(int value)
        {
            moneyField.enabled = true;
            moneyField.text = "x " + value;
            playerInventory.SetMoney(value);
        }
    }
}