using System.Collections.ObjectModel;
using System.Net;
using ModernApp.MVVM.Model;

namespace ModernApp.MVVM.ViewModel
{
    public class inventoryViewModel
    {
        private readonly inventoryModel _inventoryModel;

        public inventoryViewModel()
        {
            _inventoryModel = new inventoryModel();
        }

        public void AddItem(string itemId, string itemName,int qty,string type, string model, string special, string supplierName, string address, string email, string number, byte[] image)
        {
            
            
                _inventoryModel.SaveItem(itemId, itemName, qty, type, model, special, supplierName, address, email, number, image); // Save to the database
            
        }
        public InventoryItem GetItemById(string itemId)
        {
            return _inventoryModel.FetchItemById(itemId);
        }

        public void UpdateItem(string itemId, string itemName, int qty, string type, string model, string special, string supplierName, string address, string email, string number, byte[] imageData)
        {
            _inventoryModel.UpdateItemInDatabase(itemId, itemName, qty, type, model, special, supplierName, address, email, number, imageData);
        }
        public void DeleteItem(string itemId)
        {
            _inventoryModel.DeleteItemFromDatabase(itemId);
        }
    }
}
