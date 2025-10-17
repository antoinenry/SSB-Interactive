using System;
using System.Collections;
using UnityEngine;

namespace Shop
{
    [Serializable]
    public struct ShopItem
    {
        public string name;
        public Sprite icon;
        public int price;
    }

    public class ShopStage : Stage
    {
        public ShopItem[] inventory;
        public ShopShelfDisplay[] shelfDisplays;
        public HttpRequestLoop songPoolRequest;
        [Header("Editor Tools")]
        public ObjectMethodCaller editorButtons = new ObjectMethodCaller("RefreshInventory");

        private PollMaster poll;
        private Concert concertLoader;

        public HttpClientScriptable HttpClient { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            HttpClient = CurrentAssetsManager.GetCurrent<HttpClientScriptable>();
            concertLoader = FindObjectOfType<Concert>(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (poll) poll.onCandidateWins.AddListener(OnPollWinner);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (poll) poll.onCandidateWins.RemoveListener(OnPollWinner);
        }

        private void Update()
        {
            UpdateShelves();
        }

        protected override bool HasAllComponents()
        {
            if (base.HasAllComponents() && poll) return true;
            poll = GetComponentInChildren<PollMaster>(true);
            return base.HasAllComponents() && poll;
        }

        public void RefreshInventory()
        {
            inventory = new ShopItem[0];
            if (songPoolRequest != null)
            {
                songPoolRequest.onRequestEnd.AddListener(OnSongPoolRequestEnd);
                songPoolRequest.StartRequestCoroutine(this);
            }

        }

        private void OnSongPoolRequestEnd(HttpRequest request)
        {
            inventory = new ShopItem[0];
            // Process response
            SongInfo[] availableSongs = null;
            if (songPoolRequest != null)
            {
                songPoolRequest.onRequestEnd.RemoveListener(OnSongPoolRequestEnd);
                if (songPoolRequest.RequestStatus != HttpRequest.RequestStatus.Success)
                    availableSongs = songPoolRequest.DeserializeResponse<SongInfo[]>();
            }
            // Convert available songs into shop items
            if (availableSongs != null)
                inventory = Array.ConvertAll(availableSongs, s => CreateShopItem(s));
            else
                inventory = new ShopItem[0];
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

        private ShopItem CreateShopItem(SongInfo song)
            => new ShopItem()
            {
                name = song.Title,
                icon = null,
                price = 0
            };

        private void OnPollWinner(PollMaster.Candidate candidate)
        {
            if (shelfDisplays == null) return;
            string buttonID = candidate?.buttonID;
            int shelfIndex = Array.FindIndex(shelfDisplays, s => s.buttonID == buttonID);
            if (shelfIndex != -1) BuyItem(shelfDisplays[shelfIndex].item);           
        }

        private void BuyItem(ShopItem item)
        {
            Debug.Log("BOUGHT A " + item.name + " !");
        }
    }
}