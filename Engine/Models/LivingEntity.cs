using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public abstract class LivingEntity : BaseNotificationClass
    {
        #region Backing Variables
        private string _name;
        private string _ancestry;
        private string _charClass;
        private string _description;
        private int _experience;
        private int _cats;
        private int _strength;
        private int _dexterity;
        private int _endurance;
        private int _perception;
        private int _sensitivity;
        private int _willpower;
        private int _appearance;
        private int _presence;
        private int _empathy;
        private int _maximumHealth;
        private int _currentHealth;
        private ObservableCollection<ItemQuantity> _inventory;
        private Item _equippedWeapon;
        #endregion

        #region Public Variables
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Ancestry
        {
            get { return _ancestry; }
            private set
            {
                _ancestry = value;
                OnPropertyChanged();
            }
        }
        public string CharClass
        {
            get { return _charClass; }
            private set
            {
                _charClass = value;
                OnPropertyChanged();
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        public int Experience
        {
            get { return _experience; }
            set
            {
                _experience = value;
                OnPropertyChanged();
            }
        }
        public int Cats
        {
            get { return _cats; }
            set
            {
                _cats = value;
                OnPropertyChanged();
            }
        }
        public int Strength
        {
            get { return _strength; }
            set
            {
                _strength = value;
                OnPropertyChanged();
            }
        }
        public int Dexterity
        {
            get { return _dexterity; }
            set
            {
                _dexterity = value;
                OnPropertyChanged();
            }
        }
        public int Endurance
        {
            get { return _endurance; }
            set
            {
                _endurance = value;
                OnPropertyChanged();
            }
        }
        public int Perception
        {
            get { return _perception; }
            set
            {
                _perception = value;
                OnPropertyChanged();
            }
        }
        public int Sensitivity
        {
            get { return _sensitivity; }
            set
            {
                _sensitivity = value;
                OnPropertyChanged();
            }
        }
        public int Willpower
        {
            get { return _willpower; }
            set
            {
                _willpower = value;
                OnPropertyChanged();
            }
        }
        public int Appearance
        {
            get { return _appearance; }
            set
            {
                _appearance = value;
                OnPropertyChanged();
            }
        }
        public int Presence
        {
            get { return _presence; }
            set
            {
                _presence = value;
                OnPropertyChanged();
            }
        }
        public int Empathy
        {
            get { return _empathy; }
            set
            {
                _empathy = value;
                OnPropertyChanged();
            }
        }
        public int MaximumHealth
        {
            get { return _maximumHealth; }
            set
            {
                _maximumHealth = value;
                OnPropertyChanged();
            }
        }
        public int CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value;
                OnPropertyChanged();
            }
        }

        public bool IsDead => CurrentHealth <= 0;

        public event EventHandler<string> OnActionPerformed;
        public event EventHandler OnKilled;

        public ObservableCollection<ItemQuantity> Inventory
        {
            get { return _inventory; }
            set
            {
                _inventory = value;
                OnPropertyChanged();
            }
        }
        public Item EquippedWeapon
        {
            get { return _equippedWeapon; }
            set
            {
                if (_equippedWeapon != null)
                    _equippedWeapon.Action.OnActionPerformed -= RaiseActionPerformedEvent;
                _equippedWeapon = value;
                if (_equippedWeapon != null)
                    _equippedWeapon.Action.OnActionPerformed += RaiseActionPerformedEvent;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor 
        protected LivingEntity(
            string name, string ancestry, string charClass, int maxHealth, int currentHealth, string description, int experience, int cats,
            int strength, int dexterity, int endurance, int perception, int sensitivity, int willpower, int appearance, int presence, int empathy)
        {
            Name = name;
            Ancestry = ancestry;
            CharClass = charClass;
            MaximumHealth = maxHealth;
            CurrentHealth = currentHealth;
            Description = description;
            Experience = experience;
            Cats = cats;
            Strength = strength;
            Dexterity = dexterity;
            Endurance = endurance;
            Perception = perception;
            Sensitivity = sensitivity;
            Willpower = willpower;
            Appearance = appearance;
            Presence = presence;
            Empathy = empathy;
            Inventory = new ObservableCollection<ItemQuantity>();
        }
        #endregion

        #region Inventory Functions
        public void AddItemToInventory(Item item)
        {
            if (item.IsUnique)
                Inventory.Add(new ItemQuantity(item, 1));
            else
            {
                if(!Inventory.Any(i => i.BaseItem.ID == item.ID))
                {
                    Inventory.Add(new ItemQuantity(item, 0));
                }

                Inventory.First(i => i.BaseItem.ID == item.ID).Quantity++;
            }
        }
        public void RemoveItemFromInventory(Item item)
        {
            ItemQuantity itemToRemove = Inventory.FirstOrDefault(i => i.BaseItem == item);
            if(itemToRemove != null)
            {
                if(itemToRemove.Quantity == 1)
                {
                    Inventory.Remove(itemToRemove);
                }
                else
                {
                    itemToRemove.Quantity--;
                }
            }
        }
        public Item GetItemFromInventory(string aString)
        {
            Item item = Inventory.FirstOrDefault(i => i.BaseItem.Name.ToLower() == aString.ToLower())?.BaseItem ?? default;
            return item;
        }
        #endregion
        
        public void Attack(LivingEntity target)
        {
            EquippedWeapon.PerformAction(this, target);
        }
        public void TakeDamage(int hitPointsOfDamage)
        {
            CurrentHealth -= hitPointsOfDamage;
            if (IsDead)
            {
                CurrentHealth = 0;
                RaiseOnKilledEvent();
            }
        }
        public void Heal(int hitPointsToHeal)
        {
            CurrentHealth += hitPointsToHeal;
            if (CurrentHealth > MaximumHealth)
            {
                CurrentHealth = MaximumHealth;
            }
        }
        public void FullHeal()
        {
            CurrentHealth = MaximumHealth;
        }
        public void ReceiveGold(int amountOfCats)
        {
            Cats += amountOfCats;
        }
        public void SpendGold(int amountOfCats)
        {
            if (amountOfCats > Cats)
            {
                throw new ArgumentOutOfRangeException($"{Name} only has {Cats} gold, and cannot spend {amountOfCats} gold");
            }
            Cats -= amountOfCats;
        }

        #region Event Functions
        private void RaiseOnKilledEvent()
        {
            OnKilled?.Invoke(this, new System.EventArgs());
        }
        private void RaiseActionPerformedEvent(object sender, string result)
        {
            OnActionPerformed?.Invoke(this, result);
        }
        #endregion
    }
}
