using Engine.Factories;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Engine.Models
{
    public abstract class LivingEntity : BaseNotificationClass
    {
        #region Backing Variables
        private string? _name;
        private Ancestry? _ancestry;
        private string? _description;
        private ObservableCollection<Skill>? _skills;
        private ObservableCollection<Characteristic>? _characteristics;
        private ObservableCollection<ItemQuantity>? _inventory;
        private Item? _equippedWeapon;
        private Body? _body;
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
        public Ancestry? Ancestry
        {
            get { return _ancestry; }
            set
            {
                _ancestry = value;
                OnPropertyChanged();
            }
        }
        public Background? Background { get; set; }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public List<Tag> Tags = new List<Tag>();
        public ObservableCollection<Skill> Skills
        {
            get { return _skills; }
            set
            {
                _skills = value;
                OnPropertyChanged();
            }

        }
        public ObservableCollection<Characteristic> Characteristics
        {
            get { return _characteristics; }
            set
            {
                _characteristics = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public bool IsDead => CurrentBody.Parts.Any(p => p.CurrentHealth <= 0);

        public event EventHandler<string>? OnActionPerformed;
        public event EventHandler? OnKilled;

        public ObservableCollection<ItemQuantity> Inventory
        {
            get { return _inventory; }
            set
            {
                _inventory = value;
                OnPropertyChanged();
            }
        }
        public Item? EquippedWeapon
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
        public Body CurrentBody
        {
            get { return _body; }
            set
            {
                _body = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor 
        protected LivingEntity(
            string name,
            string description)
        {
            Name = name;
            Description = description;
            Inventory = new ObservableCollection<ItemQuantity>();
            Characteristics = new ObservableCollection<Characteristic>();
            Skills = new ObservableCollection<Skill>();
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
            NumberInventory();
        }
        public void AddItemsToInventory(Item item, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (item.IsUnique)
                    Inventory.Add(new ItemQuantity(item, 1));
                else
                {
                    if (!Inventory.Any(i => i.BaseItem.ID == item.ID))
                    {
                        Inventory.Add(new ItemQuantity(item, 0));
                    }

                    Inventory.First(i => i.BaseItem.ID == item.ID).Quantity++;
                }
            }
            NumberInventory();
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
            NumberInventory();
        }
        public Item GetItemFromInventory(string aString)
        {
            return Inventory.FirstOrDefault(i => i.BaseItem.Name.ToLower() == aString.ToLower())?.BaseItem ?? default;
        }
        public bool HasItem(string aString)
        {
            if (Inventory.Any(i => i.BaseItem.Name.ToLower() == aString.ToLower()))
                return true;

            return false;
        }
        public void NumberInventory()
        {
            int i = 1;
            foreach (ItemQuantity item in Inventory)
            {
                item.BaseItem.InventoryNumber = i;
                i++;
            }
        }
        #endregion

        #region Skill and Characteristic Functions
        public void AddExperienceToSkill(string aString, int num)
        {
            foreach(Skill skill in Skills)
            {
                if(skill.Name.ToLower() == aString.ToLower())
                {
                    skill.AddExperience(num);
                }
            }
        }
        public void AddExperienceToCharacteristic(string aString, int num)
        {
            foreach(Characteristic characteristic in Characteristics)
            {
                if(characteristic.Name.ToLower() == aString.ToLower())
                {
                    characteristic.AddExperience(num);
                }
            }
        }
        public void GainSkill(string aString)
        {
            Skill skill = SkillFactory._skills.FirstOrDefault(s => s.Name.ToLower() == aString.ToLower());
            if (skill != null)
                 Skills.Add(skill);
        }
        public void LoadSkills(string name, int baseLevel, double levelMulti)
        {
            Skill skill = SkillFactory._skills.FirstOrDefault(s => s.Name.ToLower() == name.ToLower());
            if (skill != null)
            {
                skill.BaseLevel = baseLevel;
                skill.CurrentRank = skill.DetermineRank();
                skill.LevelMultiplier = levelMulti;
                Skills.Add(skill);
            }
        }
        public void LoadCharacteristics(string name, int baseLevel, double levelMulti)
        {
            Characteristic characteristic = CharacteristicFactory._characteristics.FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
            if(characteristic != null)
            {
                characteristic.BaseLevel = baseLevel;
                characteristic.CurrentRank = characteristic.DetermineRank();
                characteristic.LevelMultiplier = levelMulti;
                Characteristics.Add(characteristic);
            }
        }
        #endregion

        public void Attack(LivingEntity target)
        {
            EquippedWeapon.PerformAction(this, target);
        }
        public void TakeDamage(double hitPointsOfDamage, BodyPart targetPart)
        {
            targetPart.CurrentHealth -= hitPointsOfDamage;
            if (IsDead)
            {
                RaiseOnKilledEvent();
            }
        }
        public void Heal(int hitPointsToHeal)
        {

        }
        public void FullHeal()
        {
            foreach(BodyPart part in CurrentBody.Parts)
            {
                part.CurrentHealth = part.MaximumHealth;
            }
        }

        #region Creation Functions
        public void AddRacialSkillBonus(Skill skill, double modifier)
        {
            skill.LevelMultiplier = modifier;
        }

        #endregion

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
